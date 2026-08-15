import { get, post, put, del } from "../../api/api";

export interface GovernmentTypeRequest {
    code: string;
    name: string;
}

export interface GovernmentTypeResponse {
    code: string;
    name: string;
}

export interface PaginatedGovernmentTypeResponse {
    data: GovernmentTypeResponse[];
    count: number;
}

export const getGovernmentTypes = async (pageIndex: number, pageSize: number, search?: string): Promise<PaginatedGovernmentTypeResponse> => {
    const query = new URLSearchParams({ pageIndex: pageIndex.toString(), pageSize: pageSize.toString() });
    if (search) query.append("search", search);
    return await get(`/GovernmentType?${query.toString()}`);
};

export const getGovernmentTypeByCode = async (code: string): Promise<GovernmentTypeResponse> => {
    return await get(`/GovernmentType/${code}`);
};

export const createGovernmentType = async (data: GovernmentTypeRequest): Promise<any> => {
    return await post(`/GovernmentType`, data);
};

export const updateGovernmentType = async (code: string, data: GovernmentTypeRequest): Promise<void> => {
    await put(`/GovernmentType/${code}`, data);
};

export const deleteGovernmentType = async (code: string): Promise<void> => {
    await del(`/GovernmentType/${code}`);
};
