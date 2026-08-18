export interface PdfTemplateRequest {
    code: string;
    name: string;
    htmlContent: string;
    version: number;
    isActive: boolean;
    requiresSignature: boolean;
    signaturePage: number;
}

export interface PdfTemplateResponse {
    id: string;
    code: string;
    name: string;
    htmlContent: string;
    version: number;
    isActive: boolean;
    createdAt: string;
    requiresSignature: boolean;
    signaturePage: number;

}

export interface PaginatedPdfTemplateResponse {
    data: PdfTemplateResponse[];
    count: number;
}

export interface PreviewPdfRequest {
    htmlContent: string;
    data: Record<string, unknown>;
}