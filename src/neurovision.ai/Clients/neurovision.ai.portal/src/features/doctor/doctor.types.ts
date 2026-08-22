export interface DoctorResponse {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    licenseNumber: string;
    licenseAuthorityCode: string | null;
    currentSpecializationCode: string;
    currentStatusCode: string;
    profilePictureUrl: string | null;
    bio: string | null;
    currentHealthInstitutionId: number | null;
    currentInstitutionName: string | null;
    isAvailable: boolean;
    lastActive: string;
    averageRating: number;
    totalReviews: number;
    createdAt: string;
}

export interface CreateDoctorRequest {
    firstName: string;
    lastName: string;
    licenseNumber: string;
    licenseAuthorityCode?: string;
    specialization: string;
    email: string;
    phoneNumber: string;
    languages: string;
    bio?: string;
    degrees?: string;
    hospital?: string;
    healthInstitutionId?: number;
    isAvailable: boolean;
    autoActivate: boolean;
    picture?: File;
}

export interface CatalogItem {
    code: string;
    name: string;
    description?: string | null;
}

export interface DoctorCatalogsResponse {
    specializations: CatalogItem[];
    languages: CatalogItem[];
    degreeTypes: CatalogItem[];
    licenseAuthorities: CatalogItem[];
}

export interface PaginatedDoctorResponse {
    data: DoctorResponse[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
