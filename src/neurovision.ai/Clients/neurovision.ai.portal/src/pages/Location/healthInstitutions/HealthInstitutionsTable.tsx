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

import HealthInstitutionTableSkeleton from "./HealthInstitutionTableSkeleton";
import ConfirmDialog from "../../../components/ui/dialog/ConfirmDialog";
import EditHealthInstitutionModal, {
    HealthInstitutionForm,
} from "./EditHealthInstitutionModal";

import { useAppDispatch, useAppSelector } from "../../../store/store";

import {
    fetchHealthInstitutions,
    deleteExistingHealthInstitution,
    updateExistingHealthInstitution,
    createNewHealthInstitution,
} from "../../../features/location/healthInstitutions/healthInstitution.slice";
import { fetchHealthInstitutionTypes } from "../../../features/location/healthInstitutionsType/healthInstitutionType.slice";
import { fetchCountries } from "../../../features/location/country/country.slice";
import { fetchSettlements } from "../../../features/location/settlement/settlement.slice";

import { showAlert } from "../../../features/ui/uiSlice";


export interface HealthInstitutionItem {
    id: number;
    name: string;
    typeCode: string;
    countryCode: string;
    settlementCode: number;
    address?: string;
    bedCount?: number;
    foundingDate?: string;
    phone?: string;
}


export default function HealthInstitutionsTable() {

    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.healthInstitutions.items);
    const total = useAppSelector((s) => s.healthInstitutions.totalCount);
    const loading = useAppSelector((s) => s.healthInstitutions.loading);

    const healthInstitutionTypes = useAppSelector(
        (s) => s.healthInstitutionTypes.items
    );

    const countries = useAppSelector(
        (s) => s.countries.items
    );

    const settlements = useAppSelector(
        (s) => s.settlements.items
    );

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] = useState<number | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] = useState<number | null>(null);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedItem, setSelectedItem] = useState<HealthInstitutionItem | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadHealthInstitutions = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            const result = await dispatch(
                fetchHealthInstitutions({
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

        } finally {
            await delay(700);

            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {

        loadHealthInstitutions();

    }, [page, pageSize]);



    useEffect(() => {

        dispatch(
            fetchHealthInstitutionTypes({
                pageIndex: 0,
                pageSize: 1000,
            })
        );

        dispatch(
            fetchCountries({
                pageIndex: 0,
                pageSize: 1000,
            })
        );

        dispatch(
            fetchSettlements({
                pageIndex: 0,
                pageSize: 1000,
            })
        );

    }, [dispatch]);

    const toggleDropdown = (id: number) => {
        setOpenDropdownId((prev) => (prev === id ? null : id));
    };

    const closeDropdown = () => {
        setOpenDropdownId(null);
    };

    const handleDeleteClick = (id: number) => {
        setSelectedId(id);
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {
        if (!selectedId) return;

        try {
            setDeleting(true);

            await dispatch(
                deleteExistingHealthInstitution(selectedId)
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("location.healthInstitutions.messages.deleteSuccess"),
                })
            );

            setConfirmOpen(false);
            setSelectedId(null);

            await loadHealthInstitutions();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t("location.healthInstitutions.messages.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    const handleSaveHealthInstitution = async (form: HealthInstitutionForm) => {
        setEditLoading(true);

        try {
            if (selectedItem) {
                await dispatch(
                    updateExistingHealthInstitution({
                        id: selectedItem.id,
                        request: form,
                    })
                ).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t("location.healthInstitutions.messages.updateSuccess"),
                    })
                );
            } else {
                await dispatch(createNewHealthInstitution(form)).unwrap();

                dispatch(
                    showAlert({
                        type: "success",
                        message: t("location.healthInstitutions.messages.createSuccess"),
                    })
                );
            }

            setEditOpen(false);
            setSelectedItem(null);

            await loadHealthInstitutions();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            selectedItem
                                ? "location.healthInstitutions.messages.updateError"
                                : "location.healthInstitutions.messages.createError"
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
                title={t("location.healthInstitutions.pageTitle")}
                description={t("location.healthInstitutions.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("location.healthInstitutions.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("location.healthInstitutions.title")}>
                    <div className="flex justify-end mb-3 gap-2">

                        <button
                            onClick={loadHealthInstitutions}
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
                                            {t("location.healthInstitutions.fields.name")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.healthInstitutions.fields.typeCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.healthInstitutions.fields.countryCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.healthInstitutions.fields.settlementCode")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.healthInstitutions.fields.bedCount")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.healthInstitutions.fields.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <HealthInstitutionTableSkeleton rows={5} />
                                    ) : (
                                        items.map((item) => (
                                            <TableRow
                                                key={item.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {item.name}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.typeCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.countryCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.settlementCode}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.bedCount}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">
                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() => toggleDropdown(item.id)}
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={openDropdownId === item.id}
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
                                                                {t("location.healthInstitutions.actions.edit")}
                                                            </button>

                                                            <button
                                                                onClick={() => handleDeleteClick(item.id)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("location.healthInstitutions.actions.delete")}
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
                title={t("location.healthInstitutions.messages.deleteTitle")}
                description={t("location.healthInstitutions.messages.deleteDescription")}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />
            <EditHealthInstitutionModal
                isOpen={editOpen}
                healthInstitution={selectedItem}
                loading={editLoading}
                healthInstitutionTypes={healthInstitutionTypes}
                countries={countries}
                settlements={settlements}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedItem(null);
                }}
                onSave={handleSaveHealthInstitution}
            />
        </>
    );
}