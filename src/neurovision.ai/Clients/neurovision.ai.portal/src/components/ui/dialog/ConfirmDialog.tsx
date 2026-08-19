import { useTranslation } from "react-i18next";
import Button from "../button/Button";
import { Modal } from "../modal/index";

interface ConfirmDialogProps {
    isOpen: boolean;
    title: string;
    description?: string;
    onConfirm: () => void;
    onCancel: () => void;
    loading?: boolean;
    confirmLabel?: string;
    confirmClassName?: string;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
    isOpen,
    title,
    description,
    onConfirm,
    onCancel,
    loading,
    confirmLabel,
    confirmClassName = "bg-red-500 hover:bg-red-600 text-white",
}) => {
    const { t } = useTranslation();

    return (
        <Modal isOpen={isOpen} onClose={onCancel} className="max-w-md">
            <div className="p-6">
                <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                    {title}
                </h2>

                {description && (
                    <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                        {description}
                    </p>
                )}

                <div className="mt-6 flex justify-end gap-2">
                    <Button variant="outline" onClick={onCancel} disabled={loading}>
                        {t("common.cancel")}
                    </Button>

                    <Button
                        type="button"
                        onClick={onConfirm}
                        disabled={loading}
                        className={confirmClassName}
                    >
                        {loading ? "..." : confirmLabel || t("common.delete")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
};

export default ConfirmDialog;