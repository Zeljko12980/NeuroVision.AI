import { get, post, put, del } from "../../../api/api";
import { PaginatedHealthInstitutionTypeResponse, HealthInstitutionTypeResponse, HealthInstitutionTypeRequest } from "./healthInstitutionType.type";



export const getHealthInstitutionTypes = async (pageIndex: number, pageSize: number, search?: string): Promise<PaginatedHealthInstitutionTypeResponse> => {
    const query = new URLSearchParams({ pageIndex: pageIndex.toString(), pageSize: pageSize.toString() });
    if (search) query.append("search", search);
    return await get(`/HealthInstitutionType?${query.toString()}`);
};

export const getHealthInstitutionTypeByCode = async (code: string): Promise<HealthInstitutionTypeResponse> => {
    return await get(`/HealthInstitutionType/${code}`);
};

export const createHealthInstitutionType = async (data: HealthInstitutionTypeRequest): Promise<any> => {
    return await post(`/HealthInstitutionType`, data);
};

export const updateHealthInstitutionType = async (code: string, data: HealthInstitutionTypeRequest): Promise<void> => {
    await put(`/HealthInstitutionType/${code}`, data);
};

export const deleteHealthInstitutionType = async (code: string): Promise<void> => {
    await del(`/HealthInstitutionType/${code}`);
};
