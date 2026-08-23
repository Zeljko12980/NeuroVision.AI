export interface EntityTablePageResponse {
    data: Record<string, unknown>[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
