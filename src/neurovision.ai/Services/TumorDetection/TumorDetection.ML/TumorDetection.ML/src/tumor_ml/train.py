from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path

import yaml
from ultralytics import YOLO

from tumor_ml.constants import TASKS, ULTRALYTICS_BASE_MODELS
from tumor_ml.dataset import data_yaml_path, dataset_root, prepare_dataset, validate_dataset, write_data_yaml
from tumor_ml.device import default_batch, resolve_device
from tumor_ml.export import export_best_weights, find_ultralytics_best, new_run_id
from tumor_ml.paths import CONFIGS_DIR, ROOT, RUNS_DIR, ensure_layout


def _latest_checkpoint(task: str, run_id: str | None = None) -> tuple[str, Path]:
    if run_id:
        last = RUNS_DIR / task / run_id / "weights" / "last.pt"
        if not last.is_file():
            raise FileNotFoundError(f"No checkpoint to resume: {last}")
        return run_id, last
    matches = sorted(
        (RUNS_DIR / task).glob("*/weights/last.pt"),
        key=lambda path: path.stat().st_mtime,
        reverse=True,
    )
    if not matches:
        raise FileNotFoundError(f"No {task} last.pt checkpoints under {RUNS_DIR / task}")
    last = matches[0]
    return last.parent.parent.name, last


@dataclass(frozen=True)
class TrainResult:
    task: str
    run_id: str
    weights: Path
    metrics: dict


def load_task_config(task: str, config_path: str | Path | None = None) -> dict:
    path = Path(config_path) if config_path else CONFIGS_DIR / f"{task}.yaml"
    if not path.is_absolute():
        path = ROOT / path
    if not path.exists():
        return {}
    return yaml.safe_load(path.read_text(encoding="utf-8")) or {}


def _resolve_weights(model: str) -> str:
    path = Path(model)
    candidates = [path]
    if not path.is_absolute():
        candidates.append(ROOT / path)
    for candidate in candidates:
        if candidate.is_file():
            return str(candidate.resolve())
    return model


def _dataset_argument(task: str) -> str:
    if task == "classification":
        return str(dataset_root(task))
    yaml_path = data_yaml_path(task)
    # Ultralytics resolves `path: .` relative to the yaml file location.
    return str(yaml_path)


def train_task(
    task: str,
    *,
    run_id: str | None = None,
    model: str | None = None,
    epochs: int | None = None,
    imgsz: int | None = None,
    batch: int | None = None,
    device: str | None = None,
    workers: int | None = None,
    smoke: bool = False,
    patience: int | None = None,
    from_archive: bool = False,
    from_brisc: bool = False,
    resume: bool = False,
    config: str | None = None,
    lr0: float | None = None,
    freeze: int | None = None,
) -> TrainResult:
    if task not in TASKS:
        raise ValueError(f"Unknown task '{task}'. Expected one of {TASKS}.")

    ensure_layout()
    cfg = load_task_config(task, config)
    resolved_device = resolve_device(device)
    resume_ckpt: Path | None = None
    if resume:
        resolved_run_id, resume_ckpt = _latest_checkpoint(task, run_id)
    else:
        resolved_run_id = run_id or new_run_id(task[:3])
    resolved_model = (str(resume_ckpt) if resume_ckpt else None) or model or cfg.get("model") or ULTRALYTICS_BASE_MODELS[task]
    if not resume:
        resolved_model = _resolve_weights(str(resolved_model))
    resolved_epochs = 1 if smoke else int(epochs or cfg.get("epochs") or 50)
    resolved_imgsz = 64 if smoke else int(imgsz or cfg.get("imgsz") or 640)
    resolved_batch = batch or default_batch(resolved_device, smoke=smoke)
    resolved_workers = 0 if smoke or resolved_device == "cpu" else int(workers if workers is not None else cfg.get("workers", 4))
    resolved_patience = 1 if smoke else int(patience or cfg.get("patience") or 20)
    resolved_lr0 = lr0 if lr0 is not None else cfg.get("lr0")
    resolved_lrf = cfg.get("lrf")
    resolved_freeze = freeze if freeze is not None else cfg.get("freeze")

    if smoke:
        prepare_dataset(task, smoke=True)
    elif from_brisc:
        prepare_dataset(task, from_brisc=True)
        errors = validate_dataset(task)
        if errors:
            raise FileNotFoundError("BRISC import failed:\n- " + "\n- ".join(errors))
    elif from_archive:
        prepare_dataset(task, from_archive=True)
        errors = validate_dataset(task)
        if errors:
            raise FileNotFoundError("Archive import failed:\n- " + "\n- ".join(errors))
    else:
        errors = validate_dataset(task)
        if errors:
            raise FileNotFoundError(
                "Dataset is not ready:\n- "
                + "\n- ".join(errors)
                + "\nRun: python TumorDetection.ML.py prepare --task "
                + task
                + " --from-archive or --from-brisc"
                + "  (or add --smoke for a synthetic dry-run)"
            )

    if task != "classification":
        write_data_yaml(task)

    yolo = YOLO(resolved_model)
    if resume:
        results = yolo.train(resume=True, device=resolved_device, workers=resolved_workers)
    else:
        train_kwargs: dict = {
            "data": _dataset_argument(task),
            "epochs": resolved_epochs,
            "imgsz": resolved_imgsz,
            "batch": resolved_batch,
            "device": resolved_device,
            "workers": resolved_workers,
            "patience": resolved_patience,
            "project": str(RUNS_DIR / task),
            "name": resolved_run_id,
            "exist_ok": True,
            "pretrained": True,
            "verbose": True,
        }
        if resolved_lr0 is not None:
            train_kwargs["lr0"] = float(resolved_lr0)
        if resolved_lrf is not None:
            train_kwargs["lrf"] = float(resolved_lrf)
        if resolved_freeze is not None:
            train_kwargs["freeze"] = int(resolved_freeze)
        results = yolo.train(**train_kwargs)

    metrics = {}
    results_dict = getattr(results, "results_dict", None) if results is not None else None
    if isinstance(results_dict, dict):
        for key, value in results_dict.items():
            try:
                metrics[str(key)] = float(value)
            except (TypeError, ValueError):
                metrics[str(key)] = str(value)

    best = find_ultralytics_best(task, resolved_run_id)
    exported = export_best_weights(
        task,
        resolved_run_id,
        best,
        metrics={
            "task": task,
            "run_id": resolved_run_id,
            "model": resolved_model,
            "epochs": resolved_epochs,
            "imgsz": resolved_imgsz,
            "batch": resolved_batch,
            "device": resolved_device,
            "lr0": resolved_lr0,
            "ultralytics": metrics,
        },
    )
    print(f"Exported: {exported}")
    return TrainResult(task=task, run_id=resolved_run_id, weights=exported, metrics=metrics)
