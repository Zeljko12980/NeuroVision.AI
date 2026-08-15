import type { ReactNode } from "react";

interface TumorPanelProps {
    children: ReactNode;
    className?: string;
}

export default function TumorPanel({ children, className = "" }: TumorPanelProps) {
    return (
        <div
            className={`rounded-xl border border-gray-200 bg-gray-50/60 p-5 dark:border-white/[0.05] dark:bg-white/[0.02] ${className}`}
        >
            {children}
        </div>
    );
}
