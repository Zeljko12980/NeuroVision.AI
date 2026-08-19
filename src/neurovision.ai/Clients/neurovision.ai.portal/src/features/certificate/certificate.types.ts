export interface CertificateResponse {
    id: string;
    name: string;
    userId?: string | null;
    subject: string;
    issuer: string;
    thumbprint: string;
    serialNumber: string;
    validFrom: string;
    validTo: string;
    fileName: string;
    filePath: string;
    signatureImagePath?: string | null;
    hasSignatureImage: boolean;
    isDefault: boolean;
    isExpired: boolean;
}

export interface PaginatedCertificateResponse {
    data: CertificateResponse[];
    count: number;
}

export interface UploadCertificateRequest {
    userId: string;
    name: string;
    password?: string;
    file: File;
    signatureImage: File;
}
