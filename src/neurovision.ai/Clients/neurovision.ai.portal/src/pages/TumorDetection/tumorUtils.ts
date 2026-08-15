import type { TFunction } from "i18next";

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

export function formatTumorClass(className: string | null | undefined, t: TFunction): string {
    if (!className) return "—";

    const key = className.replace(/\s+/g, "");
    const map: Record<string, string> = {
        Glioma: "glioma",
        Meningioma: "meningioma",
        Pituitary: "pituitary",
        NoTumor: "noTumor",
        NoTumor2: "noTumor",
    };

    const normalized = map[key] ?? map[className] ?? null;
    return normalized ? t(`tumor.classes.${normalized}`, className) : className;
}

export function formatScanType(scanType: string, t: TFunction): string {
    const key = scanType.toLowerCase();
    return t(`tumor.scanTypes.${key}`, scanType);
}

export function formatFileSize(bytes: number): string {
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
}

export const tumorSelectClass =
    "h-11 w-full rounded-lg border border-gray-300 px-4 dark:border-gray-700 dark:bg-gray-900";

export const tumorTextareaClass =
    "min-h-24 w-full rounded-xl border border-gray-300 p-3 dark:border-gray-700 dark:bg-gray-900";
