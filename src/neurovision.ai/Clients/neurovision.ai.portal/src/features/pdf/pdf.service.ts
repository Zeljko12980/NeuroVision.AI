import { get, post, put, del } from "../../api/api";
import { PaginatedPdfTemplateResponse, PdfTemplateResponse, PdfTemplateRequest } from "./pdf.types";


export const getPdfTemplates = async (
    pageIndex: number,
    pageSize: number,
    search?: string
): Promise<PaginatedPdfTemplateResponse> => {
    const query = new URLSearchParams({
        PageIndex: pageIndex.toString(),
        PageSize: pageSize.toString(),
    });

    if (search) {
        query.append("search", search);
    }

    return await get(`/pdf?${query.toString()}`);
};

export const getPdfTemplateById = async (
    id: string
): Promise<PdfTemplateResponse> => {
    return await get(`/pdf/${id}`);
};

export const createPdfTemplate = async (
    data: PdfTemplateRequest
): Promise<{ id: string }> => {
    return await post("/pdf", data);
};

export const updatePdfTemplate = async (
    id: string,
    data: PdfTemplateRequest
): Promise<void> => {
    await put(`/pdf/${id}`, data);
};

export const deletePdfTemplate = async (
    id: string
): Promise<void> => {
    await del(`/pdf/${id}`);
};

// export const generatePdfPreview = async (
//     request: PreviewPdfRequest
// ): Promise<Blob> => {
//     return await post("/pdf-template/preview", request, {
//         responseType: "blob",
//     });
// };