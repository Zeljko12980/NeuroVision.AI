import { useEffect, useState } from "react";
import { useSearchParams, useNavigate, Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/store";

import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";

import { setPassword } from "../../features/auth/authSlice";
import { showAlert, hideAlert } from "../../features/ui/uiSlice";

import { useTranslation } from "react-i18next";

export default function SetPasswordForm() {
    const { t } = useTranslation();

    const [params] = useSearchParams();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const email = params.get("email");
    const token = params.get("token");

    const [passwordValue, setPasswordValue] = useState("");
    const [confirm, setConfirm] = useState("");

    const { setPasswordLoading, error, setPasswordSuccess } = useAppSelector((state) => state.auth);

    // redirect after success
    useEffect(() => {
        if (setPasswordSuccess) {
            dispatch(
                showAlert({
                    message: "Password set successfully!",
                    type: "success",
                })
            );

            const timer = setTimeout(() => {
                dispatch(hideAlert());
                navigate("/signin");
            }, 2000);

            return () => clearTimeout(timer);
        }
    }, [setPasswordSuccess, dispatch, navigate]);

    const isValid = passwordValue.length >= 8 && passwordValue === confirm;

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!email || !token) {
            dispatch(showAlert({ message: t("errors.invalidLink") || "Invalid or expired link", type: "error" }));
            return;
        }

        if (passwordValue.length < 8) {
            dispatch(showAlert({ message: "Password must be at least 8 characters", type: "error" }));
            return;
        }

        if (passwordValue !== confirm) {
            dispatch(showAlert({ message: "Passwords do not match", type: "error" }));
            return;
        }

        dispatch(
            setPassword({
                email,
                token,
                password: passwordValue,
            })
        );
    };

    useEffect(() => {
        if (error) {
            dispatch(showAlert({ message: error, type: "error" }));
        }
    }, [error, dispatch]);

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">

                <div className="mb-5 sm:mb-8">
                    <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                        {t("setPassword.title") || "Set Password"}
                    </h1>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                        {t("setPassword.subtitle") || "Create a secure password for your account"}
                    </p>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="space-y-6">

                        <div>
                            <Label>Password *</Label>
                            <Input
                                type="password"
                                placeholder="Enter password"
                                value={passwordValue}
                                onChange={(e) => setPasswordValue(e.target.value)}
                            />
                        </div>

                        <div>
                            <Label>Confirm Password *</Label>
                            <Input
                                type="password"
                                placeholder="Confirm password"
                                value={confirm}
                                onChange={(e) => setConfirm(e.target.value)}
                            />
                        </div>

                        <Button
                            type="submit"
                            className="w-full"
                            size="sm"
                            disabled={!isValid || setPasswordLoading}
                        >
                            {setPasswordLoading
                                ? "Setting password..."
                                : "Set Password"}
                        </Button>

                        <div className="text-center">
                            <Link
                                to="/signin"
                                className="text-sm text-brand-500 hover:text-brand-600"
                            >
                                Back to sign in
                            </Link>
                        </div>

                    </div>
                </form>
            </div>
        </div>
    );
}