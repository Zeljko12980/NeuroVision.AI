import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import Alert from "../../components/ui/alert/Alert";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { verify2FA } from "../../features/auth/authSlice";
import { showAlert, hideAlert } from "../../features/ui/uiSlice";

export default function TwoFAForm() {
    const [code, setCode] = useState("");
    const [codeError, setCodeError] = useState("");

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const { email, loading, error, token } = useAppSelector((state) => state.auth);
    const { message, type, visible } = useAppSelector((state) => state.ui);

    useEffect(() => {
        if (!email) {
            navigate("/signin");
        }
    }, [email, navigate]);

    useEffect(() => {
        if (token) {
            dispatch(showAlert({ message: "You have successfully logged in!", type: "success" }));
            navigate("/");

            const timer = setTimeout(() => dispatch(hideAlert()), 2000);
            return () => clearTimeout(timer);
        }
    }, [token, dispatch, navigate]);

    useEffect(() => {
        if (error) {
            dispatch(showAlert({ message: error, type: "error" }));
            const timer = setTimeout(() => dispatch(hideAlert()), 5000);
            return () => clearTimeout(timer);
        }
    }, [error, dispatch]);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement> | React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();

        if (!/^\d{6}$/.test(code)) {
            setCodeError("Please enter a valid 6-digit code");
            return;
        }
        if (!email) {
            setCodeError("Email is missing. Please login again.");
            return;
        }

        setCodeError("");
        dispatch(verify2FA({ email, code }));
    };

    const isButtonDisabled = !/^\d{6}$/.test(code) || loading;

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
                <div>
                    <div className="mb-5 sm:mb-8">
                        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                            Two-Factor Authentication
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            Enter the 6-digit code from your authenticator app
                        </p>
                    </div>

                    {visible && type && (
                        <div className="fixed top-4 right-4 z-50">
                            <Alert
                                variant={type}
                                title={type === "success" ? "Success" : "Error"}
                                message={message}
                            />
                        </div>
                    )}

                    <form>
                        <div className="space-y-6">
                            <div>
                                <Label>
                                    2FA Code <span className="text-error-500">*</span>
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
                                            setCodeError("Only digits are allowed");
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
                                    onClick={handleSubmit}
                                    disabled={isButtonDisabled}
                                >
                                    {loading ? "Verifying..." : "Verify"}
                                </Button>
                            </div>
                        </div>
                    </form>

                    <div className="mt-4 text-center">
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            Didn't receive the code? <span className="text-brand-500 cursor-pointer hover:text-brand-600">Resend</span>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}