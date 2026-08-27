from __future__ import annotations

from pathlib import Path

PACKAGE_DIR = Path(__file__).resolve().parent
SRC_DIR = PACKAGE_DIR.parent
ROOT = SRC_DIR.parent

CONFIGS_DIR = ROOT / "configs"
DATASETS_DIR = ROOT / "datasets"
ARCHIVE_DIR = ROOT / "archive"
BRISC_DIR = ROOT / "brisc"
ARTIFACTS_DIR = ROOT / "artifacts"
OUTPUTS_DIR = ROOT / "outputs"
RUNS_DIR = ROOT / "runs"
HF_CACHE_DIR = ROOT / ".hf-cache"


def ensure_layout() -> None:
    for path in (ARTIFACTS_DIR, OUTPUTS_DIR, RUNS_DIR, DATASETS_DIR, CONFIGS_DIR):
        path.mkdir(parents=True, exist_ok=True)
    for task in ("detection", "classification", "segmentation"):
        (ARTIFACTS_DIR / task).mkdir(parents=True, exist_ok=True)
        (DATASETS_DIR / task).mkdir(parents=True, exist_ok=True)
