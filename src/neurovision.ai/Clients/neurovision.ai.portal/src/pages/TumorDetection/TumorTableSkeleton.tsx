import { TableRow, TableCell } from "../../components/ui/table";

interface TumorTableSkeletonProps {
    rows?: number;
    columns?: number;
}

export default function TumorTableSkeleton({
    rows = 5,
    columns = 5,
}: TumorTableSkeletonProps) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">
                    {Array.from({ length: columns }).map((__, colIdx) => (
                        <TableCell key={colIdx} className="px-5 py-4">
                            <div className="h-4 w-full max-w-[120px] bg-gray-200 dark:bg-white/10 rounded" />
                        </TableCell>
                    ))}
                </TableRow>
            ))}
        </>
    );
}
