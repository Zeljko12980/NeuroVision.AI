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

import { useAppDispatch, useAppSelector } from "../../store/store";
import { fetchRoles, deleteRole, updateRole } from "../../features/role/roleSlice";
import ConfirmDialog from "../../components/ui/dialog/ConfirmDialog";
import EditRoleModal from "../../components/ui/dialog/EditRoleModal";
import { showAlert } from "../../features/ui/uiSlice";
import RoleTableSkeleton from "../Auth/RoleTableSkeleton";

export default function RolesPage() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const roles = useAppSelector((state) => state.roles.roles);
    const total = useAppSelector((state) => state.roles.totalCount);
    const loading = useAppSelector((state) => state.roles.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedRole, setSelectedRole] = useState<any | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

    const loadRoles = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            await Promise.all([
                dispatch(fetchRoles({ pageIndex: page, pageSize })).unwrap(),
                delay(700)
            ]);
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadRoles();
    }, [page, pageSize]);

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

            await dispatch(deleteRole(selectedId)).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("roles.deleteSuccess"),
                })
            );

            setConfirmOpen(false);
            setSelectedId(null);

            await loadRoles();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message || t("roles.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("roles.pageTitle")}
                description={t("roles.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("roles.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("roles.title")}>

                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadRoles}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon className={`w-5 h-5 bg-color-gray ${spinning ? "animate-spin" : "rotate-90"}`} />
                        </button>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">

                        <div className="flex-1 overflow-y-auto">

                            <Table>

                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">

                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("roles.role")}
                                        </TableCell>

                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("roles.description")}
                                        </TableCell>

                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("roles.users")}
                                        </TableCell>

                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("roles.status")}
                                        </TableCell>

                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("roles.actions")}
                                        </TableCell>
                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (
                                        <RoleTableSkeleton rows={5} />
                                    ) : (
                                        roles.map((role) => (
                                            <TableRow
                                                key={role.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {role.name}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {role.description}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {role.userCount ?? 0}
                                                </TableCell>

                                                <TableCell className="px-5 py-4">
                                                    <Badge
                                                        size="sm"
                                                        color={role.status === "Active" ? "success" : "error"}
                                                    >
                                                        {role.status}
                                                    </Badge>
                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">

                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() => toggleDropdown(role.id)}
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={openDropdownId === role.id}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedRole(role);
                                                                    setEditOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("roles.edit")}
                                                            </button>

                                                            <button
                                                                onClick={() => handleDeleteClick(role.id)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("roles.delete")}
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
                title={t("roles.deleteTitle")}
                description={t("roles.deleteConfirm")}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <EditRoleModal
                isOpen={editOpen}
                role={selectedRole}
                loading={editLoading}
                onClose={() => setEditOpen(false)}
                onSave={async (updatedRole) => {
                    try {
                        setEditLoading(true);

                        await dispatch(updateRole({
                            id: updatedRole.id,
                            roleName: updatedRole.name,
                            description: updatedRole.description,
                        })).unwrap();

                        dispatch(
                            showAlert({
                                type: "success",
                                message: t("roles.updateSuccess"),
                            })
                        );

                        setEditOpen(false);
                        setSelectedRole(null);
                        await loadRoles();

                    } catch (err: any) {
                        dispatch(
                            showAlert({
                                type: "error",
                                message: err?.message || t("roles.updateError"),
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