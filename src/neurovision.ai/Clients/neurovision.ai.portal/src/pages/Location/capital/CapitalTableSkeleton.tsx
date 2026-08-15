import { TableRow, TableCell } from "../../../components/ui/table";


interface CapitalTableSkeletonProps {
    rows?: number;
}

export default function CapitalTableSkeleton({
    rows = 5,
}: CapitalTableSkeletonProps) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-16 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-28 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-8 bg-gray-200 dark:bg-white/10 rounded h-8" />
                    </TableCell>
                </TableRow>
            ))}
        </>
    );
}
