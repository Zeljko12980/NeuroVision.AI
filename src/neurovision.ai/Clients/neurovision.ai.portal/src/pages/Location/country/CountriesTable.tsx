import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../../components/common/PageMeta";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import ComponentCard from "../../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../../components/ui/table";

import { RefreshIcon } from "../../../icons";

import Pagination from "../../../components/ui/pagination/Pagination";
import { Dropdown } from "../../../components/ui/dropdown/Dropdown";


import ConfirmDialog from "../../../components/ui/dialog/ConfirmDialog";
import EditCountryModal from "./EditCountryModal";

import { useAppDispatch, useAppSelector } from "../../../store/store";
import {
    fetchCountries,
    deleteExistingCountry,
    updateExistingCountry,
} from "../../../features/location/country/country.slice";

import { showAlert } from "../../../features/ui/uiSlice";
import CountryTableSkeleton from "./CountryTableSkeleton";

export default function CountriesTable() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.countries.items);
    const total = useAppSelector((s) => s.countries.totalCount);
    const loading = useAppSelector((s) => s.countries.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] =
        useState<string | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedCountry, setSelectedCountry] = useState<any | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedCode, setSelectedCode] =
        useState<string | null>(null);
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadCountries = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            const result = await dispatch(
                fetchCountries({
                    pageIndex: page,
                    pageSize,
                })
            ).unwrap();


            const totalPages = Math.ceil(
                result.count / pageSize
            );

            if (
                page >= totalPages &&
                totalPages > 0
            ) {
                setPage(totalPages - 1);
                return;
            }


            if (
                totalPages === 0 &&
                page !== 0
            ) {
                setPage(0);
                return;
            }


            await delay(700);

        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadCountries();
    }, [page, pageSize]);

    const toggleDropdown = (code: string) => {
        setOpenDropdownId((prev) =>
            prev === code ? null : code
        );
    };

    const closeDropdown = () => setOpenDropdownId(null);

    const handleDeleteClick = (code: string) => {
        setSelectedCode(code);
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {
        if (!selectedCode) return;

        try {
            setDeleting(true);

            await dispatch(
                deleteExistingCountry(selectedCode)
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.countries.messages.deleteSuccess"
                    ),
                })
            );

            setConfirmOpen(false);
            setSelectedCode(null);

            await loadCountries();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            "location.countries.messages.deleteError"
                        ),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("location.countries.pageTitle")}
                description={t("location.countries.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("location.countries.pageTitle")}
            />

            <div className="space-y-6">
                <ComponentCard title={t("location.countries.title")}>

                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadCountries}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`w-5 h-5 bg-color-gray ${spinning ? "animate-spin" : "rotate-90"
                                    }`}
                            />
                        </button>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">

                        <div className="flex-1 overflow-y-auto">

                            <Table>

                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">

                                    <TableRow>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.countries.columns.code")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.countries.columns.name")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.countries.columns.founding")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.countries.columns.calling")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.countries.columns.actions")}
                                        </TableCell>

                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (
                                        <CountryTableSkeleton rows={5} />
                                    ) : (
                                        items.map((country) => (
                                            <TableRow
                                                key={country.code}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {country.code}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {country.name}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {country.foundingDate
                                                        ? new Date(
                                                            country.foundingDate
                                                        ).toLocaleDateString()
                                                        : ""}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {country.callingCode ?? "-"}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">

                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            toggleDropdown(
                                                                country.code
                                                            )
                                                        }
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={
                                                            openDropdownId ===
                                                            country.code
                                                        }
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedCountry(
                                                                        country
                                                                    );
                                                                    setEditOpen(
                                                                        true
                                                                    );
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.countries.actions.edit"
                                                                )}
                                                            </button>

                                                            <button
                                                                onClick={() =>
                                                                    handleDeleteClick(
                                                                        country.code
                                                                    )
                                                                }
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.countries.actions.delete"
                                                                )}
                                                            </button>

                                                        </div>
                                                    </Dropdown>

                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}

                                </TableBody>

                            </Table>

                        </div>

                        <div className="border-t border-gray-100 dark:border-white/[0.05] shrink-0">
                            <Pagination
                                currentPage={page + 1}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={(p) => setPage(p - 1)}
                                onPageSizeChange={(size) => {
                                    setPageSize(size);
                                    setPage(0);
                                }}
                            />
                        </div>

                    </div>

                </ComponentCard>
            </div>

            <ConfirmDialog
                isOpen={confirmOpen}
                title={t("location.countries.messages.deleteTitle")}
                description={t(
                    "location.countries.messages.deleteDescription"
                )}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <EditCountryModal
                isOpen={editOpen}
                country={selectedCountry}
                loading={editLoading}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedCountry(null);
                }}
                onSave={async (updatedCountry) => {
                    try {
                        setEditLoading(true);

                        await dispatch(
                            updateExistingCountry({
                                code: updatedCountry.code,
                                request: {
                                    name: updatedCountry.name,
                                    foundingDate: updatedCountry.foundingDate,
                                    capitalSettlementCode: updatedCountry.capitalSettlementCode,
                                    governmentTypeCode: updatedCountry.governmentTypeCode,
                                    callingCode: updatedCountry.callingCode,
                                    anthem: updatedCountry.anthem,
                                    coatOfArms: updatedCountry.coatOfArms,
                                    flag: updatedCountry.flag,
                                    code: updatedCountry.code,
                                },
                            })
                        ).unwrap();

                        dispatch(
                            showAlert({
                                type: "success",
                                message: t(
                                    "location.countries.messages.updateSuccess"
                                ),
                            })
                        );

                        setEditOpen(false);
                        setSelectedCountry(null);

                        await loadCountries();
                    } catch (err: any) {
                        dispatch(
                            showAlert({
                                type: "error",
                                message:
                                    err?.message ??
                                    t(
                                        "location.countries.messages.updateError"
                                    ),
                            })
                        );
                    } finally {
                        setEditLoading(false);
                    }
                }}
            />
        </>
    );
}