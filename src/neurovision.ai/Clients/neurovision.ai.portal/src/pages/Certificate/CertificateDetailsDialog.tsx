import { useTranslation } from "react-i18next";
import { Modal } from "../../components/ui/modal";
import { CertificateResponse } from "../../features/certificate/certificate.types";
import Button from "../../components/ui/button/Button";

interface CertificateDetailsDialogProps {
    isOpen: boolean;
    certificate: CertificateResponse | null;
    onClose: () => void;
}

export default function CertificateDetailsDialog({
    isOpen,
    certificate,
    onClose,
}: CertificateDetailsDialogProps) {
    const { t, i18n } = useTranslation();

    if (!certificate) return null;

    const formatDate = (value: string) =>
        new Date(value).toLocaleDateString(i18n.language);

    const rows = [
        { label: t("certificate.columns.name"), value: certificate.name },
        {
            label: t("certificate.columns.user"),
            value: certificate.userId ?? t("certificate.status.unassigned"),
        },
        { label: t("certificate.columns.subject"), value: certificate.subject },
        { label: t("certificate.columns.issuer"), value: certificate.issuer },
        { label: t("certificate.columns.thumbprint"), value: certificate.thumbprint },
        { label: t("certificate.detailsModal.serialNumber"), value: certificate.serialNumber },
        { label: t("certificate.detailsModal.fileName"), value: certificate.fileName },
        {
            label: t("certificate.columns.signature"),
            value: certificate.hasSignatureImage ? t("common.yes") : t("common.no"),
        },
        { label: t("certificate.columns.validFrom"), value: formatDate(certificate.validFrom) },
        { label: t("certificate.columns.validTo"), value: formatDate(certificate.validTo) },
    ];

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-2xl">
            <div className="rounded-2xl bg-white p-6 dark:bg-gray-900">
                <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
                    {t("certificate.detailsModal.title")}
                </h2>

                <div className="mt-5 space-y-3">
                    {rows.map((row) => (
                        <div key={row.label}>
                            <p className="text-xs font-semibold uppercase text-gray-500">
                                {row.label}
                            </p>
                            <p className="break-all text-sm text-gray-800 dark:text-gray-200">
                                {row.value}
                            </p>
                        </div>
                    ))}
                </div>

                <div className="mt-6 flex justify-end">
                    <Button variant="outline" onClick={onClose}>
                        {t("common.close")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}
