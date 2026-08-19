import { useState } from "react";
import { useTranslation } from "react-i18next";
import Button from "../button/Button";
import { Dropdown } from "../dropdown/Dropdown";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    pageSizeOptions?: number[];
    onPageChange: (page: number) => void;
    onPageSizeChange: (size: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({
    currentPage,
    totalPages,
    pageSize,
    pageSizeOptions = [5, 10, 20, 50],
    onPageChange,
    onPageSizeChange,
}) => {
    const { t } = useTranslation();
    const [open, setOpen] = useState(false);

    const getPages = () => {
        const pages: (number | null)[] = [];

        for (let i = 1; i <= 5; i++) {
            pages.push(i <= totalPages ? i : null);
        }

        return pages;
    };

    const pages = getPages();

    const handlePageSizeChange = (size: number) => {
        onPageSizeChange(size);
        onPageChange(1);
        setOpen(false);
    };

    return (
        <div className="grid grid-cols-3 items-center px-4 py-3 min-h-[64px] border-t border-gray-100 dark:border-white/[0.05]">

            <div className="flex items-center gap-3 justify-self-start">
                <span className="text-sm text-gray-500 whitespace-nowrap">
                    {t("common.pagination.pageOf", { current: currentPage, total: totalPages })}
                </span>

                <div className="relative z-50">
                    <Button
                        size="sm"
                        variant="outline"
                        className="h-8 px-3 text-xs"
                        onClick={() => setOpen(!open)}
                    >
                        {t("common.pagination.perPage", { size: pageSize })}
                    </Button>

                    <Dropdown
                        isOpen={open}
                        onClose={() => setOpen(false)}
                        className="w-28"
                    >
                        <div className="py-2 flex flex-col">
                            {pageSizeOptions.map((size) => (
                                <button
                                    key={size}
                                    onClick={() => handlePageSizeChange(size)}
                                    className={`px-3 py-2 text-sm text-left hover:bg-gray-100 dark:hover:bg-white/5 ${size === pageSize
                                            ? "text-black dark:text-white font-semibold"
                                            : "text-gray-500"
                                        }`}
                                >
                                    {t("common.pagination.perPage", { size })}
                                </button>
                            ))}
                        </div>
                    </Dropdown>
                </div>
            </div>

            <div className="flex items-center gap-2 justify-self-center">

                <Button
                    size="sm"
                    variant="outline"
                    className="w-9 h-9 p-0 flex items-center justify-center"
                    onClick={() => onPageChange(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    ‹
                </Button>

                {pages.map((page, idx) => (
                    <Button
                        key={idx}
                        size="sm"
                        variant={page === currentPage ? "primary" : "outline"}
                        className="w-9 h-9 p-0 flex items-center justify-center"
                        onClick={() => page && onPageChange(page)}
                        disabled={!page}
                    >
                        {page ?? "•"}
                    </Button>
                ))}

                <Button
                    size="sm"
                    variant="outline"
                    className="w-9 h-9 p-0 flex items-center justify-center"
                    onClick={() => onPageChange(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    ›
                </Button>

            </div>

            <div className="justify-self-end" />

        </div>
    );
};

export default Pagination;