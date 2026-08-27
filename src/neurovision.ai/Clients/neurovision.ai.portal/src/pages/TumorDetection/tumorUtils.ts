import type { TFunction } from "i18next";
import type { AnalysisResponse, DetectionFindingResponse } from "../../features/tumorDetection/tumorDetection.types";

export function tumorStatusColor(
    status: string
): "success" | "warning" | "error" | "info" | "primary" {
    switch (status) {
        case "Completed":
        case "Corrected":
            return "success";
        case "Processing":
            return "warning";
        case "Failed":
            return "error";
        default:
            return "info";
    }
}

const CLASS_ALIASES: Record<string, string> = {
    glioma: "glioma",
    gliomatumor: "glioma",
    meningioma: "meningioma",
    meningiomatumor: "meningioma",
    pituitary: "pituitary",
    pituitarytumor: "pituitary",
    notumor: "noTumor",
    notumor2: "noTumor",
    healthy: "noTumor",
};

export function normalizeTumorClassKey(className: string | null | undefined): string | null {
    if (!className) return null;
    const key = className.toLowerCase().replace(/[_-]+/g, " ").replace(/\s+/g, "").trim();
    return CLASS_ALIASES[key] ?? CLASS_ALIASES[className.replace(/\s+/g, "")] ?? null;
}

export function formatTumorClass(className: string | null | undefined, t: TFunction): string {
    if (!className) return "—";
    const normalized = normalizeTumorClassKey(className);
    return normalized ? t(`tumor.classes.${normalized}`, className) : className;
}

export function isNoTumorClass(className: string | null | undefined): boolean {
    return normalizeTumorClassKey(className) === "noTumor";
}

export function primaryDetection(
    detections: DetectionFindingResponse[] | undefined
): DetectionFindingResponse | null {
    if (!detections?.length) return null;
    return detections.reduce((best, item) => (item.confidence > best.confidence ? item : best));
}

export function primaryFindingClass(analysis: Pick<AnalysisResponse, "classificationClass" | "detections">): string | null {
    return primaryDetection(analysis.detections)?.className ?? analysis.classificationClass ?? null;
}

export function tumorClassesDisagree(left: string | null | undefined, right: string | null | undefined): boolean {
    const a = normalizeTumorClassKey(left);
    const b = normalizeTumorClassKey(right);
    return !!a && !!b && a !== b;
}

export function tumorClassToCorrectionValue(className: string | null | undefined): string {
    switch (normalizeTumorClassKey(className)) {
        case "meningioma":
            return "2";
        case "pituitary":
            return "3";
        case "noTumor":
            return "4";
        default:
            return "1";
    }
}

export function formatScanType(scanType: string, t: TFunction): string {
    const key = scanType.toLowerCase();
    return t(`tumor.scanTypes.${key}`, scanType);
}

export function formatFileSize(bytes: number): string {
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
}

export function formatPatientName(
    patients: { id: string; firstName: string; lastName: string }[],
    patientId: string
): string {
    const patient = patients.find((item) => item.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : patientId;
}

export const tumorSelectClass =
    "h-11 w-full rounded-lg border border-gray-300 px-4 dark:border-gray-700 dark:bg-gray-900";

export const tumorTextareaClass =
    "min-h-24 w-full rounded-xl border border-gray-300 p-3 dark:border-gray-700 dark:bg-gray-900";
