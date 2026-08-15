import { get, post, put, del } from "../../../api/api";
import { PaginatedHealthInstitutionResponse, HealthInstitutionResponse, HealthInstitutionRequest } from "./healthInstitution.types";


export const getHealthInstitutions = async (pageIndex: number, pageSize: number, search?: string): Promise<PaginatedHealthInstitutionResponse> => {
    const query = new URLSearchParams({ pageIndex: pageIndex.toString(), pageSize: pageSize.toString() });
    if (search) query.append("search", search);
    return await get(`/HealthInstitution?${query.toString()}`);
};

export const getHealthInstitutionById = async (id: number): Promise<HealthInstitutionResponse> => {
    return await get(`/HealthInstitution/${id}`);
};

export const createHealthInstitution = async (data: HealthInstitutionRequest): Promise<any> => {
    return await post(`/HealthInstitution`, data);
};

export const updateHealthInstitution = async (id: number, data: HealthInstitutionRequest): Promise<void> => {
    await put(`/HealthInstitution/${id}`, data);
};

export const deleteHealthInstitution = async (id: number): Promise<void> => {
    await del(`/HealthInstitution/${id}`);
};