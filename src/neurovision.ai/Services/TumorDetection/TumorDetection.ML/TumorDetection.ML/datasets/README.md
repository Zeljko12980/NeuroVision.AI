Put MRI images here. `python TumorDetection.ML.py prepare` creates the folders.

Detection / segmentation (YOLO):
  datasets/detection/images/train  + labels/train
  datasets/detection/images/val    + labels/val
  Same layout under datasets/segmentation (polygon labels).

Classification:
  datasets/classification/train/{glioma,meningioma,pituitary,notumor}
  datasets/classification/val/{glioma,meningioma,pituitary,notumor}

Class ids: 0 glioma, 1 meningioma, 2 pituitary, 3 notumor.
