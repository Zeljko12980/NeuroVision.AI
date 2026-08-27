NeuroVision tumor ML (YOLO11)

The project folder is:
  Services/TumorDetection/TumorDetection.ML/TumorDetection.ML

Windows notes
  PowerShell blocks Activate.ps1. Do not activate the venv.
  Torch also fails if the venv sits in this nested repo (260-char path limit).
  setup.cmd puts the venv at C:\nv-ml-venv and links .venv to it.

Setup
  cd Services\TumorDetection\TumorDetection.ML\TumorDetection.ML
  setup.cmd

  or:
  python -m venv C:\nv-ml-venv
  C:\nv-ml-venv\Scripts\python.exe -m pip install -r requirements.txt

Download pretrained weights used by the API
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py download

Prepare dataset folders
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py prepare
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py prepare --smoke

Train (exports artifacts/<task>/<run-id>/weights/best.pt)
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py train --task detection
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py train --task all --smoke
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py train --task segmentation --epochs 80 --device 0

Infer (C# API calls scripts/test_pipeline.py)
  C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py infer path\to\scan.jpg
  C:\nv-ml-venv\Scripts\python.exe scripts\test_pipeline.py path\to\scan.jpg --detection-run hf_yolo11_brain_mri

Artifact layout expected by TumorDetectionService
  artifacts/detection/<run>/weights/best.pt
  artifacts/classification/<run>/weights/best.pt
  artifacts/segmentation/<run>/weights/best.pt
