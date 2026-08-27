#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent
sys.path.insert(0, str(ROOT / "src"))

from tumor_ml.constants import TASKS
from tumor_ml.dataset import prepare_dataset, validate_dataset
from tumor_ml.infer import run_pipeline
from tumor_ml.paths import ensure_layout
from tumor_ml.pretrained import download_pretrained_models
from tumor_ml.train import train_task


def _tasks(value: str) -> tuple[str, ...]:
    if value == "all":
        return TASKS
    if value not in TASKS:
        raise argparse.ArgumentTypeError(f"Unknown task '{value}'. Use detection, classification, segmentation, or all.")
    return (value,)


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        prog="TumorDetection.ML",
        description="Train, export, and run NeuroVision brain-tumor YOLO models.",
    )
    sub = parser.add_subparsers(dest="command", required=True)

    download = sub.add_parser("download", help="Download Hugging Face pretrained weights into artifacts/")
    download.add_argument("--task", default="all", type=_tasks)

    prepare = sub.add_parser("prepare", help="Create YOLO dataset folders (and optional smoke images)")
    prepare.add_argument("--task", default="all", type=_tasks)
    prepare.add_argument("--smoke", action="store_true", help="Generate a tiny synthetic dataset for a dry-run")
    prepare.add_argument("--from-archive", action="store_true", help="Import MRI images from the local archive/ folder")
    prepare.add_argument("--from-brisc", action="store_true", help="Import BRISC 2025 (classification + mask) into YOLO folders")

    validate = sub.add_parser("validate-data", help="Check that a dataset is ready for training")
    validate.add_argument("--task", default="all", type=_tasks)

    train = sub.add_parser("train", help="Train detection, classification, and/or segmentation")
    train.add_argument("--task", default="all", type=_tasks)
    train.add_argument("--run-id")
    train.add_argument("--model", help="Base weights, e.g. yolo11n.pt or a previous artifacts/.../best.pt")
    train.add_argument("--epochs", type=int)
    train.add_argument("--imgsz", type=int)
    train.add_argument("--batch", type=int)
    train.add_argument("--device", default="auto")
    train.add_argument("--workers", type=int)
    train.add_argument("--patience", type=int)
    train.add_argument("--lr0", type=float, help="Initial learning rate (use ~0.001 when fine-tuning best.pt)")
    train.add_argument("--freeze", type=int, help="Freeze the first N backbone layers")
    train.add_argument("--config", help="Optional yaml, e.g. configs/detection_finetune.yaml")
    train.add_argument("--smoke", action="store_true", help="1-epoch CPU-friendly dry run on synthetic images")
    train.add_argument("--from-archive", action="store_true", help="Import archive/ MRI dataset before training")
    train.add_argument("--from-brisc", action="store_true", help="Import BRISC 2025 dataset before training")
    train.add_argument("--resume", action="store_true", help="Continue the latest (or --run-id) interrupted run")

    infer = sub.add_parser("infer", help="Run the detection + classification + segmentation pipeline")
    infer.add_argument("image")
    infer.add_argument("--detection-run")
    infer.add_argument("--classification-run")
    infer.add_argument("--segmentation-run")

    return parser


def main(argv: list[str] | None = None) -> int:
    ensure_layout()
    args = build_parser().parse_args(argv)

    if args.command == "download":
        download_pretrained_models(args.task)
        return 0

    if args.command == "prepare":
        for task in args.task:
            path = prepare_dataset(
                task,
                smoke=args.smoke,
                from_archive=args.from_archive,
                from_brisc=getattr(args, "from_brisc", False),
            )
            print(f"{task}: {path}")
        return 0

    if args.command == "validate-data":
        failed = False
        for task in args.task:
            errors = validate_dataset(task)
            if errors:
                failed = True
                print(f"{task} is not ready:")
                for error in errors:
                    print(f"  - {error}")
            else:
                print(f"{task}: OK")
        return 1 if failed else 0

    if args.command == "train":
        for task in args.task:
            result = train_task(
                task,
                run_id=args.run_id,
                model=args.model,
                epochs=args.epochs,
                imgsz=args.imgsz,
                batch=args.batch,
                device=args.device,
                workers=args.workers,
                smoke=args.smoke,
                patience=args.patience,
                from_archive=args.from_archive,
                from_brisc=args.from_brisc,
                resume=args.resume,
                config=args.config,
                lr0=args.lr0,
                freeze=args.freeze,
            )
            print(f"{result.task} run {result.run_id} -> {result.weights}")
        return 0

    if args.command == "infer":
        run_pipeline(
            args.image,
            detection_run=args.detection_run,
            classification_run=args.classification_run,
            segmentation_run=args.segmentation_run,
        )
        return 0

    return 1


if __name__ == "__main__":
    raise SystemExit(main())
