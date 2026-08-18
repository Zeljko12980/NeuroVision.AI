import { useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { forgotPassword } from "../../features/auth/authSlice";
import { showAlert } from "../../features/ui/uiSlice";

export default function ForgotPasswordForm() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const [email, setEmail] = useState("");
    const [emailError, setEmailError] = useState("");

    const { forgotPasswordLoading } = useAppSelector((state) => state.auth);

    const isValidEmail = (value: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!isValidEmail(email)) {
            setEmailError(t("errors.invalidEmail"));
            return;
        }

        setEmailError("");
        const result = await dispatch(forgotPassword({ email }));
        if (forgotPassword.fulfilled.match(result)) {
            dispatch(showAlert({ message: t("forgotPassword.sent"), type: "success" }));
        } else {
            dispatch(
                showAlert({
                    message: (result.payload as string) || t("forgotPassword.sent"),
                    type: "error",
                })
            );
        }
    };

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
                <div className="mb-5 sm:mb-8">
                    <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                        {t("forgotPassword.title")}
                    </h1>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                        {t("forgotPassword.subtitle")}
                    </p>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="space-y-6">
                        <div>
                            <Label>{t("signin.emailLabel")} *</Label>
                            <Input
                                placeholder={t("signin.emailPlaceholder")}
                                value={email}
                                onChange={(e) => {
                                    setEmail(e.target.value);
                                    setEmailError(
                                        isValidEmail(e.target.value) ? "" : t("errors.invalidEmail")
                                    );
                                }}
                                error={!!emailError}
                                hint={emailError}
                            />
                        </div>

                        <Button
                            type="submit"
                            className="w-full"
                            size="sm"
                            disabled={!isValidEmail(email) || forgotPasswordLoading}
                        >
                            {forgotPasswordLoading
                                ? t("forgotPassword.sending")
                                : t("forgotPassword.submit")}
                        </Button>

                        <div className="text-center">
                            <Link
                                to="/signin"
                                className="text-sm text-brand-500 hover:text-brand-600 dark:text-brand-400"
                            >
                                {t("forgotPassword.backToSignIn")}
                            </Link>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}
