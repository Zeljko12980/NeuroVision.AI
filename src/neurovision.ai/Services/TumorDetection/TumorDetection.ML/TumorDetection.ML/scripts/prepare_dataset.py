#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from tumor_ml.constants import TASKS
from tumor_ml.dataset import prepare_dataset, validate_dataset


def main() -> int:
    parser = argparse.ArgumentParser(description="Create or validate YOLO dataset folders.")
    parser.add_argument("--task", default="all", choices=[*TASKS, "all"])
    parser.add_argument("--smoke", action="store_true")
    parser.add_argument("--validate", action="store_true")
    args = parser.parse_args()
    tasks = TASKS if args.task == "all" else (args.task,)

    if args.validate:
        failed = False
        for task in tasks:
            errors = validate_dataset(task)
            if errors:
                failed = True
                print(f"{task}:")
                for error in errors:
                    print(f"  - {error}")
            else:
                print(f"{task}: OK")
        return 1 if failed else 0

    for task in tasks:
        print(prepare_dataset(task, smoke=args.smoke))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
