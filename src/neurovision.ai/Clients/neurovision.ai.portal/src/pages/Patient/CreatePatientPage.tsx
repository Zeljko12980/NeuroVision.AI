import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
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

import { createPatient } from "../../features/patient/patientSlice";
import { getPatientCatalogs } from "../../features/patient/patientService";
import { PatientCatalogsResponse } from "../../features/patient/patient.types";
import { fetchDoctors } from "../../features/doctor/doctorSlice";
import { fetchHealthInstitutions } from "../../features/location/healthInstitutions/healthInstitution.slice";
import { showAlert } from "../../features/ui/uiSlice";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";

const emptyCatalogs: PatientCatalogsResponse = {
    statuses: [],
    genders: [],
    bloodTypes: [],
    languages: [],
    allergies: [],
    conditions: [],
    insurancePayers: [],
    relationshipTypes: [],
    consentTypes: [],
};

export default function CreatePatientPage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { role, userId } = getUserInfoFromClaims(claims || {});
    const isDoctor = role.toLowerCase() === "doctor";
    const listPath = location.pathname.startsWith("/admin") ? "/admin/patients" : "/patients/list";

    const hospitals = useAppSelector((state) => state.healthInstitutions.items);
    const doctors = useAppSelector((state) => state.doctor.items);

    const [form, setForm] = useState({
        firstName: "",
        lastName: "",
        email: "",
        phoneNumber: "",
        dateOfBirth: "",
        gender: "",
        bloodType: "",
        nationalId: "",
        languages: [] as string[],
        allergies: [] as string[],
        conditions: [] as string[],
        notes: "",
        hospital: "",
        healthInstitutionId: undefined as number | undefined,
        assignedDoctorId: "",
        insurancePayerCode: "",
        insurancePolicyNumber: "",
        addressLine: "",
        heightCm: "",
        weightKg: "",
        emergencyContactName: "",
        emergencyContactPhone: "",
        emergencyRelationshipCode: "",
        autoActivate: true,
    });

    const [catalogs, setCatalogs] = useState<PatientCatalogsResponse>(emptyCatalogs);
    const [picture, setPicture] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const toOptions = (items: { code: string; name: string }[]) =>
        items.map((item) => ({
            value: item.code,
            label: item.name,
        }));

    const genderOptions = useMemo(() => toOptions(catalogs.genders ?? []), [catalogs.genders]);
    const bloodTypeOptions = useMemo(() => toOptions(catalogs.bloodTypes ?? []), [catalogs.bloodTypes]);
    const languageOptions = useMemo(() => toOptions(catalogs.languages ?? []), [catalogs.languages]);
    const allergyOptions = useMemo(() => toOptions(catalogs.allergies ?? []), [catalogs.allergies]);
    const conditionOptions = useMemo(() => toOptions(catalogs.conditions ?? []), [catalogs.conditions]);
    const insuranceOptions = useMemo(
        () => toOptions(catalogs.insurancePayers ?? []),
        [catalogs.insurancePayers]
    );
    const relationshipOptions = useMemo(
        () => toOptions(catalogs.relationshipTypes ?? []),
        [catalogs.relationshipTypes]
    );
    const hospitalOptions = useMemo(
        () =>
            hospitals.map((hospital) => ({
                value: String(hospital.id),
                label: hospital.name,
            })),
        [hospitals]
    );
    const doctorOptions = useMemo(
        () =>
            doctors.map((doctor) => ({
                value: doctor.id,
                label: `${doctor.firstName} ${doctor.lastName}`,
            })),
        [doctors]
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
                const patientCatalogs = await getPatientCatalogs();
                setCatalogs(patientCatalogs);
            } catch (err: unknown) {
                dispatch(
                    showAlert({
                        type: "error",
                        message:
                            typeof err === "string"
                                ? err
                                : err instanceof Error
                                    ? err.message
                                    : t("patient.catalogsLoadError"),
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
            } catch {
                // hospital lookup is optional
            }

            if (!isDoctor) {
                try {
                    await dispatch(fetchDoctors({ pageIndex: 0, pageSize: 100 })).unwrap();
                } catch {
                    // doctor assignment is optional for admins
                }
            }
        };

        loadLookups();
    }, [dispatch, t, isDoctor]);

    useEffect(() => {
        return () => {
            if (preview) URL.revokeObjectURL(preview);
        };
    }, [preview]);

    const isFormValid =
        form.firstName.trim() &&
        form.lastName.trim() &&
        form.email.trim() &&
        form.phoneNumber.trim() &&
        form.dateOfBirth.trim() &&
        form.gender.trim() &&
        form.languages.length > 0;

    const handleSubmit = async () => {
        if (!isFormValid) {
            dispatch(
                showAlert({
                    message: t("patient.requiredError"),
                    type: "error",
                })
            );
            return;
        }

        try {
            setLoading(true);

            await dispatch(
                createPatient({
                    firstName: form.firstName,
                    lastName: form.lastName,
                    email: form.email,
                    phoneNumber: form.phoneNumber,
                    dateOfBirth: form.dateOfBirth,
                    gender: form.gender,
                    bloodType: form.bloodType || undefined,
                    nationalId: form.nationalId || undefined,
                    languages: form.languages.join(","),
                    allergies: form.allergies.join(",") || undefined,
                    conditions: form.conditions.join(",") || undefined,
                    notes: form.notes || undefined,
                    hospital: form.hospital || undefined,
                    healthInstitutionId: form.healthInstitutionId,
                    assignedDoctorId: isDoctor ? userId || undefined : form.assignedDoctorId || undefined,
                    insurancePayerCode: form.insurancePayerCode || undefined,
                    insurancePolicyNumber: form.insurancePolicyNumber || undefined,
                    addressLine: form.addressLine || undefined,
                    heightCm: form.heightCm ? Number(form.heightCm) : undefined,
                    weightKg: form.weightKg ? Number(form.weightKg) : undefined,
                    emergencyContactName: form.emergencyContactName || undefined,
                    emergencyContactPhone: form.emergencyContactPhone || undefined,
                    emergencyRelationshipCode: form.emergencyRelationshipCode || undefined,
                    autoActivate: form.autoActivate,
                    picture: picture || undefined,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    message: t("patient.success"),
                    type: "success",
                })
            );

            navigate(listPath);
        } catch (err: unknown) {
            const message =
                typeof err === "string"
                    ? err
                    : err instanceof Error
                        ? err.message
                        : t("patient.error");
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
                title={`${t("patient.pageTitle")} | NeuroVision.AI`}
                description={t("patient.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("patient.pageTitle")} />

            <div className="max-w-3xl mx-auto h-[80vh] flex flex-col">
                <ComponentCard title={t("patient.title")}>
                    <div className="space-y-6 overflow-y-auto pr-2">
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.firstName")} *</Label>
                                <Input
                                    value={form.firstName}
                                    onChange={(e) => handleChange("firstName", e.target.value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.lastName")} *</Label>
                                <Input
                                    value={form.lastName}
                                    onChange={(e) => handleChange("lastName", e.target.value)}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.email")} *</Label>
                                <Input
                                    type="email"
                                    value={form.email}
                                    onChange={(e) => handleChange("email", e.target.value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.phoneNumber")} *</Label>
                                <Input
                                    value={form.phoneNumber}
                                    onChange={(e) => handleChange("phoneNumber", e.target.value)}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.dateOfBirth")} *</Label>
                                <Input
                                    type="date"
                                    value={form.dateOfBirth}
                                    onChange={(e) => handleChange("dateOfBirth", e.target.value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.gender")} *</Label>
                                <CustomSelect
                                    options={genderOptions}
                                    value={form.gender}
                                    placeholder={t("patient.genderPlaceholder")}
                                    onChange={(value) => handleChange("gender", value)}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.bloodType")}</Label>
                                <CustomSelect
                                    options={bloodTypeOptions}
                                    value={form.bloodType}
                                    placeholder={t("patient.bloodTypePlaceholder")}
                                    onChange={(value) => handleChange("bloodType", value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.nationalId")}</Label>
                                <Input
                                    value={form.nationalId}
                                    onChange={(e) => handleChange("nationalId", e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("patient.languages")} *</Label>
                            <CatalogMultiSelect
                                options={languageOptions}
                                values={form.languages}
                                placeholder={t("patient.languagesPlaceholder")}
                                onChange={(values) =>
                                    setForm((prev) => ({ ...prev, languages: values }))
                                }
                            />
                        </div>

                        <div>
                            <Label>{t("patient.hospital")}</Label>
                            <CustomSelect
                                options={hospitalOptions}
                                value={
                                    form.healthInstitutionId != null
                                        ? String(form.healthInstitutionId)
                                        : ""
                                }
                                placeholder={t("patient.hospitalPlaceholder")}
                                onChange={handleHospitalChange}
                            />
                        </div>

                        {!isDoctor && (
                            <div>
                                <Label>{t("patient.assignedDoctor")}</Label>
                                <CustomSelect
                                    options={doctorOptions}
                                    value={form.assignedDoctorId}
                                    placeholder={t("patient.assignedDoctorPlaceholder")}
                                    onChange={(value) => handleChange("assignedDoctorId", value)}
                                />
                            </div>
                        )}

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.allergies")}</Label>
                                <CatalogMultiSelect
                                    options={allergyOptions}
                                    values={form.allergies}
                                    placeholder={t("patient.allergiesPlaceholder")}
                                    onChange={(values) =>
                                        setForm((prev) => ({ ...prev, allergies: values }))
                                    }
                                />
                            </div>
                            <div>
                                <Label>{t("patient.conditions")}</Label>
                                <CatalogMultiSelect
                                    options={conditionOptions}
                                    values={form.conditions}
                                    placeholder={t("patient.conditionsPlaceholder")}
                                    onChange={(values) =>
                                        setForm((prev) => ({ ...prev, conditions: values }))
                                    }
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.insurancePayer")}</Label>
                                <CustomSelect
                                    options={insuranceOptions}
                                    value={form.insurancePayerCode}
                                    placeholder={t("patient.insurancePayerPlaceholder")}
                                    onChange={(value) => handleChange("insurancePayerCode", value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.insurancePolicy")}</Label>
                                <Input
                                    value={form.insurancePolicyNumber}
                                    onChange={(e) => handleChange("insurancePolicyNumber", e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("patient.address")}</Label>
                            <Input
                                value={form.addressLine}
                                onChange={(e) => handleChange("addressLine", e.target.value)}
                            />
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.heightCm")}</Label>
                                <Input
                                    type="number"
                                    value={form.heightCm}
                                    onChange={(e) => handleChange("heightCm", e.target.value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.weightKg")}</Label>
                                <Input
                                    type="number"
                                    value={form.weightKg}
                                    onChange={(e) => handleChange("weightKg", e.target.value)}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label>{t("patient.emergencyName")}</Label>
                                <Input
                                    value={form.emergencyContactName}
                                    onChange={(e) => handleChange("emergencyContactName", e.target.value)}
                                />
                            </div>
                            <div>
                                <Label>{t("patient.emergencyPhone")}</Label>
                                <Input
                                    value={form.emergencyContactPhone}
                                    onChange={(e) => handleChange("emergencyContactPhone", e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("patient.emergencyRelationship")}</Label>
                            <CustomSelect
                                options={relationshipOptions}
                                value={form.emergencyRelationshipCode}
                                placeholder={t("patient.emergencyRelationshipPlaceholder")}
                                onChange={(value) => handleChange("emergencyRelationshipCode", value)}
                            />
                        </div>

                        <div>
                            <Label>{t("patient.notes")}</Label>
                            <Input
                                value={form.notes}
                                onChange={(e) => handleChange("notes", e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>{t("patient.profilePicture")}</Label>
                            <FileInput onChange={handleFileChange} />

                            {preview && (
                                <div className="mt-3 space-y-2">
                                    <ResponsiveImage src={preview} />
                                    <Button variant="outline" onClick={removeImage}>
                                        {t("patient.removeImage")}
                                    </Button>
                                </div>
                            )}
                        </div>

                        <div className="flex justify-end gap-2 pt-4 border-t border-gray-200 dark:border-gray-800">
                            <Button
                                variant="outline"
                                onClick={() => navigate(listPath)}
                                disabled={loading}
                            >
                                {t("patient.cancel")}
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
                                {loading ? t("patient.creating") : t("patient.create")}
                            </Button>
                        </div>
                    </div>
                </ComponentCard>
            </div>
        </>
    );
}
