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

import SettlementTableSkeleton from "./SettlementTableSkeleton";

import ConfirmDialog from "../../../components/ui/dialog/ConfirmDialog";
import EditSettlementModal from "./EditSettlementModal";

import { useAppDispatch, useAppSelector } from "../../../store/store";



import { showAlert } from "../../../features/ui/uiSlice";
import { fetchSettlements, updateExistingSettlement, createNewSettlement, deleteExistingSettlement } from "../../../features/location/settlement/settlement.slice";

export interface SettlementItem {
    countryCode: string;
    code: number;
    name: string;
    postalCode?: string | null;
}

export interface SettlementForm {
    countryCode: string;
    code: number;
    name: string;
    postalCode?: string;
}

export default function SettlementsTable() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.settlements.items ?? []);
    const total = useAppSelector((s) => s.settlements.totalCount ?? 0);
    const loading = useAppSelector((s) => s.settlements.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

    const [confirmOpen, setConfirmOpen] = useState(false);

    const [selected, setSelected] = useState<{
        countryCode: string;
        code: number;
    } | null>(null);

    const [deleting, setDeleting] = useState(false);

    const [editOpen, setEditOpen] = useState(false);
    const [editItem, setEditItem] = useState<SettlementItem | null>(null);

    const totalPages = Math.max(1, Math.ceil(total / pageSize));


    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadSettlements = async () => {
        setFetching(true);
        setSpinning(true);

        try {
            const result = await dispatch(
                fetchSettlements({
                    pageIndex: page,
                    pageSize,
                })
            ).unwrap();

            const pages = Math.ceil(result.count / pageSize);

            if (page >= pages && pages > 0) {
                setPage(pages - 1);
                return;
            }

            if (pages === 0 && page !== 0) {
                setPage(0);
                return;
            }
        } finally {
            await delay(700);

            setFetching(false);
            setSpinning(false);
        }
    };

    useEffect(() => {
        loadSettlements();
    }, [dispatch, page, pageSize]);

    const toggleDropdown = (id: string) => {
        setOpenDropdownId((prev) => (prev === id ? null : id));
    };

    const closeDropdown = () => {
        setOpenDropdownId(null);
    };

    const handleDelete = (
        countryCode: string,
        code: number
    ) => {
        setSelected({
            countryCode,
            code,
        });

        setConfirmOpen(true);
        closeDropdown();
    };

    const openEdit = (
        item?: SettlementItem
    ) => {
        setEditItem(item ?? null);
        setEditOpen(true);
        closeDropdown();
    };

    const closeEdit = () => {
        setEditItem(null);
        setEditOpen(false);
    };

    const handleSave = async (data: SettlementForm) => {
        try {
            if (editItem) {
                await dispatch(
                    updateExistingSettlement({
                        countryCode: editItem.countryCode,
                        code: editItem.code,
                        request: {
                            countryCode: data.countryCode,
                            code: data.code,
                            name: data.name,
                            postalCode: data.postalCode,
                        },
                    })
                ).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t(
                            "location.settlements.messages.updateSuccess"
                        ),
                    })
                );
            } else {
                await dispatch(
                    createNewSettlement({
                        countryCode: data.countryCode,
                        code: data.code,
                        name: data.name,
                        postalCode: data.postalCode,
                    })
                ).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t(
                            "location.settlements.messages.createSuccess"
                        ),
                    })
                );
            }

            closeEdit();
            await loadSettlements();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        (editItem
                            ? t(
                                "location.settlements.messages.updateError"
                            )
                            : t(
                                "location.settlements.messages.createError"
                            )),
                })
            );
        }
    };

    const confirmDelete = async () => {
        if (!selected) return;

        try {
            setDeleting(true);

            await dispatch(
                deleteExistingSettlement({
                    countryCode: selected.countryCode,
                    code: selected.code,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.settlements.messages.deleteSuccess"
                    ),
                })
            );

            setConfirmOpen(false);
            setSelected(null);

            await loadSettlements();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            "location.settlements.messages.deleteError"
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
                title={t("location.settlements.pageTitle")}
                description={t("location.settlements.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("location.settlements.pageTitle")}
            />

            <div className="space-y-6">
                <ComponentCard
                    title={t("location.settlements.title")}
                >
                    <div className="flex justify-end mb-3">

                        <button
                            onClick={loadSettlements}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`w-5 h-5 ${spinning
                                        ? "animate-spin"
                                        : "rotate-90"
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
                                            {t("location.settlements.fields.country")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.settlements.fields.code")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.settlements.fields.name")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.settlements.fields.actions")}
                                        </TableCell>

                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (

                                        <SettlementTableSkeleton rows={5} />

                                    ) : (

                                        items.map((item) => {

                                            const id = `${item.countryCode}-${item.code}`;

                                            return (
                                                <TableRow
                                                    key={id}
                                                    className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                                >

                                                    <TableCell className="px-5 py-4 text-sm font-semibold">
                                                        {item.countryCode}
                                                    </TableCell>

                                                    <TableCell className="px-5 py-4 text-sm">
                                                        {item.code}
                                                    </TableCell>

                                                    <TableCell className="px-5 py-4 text-sm">
                                                        {item.name}
                                                    </TableCell>

                                                    <TableCell className="px-5 py-4 relative">

                                                        <button
                                                            className="w-8 h-8 flex items-center justify-center"
                                                            onClick={() =>
                                                                toggleDropdown(id)
                                                            }
                                                        >
                                                            ⋮
                                                        </button>

                                                        <Dropdown
                                                            isOpen={
                                                                openDropdownId ===
                                                                id
                                                            }
                                                            onClose={closeDropdown}
                                                            className="w-44"
                                                        >
                                                            <div className="py-2 flex flex-col">

                                                                <button
                                                                    onClick={() =>
                                                                        openEdit(item)
                                                                    }
                                                                    className="px-4 py-2 text-left hover:bg-gray-100"
                                                                >
                                                                    {t(
                                                                        "location.settlements.actions.edit"
                                                                    )}
                                                                </button>

                                                                <button
                                                                    onClick={() =>
                                                                        handleDelete(
                                                                            item.countryCode,
                                                                            item.code
                                                                        )
                                                                    }
                                                                    className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                                >
                                                                    {t(
                                                                        "location.settlements.actions.delete"
                                                                    )}
                                                                </button>

                                                            </div>
                                                        </Dropdown>

                                                    </TableCell>

                                                </TableRow>
                                            );
                                        })

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
                title={t(
                    "location.settlements.messages.deleteTitle"
                )}
                description={t(
                    "location.settlements.messages.deleteDescription"
                )}
                onConfirm={confirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <EditSettlementModal
                isOpen={editOpen}
                item={editItem}
                loading={loading}
                onClose={closeEdit}
                onSave={handleSave}
            />

        </>
    );
}