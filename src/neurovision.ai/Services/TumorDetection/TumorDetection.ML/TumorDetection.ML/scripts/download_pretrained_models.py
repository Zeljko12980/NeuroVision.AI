#!/usr/bin/env python3
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from tumor_ml.pretrained import download_pretrained_models

if __name__ == "__main__":
    download_pretrained_models()
