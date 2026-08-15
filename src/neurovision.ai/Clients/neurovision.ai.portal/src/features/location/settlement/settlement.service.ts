import { get, post, put, del } from "../../../api/api";
import { PaginatedSettlementResponse, SettlementResponse, SettlementRequest } from "./settlements.type";

export type { PaginatedSettlementResponse, SettlementResponse, SettlementRequest } from "./settlements.type";


export const getSettlements = async (pageIndex: number, pageSize: number, search?: string): Promise<PaginatedSettlementResponse> => {
    const query = new URLSearchParams({ pageIndex: pageIndex.toString(), pageSize: pageSize.toString() });
    if (search) query.append("search", search);
    return await get(`/settlement?${query.toString()}`);
};

export const getSettlementByKey = async (countryCode: string, code: number): Promise<SettlementResponse> => {
    return await get(`/settlement/${countryCode}/${code}`);
};

export const createSettlement = async (data: SettlementRequest): Promise<any> => {
    return await post(`/settlement`, data);
};

export const updateSettlement = async (countryCode: string, code: number, data: SettlementRequest): Promise<void> => {
    await put(`/settlement/${countryCode}/${code}`, data);
};

export const deleteSettlement = async (countryCode: string, code: number): Promise<void> => {
    await del(`/settlement/${countryCode}/${code}`);
};
