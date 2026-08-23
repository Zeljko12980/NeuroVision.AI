import { del, get, post } from "../../api/api";
import {
    CreatePatientRequest,
    PatientCatalogsResponse,
    PatientResponse,
    PaginatedPatientResponse,
} from "./patient.types";

export const getPatients = async (
    pageIndex: number,
    pageSize: number,
    search?: string
): Promise<PaginatedPatientResponse> => {
    const query = new URLSearchParams({
        pageIndex: pageIndex.toString(),
        pageSize: pageSize.toString(),
    });

    if (search) query.append("search", search);

    return await get(`/patient?${query.toString()}`);
};

export const getPatientById = async (id: string): Promise<PatientResponse> => {
    return await get(`/patient/${id}`);
};

export const getPatientCatalogs = async (): Promise<PatientCatalogsResponse> => {
    return await get(`/patient/catalogs`);
};

export const createPatientRequest = async (
    data: CreatePatientRequest
): Promise<PatientResponse> => {
    const formData = new FormData();

    formData.append("FirstName", data.firstName);
    formData.append("LastName", data.lastName);
    formData.append("Email", data.email);
    formData.append("PhoneNumber", data.phoneNumber);
    formData.append("DateOfBirth", data.dateOfBirth);
    formData.append("Gender", data.gender);
    formData.append("Languages", data.languages);
    formData.append("AutoActivate", String(data.autoActivate));

    if (data.bloodType) formData.append("BloodType", data.bloodType);
    if (data.nationalId) formData.append("NationalId", data.nationalId);
    if (data.allergies) formData.append("Allergies", data.allergies);
    if (data.conditions) formData.append("Conditions", data.conditions);
    if (data.notes) formData.append("Notes", data.notes);
    if (data.hospital) formData.append("Hospital", data.hospital);
    if (data.healthInstitutionId != null) {
        formData.append("HealthInstitutionId", String(data.healthInstitutionId));
    }
    if (data.assignedDoctorId) formData.append("AssignedDoctorId", data.assignedDoctorId);
    if (data.insurancePayerCode) formData.append("InsurancePayerCode", data.insurancePayerCode);
    if (data.insurancePolicyNumber) {
        formData.append("InsurancePolicyNumber", data.insurancePolicyNumber);
    }
    if (data.addressLine) formData.append("AddressLine", data.addressLine);
    if (data.heightCm != null) formData.append("HeightCm", String(data.heightCm));
    if (data.weightKg != null) formData.append("WeightKg", String(data.weightKg));
    if (data.emergencyContactName) formData.append("EmergencyContactName", data.emergencyContactName);
    if (data.emergencyContactPhone) formData.append("EmergencyContactPhone", data.emergencyContactPhone);
    if (data.emergencyRelationshipCode) {
        formData.append("EmergencyRelationshipCode", data.emergencyRelationshipCode);
    }
    if (data.picture) formData.append("Picture", data.picture);

    return await post(`/patient`, formData);
};

export const deletePatientRequest = async (id: string): Promise<void> => {
    await del(`/patient/${id}`);
};

export const resolvePatientImageUrl = (path?: string | null): string | undefined => {
    if (!path) return undefined;
    if (/^https?:\/\//i.test(path) || path.startsWith("blob:") || path.startsWith("data:")) {
        return path;
    }

    const api = (import.meta.env.VITE_API_URL ?? "http://localhost:5000/api").replace(/\/$/, "");
    const origin = api.replace(/\/api$/i, "");
    return `${origin}/${path.replace(/^\//, "")}`;
};

export const getPatientByEmail = async (email: string): Promise<PatientResponse | null> => {
    const result = await getPatients(0, 50, email);
    const normalized = email.toLowerCase();
    return (
        result.data.find((item) => item.email.toLowerCase() === normalized) ?? null
    );
};
