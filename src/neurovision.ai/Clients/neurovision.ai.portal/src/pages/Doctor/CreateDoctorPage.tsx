import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";


import ComponentCard from "../../components/common/ComponentCard";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";

import Input from "../../components/form/input/InputField";
import Label from "../../components/form/Label";
import Button from "../../components/ui/button/Button";
import FileInput from "../../components/form/input/FileInput";
import ResponsiveImage from "../../components/ui/images/ResponsiveImage";

import { createDoctor } from "../../features/doctor/doctorSlice";
import { showAlert } from "../../features/ui/uiSlice";
import { useAppDispatch } from "../../store/store";

export default function CreateDoctorPage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [form, setForm] = useState({
        firstName: "",
        lastName: "",
        licenseNumber: "",
        specialization: "",
        email: "",
        phoneNumber: "",
        languages: "",
        bio: "",
        degrees: "",
        hospital: "",
        isAvailable: true,
        autoActivate: true,
    });

    const [picture, setPicture] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleChange = (key: string, value: string) => {
        setForm((prev) => ({ ...prev, [key]: value }));
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            const file = e.target.files[0];
            setPicture(file);
            setPreview(URL.createObjectURL(file));
        }
    };

    const removeImage = () => {
        setPicture(null);
        setPreview(null);
    };

    useEffect(() => {
        return () => {
            if (preview) URL.revokeObjectURL(preview);
        };
    }, [preview]);

    const isFormValid =
        form.firstName.trim() &&
        form.lastName.trim() &&
        form.licenseNumber.trim() &&
        form.specialization.trim() &&
        form.email.trim() &&
        form.phoneNumber.trim() &&
        form.languages.trim() &&
        form.bio.trim() &&
        form.degrees.trim() &&
        form.hospital.trim() &&
        picture !== null;

    const handleSubmit = async () => {
        if (!isFormValid) {
            dispatch(
                showAlert({
                    message: t("doctor.requiredError"),
                    type: "error",
                })
            );
            return;
        }

        try {
            setLoading(true);

            await dispatch(
                createDoctor({
                    ...form,
                    picture: picture || undefined,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    message: t("doctor.success"),
                    type: "success",
                })
            );

            navigate("/admin/doctors");
        } catch (err: any) {
            dispatch(
                showAlert({
                    message: err?.message || t("doctor.error"),
                    type: "error",
                })
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <PageMeta
                title={`${t("doctor.pageTitle")} | NeuroVision.AI`}
                description={t("doctor.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("doctor.pageTitle")} />

            <div className="max-w-3xl mx-auto h-[80vh] flex flex-col">
                <ComponentCard title={t("doctor.title")}>
                    <div className="space-y-6 overflow-y-auto pr-2">

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("doctor.firstName")} *</Label>
                                <Input
                                    value={form.firstName}
                                    onChange={(e) => handleChange("firstName", e.target.value)}
                                />
                            </div>

                            <div>
                                <Label>{t("doctor.lastName")} *</Label>
                                <Input
                                    value={form.lastName}
                                    onChange={(e) => handleChange("lastName", e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("doctor.licenseNumber")} *</Label>
                            <Input
                                value={form.licenseNumber}
                                onChange={(e) => handleChange("licenseNumber", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.specialization")} *</Label>
                            <Input
                                value={form.specialization}
                                onChange={(e) => handleChange("specialization", e.target.value)}
                            />
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("doctor.email")} *</Label>
                                <Input
                                    value={form.email}
                                    onChange={(e) => handleChange("email", e.target.value)}
                                />
                            </div>

                            <div>
                                <Label>{t("doctor.phoneNumber")} *</Label>
                                <Input
                                    value={form.phoneNumber}
                                    onChange={(e) => handleChange("phoneNumber", e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("doctor.languages")} *</Label>
                            <Input
                                value={form.languages}
                                onChange={(e) => handleChange("languages", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.hospital")} *</Label>
                            <Input
                                value={form.hospital}
                                onChange={(e) => handleChange("hospital", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.degrees")} *</Label>
                            <Input
                                value={form.degrees}
                                onChange={(e) => handleChange("degrees", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.bio")} *</Label>
                            <Input
                                value={form.bio}
                                onChange={(e) => handleChange("bio", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.profilePicture")} *</Label>
                            <FileInput onChange={handleFileChange} />

                            {preview && (
                                <div className="mt-3 space-y-2">
                                    <ResponsiveImage src={preview} />
                                    <Button variant="outline" onClick={removeImage}>
                                        {t("doctor.removeImage")}
                                    </Button>
                                </div>
                            )}
                        </div>

                        <div className="flex justify-end gap-2 pt-4 border-t border-gray-200 dark:border-gray-800">
                            <Button
                                variant="outline"
                                onClick={() => navigate("/admin/doctors")}
                                disabled={loading}
                            >
                                {t("doctor.cancel")}
                            </Button>

                            <Button
                                onClick={handleSubmit}
                                disabled={loading || !isFormValid}
                                className={`text-white ${isFormValid
                                        ? "bg-blue-600 hover:bg-blue-700"
                                        : "bg-gray-400 cursor-not-allowed"
                                    }`}
                            >
                                {loading ? t("doctor.creating") : t("doctor.create")}
                            </Button>
                        </div>

                    </div>
                </ComponentCard>
            </div>
        </>
    );
}