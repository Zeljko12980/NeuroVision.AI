import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Label from "../../components/form/Label";
import Button from "../../components/ui/button/Button";
import FileInput from "../../components/form/input/FileInput";

import { useAppDispatch, useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import { uploadBrainScan } from "../../features/tumorDetection/tumorDetection.slice";
import { showAlert } from "../../features/ui/uiSlice";
import type { ScanType } from "../../features/tumorDetection/tumorDetection.types";
import { tumorSelectClass } from "./tumorUtils";

interface UploadScanPageProps {
    redirectPath: string;
    translationKey: "doctor" | "patient";
}

export default function UploadScanPage({
    redirectPath,
    translationKey,
}: UploadScanPageProps) {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});
    const uploading = useAppSelector((s) => s.tumorDetection.uploading);

    const baseKey = `tumor.upload.${translationKey}`;

    const [scanType, setScanType] = useState<ScanType>("Mri");
    const [file, setFile] = useState<File | null>(null);
    const [patientId, setPatientId] = useState(userId);

    const handleSubmit = async () => {
        if (!file || !patientId || !userId) {
            dispatch(showAlert({ type: "error", message: t(`${baseKey}.messages.required`) }));
            return;
        }

        try {
            await dispatch(
                uploadBrainScan({
                    patientId,
                    uploadedByUserId: userId,
                    scanType,
                    file,
                })
            ).unwrap();

            dispatch(showAlert({ type: "success", message: t(`${baseKey}.messages.success`) }));
            navigate(redirectPath);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.error`),
                })
            );
        }
    };

    return (
        <>
            <PageMeta
                title={t(`${baseKey}.pageTitle`)}
                description={t(`${baseKey}.pageDescription`)}
            />
            <PageBreadcrumb pageTitle={t(`${baseKey}.pageTitle`)} />

            <div className="space-y-6">
                <ComponentCard title={t(`${baseKey}.title`)}>
                    <div className="grid max-w-xl gap-5">
                        {translationKey === "doctor" && (
                            <div>
                                <Label>{t(`${baseKey}.fields.patientId`)}</Label>
                                <input
                                    className={tumorSelectClass}
                                    value={patientId}
                                    onChange={(e) => setPatientId(e.target.value)}
                                />
                            </div>
                        )}

                        <div>
                            <Label>{t(`${baseKey}.fields.scanType`)}</Label>
                            <select
                                className={tumorSelectClass}
                                value={scanType}
                                onChange={(e) => setScanType(e.target.value as ScanType)}
                            >
                            <option value="Mri">{t("tumor.scanTypes.mri")}</option>
                            <option value="Ct">{t("tumor.scanTypes.ct")}</option>
                        </select>
                    </div>

                    <div>
                        <Label>{t(`${baseKey}.fields.file`)}</Label>
                        <FileInput onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
                    </div>

                    <Button disabled={uploading || !file} onClick={handleSubmit}>
                        {uploading ? t(`${baseKey}.actions.uploading`) : t(`${baseKey}.actions.upload`)}
                    </Button>
                </div>
            </ComponentCard>
            </div>
        </>
    );
}
