from __future__ import annotations

import json
import shutil
from datetime import datetime, timezone
from pathlib import Path

from tumor_ml.constants import TASKS
from tumor_ml.paths import ARTIFACTS_DIR, RUNS_DIR


def new_run_id(prefix: str = "train") -> str:
    stamp = datetime.now(timezone.utc).strftime("%Y%m%d_%H%M%S")
    return f"{prefix}_{stamp}"


def artifact_weights_dir(task: str, run_id: str) -> Path:
    if task not in TASKS:
        raise ValueError(f"Unknown task '{task}'. Expected one of {TASKS}.")
    return ARTIFACTS_DIR / task / run_id / "weights"


def export_best_weights(task: str, run_id: str, source_best: Path, metrics: dict | None = None) -> Path:
    dest_dir = artifact_weights_dir(task, run_id)
    dest_dir.mkdir(parents=True, exist_ok=True)
    dest = dest_dir / "best.pt"
    shutil.copy2(source_best, dest)

    if metrics is not None:
        (dest_dir.parent / "metrics.json").write_text(
            json.dumps(metrics, indent=2),
            encoding="utf-8",
        )

    return dest


def find_ultralytics_best(task: str, run_id: str) -> Path:
    candidates = [
        RUNS_DIR / task / run_id / "weights" / "best.pt",
        RUNS_DIR / "detect" / run_id / "weights" / "best.pt",
        RUNS_DIR / "classify" / run_id / "weights" / "best.pt",
        RUNS_DIR / "segment" / run_id / "weights" / "best.pt",
    ]
    for path in candidates:
        if path.is_file():
            return path
    raise FileNotFoundError(
        f"Could not find Ultralytics best.pt for task={task} run_id={run_id}. Looked in: "
        + ", ".join(str(p) for p in candidates)
    )
