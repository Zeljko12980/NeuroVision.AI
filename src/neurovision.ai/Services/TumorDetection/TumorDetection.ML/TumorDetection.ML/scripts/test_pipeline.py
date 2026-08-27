#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from tumor_ml.infer import run_pipeline


def main() -> int:
    parser = argparse.ArgumentParser(description="Run the NeuroVision tumor analysis pipeline.")
    parser.add_argument("image")
    parser.add_argument("--detection-run")
    parser.add_argument("--classification-run")
    parser.add_argument("--segmentation-run")
    args = parser.parse_args()
    run_pipeline(
        args.image,
        detection_run=args.detection_run,
        classification_run=args.classification_run,
        segmentation_run=args.segmentation_run,
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
