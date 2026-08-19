import { del, get, post } from "../../api/api";
import {
    CertificateResponse,
    PaginatedCertificateResponse,
    UploadCertificateRequest,
} from "./certificate.types";

export const getCertificates = async (
    pageIndex: number,
    pageSize: number
): Promise<PaginatedCertificateResponse> => {
    const query = new URLSearchParams({
        PageIndex: pageIndex.toString(),
        PageSize: pageSize.toString(),
    });

    return await get(`/certificate?${query.toString()}`);
};

export const getCertificateById = async (
    id: string
): Promise<CertificateResponse> => {
    return await get(`/certificate/${id}`);
};

export const uploadCertificate = async (
    data: UploadCertificateRequest
): Promise<CertificateResponse> => {
    const formData = new FormData();
    formData.append("userId", data.userId);
    formData.append("name", data.name);
    formData.append("file", data.file);
    formData.append("signatureImage", data.signatureImage);

    if (data.password) {
        formData.append("password", data.password);
    }

    return await post("/certificate", formData);
};

export const deleteCertificate = async (id: string): Promise<void> => {
    await del(`/certificate/${id}`);
};
