export interface HealthInstitutionTypeRequest {
    code: string;
    name: string;
}

export interface HealthInstitutionTypeResponse {
    code: string;
    name: string;
}

export interface PaginatedHealthInstitutionTypeResponse {
    data: HealthInstitutionTypeResponse[];
    count: number;
}