import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";
import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";

import { useAppDispatch, useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import { loadScans } from "../../features/tumorDetection/tumorDetection.slice";
import { showAlert } from "../../features/ui/uiSlice";
import TumorTableSkeleton from "./TumorTableSkeleton";
import TumorRefreshButton from "./TumorRefreshButton";
import TumorTableCard, { tumorTableHeaderClass } from "./TumorTableCard";
import { formatFileSize, formatScanType } from "./tumorUtils";

interface ScansTableProps {
    translationKey: "doctor" | "patient";
}

export default function ScansTable({ translationKey }: ScansTableProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId, role } = getUserInfoFromClaims(claims || {});

    const items = useAppSelector((s) => s.tumorDetection.scans);
    const total = useAppSelector((s) => s.tumorDetection.scansTotal);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const baseKey = `tumor.scans.${translationKey}`;
    const isDoctor = role === "doctor" || role === "superadministrator";
    const patientFilter = isDoctor ? undefined : userId;

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const load = async () => {
        setSpinning(true);
        setFetching(true);
        try {
            await dispatch(
                loadScans({
                    patientId: patientFilter,
                    page: page + 1,
                    pageSize,
                })
            ).unwrap();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.loadError`),
                })
            );
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        load();
    }, [page, pageSize, patientFilter]);

    return (
        <>
            <PageMeta
                title={t(`${baseKey}.pageTitle`)}
                description={t(`${baseKey}.pageDescription`)}
            />
            <PageBreadcrumb pageTitle={t(`${baseKey}.pageTitle`)} />

            <div className="space-y-6">
                <ComponentCard title={t(`${baseKey}.title`)}>
                    <div className="mb-3 flex justify-end">
                        <TumorRefreshButton
                            label={t("common.actions.refresh")}
                            spinning={spinning}
                            onClick={load}
                        />
                    </div>

                    <TumorTableCard
                        footer={
                            <Pagination
                                currentPage={page}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={setPage}
                                onPageSizeChange={(size) => {
                                    setPageSize(size);
                                    setPage(0);
                                }}
                            />
                        }
                    >
                        <Table>
                            <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.file`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.type`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.size`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.analyses`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.uploaded`)}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <TumorTableSkeleton rows={5} columns={5} />
                                    ) : items.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={5} className="px-5 py-8 text-center text-sm text-gray-500">
                                                {t(`${baseKey}.empty`)}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        items.map((scan) => (
                                            <TableRow
                                                key={scan.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-medium">
                                                    {scan.fileName}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    <Badge color="primary" size="sm">
                                                        {formatScanType(scan.scanType, t)}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {formatFileSize(scan.fileSizeBytes)}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {scan.analysisCount}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {new Date(scan.uploadedAt).toLocaleString()}
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                    </TumorTableCard>
                </ComponentCard>
            </div>
        </>
    );
}
