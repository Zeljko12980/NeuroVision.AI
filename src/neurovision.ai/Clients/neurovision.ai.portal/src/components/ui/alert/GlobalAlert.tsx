import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../store/store";
import { hideAlert } from "../../../features/ui/uiSlice";

const variantClasses = {
    success: "border-green-500 bg-green-50 text-green-700",
    error: "border-red-500 bg-red-50 text-red-700",
    warning: "border-yellow-500 bg-yellow-50 text-yellow-700",
    info: "border-blue-500 bg-blue-50 text-blue-700",
};

const GlobalAlert = () => {
    const dispatch = useAppDispatch();
    const { message, type, visible } = useAppSelector((s) => s.ui);

    useEffect(() => {
        if (!visible) return;

        const timer = setTimeout(() => {
            dispatch(hideAlert());
        }, 3000);

        return () => clearTimeout(timer);
    }, [visible]);

    if (!visible || !type) return null;

    return (
        <div className="fixed top-4 right-4 z-[99999] w-[320px] animate-fade-in">
            <div className={`relative rounded-xl border p-4 shadow-lg ${variantClasses[type]}`}>
                <button
                    onClick={() => dispatch(hideAlert())}
                    className="absolute top-2 right-2"
                >
                    ✕
                </button>
                <p className="text-sm font-medium">{message}</p>
            </div>
        </div>
    );
};

export default GlobalAlert;