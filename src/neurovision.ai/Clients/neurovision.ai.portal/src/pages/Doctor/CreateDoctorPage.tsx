import { useEffect, useMemo, useState } from "react";
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
import CustomSelect from "../../components/form/CustomSelect";
import CatalogMultiSelect from "../../components/form/CatalogMultiSelect";

import { createDoctor } from "../../features/doctor/doctorSlice";
import { getDoctorCatalogs } from "../../features/doctor/doctorService";
import { DoctorCatalogsResponse } from "../../features/doctor/doctor.types";
import { fetchHealthInstitutions } from "../../features/location/healthInstitutions/healthInstitution.slice";
import { showAlert } from "../../features/ui/uiSlice";
import { useAppDispatch, useAppSelector } from "../../store/store";

const emptyCatalogs: DoctorCatalogsResponse = {
    specializations: [],
    languages: [],
    degreeTypes: [],
    licenseAuthorities: [],
};

export default function CreateDoctorPage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const hospitals = useAppSelector((state) => state.healthInstitutions.items);

    const [form, setForm] = useState({
        firstName: "",
        lastName: "",
        licenseNumber: "",
        licenseAuthorityCode: "",
        specialization: "",
        email: "",
        phoneNumber: "",
        languages: [] as string[],
        bio: "",
        degrees: [] as string[],
        hospital: "",
        healthInstitutionId: undefined as number | undefined,
        isAvailable: true,
        autoActivate: true,
    });

    const [catalogs, setCatalogs] = useState<DoctorCatalogsResponse>(emptyCatalogs);
    const [picture, setPicture] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const toOptions = (items: { code: string; name: string }[]) =>
        items.map((item) => ({
            value: item.code,
            label: item.name,
        }));

    const specializationOptions = useMemo(
        () => toOptions(catalogs.specializations ?? []),
        [catalogs.specializations]
    );
    const languageOptions = useMemo(
        () => toOptions(catalogs.languages ?? []),
        [catalogs.languages]
    );
    const degreeOptions = useMemo(
        () => toOptions(catalogs.degreeTypes ?? []),
        [catalogs.degreeTypes]
    );
    const licenseAuthorityOptions = useMemo(
        () => toOptions(catalogs.licenseAuthorities ?? []),
        [catalogs.licenseAuthorities]
    );
    const hospitalOptions = useMemo(
        () =>
            hospitals.map((hospital) => ({
                value: String(hospital.id),
                label: hospital.name,
            })),
        [hospitals]
    );

    const handleChange = (key: string, value: string) => {
        setForm((prev) => ({ ...prev, [key]: value }));
    };

    const handleHospitalChange = (value: string) => {
        const hospital = hospitals.find((item) => String(item.id) === value);
        setForm((prev) => ({
            ...prev,
            healthInstitutionId: hospital?.id,
            hospital: hospital?.name ?? "",
        }));
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
        const loadLookups = async () => {
            try {
                const doctorCatalogs = await getDoctorCatalogs();
                setCatalogs(doctorCatalogs);
            } catch (err: unknown) {
                dispatch(
                    showAlert({
                        type: "error",
                        message:
                            typeof err === "string"
                                ? err
                                : err instanceof Error
                                    ? err.message
                                    : t("doctor.catalogsLoadError"),
                    })
                );
            }

            try {
                await dispatch(
                    fetchHealthInstitutions({
                        pageIndex: 0,
                        pageSize: 100,
                    })
                ).unwrap();
            } catch (err: unknown) {
                dispatch(
                    showAlert({
                        type: "error",
                        message:
                            typeof err === "string"
                                ? err
                                : err instanceof Error
                                    ? err.message
                                    : t("doctor.catalogsLoadError"),
                    })
                );
            }
        };

        loadLookups();
    }, [dispatch, t]);

    useEffect(() => {
        return () => {
            if (preview) URL.revokeObjectURL(preview);
        };
    }, [preview]);

    const isFormValid =
        form.firstName.trim() &&
        form.lastName.trim() &&
        form.licenseNumber.trim() &&
        form.licenseAuthorityCode.trim() &&
        form.specialization.trim() &&
        form.email.trim() &&
        form.phoneNumber.trim() &&
        form.languages.length > 0 &&
        form.bio.trim() &&
        form.degrees.length > 0 &&
        form.hospital.trim() &&
        form.healthInstitutionId != null &&
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
                    firstName: form.firstName,
                    lastName: form.lastName,
                    licenseNumber: form.licenseNumber,
                    licenseAuthorityCode: form.licenseAuthorityCode,
                    specialization: form.specialization,
                    email: form.email,
                    phoneNumber: form.phoneNumber,
                    languages: form.languages.join(","),
                    bio: form.bio,
                    degrees: form.degrees.join(","),
                    hospital: form.hospital,
                    healthInstitutionId: form.healthInstitutionId,
                    isAvailable: form.isAvailable,
                    autoActivate: form.autoActivate,
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
        } catch (err: unknown) {
            const message =
                typeof err === "string"
                    ? err
                    : err instanceof Error
                        ? err.message
                        : t("doctor.error");
            dispatch(
                showAlert({
                    message,
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

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("doctor.licenseNumber")} *</Label>
                                <Input
                                    value={form.licenseNumber}
                                    onChange={(e) => handleChange("licenseNumber", e.target.value)}
                                />
                            </div>

                            <div>
                                <Label>{t("doctor.licenseAuthority")} *</Label>
                                <CustomSelect
                                    options={licenseAuthorityOptions}
                                    value={form.licenseAuthorityCode}
                                    placeholder={t("doctor.licenseAuthorityPlaceholder")}
                                    onChange={(value) => handleChange("licenseAuthorityCode", value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("doctor.specialization")} *</Label>
                            <CustomSelect
                                options={specializationOptions}
                                value={form.specialization}
                                placeholder={t("doctor.specializationPlaceholder")}
                                onChange={(value) => handleChange("specialization", value)}
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
                            <CatalogMultiSelect
                                options={languageOptions}
                                values={form.languages}
                                placeholder={t("doctor.languagesPlaceholder")}
                                onChange={(values) =>
                                    setForm((prev) => ({ ...prev, languages: values }))
                                }
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.hospital")} *</Label>
                            <CustomSelect
                                options={hospitalOptions}
                                value={
                                    form.healthInstitutionId != null
                                        ? String(form.healthInstitutionId)
                                        : ""
                                }
                                placeholder={t("doctor.hospitalPlaceholder")}
                                onChange={handleHospitalChange}
                            />
                        </div>

                        <div>
                            <Label>{t("doctor.degrees")} *</Label>
                            <CatalogMultiSelect
                                options={degreeOptions}
                                values={form.degrees}
                                placeholder={t("doctor.degreesPlaceholder")}
                                onChange={(values) =>
                                    setForm((prev) => ({ ...prev, degrees: values }))
                                }
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
                                className={`text-white ${
                                    isFormValid
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
