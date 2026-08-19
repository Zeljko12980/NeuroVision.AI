import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";

import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";
import { Dropdown } from "../../components/ui/dropdown/Dropdown";
import ConfirmDialog from "../../components/ui/dialog/ConfirmDialog";

import { RefreshIcon } from "../../icons";

import { useAppDispatch, useAppSelector } from "../../store/store";
import {
    fetchCertificates,
    removeCertificate,
} from "../../features/certificate/certificateSlice";
import { CertificateResponse } from "../../features/certificate/certificate.types";
import { showAlert } from "../../features/ui/uiSlice";
import { getUsersRequest } from "../../features/user/userService";

import CertificateTableSkeleton from "./CertificateTableSkeleton";
import CertificateDetailsDialog from "./CertificateDetailsDialog";

export default function CertificateTable() {
    const { t, i18n } = useTranslation();
    const dispatch = useAppDispatch();

    const certificates = useAppSelector((state) => state.certificate.certificates);
    const total = useAppSelector((state) => state.certificate.totalCount);
    const loading = useAppSelector((state) => state.certificate.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [deleting, setDeleting] = useState(false);
    const [detailsOpen, setDetailsOpen] = useState(false);
    const [selectedCertificate, setSelectedCertificate] =
        useState<CertificateResponse | null>(null);
    const [userNames, setUserNames] = useState<Record<string, string>>({});

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadCertificates = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            await Promise.all([
                dispatch(
                    fetchCertificates({
                        pageIndex: page,
                        pageSize,
                    })
                ).unwrap(),
                delay(600),
            ]);
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        typeof err === "string" && err.trim()
                            ? err
                            : t("certificate.messages.loadError"),
                })
            );
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadCertificates();
    }, [page, pageSize]);

    useEffect(() => {
        const loadUsers = async () => {
            try {
                const response = await getUsersRequest(0, 200);
                const names: Record<string, string> = {};
                for (const user of response.data) {
                    names[user.id] = user.userName || user.email;
                }
                setUserNames(names);
            } catch {
                setUserNames({});
            }
        };

        loadUsers();
    }, []);

    const toggleDropdown = (id: string) => {
        setOpenDropdownId((prev) => (prev === id ? null : id));
    };

    const closeDropdown = () => setOpenDropdownId(null);

    const handleDeleteClick = (id: string) => {
        setSelectedId(id);
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {
        if (!selectedId) return;

        try {
            setDeleting(true);
            await dispatch(removeCertificate(selectedId)).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("certificate.messages.deleteSuccess"),
                })
            );

            setConfirmOpen(false);
            setSelectedId(null);
            await loadCertificates();
        } catch {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("certificate.messages.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    const statusLabel = (certificate: CertificateResponse) => {
        if (certificate.isExpired) return t("certificate.status.expired");
        if (certificate.isDefault) return t("certificate.status.default");
        return t("certificate.status.valid");
    };

    const doctorLabel = (certificate: CertificateResponse) => {
        if (!certificate.userId) return t("certificate.status.unassigned");
        return userNames[certificate.userId] ?? certificate.userId;
    };

    const statusColor = (certificate: CertificateResponse) => {
        if (certificate.isExpired) return "error";
        if (certificate.isDefault) return "info";
        return "success";
    };

    return (
        <>
            <PageMeta
                title={t("certificate.pageTitle")}
                description={t("certificate.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("certificate.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("certificate.title")}>
                    <div className="mb-3 flex justify-end">
                        <button
                            onClick={loadCertificates}
                            disabled={loading || spinning}
                            className="flex h-9 w-9 items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`bg-color-gray h-5 w-5 ${
                                    spinning ? "animate-spin" : "rotate-90"
                                }`}
                            />
                        </button>
                    </div>

                    <div className="flex h-[520px] flex-col rounded-xl border border-gray-200 dark:border-white/[0.05]">
                        <div className="flex-1 overflow-y-auto">
                            <Table>
                                <TableHeader className="sticky top-0 z-10 border-b border-gray-100 bg-white dark:border-white/[0.05] dark:bg-gray-900">
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.name")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.user")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.validTo")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.signature")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.status")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("certificate.columns.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <CertificateTableSkeleton rows={5} />
                                    ) : certificates.length === 0 ? (
                                        <TableRow>
                                            <TableCell
                                                colSpan={6}
                                                className="py-10 text-center text-gray-500"
                                            >
                                                {t("certificate.messages.empty")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        certificates.map((certificate) => (
                                            <TableRow
                                                key={certificate.id}
                                                className="transition hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                <TableCell className="px-5 py-4 font-semibold">
                                                    {certificate.name}
                                                </TableCell>
                                                <TableCell className="max-w-xs truncate px-5 py-4">
                                                    {doctorLabel(certificate)}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    {new Date(certificate.validTo).toLocaleDateString(
                                                        i18n.language
                                                    )}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    {certificate.hasSignatureImage
                                                        ? t("common.yes")
                                                        : t("common.no")}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge size="sm" color={statusColor(certificate)}>
                                                        {statusLabel(certificate)}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="relative px-5 py-4">
                                                    <button
                                                        className="flex h-8 w-8 items-center justify-center"
                                                        onClick={() => toggleDropdown(certificate.id)}
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={openDropdownId === certificate.id}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="flex flex-col py-2">
                                                            <button
                                                                onClick={() => {
                                                                    setSelectedCertificate(certificate);
                                                                    setDetailsOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("certificate.actions.view")}
                                                            </button>
                                                            <button
                                                                onClick={() =>
                                                                    handleDeleteClick(certificate.id)
                                                                }
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("certificate.actions.delete")}
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

                        <div className="shrink-0 border-t border-gray-100 dark:border-white/[0.05]">
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
                title={t("certificate.dialogs.deleteTitle")}
                description={t("certificate.dialogs.deleteDescription")}
                onConfirm={handleConfirmDelete}
                onCancel={() => {
                    setConfirmOpen(false);
                    setSelectedId(null);
                }}
                loading={deleting}
            />

            <CertificateDetailsDialog
                isOpen={detailsOpen}
                certificate={selectedCertificate}
                onClose={() => {
                    setDetailsOpen(false);
                    setSelectedCertificate(null);
                }}
            />
        </>
    );
}
