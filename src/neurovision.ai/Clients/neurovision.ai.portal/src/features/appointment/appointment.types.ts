export interface AppointmentResponse {
    id: string;
    patientId: string;
    doctorId: string;
    typeCode: string;
    statusCode: string;
    startsAt: string;
    endsAt: string;
    title: string;
    notes?: string | null;
    healthInstitutionId?: number | null;
    createdAt: string;
    cancelledAt?: string | null;
    completedAt?: string | null;
}

export interface CatalogItem {
    code: string;
    name: string;
    description?: string | null;
}

export interface AppointmentCatalogsResponse {
    types: CatalogItem[];
    statuses: CatalogItem[];
}

export interface CreateAppointmentRequest {
    patientId: string;
    doctorId: string;
    typeCode: string;
    startsAt: string;
    endsAt: string;
    title: string;
    notes?: string;
    healthInstitutionId?: number;
}

export interface RescheduleAppointmentRequest {
    startsAt: string;
    endsAt: string;
    title: string;
    notes?: string;
}

export interface AppointmentRangeQuery {
    from: string;
    to: string;
    patientId?: string;
    doctorId?: string;
}
