import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import Alert from "../../components/ui/alert/Alert";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { verify2FA, resend2FA } from "../../features/auth/authSlice";
import { showAlert, hideAlert } from "../../features/ui/uiSlice";

export default function TwoFAForm() {
    const { t } = useTranslation(); 

    const [code, setCode] = useState("");
    const [codeError, setCodeError] = useState("");
    const [cooldown, setCooldown] = useState(0);

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const {
        email,
        loading,
        error,
        token,
        resendLoading,
        resendMessage,
    } = useAppSelector((state) => state.auth);

    const { message, type, visible } = useAppSelector((state) => state.ui);

    useEffect(() => {
        if (!email) {
            navigate("/signin");
        }
    }, [email, navigate]);

    useEffect(() => {
        if (token) {
            dispatch(
                showAlert({
                    message: t("twoFA.successLogin"),
                    type: "success",
                })
            );

            navigate("/");

            const timer = setTimeout(() => dispatch(hideAlert()), 2000);
            return () => clearTimeout(timer);
        }
    }, [token, dispatch, navigate, t]);

    useEffect(() => {
        if (error) {
            dispatch(showAlert({ message: t(error), type: "error" }));

            const timer = setTimeout(() => dispatch(hideAlert()), 5000);
            return () => clearTimeout(timer);
        }
    }, [error, dispatch, t]);

    useEffect(() => {
        if (resendMessage) {
            dispatch(showAlert({ message: t(resendMessage), type: "success" }));

            const timer = setTimeout(() => dispatch(hideAlert()), 3000);
            return () => clearTimeout(timer);
        }
    }, [resendMessage, dispatch, t]);

    useEffect(() => {
        if (cooldown <= 0) return;

        const timer = setInterval(() => {
            setCooldown((prev) => prev - 1);
        }, 1000);

        return () => clearInterval(timer);
    }, [cooldown]);

    const handleSubmit = (
        e: React.FormEvent<HTMLFormElement> | React.MouseEvent<HTMLButtonElement>
    ) => {
        e.preventDefault();

        if (!/^\d{6}$/.test(code)) {
            setCodeError(t("twoFA.invalidCode"));
            return;
        }

        if (!email) {
            setCodeError(t("twoFA.missingEmail"));
            return;
        }

        setCodeError("");
        dispatch(verify2FA({ email, code }));
    };

    const handleResend = () => {
        if (!email || cooldown > 0) return;

        dispatch(resend2FA({ email }));
        setCooldown(30);
    };

    const isButtonDisabled = !/^\d{6}$/.test(code) || loading;

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
                <div>
                    <div className="mb-5 sm:mb-8">
                        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                            {t("twoFA.title")}
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {t("twoFA.subtitle")}
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
                                <Label>
                                    {t("twoFA.code")} <span className="text-error-500">*</span>
                                </Label>
                                <Input
                                    type="text"
                                    placeholder="123456"
                                    value={code}
                                    onChange={(e) => {
                                        const val = e.target.value;

                                        if (/^\d{0,6}$/.test(val)) {
                                            setCode(val);
                                            setCodeError("");
                                        } else {
                                            setCodeError(t("twoFA.onlyDigits"));
                                        }
                                    }}
                                    error={!!codeError}
                                    hint={codeError}
                                />
                            </div>

                            <div>
                                <Button
                                    className="w-full"
                                    size="sm"
                                    type="submit"
                                    disabled={isButtonDisabled}
                                >
                                    {loading ? t("twoFA.verifying") : t("twoFA.verify")}
                                </Button>
                            </div>
                        </div>
                    </form>

                    <div className="mt-4 text-center">
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {t("twoFA.didntReceive")}{" "}
                            <span
                                onClick={handleResend}
                                className={`cursor-pointer ${
                                    cooldown > 0 || resendLoading
                                        ? "text-gray-400 cursor-not-allowed"
                                        : "text-brand-500 hover:text-brand-600"
                                }`}
                            >
                                {cooldown > 0
                                    ? t("twoFA.resendIn", { seconds: cooldown })
                                    : resendLoading
                                    ? t("twoFA.sending")
                                    : t("twoFA.resend")}
                            </span>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}