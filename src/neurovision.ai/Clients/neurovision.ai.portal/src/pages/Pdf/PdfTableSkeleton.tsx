import { TableRow, TableCell } from "../../components/ui/table";


interface PdfTemplateTableSkeletonProps {
    rows?: number;
}

export default function PdfTemplateTableSkeleton({
    rows = 5,
}: PdfTemplateTableSkeletonProps) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-40 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-24 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-12 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-5 w-16 bg-gray-200 dark:bg-white/10 rounded-full" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-28 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                    <TableCell className="px-5 py-4">
                        <div className="h-8 w-8 bg-gray-200 dark:bg-white/10 rounded" />
                    </TableCell>

                </TableRow>
            ))}
        </>
    );
}