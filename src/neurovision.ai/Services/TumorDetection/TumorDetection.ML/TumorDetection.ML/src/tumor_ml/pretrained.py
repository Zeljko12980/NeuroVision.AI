from __future__ import annotations

from pathlib import Path

from huggingface_hub import hf_hub_download, list_repo_files

from tumor_ml.constants import PRETRAINED_HF_MODELS, TASKS
from tumor_ml.paths import ARTIFACTS_DIR, HF_CACHE_DIR, ensure_layout


def _pick_weights_file(repo_id: str) -> str:
    files = [name for name in list_repo_files(repo_id) if name.lower().endswith(".pt")]
    if not files:
        raise FileNotFoundError(f"No .pt weights found in Hugging Face repo {repo_id}")

    preferred = ("best.pt", "last.pt", "weights/best.pt", "weights/last.pt")
    for name in preferred:
        if name in files:
            return name
    return files[0]


def download_pretrained_models(tasks: tuple[str, ...] | None = None) -> dict[str, Path]:
    ensure_layout()
    selected = tasks or TASKS
    downloaded: dict[str, Path] = {}

    for task in selected:
        spec = PRETRAINED_HF_MODELS[task]
        dest_dir = ARTIFACTS_DIR / task / spec["run_id"] / "weights"
        dest_dir.mkdir(parents=True, exist_ok=True)
        dest = dest_dir / "best.pt"
        if dest.exists():
            downloaded[task] = dest
            continue

        filename = _pick_weights_file(spec["repo"])
        cached = hf_hub_download(
            repo_id=spec["repo"],
            filename=filename,
            cache_dir=str(HF_CACHE_DIR),
        )
        dest.write_bytes(Path(cached).read_bytes())
        downloaded[task] = dest
        print(f"{task}: {dest}")

    return downloaded
