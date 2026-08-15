import { useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { confirmEmail } from "../../features/auth/authSlice";

export default function ConfirmEmail() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const email = searchParams.get("email");
    const token = searchParams.get("token");

    const {
        confirmEmailLoading,
        confirmEmailSuccess,
        error
    } = useAppSelector((state) => state.auth);

    useEffect(() => {
        if (!email || !token) return;

        dispatch(confirmEmail({ email, token }));
    }, [email, token, dispatch]);

    useEffect(() => {
        if (confirmEmailSuccess) {
            setTimeout(() => {
                navigate("/signin");
            }, 2000);
        }
    }, [confirmEmailSuccess, navigate]);

    return (
        <div className="flex flex-col justify-center w-full max-w-md mx-auto">
            <div className="bg-white dark:bg-gray-800 shadow-lg rounded-2xl p-8 text-center">

                {confirmEmailLoading && (
                    <>
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
                        <h2 className="mt-4 text-lg font-semibold text-gray-700 dark:text-gray-200">
                            Confirming your email...
                        </h2>
                        <p className="text-sm text-gray-500 mt-2">
                            Please wait while we verify your account.
                        </p>
                    </>
                )}

                {confirmEmailSuccess && (
                    <>
                        <div className="text-green-500 text-5xl">✓</div>
                        <h2 className="mt-4 text-xl font-bold text-gray-800 dark:text-white">
                            Email Verified!
                        </h2>
                        <p className="text-sm text-gray-500 mt-2">
                            Redirecting to login...
                        </p>
                    </>
                )}

                {error && (
                    <>
                        <div className="text-red-500 text-5xl">✕</div>
                        <h2 className="mt-4 text-xl font-bold text-gray-800 dark:text-white">
                            Verification Failed
                        </h2>
                        <p className="text-sm text-gray-500 mt-2">
                            {error || "The link is invalid or has expired."}
                        </p>

                        <a
                            href="/resend-confirmation"
                            className="inline-block mt-6 bg-red-600 text-white px-5 py-2 rounded-lg hover:bg-red-700 transition"
                        >
                            Resend Email
                        </a>
                    </>
                )}

            </div>
        </div>
    );
}