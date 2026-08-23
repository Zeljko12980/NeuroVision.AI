export interface PatientResponse {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    genderCode: string;
    bloodTypeCode: string | null;
    nationalId: string | null;
    currentStatusCode: string;
    profilePictureUrl: string | null;
    notes: string | null;
    currentHealthInstitutionId: number | null;
    currentInstitutionName: string | null;
    assignedDoctorId: string | null;
    currentInsurancePayerCode: string | null;
    currentInsurancePolicyNumber: string | null;
    addressLine: string | null;
    settlementId: number | null;
    municipalityId: number | null;
    countryId: number | null;
    heightCm: number | null;
    weightKg: number | null;
    lastActive: string;
    createdAt: string;
}

export interface CreatePatientRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    dateOfBirth: string;
    gender: string;
    bloodType?: string;
    nationalId?: string;
    languages: string;
    allergies?: string;
    conditions?: string;
    notes?: string;
    hospital?: string;
    healthInstitutionId?: number;
    assignedDoctorId?: string;
    insurancePayerCode?: string;
    insurancePolicyNumber?: string;
    addressLine?: string;
    heightCm?: number;
    weightKg?: number;
    emergencyContactName?: string;
    emergencyContactPhone?: string;
    emergencyRelationshipCode?: string;
    autoActivate: boolean;
    picture?: File;
}

export interface CatalogItem {
    code: string;
    name: string;
    description?: string | null;
}

export interface PatientCatalogsResponse {
    statuses: CatalogItem[];
    genders: CatalogItem[];
    bloodTypes: CatalogItem[];
    languages: CatalogItem[];
    allergies: CatalogItem[];
    conditions: CatalogItem[];
    insurancePayers: CatalogItem[];
    relationshipTypes: CatalogItem[];
    consentTypes: CatalogItem[];
}

export interface PaginatedPatientResponse {
    data: PatientResponse[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
