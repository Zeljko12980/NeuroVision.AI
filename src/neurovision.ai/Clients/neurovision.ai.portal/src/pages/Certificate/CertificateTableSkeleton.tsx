import { TableRow, TableCell } from "../../components/ui/table";

interface CertificateTableSkeletonProps {
    rows?: number;
}

export default function CertificateTableSkeleton({
    rows = 5,
}: CertificateTableSkeletonProps) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-40 rounded bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-36 rounded bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-28 rounded bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-12 rounded bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-5 w-16 rounded-full bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                    <TableCell className="px-5 py-4">
                        <div className="h-8 w-8 rounded bg-gray-200 dark:bg-white/10" />
                    </TableCell>
                </TableRow>
            ))}
        </>
    );
}
