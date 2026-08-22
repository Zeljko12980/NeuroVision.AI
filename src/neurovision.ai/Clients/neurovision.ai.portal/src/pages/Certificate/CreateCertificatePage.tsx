import { ChangeEvent, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";

import Input from "../../components/form/input/InputField";
import Label from "../../components/form/Label";
import FileInput from "../../components/form/input/FileInput";
import CustomSelect from "../../components/form/CustomSelect";
import Button from "../../components/ui/button/Button";

import { useAppDispatch, useAppSelector } from "../../store/store";
import { createCertificate } from "../../features/certificate/certificateSlice";
import { showAlert } from "../../features/ui/uiSlice";
import { getDoctors } from "../../features/doctor/doctorService";
import { DoctorResponse } from "../../features/doctor/doctor.types";

const ALLOWED_EXTENSIONS = [".pfx", ".p12", ".cer", ".crt", ".der"];
const ALLOWED_SIGNATURE_EXTENSIONS = [".png", ".jpg", ".jpeg", ".webp"];

const resolveErrorMessage = (err: unknown, fallback: string) => {
    if (typeof err === "string" && err.trim()) return err;
    if (err instanceof Error && err.message.trim()) return err.message;
    if (err && typeof err === "object" && "message" in err) {
        const message = (err as { message?: unknown }).message;
        if (typeof message === "string" && message.trim()) return message;
    }
    return fallback;
};

export default function CreateCertificatePage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { loading } = useAppSelector((state) => state.certificate);

    const [doctors, setDoctors] = useState<DoctorResponse[]>([]);
    const [userId, setUserId] = useState("");
    const [name, setName] = useState("");
    const [autoNameFrom, setAutoNameFrom] = useState("");
    const [password, setPassword] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [signatureImage, setSignatureImage] = useState<File | null>(null);

    useEffect(() => {
        const loadDoctors = async () => {
            try {
                const response = await getDoctors(0, 200);
                setDoctors(response.data ?? []);
            } catch {
                dispatch(
                    showAlert({
                        type: "error",
                        message: t("certificate.messages.loadDoctorsError"),
                    })
                );
            }
        };

        loadDoctors();
    }, [dispatch, t]);

    const doctorOptions = useMemo(
        () =>
            doctors.map((doctor) => ({
                value: doctor.id,
                label: `${doctor.firstName} ${doctor.lastName} (${doctor.email})`,
            })),
        [doctors]
    );

    const handleDoctorChange = (value: string) => {
        const doctor = doctors.find((item) => item.id === value);
        setUserId(value);

        if (!name.trim() || name === autoNameFrom) {
            const nextName = doctor
                ? `${doctor.firstName} ${doctor.lastName}`
                : "";
            setName(nextName);
            setAutoNameFrom(nextName);
        }
    };

    const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
        const selected = event.target.files?.[0] ?? null;
        setFile(selected);
    };

    const handleSignatureChange = (event: ChangeEvent<HTMLInputElement>) => {
        const selected = event.target.files?.[0] ?? null;
        setSignatureImage(selected);
    };

    const isAllowedExtension = (fileName: string, allowed: string[]) => {
        const extension = fileName.slice(fileName.lastIndexOf(".")).toLowerCase();
        return allowed.includes(extension);
    };

    const handleSubmit = async () => {
        if (!userId || !name.trim() || !file || !signatureImage) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("certificate.messages.requiredError"),
                })
            );
            return;
        }

        if (!isAllowedExtension(file.name, ALLOWED_EXTENSIONS)) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("certificate.create.fileHint"),
                })
            );
            return;
        }

        if (!isAllowedExtension(signatureImage.name, ALLOWED_SIGNATURE_EXTENSIONS)) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("certificate.create.signatureHint"),
                })
            );
            return;
        }

        try {
            await dispatch(
                createCertificate({
                    userId,
                    name: name.trim(),
                    password: password || undefined,
                    file,
                    signatureImage,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("certificate.messages.createSuccess"),
                })
            );

            navigate("/admin/certificates");
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message: resolveErrorMessage(
                        err,
                        t("certificate.messages.createError")
                    ),
                })
            );
        }
    };

    return (
        <>
            <PageMeta
                title={t("certificate.create.pageTitle")}
                description={t("certificate.create.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("certificate.create.pageTitle")} />

            <ComponentCard title={t("certificate.create.pageTitle")}>
                <div className="grid max-w-xl grid-cols-1 gap-5">
                    <div>
                        <Label>{t("certificate.create.doctor")}</Label>
                        <CustomSelect
                            options={doctorOptions}
                            value={userId}
                            placeholder={t("certificate.create.doctorPlaceholder")}
                            onChange={handleDoctorChange}
                        />
                    </div>

                    <div>
                        <Label>{t("certificate.create.name")}</Label>
                        <Input
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                        />
                    </div>

                    <div>
                        <Label>{t("certificate.create.password")}</Label>
                        <Input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            hint={t("certificate.create.passwordHint")}
                        />
                    </div>

                    <div>
                        <Label>{t("certificate.create.file")}</Label>
                        <FileInput accept=".pfx,.p12,.cer,.crt,.der" onChange={handleFileChange} />
                        <p className="mt-2 text-xs text-gray-500">
                            {t("certificate.create.fileHint")}
                        </p>
                        {file && (
                            <p className="mt-1 text-sm text-gray-700 dark:text-gray-300">
                                {file.name}
                            </p>
                        )}
                    </div>

                    <div>
                        <Label>{t("certificate.create.signatureImage")}</Label>
                        <FileInput
                            accept=".png,.jpg,.jpeg,.webp"
                            onChange={handleSignatureChange}
                        />
                        <p className="mt-2 text-xs text-gray-500">
                            {t("certificate.create.signatureHint")}
                        </p>
                        {signatureImage && (
                            <p className="mt-1 text-sm text-gray-700 dark:text-gray-300">
                                {signatureImage.name}
                            </p>
                        )}
                    </div>

                    <div className="flex justify-end gap-3 pt-4">
                        <Button
                            variant="outline"
                            onClick={() => navigate("/admin/certificates")}
                        >
                            {t("common.cancel")}
                        </Button>

                        <Button onClick={handleSubmit} disabled={loading}>
                            {loading
                                ? t("common.creating")
                                : t("certificate.create.create")}
                        </Button>
                    </div>
                </div>
            </ComponentCard>
        </>
    );
}
