import { FormEvent, useState } from "react";
import { useTranslation } from "react-i18next";

import Button from "../ui/button/Button";
import Input from "../form/input/InputField";
import Label from "../form/Label";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { changePassword } from "../../features/auth/authSlice";
import { showAlert } from "../../features/ui/uiSlice";

export default function ChangePasswordCard() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const { changePasswordLoading } = useAppSelector((state) => state.auth);

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [localError, setLocalError] = useState("");

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        if (newPassword.length < 8) {
            setLocalError(t("changePassword.tooShort"));
            return;
        }

        if (newPassword !== confirmPassword) {
            setLocalError(t("changePassword.mismatch"));
            return;
        }

        if (newPassword === currentPassword) {
            setLocalError(t("changePassword.sameAsCurrent"));
            return;
        }

        setLocalError("");

        const result = await dispatch(changePassword({ currentPassword, newPassword }));
        if (changePassword.fulfilled.match(result)) {
            setCurrentPassword("");
            setNewPassword("");
            setConfirmPassword("");
            dispatch(showAlert({ message: t("changePassword.success"), type: "success" }));
        } else {
            dispatch(
                showAlert({
                    message: (result.payload as string) || t("changePassword.error"),
                    type: "error",
                })
            );
        }
    };

    return (
        <div className="p-5 border border-gray-200 rounded-2xl dark:border-gray-800 lg:p-6">
            <h4 className="mb-6 text-lg font-semibold text-gray-800 dark:text-white/90">
                {t("changePassword.title")}
            </h4>

            <form onSubmit={handleSubmit} className="space-y-5 max-w-md">
                {localError && (
                    <p className="text-sm text-error-500">{localError}</p>
                )}

                <div>
                    <Label>{t("changePassword.current")} *</Label>
                    <Input
                        type="password"
                        value={currentPassword}
                        onChange={(e) => setCurrentPassword(e.target.value)}
                    />
                </div>

                <div>
                    <Label>{t("changePassword.new")} *</Label>
                    <Input
                        type="password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                    />
                </div>

                <div>
                    <Label>{t("changePassword.confirm")} *</Label>
                    <Input
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                    />
                </div>

                <Button
                    type="submit"
                    size="sm"
                    disabled={
                        changePasswordLoading ||
                        !currentPassword ||
                        !newPassword ||
                        !confirmPassword
                    }
                >
                    {changePasswordLoading
                        ? t("changePassword.saving")
                        : t("changePassword.submit")}
                </Button>
            </form>
        </div>
    );
}
