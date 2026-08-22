import { TableCell, TableRow } from "../../components/ui/table";

export default function DoctorsTableSkeleton({ rows = 5 }: { rows?: number }) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">
                    <TableCell className="px-5 py-4">
                        <div className="flex items-center gap-3">
                            <div className="h-10 w-10 rounded-full bg-gray-200 dark:bg-white/10" />
                            <div className="h-4 w-40 bg-gray-200 dark:bg-white/10 rounded" />
                        </div>
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-44 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-16 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-36 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-5 w-16 bg-gray-200 dark:bg-white/10 rounded-full" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-5 w-20 bg-gray-200 dark:bg-white/10 rounded-full" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-8 w-8 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>
                </TableRow>
            ))}
        </>
    );
}
