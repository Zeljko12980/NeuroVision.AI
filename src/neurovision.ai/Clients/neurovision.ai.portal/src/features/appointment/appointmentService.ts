import { get, post, put } from "../../api/api";
import {
    AppointmentCatalogsResponse,
    AppointmentRangeQuery,
    AppointmentResponse,
    CreateAppointmentRequest,
    RescheduleAppointmentRequest,
} from "./appointment.types";

export const getAppointments = async (
    query: AppointmentRangeQuery
): Promise<AppointmentResponse[]> => {
    const params = new URLSearchParams({
        from: query.from,
        to: query.to,
    });

    if (query.patientId) params.append("patientId", query.patientId);
    if (query.doctorId) params.append("doctorId", query.doctorId);

    return await get(`/appointment?${params.toString()}`);
};

export const getAppointmentCatalogs = async (): Promise<AppointmentCatalogsResponse> => {
    return await get("/appointment/catalogs");
};

export const createAppointmentRequest = async (
    payload: CreateAppointmentRequest
): Promise<AppointmentResponse> => {
    return await post("/appointment", payload);
};

export const rescheduleAppointmentRequest = async (
    id: string,
    payload: RescheduleAppointmentRequest
): Promise<AppointmentResponse> => {
    return await put(`/appointment/${id}`, payload);
};

export const cancelAppointmentRequest = async (
    id: string
): Promise<AppointmentResponse> => {
    return await post(`/appointment/${id}/cancel`, {});
};
