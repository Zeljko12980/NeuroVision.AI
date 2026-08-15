import { useTranslation } from "react-i18next";
import { Modal } from "../modal";

interface PdfTemplatePreviewDialogProps {
    isOpen: boolean;
    title: string;
    html: string;
    onClose: () => void;
}

export default function PdfTemplatePreviewDialog({
    isOpen,
    title,
    html,
    onClose,
}: PdfTemplatePreviewDialogProps) {
    const { t } = useTranslation();

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            className="max-w-5xl"
        >
            <div className="flex h-[90vh] flex-col overflow-hidden rounded-2xl bg-white dark:bg-gray-900">

                <div className="border-b border-gray-200 px-6 py-5 dark:border-gray-800">
                    <h2 className="text-2xl font-semibold text-gray-900 dark:text-white">
                        {t("pdf.previewModal.title")}
                    </h2>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        {title}
                    </p>
                </div>

                <iframe
                    title="preview"
                    className="flex-1 w-full bg-white"
                    srcDoc={html}
                />

                <div className="flex justify-end border-t border-gray-200 px-6 py-4 dark:border-gray-800">
                    <span className="text-sm text-gray-500 dark:text-gray-400">
                        {t("pdf.previewModal.readOnly")}
                    </span>
                </div>

            </div>
        </Modal>
    );
}