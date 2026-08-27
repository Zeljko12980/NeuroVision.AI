import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../components/ui/modal";
import { fetchBlobUrl, getScanImagePath } from "../../api/api";

interface ScanPreviewModalProps {
    scanId: string | null;
    fileName?: string;
    translationKey: "doctor" | "patient";
    onClose: () => void;
}

export default function ScanPreviewModal({
    scanId,
    fileName,
    translationKey,
    onClose,
}: ScanPreviewModalProps) {
    const { t } = useTranslation();
    const baseKey = `tumor.scans.${translationKey}`;
    const [imageUrl, setImageUrl] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(false);

    useEffect(() => {
        if (!scanId) {
            setImageUrl((prev) => {
                if (prev) URL.revokeObjectURL(prev);
                return null;
            });
            setError(false);
            setLoading(false);
            return;
        }

        let cancelled = false;
        let objectUrl: string | null = null;

        const load = async () => {
            setLoading(true);
            setError(false);
            try {
                objectUrl = await fetchBlobUrl(getScanImagePath(scanId));
                if (!cancelled) {
                    setImageUrl((prev) => {
                        if (prev) URL.revokeObjectURL(prev);
                        return objectUrl;
                    });
                } else {
                    URL.revokeObjectURL(objectUrl);
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
        };
    }, [scanId]);

    useEffect(() => {
        return () => {
            if (imageUrl) URL.revokeObjectURL(imageUrl);
        };
    }, [imageUrl]);

    return (
        <Modal isOpen={!!scanId} onClose={onClose} className="max-w-4xl">
            <div className="p-6 sm:p-8">
                <h2 className="pr-12 text-lg font-semibold text-gray-800 dark:text-white">
                    {fileName ?? t(`${baseKey}.preview.title`)}
                </h2>
                <div className="mt-4 rounded-xl border border-gray-200 bg-gray-50 p-4 dark:border-white/[0.05] dark:bg-gray-900/40">
                    {loading && (
                        <div className="flex h-[420px] items-center justify-center text-sm text-gray-500">
                            {t(`${baseKey}.preview.loading`)}
                        </div>
                    )}
                    {!loading && error && (
                        <div className="flex h-[420px] items-center justify-center text-sm text-gray-500">
                            {t(`${baseKey}.preview.error`)}
                        </div>
                    )}
                    {!loading && !error && imageUrl && (
                        <img
                            src={imageUrl}
                            alt={fileName ?? t(`${baseKey}.preview.title`)}
                            className="mx-auto max-h-[70vh] w-full rounded-lg object-contain"
                        />
                    )}
                </div>
            </div>
        </Modal>
    );
}
