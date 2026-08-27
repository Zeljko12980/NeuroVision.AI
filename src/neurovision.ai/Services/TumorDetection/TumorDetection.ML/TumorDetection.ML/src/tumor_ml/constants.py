from __future__ import annotations

CLASS_NAMES: tuple[str, ...] = ("glioma", "meningioma", "pituitary", "notumor")

CLASS_ALIASES = {
    "glioma": "glioma",
    "glioma tumor": "glioma",
    "meningioma": "meningioma",
    "meningioma tumor": "meningioma",
    "pituitary": "pituitary",
    "pituitary tumor": "pituitary",
    "no tumor": "notumor",
    "no_tumor": "notumor",
    "notumor": "notumor",
    "healthy": "notumor",
}

TASKS = ("detection", "classification", "segmentation")

ULTRALYTICS_BASE_MODELS = {
    "detection": "yolo11n.pt",
    "classification": "yolo11n-cls.pt",
    "segmentation": "yolo11n-seg.pt",
}

PRETRAINED_HF_MODELS = {
    "detection": {
        "repo": "findingmllll/yolov11-brain-tumor-mri",
        "run_id": "hf_yolo11_brain_mri",
    },
    "classification": {
        "repo": "Lomuto/yolov11-brain-tumor-classification",
        "run_id": "hf_yolov11_brain_cls",
    },
    "segmentation": {
        "repo": "sajjadhadi/YOLOv11-Tumor-Detection",
        "run_id": "hf_yolo11_tumor_seg",
    },
}


def normalize_class_name(name: str) -> str:
    key = " ".join(name.lower().replace("_", " ").replace("-", " ").split())
    return CLASS_ALIASES.get(key, key)
