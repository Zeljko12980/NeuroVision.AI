import { TableRow, TableCell } from "../../components/ui/table";

export default function RoleTableSkeleton({ rows = 5 }: { rows?: number }) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-32 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-48 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-10 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-5 w-16 bg-gray-200 dark:bg-white/10 rounded-full" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-8 w-8 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>
                </TableRow>
            ))}
        </>
    );
}