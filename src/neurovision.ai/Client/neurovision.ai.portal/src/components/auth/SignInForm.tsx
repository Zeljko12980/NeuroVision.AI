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

export default function SignInForm() {
    const [showPassword, setShowPassword] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [emailError, setEmailError] = useState("");

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const { requires2FA, error, loading } = useAppSelector((state) => state.auth);
    const { message, type, visible } = useAppSelector((state) => state.ui);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement> | React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        if (!isValidEmail(email)) {
            setEmailError("Please enter a valid email address");
            return;
        }
        dispatch(login({ email, password }));
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
                            Sign In
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            Enter your email and password to sign in!
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
                                    Email <span className="text-error-500">*</span>
                                </Label>
                                <Input
                                    placeholder="info@gmail.com"
                                    value={email}
                                    onChange={(e) => {
                                        setEmail(e.target.value);
                                        if (!isValidEmail(e.target.value)) {
                                            setEmailError("Please enter a valid email address");
                                        } else {
                                            setEmailError("");
                                        }
                                    }}
                                    error={!!emailError}
                                    hint={emailError}
                                />
                            </div>

                            <div>
                                <Label>
                                    Password <span className="text-error-500">*</span>
                                </Label>
                                <div className="relative">
                                    <Input
                                        type={showPassword ? "text" : "password"}
                                        placeholder="Enter your password"
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                    />
                                    <span
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                                    >
                                        {showPassword ? (
                                            <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                                        ) : (
                                            <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                                        )}
                                    </span>
                                </div>
                            </div>

                            <div className="flex items-center justify-between">
                                <Link
                                    to="/reset-password"
                                    className="text-sm text-brand-500 hover:text-brand-600 dark:text-brand-400"
                                >
                                    Forgot password?
                                </Link>
                            </div>

                            <div>
                                <Button
                                    className="w-full"
                                    size="sm"
                                    onClick={handleSubmit}
                                    disabled={isButtonDisabled}
                                >
                                    {loading ? "Signing in..." : "Sign in"}
                                </Button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}