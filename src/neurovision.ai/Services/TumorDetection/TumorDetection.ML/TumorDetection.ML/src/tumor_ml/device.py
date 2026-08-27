from __future__ import annotations

import torch


def resolve_device(requested: str | None = None) -> str:
    if requested and requested.lower() not in {"", "auto"}:
        return requested
    if torch.cuda.is_available():
        return "0"
    return "cpu"


def default_batch(device: str, smoke: bool = False) -> int:
    if smoke:
        return 2
    return 8 if device != "cpu" else 4
