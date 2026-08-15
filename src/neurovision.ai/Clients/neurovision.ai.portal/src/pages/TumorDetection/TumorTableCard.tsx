import type { ReactNode } from "react";

export const TUMOR_TABLE_HEIGHT = "h-[520px]";

export const tumorTableHeaderClass =
    "sticky top-0 z-10 border-b border-gray-100 bg-white dark:border-white/[0.05] dark:bg-gray-900";

interface TumorTableCardProps {
    children: ReactNode;
    footer?: ReactNode;
    height?: string;
}

export default function TumorTableCard({
    children,
    footer,
    height = TUMOR_TABLE_HEIGHT,
}: TumorTableCardProps) {
    return (
        <div
            className={`flex flex-col rounded-xl border border-gray-200 dark:border-white/[0.05] ${height}`}
        >
            <div className="flex-1 overflow-y-auto">{children}</div>
            {footer != null && (
                <div className="border-t border-gray-100 p-4 dark:border-white/[0.05]">
                    {footer}
                </div>
            )}
        </div>
    );
}
