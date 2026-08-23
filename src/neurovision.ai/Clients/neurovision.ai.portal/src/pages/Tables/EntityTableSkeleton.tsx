import { TableCell, TableRow } from "../../components/ui/table";

export default function EntityTableSkeleton({
    columns,
    rows = 5,
}: {
    columns: number;
    rows?: number;
}) {
    return (
        <>
            {Array.from({ length: rows }).map((_, rowIndex) => (
                <TableRow key={rowIndex} className="animate-pulse">
                    {Array.from({ length: columns }).map((__, colIndex) => (
                        <TableCell key={colIndex} className="px-5 py-4">
                            <div className="h-4 w-28 rounded bg-gray-200 dark:bg-white/10" />
                        </TableCell>
                    ))}
                </TableRow>
            ))}
        </>
    );
}
