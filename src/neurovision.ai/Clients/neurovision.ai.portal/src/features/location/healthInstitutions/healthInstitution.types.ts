export interface HealthInstitutionRequest {

    name: string;

    typeCode: string;

    countryCode: string;

    settlementCode: number;

    address?: string | null;

    bedCount?: number | null;

    foundingDate?: string | null;

    phone?: string | null;

}


export interface HealthInstitutionResponse {

    id: number;

    name: string;

    typeCode: string;

    countryCode: string;

    settlementCode: number;

    address: string | null;

    bedCount: number | null;

    foundingDate: string | null;

    phone: string | null;

}


export interface PaginatedHealthInstitutionResponse {

    data: HealthInstitutionResponse[];

    count: number;

}