import { get } from "../../api/api";


export type HealthStatus =
    | "Healthy"
    | "Unhealthy"
    | "Degraded";



export interface ServiceHealth {
    name: string;
    status: HealthStatus;
    error: string | null;
    duration: string;
}



export interface PaginatedResult<T> {
    pageIndex: number;
    pageSize: number;
    count: number;
    data: T[];
}



export interface SystemHealthResponse {
    status: HealthStatus;
    services: PaginatedResult<ServiceHealth>;
    healthyCount?: number;
    unhealthyCount?: number;
}



export const getSystemHealth = async (
    pageIndex = 0,
    pageSize = 10
): Promise<SystemHealthResponse> => {

    return await get(
        `/system/health?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
};