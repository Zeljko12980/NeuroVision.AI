#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from tumor_ml.constants import TASKS
from tumor_ml.train import train_task


def main() -> int:
    parser = argparse.ArgumentParser(description="Train YOLO11 tumor models.")
    parser.add_argument("--task", default="all", choices=[*TASKS, "all"])
    parser.add_argument("--run-id")
    parser.add_argument("--model")
    parser.add_argument("--epochs", type=int)
    parser.add_argument("--imgsz", type=int)
    parser.add_argument("--batch", type=int)
    parser.add_argument("--device", default="auto")
    parser.add_argument("--smoke", action="store_true")
    args = parser.parse_args()
    tasks = TASKS if args.task == "all" else (args.task,)
    for task in tasks:
        train_task(
            task,
            run_id=args.run_id,
            model=args.model,
            epochs=args.epochs,
            imgsz=args.imgsz,
            batch=args.batch,
            device=args.device,
            smoke=args.smoke,
        )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
