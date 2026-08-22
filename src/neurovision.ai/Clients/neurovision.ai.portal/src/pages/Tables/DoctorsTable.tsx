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
import { RefreshIcon } from "../../icons";
import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";
import { Dropdown } from "../../components/ui/dropdown/Dropdown";
import Input from "../../components/form/input/InputField";
import ConfirmDialog from "../../components/ui/dialog/ConfirmDialog";
import DoctorsTableSkeleton from "./DoctorsTableSkeleton";
import ProfileAvatar from "../../components/UserProfile/ProfileAvatar";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { deleteExistingDoctor, fetchDoctors } from "../../features/doctor/doctorSlice";
import { resolveDoctorImageUrl } from "../../features/doctor/doctorService";
import { showAlert } from "../../features/ui/uiSlice";

const statusColor = (code: string) => {
    switch (code) {
        case "ACT":
            return "success" as const;
        case "PEND":
            return "warning" as const;
        case "SUSP":
            return "error" as const;
        default:
            return "light" as const;
    }
};

export default function DoctorsTable() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((state) => state.doctor.items);
    const total = useAppSelector((state) => state.doctor.totalCount);
    const loading = useAppSelector((state) => state.doctor.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);
    const [search, setSearch] = useState("");
    const [appliedSearch, setAppliedSearch] = useState("");
    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const loadDoctors = async () => {
        setSpinning(true);
        setFetching(true);
        try {
            const result = await dispatch(
                fetchDoctors({
                    pageIndex: page,
                    pageSize,
                    search: appliedSearch || undefined,
                })
            ).unwrap();

            const pages = Math.ceil(result.count / pageSize);
            if (page >= pages && pages > 0) {
                setPage(pages - 1);
            } else if (pages === 0 && page !== 0) {
                setPage(0);
            }
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        typeof err === "string"
                            ? err
                            : err instanceof Error
                                ? err.message
                                : t("doctors.messages.loadError"),
                })
            );
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadDoctors();
    }, [page, pageSize, appliedSearch]);

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
            await dispatch(deleteExistingDoctor(selectedId)).unwrap();
            dispatch(
                showAlert({
                    type: "success",
                    message: t("doctors.messages.deleteSuccess"),
                })
            );
            setConfirmOpen(false);
            setSelectedId(null);
            await loadDoctors();
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        typeof err === "string"
                            ? err
                            : err instanceof Error
                                ? err.message
                                : t("doctors.messages.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("doctors.pageTitle")}
                description={t("doctors.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("doctors.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("doctors.title")}>
                    <div className="mb-3 flex items-center justify-between gap-3">
                        <form
                            className="w-full max-w-xs"
                            onSubmit={(e) => {
                                e.preventDefault();
                                setPage(0);
                                setAppliedSearch(search);
                            }}
                        >
                            <Input
                                value={search}
                                placeholder={t("doctors.searchPlaceholder")}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </form>

                        <div className="flex items-center gap-2">
                            <button
                                onClick={loadDoctors}
                                disabled={loading || spinning}
                                className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                            >
                                <RefreshIcon
                                    className={`w-5 h-5 ${spinning ? "animate-spin" : "rotate-90"}`}
                                />
                            </button>
                        </div>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">
                        <div className="flex-1 overflow-y-auto">
                            <Table>
                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.name")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.email")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.specialization")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.hospital")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.status")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.availability")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("doctors.columns.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <DoctorsTableSkeleton rows={5} />
                                    ) : items.length === 0 ? (
                                        <TableRow>
                                            <TableCell className="px-5 py-8 text-sm text-gray-500" colSpan={7}>
                                                {t("doctors.empty")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        items.map((doctor) => (
                                            <TableRow
                                                key={doctor.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    <div className="flex items-center gap-3">
                                                        <div className="h-10 w-10 overflow-hidden rounded-full bg-gray-100 dark:bg-white/10">
                                                            <ProfileAvatar
                                                                src={resolveDoctorImageUrl(doctor.profilePictureUrl)}
                                                                alt={`${doctor.firstName} ${doctor.lastName}`}
                                                                className="h-full w-full object-cover"
                                                            />
                                                        </div>
                                                        <span>
                                                            {doctor.firstName} {doctor.lastName}
                                                        </span>
                                                    </div>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {doctor.email}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {doctor.currentSpecializationCode}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {doctor.currentInstitutionName || "—"}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge size="sm" color={statusColor(doctor.currentStatusCode)}>
                                                        {t(`doctors.status.${doctor.currentStatusCode}`, {
                                                            defaultValue: doctor.currentStatusCode,
                                                        })}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge
                                                        size="sm"
                                                        color={doctor.isAvailable ? "success" : "light"}
                                                    >
                                                        {doctor.isAvailable
                                                            ? t("doctors.availability.available")
                                                            : t("doctors.availability.unavailable")}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 relative">
                                                    <button
                                                        className="dropdown-toggle w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            setOpenDropdownId((prev) =>
                                                                prev === doctor.id ? null : doctor.id
                                                            )
                                                        }
                                                    >
                                                        ⋮
                                                    </button>
                                                    <Dropdown
                                                        isOpen={openDropdownId === doctor.id}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">
                                                            <button
                                                                onClick={() => handleDeleteClick(doctor.id)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("doctors.actions.delete")}
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
                title={t("doctors.messages.deleteTitle")}
                description={t("doctors.messages.deleteDescription")}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />
        </>
    );
}
