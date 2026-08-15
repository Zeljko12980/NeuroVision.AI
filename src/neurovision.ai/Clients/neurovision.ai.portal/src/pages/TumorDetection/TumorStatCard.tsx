import type { ReactNode } from "react";

interface TumorStatCardProps {
    label: string;
    value: ReactNode;
    accent?: string;
}

export default function TumorStatCard({
    label,
    value,
    accent = "text-gray-800 dark:text-white",
}: TumorStatCardProps) {
    return (
        <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm dark:border-white/[0.05] dark:bg-white/[0.02]">
            <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-500">
                {label}
            </p>
            <div className={`text-lg font-semibold ${accent}`}>{value}</div>
        </div>
    );
}
