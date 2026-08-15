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
import EditGovernmentTypeModal, {
    GovernmentTypeForm,
} from "./EditGovernmentTypeModal";

import { useAppDispatch, useAppSelector } from "../../../store/store";

import {
    fetchGovernmentTypes,
    deleteExistingGovernmentType,
    updateExistingGovernmentType,
    createNewGovernmentType,
} from "../../../features/location/governmentTypeSlice";

import { showAlert } from "../../../features/ui/uiSlice";
import GovernmentTypeTableSkeleton from "./GovernmentTypeTableSkeleton";


interface GovernmentTypeItem {
    code: string;
    name: string;
}


export default function GovernmentTypesTable() {

    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.governmentTypes.items);
    const total = useAppSelector((s) => s.governmentTypes.totalCount);
    const loading = useAppSelector((s) => s.governmentTypes.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedCode, setSelectedCode] = useState<string | null>(null);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedItem, setSelectedItem] = useState<GovernmentTypeItem | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadGovernmentTypes = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            const result = await dispatch(
                fetchGovernmentTypes({
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
        loadGovernmentTypes();
    }, [page, pageSize]);

    const toggleDropdown = (code: string) => {
        setOpenDropdownId((prev) => (prev === code ? null : code));
    };

    const closeDropdown = () => {
        setOpenDropdownId(null);
    };

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
                deleteExistingGovernmentType(selectedCode)
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("location.governmentTypes.messages.deleteSuccess"),
                })
            );

            setConfirmOpen(false);
            setSelectedCode(null);

            await loadGovernmentTypes();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t("location.governmentTypes.messages.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    const handleSaveGovernmentType = async (form: GovernmentTypeForm) => {
        setEditLoading(true);

        try {
            if (selectedItem) {
                await dispatch(
                    updateExistingGovernmentType({
                        code: selectedItem.code,
                        request: form,
                    })
                ).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t("location.governmentTypes.messages.updateSuccess"),
                    })
                );
            } else {
                await dispatch(createNewGovernmentType(form)).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t("location.governmentTypes.messages.createSuccess"),
                    })
                );
            }

            setEditOpen(false);
            setSelectedItem(null);

            await loadGovernmentTypes();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            selectedItem
                                ? "location.governmentTypes.messages.updateError"
                                : "location.governmentTypes.messages.createError"
                        ),
                })
            );

            // Re-throw so the modal's handleSubmit doesn't call onClose() —
            // the modal should stay open when the save fails.
            throw err;
        } finally {
            setEditLoading(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("location.governmentTypes.pageTitle")}
                description={t("location.governmentTypes.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("location.governmentTypes.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("location.governmentTypes.title")}>
                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadGovernmentTypes}
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
                                            {t("location.governmentTypes.fields.code")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.governmentTypes.fields.name")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.governmentTypes.fields.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <GovernmentTypeTableSkeleton rows={5} />
                                    ) : (
                                        items.map((item) => (
                                            <TableRow
                                                key={item.code}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {item.code}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.name}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">
                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() => toggleDropdown(item.code)}
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={openDropdownId === item.code}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">
                                                            <button
                                                                onClick={() => {
                                                                    setSelectedItem(item);
                                                                    setEditOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("location.governmentTypes.actions.edit")}
                                                            </button>

                                                            <button
                                                                onClick={() => handleDeleteClick(item.code)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("location.governmentTypes.actions.delete")}
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
                title={t("location.governmentTypes.messages.deleteTitle")}
                description={t("location.governmentTypes.messages.deleteDescription")}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <EditGovernmentTypeModal
                isOpen={editOpen}
                governmentType={selectedItem}
                loading={editLoading}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedItem(null);
                }}
                onSave={handleSaveGovernmentType}
            />
        </>
    );
}