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
import Input from "../../components/form/input/InputField";
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
    loadAnalyses,
    loadScans,
    runAnalysis,
} from "../../features/tumorDetection/tumorDetection.slice";
import { showAlert } from "../../features/ui/uiSlice";
import TumorTableSkeleton from "./TumorTableSkeleton";
import TumorRefreshButton from "./TumorRefreshButton";
import TumorTableCard, { tumorTableHeaderClass } from "./TumorTableCard";
import PatientSelect from "./PatientSelect";
import { formatPatientName, formatTumorClass, primaryFindingClass, tumorSelectClass, tumorStatusColor } from "./tumorUtils";
import { useTumorAnalysisHub } from "../../features/tumorDetection/useTumorAnalysisHub";
import type { AnalysisStatusNotification } from "../../features/tumorDetection/tumorDetection.types";

interface AnalysesTableProps {
    detailPathPrefix: string;
    translationKey: "doctor" | "patient";
    archived?: boolean;
}

export default function AnalysesTable({
    detailPathPrefix,
    translationKey,
    archived = false,
}: AnalysesTableProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});

    const analyses = useAppSelector((s) => s.tumorDetection.analyses);
    const scans = useAppSelector((s) => s.tumorDetection.scans);
    const patients = useAppSelector((s) => s.patient.items);
    const total = useAppSelector((s) => s.tumorDetection.analysesTotal);
    const starting = useAppSelector((s) => s.tumorDetection.startingAnalysis);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [selectedScanId, setSelectedScanId] = useState("");
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [selectedPatientId, setSelectedPatientId] = useState("");
    const [fromInput, setFromInput] = useState("");
    const [toInput, setToInput] = useState("");
    const [appliedFrom, setAppliedFrom] = useState("");
    const [appliedTo, setAppliedTo] = useState("");

    const baseKey = `tumor.analyses.${translationKey}${archived ? "Archive" : ""}`;
    const isDoctor = translationKey === "doctor";
    const patientFilter = isDoctor ? selectedPatientId || undefined : userId;
    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    const load = useCallback(async () => {
        setSpinning(true);
        setFetching(true);
        try {
            await dispatch(
                loadAnalyses({
                    patientId: patientFilter,
                    from: appliedFrom || undefined,
                    to: appliedTo || undefined,
                    page: page + 1,
                    pageSize,
                    archived,
                })
            ).unwrap();

            if (!archived) {
                const scanResult = await dispatch(
                    loadScans({ patientId: patientFilter, page: 1, pageSize: 100 })
                ).unwrap();
                if (scanResult.items.length > 0 && !selectedScanId) {
                    setSelectedScanId(scanResult.items[0].id);
                }
            }
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
    }, [archived, appliedFrom, appliedTo, baseKey, dispatch, page, pageSize, patientFilter, selectedScanId, t]);

    useEffect(() => {
        load();
    }, [load]);

    const handleAnalysisStatusChanged = useCallback(
        (notification: AnalysisStatusNotification) => {
            if (patientFilter && notification.patientId !== patientFilter) {
                return;
            }

            if (
                notification.status === "Completed" ||
                notification.status === "Corrected" ||
                notification.status === "Failed"
            ) {
                load();

                dispatch(
                    showAlert({
                        type: notification.status === "Failed" ? "error" : "success",
                        message:
                            notification.status === "Failed"
                                ? t(`${baseKey}.messages.analysisFailed`)
                                : t(`${baseKey}.messages.analysisSuccess`),
                    })
                );
            } else if (notification.status === "Processing") {
                load();
            }
        },
        [patientFilter, dispatch, load, t, baseKey]
    );

    useTumorAnalysisHub({
        patientId: patientFilter,
        isDoctor,
        onStatusChanged: handleAnalysisStatusChanged,
    });

    useEffect(() => {
        if (scans.length > 0 && !selectedScanId) {
            setSelectedScanId(scans[0].id);
        }
    }, [scans, selectedScanId]);

    const handleStart = async () => {
        if (!selectedScanId || !userId) return;

        try {
            await dispatch(
                runAnalysis({ brainScanId: selectedScanId })
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(`${baseKey}.messages.analysisStarted`),
                })
            );

            await load();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.analysisFailed`),
                })
            );
        }
    };

    const applyFilters = () => {
        setAppliedFrom(fromInput);
        setAppliedTo(toInput);
        setPage(0);
    };

    const clearFilters = () => {
        setSelectedPatientId("");
        setSelectedScanId("");
        setFromInput("");
        setToInput("");
        setAppliedFrom("");
        setAppliedTo("");
        setPage(0);
    };

    const handlePatientChange = (patientId: string) => {
        setSelectedPatientId(patientId);
        setSelectedScanId("");
        setPage(0);
    };

    return (
        <>
            <PageMeta
                title={t(`${baseKey}.pageTitle`)}
                description={t(`${baseKey}.pageDescription`)}
            />
            <PageBreadcrumb pageTitle={t(`${baseKey}.pageTitle`)} />

            <div className="space-y-6">
                {!archived && (
                    <ComponentCard title={t(`${baseKey}.startTitle`)}>
                        <div className="flex flex-wrap items-end gap-4">
                            {isDoctor && (
                                <div className="min-w-[240px] flex-1">
                                    <Label>{t("tumor.patient.label")}</Label>
                                    <PatientSelect
                                        value={selectedPatientId}
                                        allowAll
                                        onChange={handlePatientChange}
                                    />
                                </div>
                            )}
                            <div className="min-w-[280px] flex-1">
                                <Label>{t(`${baseKey}.fields.scan`)}</Label>
                                <select
                                    className={tumorSelectClass}
                                    value={selectedScanId}
                                    onChange={(e) => setSelectedScanId(e.target.value)}
                                >
                                    {scans.length === 0 && (
                                        <option value="">{t(`${baseKey}.fields.scanEmpty`)}</option>
                                    )}
                                    {scans.map((scan) => (
                                        <option key={scan.id} value={scan.id}>
                                            {isDoctor && !patientFilter
                                                ? `${formatPatientName(patients, scan.patientId)} — ${scan.fileName}`
                                                : scan.fileName}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <Button
                                disabled={starting || !selectedScanId || scans.length === 0}
                                onClick={handleStart}
                            >
                                {starting
                                    ? t(`${baseKey}.actions.running`)
                                    : t(`${baseKey}.actions.run`)}
                            </Button>
                        </div>
                    </ComponentCard>
                )}

                <ComponentCard title={t(`${baseKey}.title`)}>
                    <div className="mb-4 flex flex-wrap items-end gap-3">
                        {isDoctor && (
                            <div className="min-w-[240px] flex-1">
                                <Label>{t("tumor.patient.label")}</Label>
                                <PatientSelect
                                    value={selectedPatientId}
                                    allowAll
                                    onChange={handlePatientChange}
                                />
                            </div>
                        )}
                        <div className="min-w-[160px]">
                            <Label>{t("tumor.analyses.filters.from")}</Label>
                            <Input
                                type="date"
                                value={fromInput}
                                onChange={(e) => setFromInput(e.target.value)}
                            />
                        </div>
                        <div className="min-w-[160px]">
                            <Label>{t("tumor.analyses.filters.to")}</Label>
                            <Input
                                type="date"
                                value={toInput}
                                onChange={(e) => setToInput(e.target.value)}
                            />
                        </div>
                        <Button onClick={applyFilters}>{t("tumor.analyses.filters.apply")}</Button>
                        <Button variant="outline" onClick={clearFilters}>
                            {t("tumor.analyses.filters.clear")}
                        </Button>
                        <div className="ml-auto">
                            <TumorRefreshButton
                                label={t("common.actions.refresh")}
                                spinning={spinning}
                                onClick={load}
                            />
                        </div>
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
                                            {t(`${baseKey}.columns.scan`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.status`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.classification`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.confidence`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.requested`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.actions`)}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <TumorTableSkeleton rows={5} columns={isDoctor ? 7 : 6} />
                                    ) : analyses.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={isDoctor ? 7 : 6} className="px-5 py-8 text-center text-sm text-gray-500">
                                                {t(`${baseKey}.empty`)}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        analyses.map((analysis) => (
                                            <TableRow
                                                key={analysis.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                {isDoctor && (
                                                    <TableCell className="px-5 py-4 text-sm">
                                                        {formatPatientName(patients, analysis.patientId)}
                                                    </TableCell>
                                                )}
                                                <TableCell className="px-5 py-4 text-sm font-medium">
                                                    {analysis.scanFileName}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    <Badge color={tumorStatusColor(analysis.status)} size="sm">
                                                        {t(`tumor.status.${analysis.status}`, analysis.status)}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {primaryFindingClass(analysis) ? (
                                                        <Badge color="primary" size="sm">
                                                            {formatTumorClass(primaryFindingClass(analysis), t)}
                                                        </Badge>
                                                    ) : (
                                                        "—"
                                                    )}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    {analysis.overallConfidence != null
                                                        ? `${(analysis.overallConfidence * 100).toFixed(1)}%`
                                                        : "-"}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {new Date(analysis.requestedAt).toLocaleString()}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    <Link to={`${detailPathPrefix}/${analysis.id}`}>
                                                        <Button size="sm" variant="outline">
                                                            {t(`${baseKey}.actions.view`)}
                                                        </Button>
                                                    </Link>
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
