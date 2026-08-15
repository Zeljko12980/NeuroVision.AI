import { useMemo, useState } from "react";

export function useClientPagination<T>(items: T[], initialPageSize = 5) {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(initialPageSize);

    const totalPages = Math.max(1, Math.ceil(items.length / pageSize));

    const safePage = Math.min(page, totalPages - 1);

    const slice = useMemo(
        () => items.slice(safePage * pageSize, safePage * pageSize + pageSize),
        [items, safePage, pageSize]
    );

    const setPageSafe = (next: number) => {
        setPage(Math.max(0, Math.min(next, totalPages - 1)));
    };

    const resetPage = () => setPage(0);

    return {
        page: safePage,
        setPage: setPageSafe,
        pageSize,
        setPageSize: (size: number) => {
            setPageSize(size);
            setPage(0);
        },
        totalPages,
        slice,
        total: items.length,
        resetPage,
    };
}
