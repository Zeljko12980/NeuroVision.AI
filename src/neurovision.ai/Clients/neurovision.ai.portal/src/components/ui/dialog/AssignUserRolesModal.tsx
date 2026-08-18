import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Modal } from "../modal/index";
import Button from "../button/Button";
import { AdminUserDto } from "../../../features/user/userService";
import { RoleDto } from "../../../features/role/roleService";

const SUPER_ADMIN_ROLE = "SuperAdministrator";

interface AssignUserRolesModalProps {
    isOpen: boolean;
    user: AdminUserDto | null;
    roles: RoleDto[];
    loading?: boolean;
    lockBusy?: boolean;
    onClose: () => void;
    onSave: (roles: string[]) => Promise<void> | void;
    onLock: () => Promise<void> | void;
    onUnlock: () => Promise<void> | void;
}

export default function AssignUserRolesModal({
    isOpen,
    user,
    roles,
    loading,
    lockBusy,
    onClose,
    onSave,
    onLock,
    onUnlock,
}: AssignUserRolesModalProps) {
    const { t } = useTranslation();
    const [selected, setSelected] = useState<string[]>([]);

    useEffect(() => {
        setSelected(user?.roles?.filter((role) => role !== SUPER_ADMIN_ROLE) ?? []);
    }, [user]);

    if (!isOpen || !user) return null;

    const isSuperAdmin = user.roles?.includes(SUPER_ADMIN_ROLE);
    const assignableRoles = roles.filter((role) => role.name !== SUPER_ADMIN_ROLE);

    const toggleRole = (roleName: string) => {
        setSelected((prev) =>
            prev.includes(roleName)
                ? prev.filter((item) => item !== roleName)
                : [...prev, roleName]
        );
    };

    const handleSubmit = async () => {
        if (selected.length === 0) return;
        await onSave(selected);
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-lg">
            <div className="p-6 space-y-5">
                <div>
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                        {t("users.modifyTitle")}
                    </h2>
                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        {user.userName} · {user.email}
                    </p>
                </div>

                <div className="flex items-center justify-between gap-3 rounded-lg border border-gray-200 px-3 py-3 dark:border-gray-700">
                    <div>
                        <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                            {t("users.status")}
                        </p>
                        <p className="text-xs text-gray-500 dark:text-gray-400">
                            {user.isLockedOut ? t("users.locked") : t("users.active")}
                        </p>
                    </div>
                    {!isSuperAdmin && (
                        <Button
                            type="button"
                            size="sm"
                            variant={user.isLockedOut ? "primary" : "outline"}
                            disabled={lockBusy || loading}
                            onClick={user.isLockedOut ? onUnlock : onLock}
                        >
                            {user.isLockedOut
                                ? lockBusy
                                    ? t("users.unlocking")
                                    : t("users.unlock")
                                : lockBusy
                                  ? t("users.locking")
                                  : t("users.lock")}
                        </Button>
                    )}
                </div>

                {!isSuperAdmin && (
                    <div className="space-y-2">
                        {assignableRoles.map((role) => (
                            <label
                                key={role.id}
                                className="flex items-center gap-3 rounded-lg border border-gray-200 px-3 py-2 dark:border-gray-700"
                            >
                                <input
                                    type="checkbox"
                                    checked={selected.includes(role.name)}
                                    onChange={() => toggleRole(role.name)}
                                    className="h-4 w-4 rounded border-gray-300"
                                />
                                <span className="text-sm text-gray-800 dark:text-white/90">
                                    {role.name}
                                </span>
                            </label>
                        ))}
                    </div>
                )}

                <div className="flex justify-end gap-2 pt-2">
                    <Button variant="outline" type="button" onClick={onClose} disabled={loading}>
                        {t("users.cancel")}
                    </Button>
                    {!isSuperAdmin && (
                        <Button
                            type="button"
                            onClick={handleSubmit}
                            disabled={loading || selected.length === 0}
                        >
                            {loading ? t("users.saving") : t("users.saveRoles")}
                        </Button>
                    )}
                </div>
            </div>
        </Modal>
    );
}
