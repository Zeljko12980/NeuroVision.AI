import { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import { useTranslation } from "react-i18next";

import { useAppDispatch, useAppSelector } from "../../../store/store";
import { hideAlert } from "../../../features/ui/uiSlice";
import Alert from "./Alert";

function getToastRoot(): HTMLElement {
    let root = document.getElementById("toast-root");
    if (!root) {
        root = document.createElement("div");
        root.id = "toast-root";
        document.documentElement.appendChild(root);
    }

    root.style.cssText = [
        "position:fixed",
        "top:16px",
        "right:16px",
        "left:auto",
        "bottom:auto",
        "z-index:2147483647",
        "width:min(360px, calc(100vw - 32px))",
        "margin:0",
        "pointer-events:auto",
    ].join(";");

    return root;
}

const GlobalAlert = () => {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const { message, type, visible } = useAppSelector((s) => s.ui);
    const [root, setRoot] = useState<HTMLElement | null>(null);

    useEffect(() => {
        setRoot(getToastRoot());
    }, []);

    useEffect(() => {
        if (!visible) return;

        const timer = window.setTimeout(() => {
            dispatch(hideAlert());
        }, 5000);

        return () => window.clearTimeout(timer);
    }, [visible, message, type, dispatch]);

    if (!root || !visible || !type) return null;

    return createPortal(
        <Alert
            variant={type}
            title={t(`alerts.${type}`)}
            message={message}
            onClose={() => dispatch(hideAlert())}
        />,
        root
    );
};

export default GlobalAlert;
