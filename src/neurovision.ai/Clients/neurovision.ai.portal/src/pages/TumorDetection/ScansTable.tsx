import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";
import Label from "../../components/form/Label";
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
import PatientSelect from "./PatientSelect";
import { formatFileSize, formatPatientName, formatScanType } from "./tumorUtils";
import ScanPreviewModal from "./ScanPreviewModal";
import type { BrainScanResponse } from "../../features/tumorDetection/tumorDetection.types";

interface ScansTableProps {
    translationKey: "doctor" | "patient";
}

export default function ScansTable({ translationKey }: ScansTableProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});

    const items = useAppSelector((s) => s.tumorDetection.scans);
    const patients = useAppSelector((s) => s.patient.items);
    const total = useAppSelector((s) => s.tumorDetection.scansTotal);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [selectedPatientId, setSelectedPatientId] = useState("");
    const [previewScan, setPreviewScan] = useState<BrainScanResponse | null>(null);

    const baseKey = `tumor.scans.${translationKey}`;
    const isDoctor = translationKey === "doctor";
    const patientFilter = isDoctor ? selectedPatientId || undefined : userId;

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
                    <div className="mb-3 flex flex-wrap items-end justify-between gap-3">
                        {isDoctor ? (
                            <div className="min-w-[240px] max-w-sm flex-1">
                                <Label>{t("tumor.patient.label")}</Label>
                                <PatientSelect
                                    value={selectedPatientId}
                                    allowAll
                                    onChange={(patientId) => {
                                        setSelectedPatientId(patientId);
                                        setPage(0);
                                    }}
                                />
                            </div>
                        ) : (
                            <div />
                        )}
                        <TumorRefreshButton
                            label={t("common.actions.refresh")}
                            spinning={spinning}
                            onClick={load}
                        />
                    </div>

                    <TumorTableCard
                        footer={
                            <Pagination
                                currentPage={page + 1}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={(nextPage) => setPage(nextPage - 1)}
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
                                        {isDoctor && (
                                            <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                                {t("tumor.patient.label")}
                                            </TableCell>
                                        )}
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
                                        <TumorTableSkeleton rows={5} columns={isDoctor ? 6 : 5} />
                                    ) : items.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={isDoctor ? 6 : 5} className="px-5 py-8 text-center text-sm text-gray-500">
                                                {t(`${baseKey}.empty`)}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        items.map((scan) => (
                                            <TableRow
                                                key={scan.id}
                                                className="cursor-pointer hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                                onClick={() => setPreviewScan(scan)}
                                            >
                                                {isDoctor && (
                                                    <TableCell className="px-5 py-4 text-sm">
                                                        {formatPatientName(patients, scan.patientId)}
                                                    </TableCell>
                                                )}
                                                <TableCell className="px-5 py-4 text-sm font-medium text-brand-600 underline-offset-2 hover:underline dark:text-brand-400">
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

            <ScanPreviewModal
                scanId={previewScan?.id ?? null}
                fileName={previewScan?.fileName}
                translationKey={translationKey}
                onClose={() => setPreviewScan(null)}
            />
        </>
    );
}
