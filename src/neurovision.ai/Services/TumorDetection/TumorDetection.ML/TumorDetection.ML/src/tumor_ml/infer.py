from __future__ import annotations

import json
from pathlib import Path

import cv2
import numpy as np
from ultralytics import YOLO

from tumor_ml.constants import CLASS_NAMES, normalize_class_name
from tumor_ml.paths import ARTIFACTS_DIR, OUTPUTS_DIR, ensure_layout


def _latest_run(task: str) -> str | None:
    root = ARTIFACTS_DIR / task
    if not root.exists():
        return None
    runs = [p for p in root.iterdir() if (p / "weights" / "best.pt").is_file()]
    if not runs:
        return None
    return sorted(runs, key=lambda p: p.stat().st_mtime)[-1].name


def resolve_weights(task: str, run_id: str | None) -> Path:
    selected = run_id or _latest_run(task)
    if not selected:
        raise FileNotFoundError(
            f"No {task} weights under {ARTIFACTS_DIR / task}. "
            "Download pretrained models or train first."
        )
    path = ARTIFACTS_DIR / task / selected / "weights" / "best.pt"
    if not path.is_file():
        raise FileNotFoundError(f"Missing weights: {path}")
    return path


def _xyxy_to_yolo(x1: float, y1: float, x2: float, y2: float, width: int, height: int) -> list[float]:
    xc = ((x1 + x2) / 2) / max(width, 1)
    yc = ((y1 + y2) / 2) / max(height, 1)
    w = (x2 - x1) / max(width, 1)
    h = (y2 - y1) / max(height, 1)
    return [round(xc, 6), round(yc, 6), round(w, 6), round(h, 6)]


def run_pipeline(
    image_path: str | Path,
    *,
    detection_run: str | None = None,
    classification_run: str | None = None,
    segmentation_run: str | None = None,
) -> Path:
    ensure_layout()
    source = Path(image_path).resolve()
    if not source.is_file():
        raise FileNotFoundError(f"Image not found: {source}")

    image = cv2.imread(str(source))
    if image is None:
        raise ValueError(f"Could not read image: {source}")
    height, width = image.shape[:2]

    out_dir = OUTPUTS_DIR / source.stem
    det_dir = out_dir / "detection"
    seg_dir = out_dir / "segmentation"
    det_dir.mkdir(parents=True, exist_ok=True)
    seg_dir.mkdir(parents=True, exist_ok=True)

    detection_model = YOLO(str(resolve_weights("detection", detection_run)))
    classification_model = YOLO(str(resolve_weights("classification", classification_run)))
    segmentation_model = YOLO(str(resolve_weights("segmentation", segmentation_run)))

    det_result = detection_model.predict(source=str(source), verbose=False)[0]
    cls_result = classification_model.predict(source=str(source), verbose=False)[0]
    seg_result = segmentation_model.predict(source=str(source), verbose=False)[0]

    boxes = []
    if det_result.boxes is not None:
        for box in det_result.boxes:
            xyxy = box.xyxy[0].tolist()
            class_id = int(box.cls[0])
            name = det_result.names.get(class_id, CLASS_NAMES[class_id] if class_id < len(CLASS_NAMES) else "glioma")
            boxes.append(
                {
                    "class": normalize_class_name(str(name)),
                    "confidence": float(box.conf[0]),
                    "bbox": _xyxy_to_yolo(xyxy[0], xyxy[1], xyxy[2], xyxy[3], width, height),
                }
            )

    annotated_det = det_result.plot()
    cv2.imwrite(str(det_dir / source.name), annotated_det)

    probabilities: dict[str, float] = {}
    predicted_class = "glioma"
    predicted_conf = 0.0
    if cls_result.probs is not None:
        top = int(cls_result.probs.top1)
        predicted_class = normalize_class_name(str(cls_result.names.get(top, "glioma")))
        predicted_conf = float(cls_result.probs.top1conf)
        for index, name in cls_result.names.items():
            probabilities[normalize_class_name(str(name))] = float(cls_result.probs.data[int(index)])

    tumor_area_ratio = 0.0
    mask_path = None
    if seg_result.masks is not None and len(seg_result.masks.data) > 0:
        combined = np.zeros((height, width), dtype=np.uint8)
        for mask in seg_result.masks.data:
            resized = cv2.resize(mask.cpu().numpy(), (width, height), interpolation=cv2.INTER_NEAREST)
            combined[resized > 0.5] = 255
        tumor_area_ratio = float(np.count_nonzero(combined) / combined.size)
        mask_path = str((seg_dir / "mask.png").resolve())
        cv2.imwrite(mask_path, combined)
        annotated_seg = seg_result.plot()
        cv2.imwrite(str(seg_dir / source.name), annotated_seg)

    report = {
        "image": str(source),
        "detection": {"boxes": boxes},
        "classification": {
            "class": predicted_class,
            "confidence": predicted_conf,
            "probabilities": probabilities,
        },
        "segmentation": {
            "tumor_area_ratio": tumor_area_ratio,
            "mask_path": mask_path,
        },
    }

    report_path = out_dir / "report.json"
    report_path.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(f"Report: {report_path.resolve()}")
    return report_path
