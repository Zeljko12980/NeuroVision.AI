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
import { useAppDispatch, useAppSelector } from "../../store/store";
import { deleteUser, fetchUsers, lockUser, unlockUser } from "../../features/user/userSlice";
import { fetchRoles } from "../../features/role/roleSlice";
import { updateUserRolesRequest } from "../../features/user/userService";
import AssignUserRolesModal from "../../components/ui/dialog/AssignUserRolesModal";
import RoleTableSkeleton from "../Auth/RoleTableSkeleton";
import { showAlert } from "../../features/ui/uiSlice";
import { AdminUserDto } from "../../features/user/userService";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";

export default function UsersTable() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const users = useAppSelector((state) => state.users.users);
    const total = useAppSelector((state) => state.users.totalCount);
    const loading = useAppSelector((state) => state.users.loading);
    const roles = useAppSelector((state) => state.roles.roles);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);
    const [search, setSearch] = useState("");
    const [appliedSearch, setAppliedSearch] = useState("");
    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const claims = useAppSelector(selectUserClaims);
    const { userId: currentUserId } = getUserInfoFromClaims(claims || {});

    const [editUser, setEditUser] = useState<AdminUserDto | null>(null);
    const [editLoading, setEditLoading] = useState(false);
    const [lockBusy, setLockBusy] = useState(false);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const loadUsers = async () => {
        setSpinning(true);
        setFetching(true);
        try {
            await dispatch(
                fetchUsers({ pageIndex: page, pageSize, search: appliedSearch })
            ).unwrap();
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadUsers();
    }, [page, pageSize, appliedSearch]);

    useEffect(() => {
        dispatch(fetchRoles({ pageIndex: 0, pageSize: 50 }));
    }, [dispatch]);

    const closeDropdown = () => setOpenDropdownId(null);

    const handleLock = async () => {
        const id = editUser?.id;
        if (!id) return;
        try {
            setLockBusy(true);
            await dispatch(lockUser(id)).unwrap();
            dispatch(showAlert({ type: "success", message: t("users.lockSuccess") }));
            await loadUsers();
            setEditUser((current) =>
                current && current.id === id
                    ? { ...current, isLockedOut: true }
                    : current
            );
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err instanceof Error ? err.message : t("users.lockError"),
                })
            );
        } finally {
            setLockBusy(false);
        }
    };

    const handleUnlock = async () => {
        const id = editUser?.id;
        if (!id) return;
        try {
            setLockBusy(true);
            await dispatch(unlockUser(id)).unwrap();
            dispatch(showAlert({ type: "success", message: t("users.unlockSuccess") }));
            await loadUsers();
            setEditUser((current) =>
                current && current.id === id
                    ? { ...current, isLockedOut: false, lockoutEnd: null }
                    : current
            );
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err instanceof Error ? err.message : t("users.unlockError"),
                })
            );
        } finally {
            setLockBusy(false);
        }
    };

    const handleDeleteClick = (id: string) => {
        closeDropdown();
        if (id === currentUserId) {
            dispatch(showAlert({ type: "error", message: t("users.cannotDeleteSelf") }));
            return;
        }
        setSelectedId(id);
        setConfirmOpen(true);
    };

    const handleConfirmDelete = async () => {
        if (!selectedId) return;
        try {
            setDeleting(true);
            await dispatch(deleteUser(selectedId)).unwrap();
            dispatch(showAlert({ type: "success", message: t("users.deleteSuccess") }));
            setConfirmOpen(false);
            setSelectedId(null);
            await loadUsers();
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err instanceof Error ? err.message : t("users.deleteError"),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <PageMeta title={t("users.pageTitle")} description={t("users.pageDescription")} />
            <PageBreadcrumb pageTitle={t("users.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("users.title")}>
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
                                placeholder={t("users.searchPlaceholder")}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </form>
                        <button
                            onClick={loadUsers}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`w-5 h-5 ${spinning ? "animate-spin" : "rotate-90"}`}
                            />
                        </button>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">
                        <div className="flex-1 overflow-y-auto">
                            <Table>
                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("users.userName")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("users.email")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("users.roles")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("users.status")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("users.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <RoleTableSkeleton rows={5} />
                                    ) : (
                                        users.map((user) => (
                                            <TableRow
                                                key={user.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {user.userName}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {user.email}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {(user.roles ?? []).join(", ") || "—"}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge
                                                        size="sm"
                                                        color={user.isLockedOut ? "error" : "success"}
                                                    >
                                                        {user.isLockedOut
                                                            ? t("users.locked")
                                                            : t("users.active")}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 relative">
                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            setOpenDropdownId((prev) =>
                                                                prev === user.id ? null : user.id
                                                            )
                                                        }
                                                    >
                                                        ⋮
                                                    </button>
                                                    <Dropdown
                                                        isOpen={openDropdownId === user.id}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">
                                                            <button
                                                                onClick={() => {
                                                                    setEditUser(user);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("users.modify")}
                                                            </button>
                                                            {user.id !== currentUserId && (
                                                                <button
                                                                    onClick={() => handleDeleteClick(user.id)}
                                                                    className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                                >
                                                                    {t("users.delete")}
                                                                </button>
                                                            )}
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

            <AssignUserRolesModal
                isOpen={!!editUser}
                user={editUser}
                roles={roles}
                loading={editLoading}
                lockBusy={lockBusy}
                onClose={() => setEditUser(null)}
                onLock={handleLock}
                onUnlock={handleUnlock}
                onSave={async (nextRoles) => {
                    if (!editUser) return;
                    try {
                        setEditLoading(true);
                        await updateUserRolesRequest({
                            userId: editUser.id,
                            roles: nextRoles,
                        });
                        dispatch(
                            showAlert({ type: "success", message: t("users.updateRolesSuccess") })
                        );
                        setEditUser(null);
                        await loadUsers();
                    } catch (err: unknown) {
                        dispatch(
                            showAlert({
                                type: "error",
                                message:
                                    err instanceof Error
                                        ? err.message
                                        : t("users.updateRolesError"),
                            })
                        );
                    } finally {
                        setEditLoading(false);
                    }
                }}
            />

            <ConfirmDialog
                isOpen={confirmOpen}
                title={t("users.deleteTitle")}
                description={t("users.deleteConfirm")}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />
        </>
    );
}
