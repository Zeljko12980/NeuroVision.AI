export interface SettlementRequest {
    countryCode: string;
    code: number;
    name: string;
    postalCode?: string | null;
}

export interface SettlementResponse {
    countryCode: string;
    code: number;
    name: string;
    postalCode?: string | null;
}

export interface PaginatedSettlementResponse {
    data: SettlementResponse[];
    count: number;
}