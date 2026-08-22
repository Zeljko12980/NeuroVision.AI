import { del, get, post } from "../../api/api";
import {
    CreateDoctorRequest,
    DoctorCatalogsResponse,
    DoctorResponse,
    PaginatedDoctorResponse,
} from "./doctor.types";

export const getDoctors = async (
    pageIndex: number,
    pageSize: number,
    search?: string
): Promise<PaginatedDoctorResponse> => {
    const query = new URLSearchParams({
        pageIndex: pageIndex.toString(),
        pageSize: pageSize.toString(),
    });

    if (search) query.append("search", search);

    return await get(`/doctor?${query.toString()}`);
};

export const getDoctorById = async (id: string): Promise<DoctorResponse> => {
    return await get(`/doctor/${id}`);
};

export const getDoctorCatalogs = async (): Promise<DoctorCatalogsResponse> => {
    return await get(`/doctor/catalogs`);
};

export const createDoctorRequest = async (
    data: CreateDoctorRequest
): Promise<DoctorResponse> => {
    const formData = new FormData();

    formData.append("FirstName", data.firstName);
    formData.append("LastName", data.lastName);
    formData.append("LicenseNumber", data.licenseNumber);
    formData.append("Specialization", data.specialization);
    formData.append("Email", data.email);
    formData.append("PhoneNumber", data.phoneNumber);
    formData.append("Languages", data.languages);
    formData.append("Bio", data.bio ?? "");
    formData.append("Degrees", data.degrees ?? "");
    formData.append("Hospital", data.hospital ?? "");
    formData.append("IsAvailable", String(data.isAvailable));
    formData.append("AutoActivate", String(data.autoActivate));

    if (data.licenseAuthorityCode) {
        formData.append("LicenseAuthorityCode", data.licenseAuthorityCode);
    }

    if (data.healthInstitutionId != null) {
        formData.append("HealthInstitutionId", String(data.healthInstitutionId));
    }

    if (data.picture) {
        formData.append("Picture", data.picture);
    }

    return await post(`/doctor`, formData);
};

export const deleteDoctorRequest = async (id: string): Promise<void> => {
    await del(`/doctor/${id}`);
};

export const resolveDoctorImageUrl = (path?: string | null): string | undefined => {
    if (!path) return undefined;
    if (/^https?:\/\//i.test(path) || path.startsWith("blob:") || path.startsWith("data:")) {
        return path;
    }

    const api = (import.meta.env.VITE_API_URL ?? "http://localhost:5000/api").replace(/\/$/, "");
    const origin = api.replace(/\/api$/i, "");
    return `${origin}/${path.replace(/^\//, "")}`;
};

export const getDoctorByEmail = async (email: string): Promise<DoctorResponse | null> => {
    const result = await getDoctors(0, 50, email);
    const normalized = email.toLowerCase();
    return (
        result.data.find((item) => item.email.toLowerCase() === normalized) ?? null
    );
};
