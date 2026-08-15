import { useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

import Button from "../../components/ui/button/Button";
import { fetchBlobUrl, getAnalysisImagePath } from "../../api/api";
import type { DetectionFindingResponse } from "../../features/tumorDetection/tumorDetection.types";

type ImageView = "annotated" | "original" | "detection" | "segmentation";

interface AnalysisImageViewerProps {
    analysisId: string;
    detections: DetectionFindingResponse[];
    hasAnnotatedImage: boolean;
    hasDetectionImage: boolean;
    hasSegmentationImage: boolean;
    translationKey: "doctor" | "patient";
}

const VIEW_PRIORITY: ImageView[] = ["detection", "segmentation", "annotated", "original"];

function pickInitialView(
    hasDetectionImage: boolean,
    hasSegmentationImage: boolean,
    hasAnnotatedImage: boolean,
    detectionCount: number
): ImageView {
    if (detectionCount > 0 && !hasDetectionImage) return "original";
    if (hasDetectionImage) return "detection";
    if (hasSegmentationImage) return "segmentation";
    if (hasAnnotatedImage) return "annotated";
    return "original";
}

export default function AnalysisImageViewer({
    analysisId,
    detections,
    hasAnnotatedImage,
    hasDetectionImage,
    hasSegmentationImage,
    translationKey,
}: AnalysisImageViewerProps) {
    const { t } = useTranslation();
    const baseKey = `tumor.detail.${translationKey}`;

    const [view, setView] = useState<ImageView>(() =>
        pickInitialView(
            hasDetectionImage,
            hasSegmentationImage,
            hasAnnotatedImage,
            detections.length
        )
    );
    const [imageUrl, setImageUrl] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(false);

    const activeKind = useMemo(() => {
        if (view === "original") return "scan";
        if (view === "detection") return hasDetectionImage ? "detection" : "scan";
        if (view === "segmentation") return hasSegmentationImage ? "segmentation" : "scan";
        return "annotated";
    }, [view, hasDetectionImage, hasSegmentationImage]);

    useEffect(() => {
        let cancelled = false;
        let objectUrl: string | null = null;

        const load = async () => {
            setLoading(true);
            setError(false);

            try {
                objectUrl = await fetchBlobUrl(getAnalysisImagePath(analysisId, activeKind));
                if (!cancelled) {
                    setImageUrl((prev) => {
                        if (prev) URL.revokeObjectURL(prev);
                        return objectUrl;
                    });
                }
            } catch {
                if (!cancelled) {
                    setImageUrl((prev) => {
                        if (prev) URL.revokeObjectURL(prev);
                        return null;
                    });
                    setError(true);
                }
            } finally {
                if (!cancelled) setLoading(false);
            }
        };

        load();

        return () => {
            cancelled = true;
            if (objectUrl) URL.revokeObjectURL(objectUrl);
        };
    }, [analysisId, activeKind]);

    useEffect(() => {
        return () => {
            if (imageUrl) URL.revokeObjectURL(imageUrl);
        };
    }, [imageUrl]);

    const showOverlay =
        detections.length > 0 &&
        (view === "original" || view === "detection" || view === "segmentation");

    const availableViews = VIEW_PRIORITY.filter((candidate) => {
        if (candidate === "original") return true;
        if (candidate === "detection") return hasDetectionImage || detections.length > 0;
        if (candidate === "segmentation") return hasSegmentationImage;
        if (candidate === "annotated") return hasAnnotatedImage;
        return false;
    });

    useEffect(() => {
        if (detections.length > 0 && view === "detection" && !hasDetectionImage) {
            setView("original");
        }
    }, [detections.length, hasDetectionImage, view]);

    return (
        <div className="space-y-4">
            <div className="flex flex-wrap gap-2">
                {availableViews.map((candidate) => (
                    <Button
                        key={candidate}
                        size="sm"
                        variant={view === candidate ? "primary" : "outline"}
                        onClick={() => setView(candidate)}
                    >
                        {t(`${baseKey}.imageViews.${candidate}`)}
                    </Button>
                ))}
            </div>

            <div className="rounded-xl border border-gray-200 bg-gray-50 p-4 dark:border-white/[0.05] dark:bg-gray-900/40">
                {loading && (
                    <div className="flex h-[420px] items-center justify-center text-sm text-gray-500">
                        {t(`${baseKey}.imageLoading`)}
                    </div>
                )}

                {!loading && error && (
                    <div className="flex h-[420px] items-center justify-center text-sm text-gray-500">
                        {t(`${baseKey}.imageLoadError`)}
                    </div>
                )}

                {!loading && !error && imageUrl && (
                    <div className="relative mx-auto max-w-3xl">
                        <img
                            src={imageUrl}
                            alt={t(`${baseKey}.imageAlt`)}
                            className="mx-auto max-h-[520px] w-full rounded-lg object-contain"
                        />

                        {showOverlay &&
                            detections.map((detection, index) => (
                                <div
                                    key={`${detection.className}-${index}`}
                                    className="pointer-events-none absolute border-2 border-red-500 bg-red-500/10"
                                    style={{
                                        left: `${(detection.xCenter - detection.width / 2) * 100}%`,
                                        top: `${(detection.yCenter - detection.height / 2) * 100}%`,
                                        width: `${detection.width * 100}%`,
                                        height: `${detection.height * 100}%`,
                                    }}
                                >
                                    <span className="absolute -top-6 left-0 rounded bg-red-500 px-1.5 py-0.5 text-xs text-white">
                                        {detection.className} {(detection.confidence * 100).toFixed(0)}%
                                    </span>
                                </div>
                            ))}
                    </div>
                )}
            </div>

            {detections.length > 0 && (
                <p className="text-sm text-gray-500">{t(`${baseKey}.imageLegend`)}</p>
            )}
        </div>
    );
}
