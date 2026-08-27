import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Button from "../../components/ui/button/Button";
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
import {
    downloadAnalysisReport,
    searchAnalysisReports,
} from "../../features/tumorDetection/tumorDetection.service";
import { showAlert } from "../../features/ui/uiSlice";
import TumorTableSkeleton from "./TumorTableSkeleton";
import TumorRefreshButton from "./TumorRefreshButton";
import TumorTableCard, { tumorTableHeaderClass } from "./TumorTableCard";
import PatientSelect from "./PatientSelect";
import { formatPatientName, formatTumorClass, tumorStatusColor } from "./tumorUtils";
import type { AnalysisReportResponse } from "../../features/tumorDetection/tumorDetection.types";

interface ReportsTableProps {
    detailPathPrefix: string;
    translationKey: "doctor" | "patient";
}

export default function ReportsTable({
    detailPathPrefix,
    translationKey,
}: ReportsTableProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});
    const patients = useAppSelector((s) => s.patient.items);

    const [reports, setReports] = useState<AnalysisReportResponse[]>([]);
    const [total, setTotal] = useState(0);
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [loading, setLoading] = useState(true);
    const [spinning, setSpinning] = useState(false);
    const [downloadingId, setDownloadingId] = useState<string | null>(null);
    const [selectedPatientId, setSelectedPatientId] = useState("");

    const baseKey = `tumor.reports.${translationKey}`;
    const isDoctor = translationKey === "doctor";
    const patientFilter = isDoctor ? selectedPatientId || undefined : userId;
    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const load = useCallback(async () => {
        setSpinning(true);
        setLoading(true);
        try {
            const result = await searchAnalysisReports({
                patientId: patientFilter,
                page: page + 1,
                pageSize,
            });
            setReports(result.items);
            setTotal(result.total);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.loadError`),
                })
            );
        } finally {
            setSpinning(false);
            setLoading(false);
        }
    }, [patientFilter, page, pageSize, dispatch, t, baseKey]);

    useEffect(() => {
        load();
    }, [load]);

    const handleDownload = async (analysisId: string) => {
        setDownloadingId(analysisId);
        try {
            await downloadAnalysisReport(analysisId);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.downloadError`),
                })
            );
        } finally {
            setDownloadingId(null);
        }
    };

    return (
        <>
            <PageMeta title={t(`${baseKey}.pageTitle`)} description={t(`${baseKey}.pageDescription`)} />
            <PageBreadcrumb pageTitle={t(`${baseKey}.pageTitle`)} />

            <div className="space-y-6">
                <ComponentCard title={t(`${baseKey}.title`)} desc={t(`${baseKey}.description`)}>
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
                            label={t(`${baseKey}.actions.refresh`)}
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
                        {loading ? (
                            <TumorTableSkeleton columns={isDoctor ? 6 : 5} rows={6} />
                        ) : (
                            <Table>
                                <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        {isDoctor && (
                                            <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                                {t("tumor.patient.label")}
                                            </TableCell>
                                        )}
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.scan`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.classification`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.status`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.generatedAt`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.actions`)}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {reports.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={isDoctor ? 6 : 5} className="px-5 py-10 text-center text-sm text-gray-500">
                                                {t(`${baseKey}.empty`)}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        reports.map((report) => (
                                            <TableRow
                                                key={report.analysisId}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                {isDoctor && (
                                                    <TableCell className="px-5 py-4 text-sm">
                                                        {formatPatientName(patients, report.patientId)}
                                                    </TableCell>
                                                )}
                                                <TableCell className="px-5 py-4">
                                                    <Link
                                                        to={`${detailPathPrefix}/${report.analysisId}`}
                                                        className="font-medium text-brand-500 hover:underline"
                                                    >
                                                        {report.scanFileName}
                                                    </Link>
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    {report.classificationClass ? (
                                                        <Badge color="primary" size="sm">
                                                            {formatTumorClass(report.classificationClass, t)}
                                                        </Badge>
                                                    ) : (
                                                        "—"
                                                    )}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge color={tumorStatusColor(report.status)} size="sm">
                                                        {t(`tumor.status.${report.status}`, report.status)}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {report.pdfGeneratedAt
                                                        ? new Date(report.pdfGeneratedAt).toLocaleString()
                                                        : "—"}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <div className="flex flex-wrap gap-2">
                                                        <Button
                                                            size="sm"
                                                            variant="outline"
                                                            onClick={() => handleDownload(report.analysisId)}
                                                            disabled={downloadingId === report.analysisId}
                                                        >
                                                            {t(`${baseKey}.actions.download`)}
                                                        </Button>
                                                        <Link to={`${detailPathPrefix}/${report.analysisId}`}>
                                                            <Button size="sm" variant="outline">
                                                                {t(`${baseKey}.actions.viewAnalysis`)}
                                                            </Button>
                                                        </Link>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                        )}
                    </TumorTableCard>
                </ComponentCard>
            </div>
        </>
    );
}
