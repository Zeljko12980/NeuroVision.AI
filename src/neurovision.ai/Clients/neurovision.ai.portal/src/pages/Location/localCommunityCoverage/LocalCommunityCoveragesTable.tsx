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
import EditLocalCommunityCoverageModal from "./EditLocalCommunityCoverageModal";

import { useAppDispatch, useAppSelector } from "../../../store/store";
import {
    fetchLocalCommunityCoverages,
    deleteExistingLocalCommunityCoverage,
    updateExistingLocalCommunityCoverage,
} from "../../../features/location/localCommunityCoverage/localCommunityCoverage.slice";

import { LocalCommunityCoverageResponse, LocalCommunityCoverageKey } from "../../../features/location/localCommunityCoverage/localCommunityCoverage.types";
import { showAlert } from "../../../features/ui/uiSlice";
import LocalCommunityCoverageTableSkeleton from "./LocalCommunityCoverageTableSkeleton";

export default function LocalCommunityCoveragesTable() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.localCommunityCoverages.items);
    const total = useAppSelector((s) => s.localCommunityCoverages.totalCount);
    const loading = useAppSelector((s) => s.localCommunityCoverages.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] =
        useState<string | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedItem, setSelectedItem] = useState<LocalCommunityCoverageResponse | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedKey, setSelectedKey] =
        useState<LocalCommunityCoverageKey | null>(null);
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const getRowId = (item: LocalCommunityCoverageResponse) =>
        [item.countryCode, item.municipalityCode, item.localCommunityIdentifier, item.settlementCode].join("-");

    const loadItems = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            const result = await dispatch(
                fetchLocalCommunityCoverages({
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
        loadItems();
    }, [page, pageSize]);

    const toggleDropdown = (id: string) => {
        setOpenDropdownId((prev) =>
            prev === id ? null : id
        );
    };

    const closeDropdown = () => setOpenDropdownId(null);

    const handleDeleteClick = (item: LocalCommunityCoverageResponse) => {
        setSelectedKey({ countryCode: item.countryCode, municipalityCode: item.municipalityCode, localCommunityIdentifier: item.localCommunityIdentifier, settlementCode: item.settlementCode });
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {
        if (!selectedKey) return;

        try {
            setDeleting(true);

            await dispatch(
                deleteExistingLocalCommunityCoverage(selectedKey)
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.localCommunityCoverages.messages.deleteSuccess"
                    ),
                })
            );

            setConfirmOpen(false);
            setSelectedKey(null);

            await loadItems();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            "location.localCommunityCoverages.messages.deleteError"
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
                title={t("location.localCommunityCoverages.pageTitle")}
                description={t("location.localCommunityCoverages.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("location.localCommunityCoverages.pageTitle")}
            />

            <div className="space-y-6">
                <ComponentCard title={t("location.localCommunityCoverages.title")}>

                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadItems}
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
                                            {t("location.localCommunityCoverages.columns.countryCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.localCommunityCoverages.columns.municipalityCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.localCommunityCoverages.columns.localCommunityIdentifier")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.localCommunityCoverages.columns.settlementCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.localCommunityCoverages.columns.actions")}
                                        </TableCell>

                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (
                                        <LocalCommunityCoverageTableSkeleton rows={5} />
                                    ) : (
                                        items.map((item) => (
                                            <TableRow
                                                key={getRowId(item)}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >

                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {item.countryCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.municipalityCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.localCommunityIdentifier}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.settlementCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">

                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            toggleDropdown(
                                                                getRowId(item)
                                                            )
                                                        }
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={
                                                            openDropdownId ===
                                                            getRowId(item)
                                                        }
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedItem(
                                                                        item
                                                                    );
                                                                    setEditOpen(
                                                                        true
                                                                    );
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.localCommunityCoverages.actions.edit"
                                                                )}
                                                            </button>

                                                            <button
                                                                onClick={() =>
                                                                    handleDeleteClick(
                                                                        item
                                                                    )
                                                                }
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.localCommunityCoverages.actions.delete"
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
                title={t("location.localCommunityCoverages.messages.deleteTitle")}
                description={t(
                    "location.localCommunityCoverages.messages.deleteDescription"
                )}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <EditLocalCommunityCoverageModal
                isOpen={editOpen}
                item={selectedItem}
                loading={editLoading}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedItem(null);
                }}
                onSave={async (updated) => {
                    try {
                        setEditLoading(true);

                        await dispatch(
                            updateExistingLocalCommunityCoverage({ key: { countryCode: updated.countryCode, municipalityCode: updated.municipalityCode, localCommunityIdentifier: updated.localCommunityIdentifier, settlementCode: updated.settlementCode }, request: updated })
                        ).unwrap();

                        dispatch(
                            showAlert({
                                type: "success",
                                message: t(
                                    "location.localCommunityCoverages.messages.updateSuccess"
                                ),
                            })
                        );

                        setEditOpen(false);
                        setSelectedItem(null);

                        await loadItems();
                    } catch (err: any) {
                        dispatch(
                            showAlert({
                                type: "error",
                                message:
                                    err?.message ??
                                    t(
                                        "location.localCommunityCoverages.messages.updateError"
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
