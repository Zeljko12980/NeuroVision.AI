import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { EyeCloseIcon, EyeIcon } from "../../icons";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import Alert from "../ui/alert/Alert";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { login } from "../../features/auth/authSlice";
import { showAlert, hideAlert } from "../../features/ui/uiSlice";
import { useTranslation } from "react-i18next";

export default function SignInForm() {
    const { t } = useTranslation();

    const [showPassword, setShowPassword] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [emailError, setEmailError] = useState("");

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const { requires2FA, error, loading } = useAppSelector((state) => state.auth);
    const { message, type, visible } = useAppSelector((state) => state.ui);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!isValidEmail(email)) {
            setEmailError(t("errors.invalidEmail"));
            return;
        }

        await dispatch(login({ email, password }));

       
    };

    useEffect(() => {
        if (requires2FA) {
            navigate("/confirm-2fa");
        }
    }, [requires2FA, navigate]);

    useEffect(() => {
        if (error) {
            dispatch(showAlert({ message: error, type: "error" }));

            const timer = setTimeout(() => {
                dispatch(hideAlert());
            }, 5000);

            return () => clearTimeout(timer);
        }
    }, [error, dispatch]);

    const isValidEmail = (email: string) => {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    };

    const isButtonDisabled = !isValidEmail(email) || password.length === 0 || loading;

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
                <div>
                    <div className="mb-5 sm:mb-8">
                        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                            {t("signin.title")}
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {t("signin.subtitle")}
                        </p>
                    </div>

                    {visible && type && (
                        <div className="fixed top-4 right-4 z-50">
                            <Alert
                                variant={type}
                                title={type === "success" ? t("alerts.success") : t("alerts.error")}
                                message={message}
                            />
                        </div>
                    )}

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
                                            isValidEmail(e.target.value)
                                                ? ""
                                                : t("errors.invalidEmail")
                                        );
                                    }}
                                    error={!!emailError}
                                    hint={emailError}
                                />
                            </div>

                            <div>
                                <Label>{t("signin.passwordLabel")} *</Label>
                                <div className="relative">
                                    <Input
                                        type={showPassword ? "text" : "password"}
                                        placeholder={t("signin.passwordPlaceholder")}
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                    />
                                    <span
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                                    >
                                        {showPassword ? <EyeIcon /> : <EyeCloseIcon />}
                                    </span>
                                </div>
                            </div>

                            <Link
                                to="/reset-password"
                                className="text-sm text-brand-500 hover:text-brand-600 dark:text-brand-400"
                            >
                                {t("signin.forgotPassword")}
                            </Link>

                            <Button type="submit" className="w-full" size="sm" disabled={isButtonDisabled}>
                                {loading ? t("signin.signingIn") : t("signin.signIn")}
                            </Button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}