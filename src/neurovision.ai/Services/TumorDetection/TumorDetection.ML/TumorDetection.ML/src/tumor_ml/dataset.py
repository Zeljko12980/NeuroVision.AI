from __future__ import annotations

import os
import random
import shutil
from pathlib import Path

import yaml
from PIL import Image

from tumor_ml.constants import CLASS_NAMES, normalize_class_name
from tumor_ml.paths import ARCHIVE_DIR, DATASETS_DIR


def dataset_root(task: str) -> Path:
    return DATASETS_DIR / task


def data_yaml_path(task: str) -> Path:
    return dataset_root(task) / "data.yaml"


def write_data_yaml(task: str) -> Path:
    root = dataset_root(task).resolve()
    payload = {
        "path": str(root),
        "train": "images/train",
        "val": "images/val",
        "names": {index: name for index, name in enumerate(CLASS_NAMES)},
        "nc": len(CLASS_NAMES),
    }
    path = data_yaml_path(task)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(yaml.safe_dump(payload, sort_keys=False), encoding="utf-8")
    return path


def _make_image(path: Path, size: int, seed: int, hue: int) -> None:
    random.seed(seed)
    img = Image.new("RGB", (size, size), color=(hue, 40, 80))
    pixels = img.load()
    for _ in range(size // 2):
        x = random.randint(0, size - 1)
        y = random.randint(0, size - 1)
        pixels[x, y] = (hue, random.randint(80, 180), random.randint(80, 180))
    path.parent.mkdir(parents=True, exist_ok=True)
    img.save(path)


def _write_detect_label(path: Path, class_id: int) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(f"{class_id} 0.50 0.50 0.35 0.35\n", encoding="utf-8")


def _write_seg_label(path: Path, class_id: int) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        f"{class_id} 0.35 0.35 0.65 0.35 0.65 0.65 0.35 0.65\n",
        encoding="utf-8",
    )


def create_folder_layout(task: str) -> Path:
    root = dataset_root(task)
    if task == "classification":
        for split in ("train", "val"):
            for name in CLASS_NAMES:
                (root / split / name).mkdir(parents=True, exist_ok=True)
    else:
        for split in ("train", "val"):
            (root / "images" / split).mkdir(parents=True, exist_ok=True)
            (root / "labels" / split).mkdir(parents=True, exist_ok=True)
        write_data_yaml(task)
    return root


def create_smoke_dataset(task: str, images_per_class: int = 4, image_size: int = 64) -> Path:
    root = create_folder_layout(task)
    for class_id, name in enumerate(CLASS_NAMES):
        for split_index, split in enumerate(("train", "val")):
            count = images_per_class if split == "train" else max(1, images_per_class // 2)
            for i in range(count):
                stem = f"{name}_{split}_{i:03d}"
                if task == "classification":
                    image_path = root / split / name / f"{stem}.jpg"
                    _make_image(image_path, image_size, seed=class_id * 100 + i, hue=40 + class_id * 40)
                    continue

                image_path = root / "images" / split / f"{stem}.jpg"
                label_path = root / "labels" / split / f"{stem}.txt"
                _make_image(image_path, image_size, seed=class_id * 100 + i + split_index, hue=40 + class_id * 40)
                if task == "segmentation":
                    _write_seg_label(label_path, class_id)
                else:
                    _write_detect_label(label_path, class_id)
    return root


def validate_dataset(task: str) -> list[str]:
    root = dataset_root(task)
    errors: list[str] = []
    if not root.exists():
        return [f"Dataset folder missing: {root}"]

    if task == "classification":
        for split in ("train", "val"):
            for name in CLASS_NAMES:
                folder = root / split / name
                if not folder.exists() or not any(folder.glob("*.*")):
                    errors.append(f"Missing {split}/{name} images in {folder}")
        return errors

    yaml_path = data_yaml_path(task)
    if not yaml_path.exists():
        errors.append(f"Missing {yaml_path}")

    for split in ("train", "val"):
        images = list((root / "images" / split).glob("*.*"))
        labels = list((root / "labels" / split).glob("*.txt"))
        if not images:
            errors.append(f"No images in {root / 'images' / split}")
        if not labels:
            errors.append(f"No labels in {root / 'labels' / split}")
    return errors


IMAGE_EXTENSIONS = {".jpg", ".jpeg", ".png", ".bmp", ".webp", ".tif", ".tiff"}
ARCHIVE_SPLITS = (("Train", "train"), ("Val", "val"))


def _link_or_copy(src: Path, dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    if dest.exists() or dest.is_symlink():
        dest.unlink()
    try:
        os.link(src, dest)
    except OSError:
        shutil.copy2(src, dest)


def _bbox_to_polygon(class_id: int, xc: float, yc: float, w: float, h: float) -> str:
    x1 = max(0.0, min(1.0, xc - w / 2))
    y1 = max(0.0, min(1.0, yc - h / 2))
    x2 = max(0.0, min(1.0, xc + w / 2))
    y2 = max(0.0, min(1.0, yc + h / 2))
    coords = (x1, y1, x2, y1, x2, y2, x1, y2)
    return " ".join([str(class_id), *(f"{value:.6f}" for value in coords)])


def _rewrite_label(src: Path, dest: Path, class_id: int, *, segmentation: bool = False) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    if not src.exists():
        dest.write_text("", encoding="utf-8")
        return
    lines: list[str] = []
    for line in src.read_text(encoding="utf-8").splitlines():
        parts = line.split()
        if len(parts) < 5:
            continue
        geometry = parts[1:]
        if segmentation and len(geometry) == 4:
            xc, yc, w, h = (float(value) for value in geometry)
            lines.append(_bbox_to_polygon(class_id, xc, yc, w, h))
        else:
            lines.append(" ".join([str(class_id), *geometry]))
    dest.write_text(("\n".join(lines) + "\n") if lines else "", encoding="utf-8")


def _clear_label_caches(root: Path) -> None:
    for cache in root.rglob("*.cache"):
        cache.unlink(missing_ok=True)


def _clear_yolo_files(root: Path) -> None:
    for split in ("train", "val"):
        for kind in ("images", "labels"):
            folder = root / kind / split
            if folder.exists():
                shutil.rmtree(folder)
            folder.mkdir(parents=True, exist_ok=True)
    _clear_label_caches(root)


def _clear_classification_files(root: Path) -> None:
    for split in ("train", "val"):
        for name in CLASS_NAMES:
            folder = root / split / name
            if folder.exists():
                shutil.rmtree(folder)
            folder.mkdir(parents=True, exist_ok=True)
    _clear_label_caches(root)


def _archive_class_dirs(split_dir: Path) -> list[tuple[Path, str, int]]:
    found: list[tuple[Path, str, int]] = []
    if not split_dir.is_dir():
        return found
    for folder in sorted(split_dir.iterdir()):
        if not folder.is_dir():
            continue
        name = normalize_class_name(folder.name)
        if name not in CLASS_NAMES:
            continue
        found.append((folder, name, CLASS_NAMES.index(name)))
    return found


def import_archive_dataset(task: str = "detection", archive_dir: Path | None = None) -> Path:
    if task not in ("detection", "classification", "segmentation"):
        raise ValueError(f"Unknown task '{task}'.")

    source = (archive_dir or ARCHIVE_DIR).resolve()
    if not source.is_dir():
        raise FileNotFoundError(f"Archive folder not found: {source}")

    root = create_folder_layout(task)
    if task == "classification":
        _clear_classification_files(root)
    else:
        _clear_yolo_files(root)

    imported = 0
    skipped = 0
    seen_splits: set[str] = set()
    seen_source_dirs: set[Path] = set()
    for archive_split, yolo_split in ARCHIVE_SPLITS:
        split_dir = source / archive_split
        if not split_dir.is_dir():
            continue
        resolved = split_dir.resolve()
        if resolved in seen_source_dirs:
            continue
        seen_source_dirs.add(resolved)
        classes = _archive_class_dirs(split_dir)
        if not classes:
            continue
        seen_splits.add(yolo_split)
        for class_dir, class_name, class_id in classes:
            images_dir = class_dir / "images"
            labels_dir = class_dir / "labels"
            if not images_dir.is_dir():
                images_dir = class_dir
            image_files = [
                path
                for path in images_dir.iterdir()
                if path.is_file() and path.suffix.lower() in IMAGE_EXTENSIONS
            ]
            for image_path in image_files:
                dest_stem = f"{class_name}_{image_path.stem}"
                if task == "classification":
                    dest_image = root / yolo_split / class_name / f"{dest_stem}{image_path.suffix.lower()}"
                    _link_or_copy(image_path, dest_image)
                else:
                    dest_image = root / "images" / yolo_split / f"{dest_stem}{image_path.suffix.lower()}"
                    dest_label = root / "labels" / yolo_split / f"{dest_stem}.txt"
                    label_path = labels_dir / f"{image_path.stem}.txt"
                    _link_or_copy(image_path, dest_image)
                    _rewrite_label(label_path, dest_label, class_id, segmentation=(task == "segmentation"))
                imported += 1
            if labels_dir.is_dir():
                skipped += max(0, len(list(labels_dir.glob("*.txt"))) - len(image_files))

    if imported == 0:
        raise FileNotFoundError(
            f"No MRI images found under {source}. Expected Train/Val folders with class/images + class/labels."
        )
    if "train" not in seen_splits or "val" not in seen_splits:
        raise FileNotFoundError(f"Archive must contain both Train and Val class folders under {source}.")

    if task != "classification":
        write_data_yaml(task)
    print(f"Imported {imported} {task} images from {source} -> {root}")
    if skipped and task != "classification":
        print(f"Skipped {skipped} labels without a matching image")
    return root


def prepare_dataset(task: str, smoke: bool = False, from_archive: bool = False, from_brisc: bool = False) -> Path:
    if smoke:
        return create_smoke_dataset(task)
    if from_brisc:
        from tumor_ml.brisc import import_brisc_dataset

        return import_brisc_dataset(task)
    if from_archive:
        return import_archive_dataset(task)
    return create_folder_layout(task)
