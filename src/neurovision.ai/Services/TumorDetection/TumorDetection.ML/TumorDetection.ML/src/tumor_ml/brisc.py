from __future__ import annotations

from pathlib import Path

import cv2
import numpy as np

from tumor_ml.constants import CLASS_NAMES, normalize_class_name
from tumor_ml.dataset import (
    IMAGE_EXTENSIONS,
    _clear_classification_files,
    _clear_yolo_files,
    _link_or_copy,
    create_folder_layout,
    write_data_yaml,
)
from tumor_ml.paths import BRISC_DIR

BRISC_SPLIT_MAP = (("train", "train"), ("test", "val"), ("valid", "val"), ("val", "val"))
BRISC_CLASS_CODES = {
    "gl": "glioma",
    "me": "meningioma",
    "pi": "pituitary",
    "no": "notumor",
}


def resolve_brisc_root(source: Path | None = None) -> Path:
    path = (source or BRISC_DIR).resolve()
    candidates = [path, path / "brisc2025"]
    if path.is_dir():
        candidates.extend(sorted(child for child in path.iterdir() if child.is_dir()))
    for candidate in candidates:
        if (candidate / "classification_task").is_dir() or (candidate / "segmentation_task").is_dir():
            return candidate
    raise FileNotFoundError(
        f"BRISC folder not found at {path}. Expected classification_task/ and segmentation_task/."
    )


def _class_from_stem(stem: str) -> str | None:
    parts = stem.lower().split("_")
    for part in parts:
        name = BRISC_CLASS_CODES.get(part)
        if name:
            return name
    return None


def _mask_to_yolo_lines(mask_path: Path, class_id: int, *, segmentation: bool) -> list[str]:
    mask = cv2.imread(str(mask_path), cv2.IMREAD_GRAYSCALE)
    if mask is None:
        return []
    height, width = mask.shape[:2]
    binary = np.where(mask > 0, 255, 0).astype(np.uint8)
    contours, _ = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    lines: list[str] = []
    for contour in contours:
        if cv2.contourArea(contour) < 8:
            continue
        if segmentation:
            epsilon = max(0.5, 0.002 * cv2.arcLength(contour, True))
            approx = cv2.approxPolyDP(contour, epsilon, True)
            if len(approx) < 3:
                approx = contour
            points = approx.reshape(-1, 2)
            if len(points) < 3:
                continue
            coords: list[str] = []
            for x, y in points:
                coords.append(f"{x / max(width, 1):.6f}")
                coords.append(f"{y / max(height, 1):.6f}")
            lines.append(" ".join([str(class_id), *coords]))
            continue
        x, y, box_w, box_h = cv2.boundingRect(contour)
        xc = (x + box_w / 2) / max(width, 1)
        yc = (y + box_h / 2) / max(height, 1)
        lines.append(
            f"{class_id} {xc:.6f} {yc:.6f} {box_w / max(width, 1):.6f} {box_h / max(height, 1):.6f}"
        )
    return lines


def _iter_split_dirs(task_root: Path) -> list[tuple[Path, str]]:
    found: list[tuple[Path, str]] = []
    seen: set[str] = set()
    for source_name, yolo_split in BRISC_SPLIT_MAP:
        split_dir = task_root / source_name
        if not split_dir.is_dir() or yolo_split in seen:
            continue
        seen.add(yolo_split)
        found.append((split_dir, yolo_split))
    return found


def import_brisc_dataset(task: str, brisc_dir: Path | None = None) -> Path:
    if task not in ("detection", "classification", "segmentation"):
        raise ValueError(f"Unknown task '{task}'.")

    source = resolve_brisc_root(brisc_dir)
    classification_root = source / "classification_task"
    segmentation_root = source / "segmentation_task"
    root = create_folder_layout(task)
    if task == "classification":
        _clear_classification_files(root)
    else:
        _clear_yolo_files(root)

    imported = 0
    skipped = 0
    seen_splits: set[str] = set()

    if task == "classification":
        if not classification_root.is_dir():
            raise FileNotFoundError(f"Missing {classification_root}")
        for split_dir, yolo_split in _iter_split_dirs(classification_root):
            seen_splits.add(yolo_split)
            for class_dir in sorted(split_dir.iterdir()):
                if not class_dir.is_dir():
                    continue
                class_name = normalize_class_name(class_dir.name)
                if class_name not in CLASS_NAMES:
                    continue
                for image_path in class_dir.iterdir():
                    if not image_path.is_file() or image_path.suffix.lower() not in IMAGE_EXTENSIONS:
                        continue
                    dest = root / yolo_split / class_name / f"{image_path.stem}{image_path.suffix.lower()}"
                    _link_or_copy(image_path, dest)
                    imported += 1
    else:
        if not segmentation_root.is_dir():
            raise FileNotFoundError(f"Missing {segmentation_root}")
        for split_dir, yolo_split in _iter_split_dirs(segmentation_root):
            images_dir = split_dir / "images"
            masks_dir = split_dir / "masks"
            if not images_dir.is_dir():
                continue
            seen_splits.add(yolo_split)
            for image_path in images_dir.iterdir():
                if not image_path.is_file() or image_path.suffix.lower() not in IMAGE_EXTENSIONS:
                    continue
                class_name = _class_from_stem(image_path.stem)
                if class_name not in CLASS_NAMES:
                    skipped += 1
                    continue
                class_id = CLASS_NAMES.index(class_name)
                dest_image = root / "images" / yolo_split / f"{image_path.stem}{image_path.suffix.lower()}"
                dest_label = root / "labels" / yolo_split / f"{image_path.stem}.txt"
                mask_path = masks_dir / f"{image_path.stem}.png"
                if not mask_path.is_file():
                    mask_path = masks_dir / f"{image_path.stem}{image_path.suffix}"
                _link_or_copy(image_path, dest_image)
                lines = _mask_to_yolo_lines(mask_path, class_id, segmentation=(task == "segmentation"))
                dest_label.parent.mkdir(parents=True, exist_ok=True)
                dest_label.write_text(("\n".join(lines) + "\n") if lines else "", encoding="utf-8")
                imported += 1

        if classification_root.is_dir():
            for split_dir, yolo_split in _iter_split_dirs(classification_root):
                negatives = split_dir / "no_tumor"
                if not negatives.is_dir():
                    negatives = split_dir / "notumor"
                if not negatives.is_dir():
                    continue
                seen_splits.add(yolo_split)
                for image_path in negatives.iterdir():
                    if not image_path.is_file() or image_path.suffix.lower() not in IMAGE_EXTENSIONS:
                        continue
                    dest_image = root / "images" / yolo_split / f"{image_path.stem}{image_path.suffix.lower()}"
                    dest_label = root / "labels" / yolo_split / f"{image_path.stem}.txt"
                    if dest_image.exists() or dest_image.is_symlink():
                        continue
                    _link_or_copy(image_path, dest_image)
                    dest_label.parent.mkdir(parents=True, exist_ok=True)
                    dest_label.write_text("", encoding="utf-8")
                    imported += 1

    if imported == 0:
        raise FileNotFoundError(f"No BRISC images imported from {source}")
    if "train" not in seen_splits or "val" not in seen_splits:
        raise FileNotFoundError(f"BRISC must contain train and test/val splits under {source}")

    if task != "classification":
        write_data_yaml(task)
    print(f"Imported {imported} {task} images from BRISC {source} -> {root}")
    if skipped:
        print(f"Skipped {skipped} files without a recognized class code")
    return root
