import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Badge from "../../components/ui/badge/Badge";
import Button from "../../components/ui/button/Button";
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
import {
    addComment,
    applyManualCorrection,
    downloadAnalysisReport,
    fetchAnalysis,
    fetchComments,
    generateAnalysisReport,
} from "../../features/tumorDetection/tumorDetection.service";
import { showAlert } from "../../features/ui/uiSlice";
import type {
    AnalysisResponse,
    AnalysisStatusNotification,
    CommentResponse,
} from "../../features/tumorDetection/tumorDetection.types";
import AnalysisImageViewer from "./AnalysisImageViewer";
import { useTumorAnalysisHub } from "../../features/tumorDetection/useTumorAnalysisHub";
import { useClientPagination } from "./useClientPagination";
import { formatTumorClass, tumorSelectClass, tumorStatusColor, tumorTextareaClass } from "./tumorUtils";
import TumorStatCard from "./TumorStatCard";
import TumorPanel from "./TumorPanel";
import TumorTableCard, { tumorTableHeaderClass } from "./TumorTableCard";

interface AnalysisDetailPageProps {
    detailPathPrefix: string;
    translationKey: "doctor" | "patient";
}

function ConfidenceBar({ value }: { value: number }) {
    const pct = Math.round(value * 100);
    const tone =
        pct >= 75 ? "bg-success-500" : pct >= 50 ? "bg-warning-500" : "bg-error-500";

    return (
        <div className="flex min-w-[140px] items-center gap-2">
            <div className="h-2 flex-1 overflow-hidden rounded-full bg-gray-100 dark:bg-white/10">
                <div className={`h-full rounded-full ${tone}`} style={{ width: `${pct}%` }} />
            </div>
            <span className="w-10 text-right text-sm font-medium">{pct}%</span>
        </div>
    );
}

export default function AnalysisDetailPage({
    detailPathPrefix,
    translationKey,
}: AnalysisDetailPageProps) {
    const { analysisId = "" } = useParams();
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId, role, name } = getUserInfoFromClaims(claims || {});

    const baseKey = `tumor.detail.${translationKey}`;

    const [analysis, setAnalysis] = useState<AnalysisResponse | null>(null);
    const [comments, setComments] = useState<CommentResponse[]>([]);
    const [commentText, setCommentText] = useState("");
    const [correctionClass, setCorrectionClass] = useState("1");
    const [loading, setLoading] = useState(true);
    const [generatingReport, setGeneratingReport] = useState(false);
    const [downloadingReport, setDownloadingReport] = useState(false);

    const detections = analysis?.detections ?? [];
    const detectionPage = useClientPagination(detections, 5);
    const commentsPage = useClientPagination(comments, 5);

    useEffect(() => {
        detectionPage.resetPage();
        commentsPage.resetPage();
    }, [analysis?.id]);

    const load = useCallback(async (silent = false) => {
        if (!analysisId) return;
        if (!silent) setLoading(true);
        try {
            const [analysisResult, commentsResult] = await Promise.all([
                fetchAnalysis(analysisId),
                fetchComments(analysisId),
            ]);
            setAnalysis(analysisResult);
            setComments(commentsResult);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t(`${baseKey}.messages.loadError`),
                })
            );
        } finally {
            if (!silent) setLoading(false);
        }
    }, [analysisId, dispatch, t, baseKey]);

    useEffect(() => {
        load();
    }, [load]);

    const handleAnalysisStatusChanged = useCallback(
        (notification: AnalysisStatusNotification) => {
            if (notification.analysisId !== analysisId) return;

            if (notification.status === "Processing") {
                setAnalysis((prev) =>
                    prev ? { ...prev, status: "Processing" } : prev
                );
                return;
            }

            if (
                notification.status === "Completed" ||
                notification.status === "Corrected" ||
                notification.status === "Failed"
            ) {
                load(true);

                dispatch(
                    showAlert({
                        type: notification.status === "Failed" ? "error" : "success",
                        message:
                            notification.status === "Failed"
                                ? t(`${baseKey}.messages.analysisFailed`)
                                : t(`${baseKey}.messages.analysisReady`),
                    })
                );
            }
        },
        [analysisId, dispatch, load, t, baseKey]
    );

    useTumorAnalysisHub({
        analysisId,
        patientId: userId,
        isDoctor: role === "doctor" || role === "superadministrator",
        onStatusChanged: handleAnalysisStatusChanged,
    });

    const handleAddComment = async () => {
        if (!analysisId || !userId || !commentText.trim()) return;
        try {
            await addComment(analysisId, userId, commentText.trim());
            setCommentText("");
            await load(true);
            dispatch(showAlert({ type: "success", message: t(`${baseKey}.messages.commentSuccess`) }));
        } catch (err: any) {
            dispatch(showAlert({ type: "error", message: err?.message ?? t(`${baseKey}.messages.commentError`) }));
        }
    };

    const handleCorrection = async () => {
        if (!analysisId || !userId) return;
        try {
            await applyManualCorrection(analysisId, {
                correctedByUserId: userId,
                correctedClass: Number(correctionClass),
            });
            await load(true);
            dispatch(showAlert({ type: "success", message: t(`${baseKey}.messages.correctionSuccess`) }));
        } catch (err: any) {
            dispatch(showAlert({ type: "error", message: err?.message ?? t(`${baseKey}.messages.correctionError`) }));
        }
    };

    const handleGenerateReport = async () => {
        if (!analysisId) return;
        setGeneratingReport(true);
        try {
            const updated = await generateAnalysisReport(analysisId, {
                doctorName: name || undefined,
            });
            setAnalysis(updated);
            dispatch(showAlert({ type: "success", message: t(`${baseKey}.messages.reportGenerated`) }));
        } catch (err: any) {
            dispatch(showAlert({ type: "error", message: err?.message ?? t(`${baseKey}.messages.reportError`) }));
        } finally {
            setGeneratingReport(false);
        }
    };

    const handleDownloadReport = async () => {
        if (!analysisId) return;
        setDownloadingReport(true);
        try {
            await downloadAnalysisReport(analysisId);
        } catch (err: any) {
            dispatch(showAlert({ type: "error", message: err?.message ?? t(`${baseKey}.messages.reportDownloadError`) }));
        } finally {
            setDownloadingReport(false);
        }
    };

    if (loading) {
        return (
            <div className="px-6 py-8">
                <div className="mb-6 h-8 w-48 animate-pulse rounded bg-gray-200 dark:bg-white/10" />
                <div className="grid gap-4 md:grid-cols-4">
                    {Array.from({ length: 4 }).map((_, i) => (
                        <div key={i} className="h-24 animate-pulse rounded-2xl bg-gray-100 dark:bg-white/5" />
                    ))}
                </div>
            </div>
        );
    }

    if (!analysis) {
        return <p className="px-6 py-8">{t(`${baseKey}.notFound`)}</p>;
    }

    const canCorrect = role === "doctor" || role === "superadministrator";
    const isDone = analysis.status === "Completed" || analysis.status === "Corrected";
    const listPath = isDone ? `${detailPathPrefix}/archive` : `${detailPathPrefix}/new`;

    return (
        <>
            <PageMeta title={t(`${baseKey}.pageTitle`)} description={t(`${baseKey}.pageDescription`)} />
            <PageBreadcrumb pageTitle={t(`${baseKey}.pageTitle`)} />

            <div className="space-y-6">
                <Link
                    to={listPath}
                    className="inline-flex text-sm font-medium text-brand-500 hover:underline"
                >
                    ← {t(`${baseKey}.actions.back`)}
                </Link>

                <ComponentCard title={analysis.scanFileName}>
                    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
                        <TumorStatCard
                            label={t(`${baseKey}.fields.status`)}
                            value={
                                <Badge color={tumorStatusColor(analysis.status)} size="sm">
                                    {t(`tumor.status.${analysis.status}`, analysis.status)}
                                </Badge>
                            }
                            accent="text-gray-800 dark:text-white"
                        />
                        <TumorStatCard
                            label={t(`${baseKey}.fields.classification`)}
                            value={formatTumorClass(analysis.classificationClass, t)}
                            accent="text-brand-600 dark:text-brand-400"
                        />
                        <TumorStatCard
                            label={t(`${baseKey}.fields.confidence`)}
                            value={
                                analysis.classificationConfidence != null ? (
                                    <ConfidenceBar value={analysis.classificationConfidence} />
                                ) : (
                                    "—"
                                )
                            }
                            accent="text-gray-800 dark:text-white"
                        />
                        <TumorStatCard
                            label={t(`${baseKey}.fields.tumorArea`)}
                            value={
                                analysis.tumorAreaRatio != null
                                    ? `${(analysis.tumorAreaRatio * 100).toFixed(2)}%`
                                    : "—"
                            }
                            accent="text-orange-600 dark:text-orange-400"
                        />
                    </div>
                </ComponentCard>

                {isDone && (
                    <ComponentCard title={t(`${baseKey}.imageTitle`)}>
                        <AnalysisImageViewer
                            analysisId={analysis.id}
                            detections={analysis.detections}
                            hasAnnotatedImage={analysis.hasAnnotatedImage}
                            hasDetectionImage={analysis.hasDetectionImage}
                            hasSegmentationImage={analysis.hasSegmentationImage}
                            translationKey={translationKey}
                        />
                    </ComponentCard>
                )}

                <ComponentCard
                    title={`${t(`${baseKey}.detectionsTitle`)} (${detections.length})`}
                >
                    <TumorTableCard
                        height="h-[460px]"
                        footer={
                            detections.length > 0 ? (
                                <Pagination
                                    currentPage={detectionPage.page}
                                    totalPages={detectionPage.totalPages}
                                    pageSize={detectionPage.pageSize}
                                    onPageChange={detectionPage.setPage}
                                    onPageSizeChange={detectionPage.setPageSize}
                                />
                            ) : undefined
                        }
                    >
                        <Table>
                            <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            #
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.class`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.confidence`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.bbox`)}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t(`${baseKey}.columns.size`)}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {detections.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={5} className="px-5 py-10 text-center">
                                                <p className="text-sm text-gray-500">
                                                    {t(`${baseKey}.emptyDetections`)}
                                                </p>
                                                {analysis.classificationClass &&
                                                    analysis.classificationClass !== "NoTumor" &&
                                                    analysis.classificationClass !== "No Tumor" && (
                                                        <p className="mt-2 text-xs text-gray-400">
                                                            {t(`${baseKey}.emptyDetectionsHint`, {
                                                                class: formatTumorClass(analysis.classificationClass, t),
                                                            })}
                                                        </p>
                                                    )}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        detectionPage.slice.map((d, index) => (
                                            <TableRow
                                                key={`${d.className}-${detectionPage.page}-${index}`}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {detectionPage.page * detectionPage.pageSize + index + 1}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge color="primary" size="sm">
                                                        {formatTumorClass(d.className, t)}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <ConfidenceBar value={d.confidence} />
                                                </TableCell>
                                                <TableCell className="px-5 py-4 font-mono text-xs text-gray-500">
                                                    x={d.xCenter.toFixed(3)}, y={d.yCenter.toFixed(3)}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {(d.width * 100).toFixed(1)}% × {(d.height * 100).toFixed(1)}%
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                    </TumorTableCard>
                </ComponentCard>

                {canCorrect && isDone && (
                    <ComponentCard title={t(`${baseKey}.correctionTitle`)}>
                        <div className="flex flex-wrap items-end gap-4">
                            <select
                                className={tumorSelectClass}
                                value={correctionClass}
                                onChange={(e) => setCorrectionClass(e.target.value)}
                            >
                                <option value="1">{t("tumor.classes.glioma")}</option>
                                <option value="2">{t("tumor.classes.meningioma")}</option>
                                <option value="3">{t("tumor.classes.pituitary")}</option>
                                <option value="4">{t("tumor.classes.noTumor")}</option>
                            </select>
                            <Button onClick={handleCorrection}>
                                {t(`${baseKey}.actions.applyCorrection`)}
                            </Button>
                        </div>
                    </ComponentCard>
                )}

                {isDone && (
                    <ComponentCard title={t(`${baseKey}.reportTitle`)}>
                        <TumorPanel>
                            {analysis.hasPdfReport ? (
                                <div className="space-y-4">
                                    <p className="text-sm text-gray-600 dark:text-gray-400">
                                        {t(`${baseKey}.reportAvailable`, {
                                            date: analysis.pdfGeneratedAt
                                                ? new Date(analysis.pdfGeneratedAt).toLocaleString()
                                                : "—",
                                        })}
                                    </p>
                                    <div className="flex flex-wrap gap-3">
                                        <Button
                                            onClick={handleDownloadReport}
                                            disabled={downloadingReport}
                                        >
                                            {t(`${baseKey}.actions.downloadReport`)}
                                        </Button>
                                        <Button
                                            variant="outline"
                                            onClick={handleGenerateReport}
                                            disabled={generatingReport}
                                        >
                                            {t(`${baseKey}.actions.regenerateReport`)}
                                        </Button>
                                    </div>
                                </div>
                            ) : (
                                <div className="space-y-4">
                                    <p className="text-sm text-gray-600 dark:text-gray-400">
                                        {t(`${baseKey}.reportEmpty`)}
                                    </p>
                                    <Button onClick={handleGenerateReport} disabled={generatingReport}>
                                        {t(`${baseKey}.actions.generateReport`)}
                                    </Button>
                                </div>
                            )}
                        </TumorPanel>
                    </ComponentCard>
                )}

                <ComponentCard title={`${t(`${baseKey}.commentsTitle`)} (${comments.length})`}>
                    <TumorTableCard
                        height="h-[360px]"
                        footer={
                            comments.length > 0 ? (
                                <Pagination
                                    currentPage={commentsPage.page}
                                    totalPages={commentsPage.totalPages}
                                    pageSize={commentsPage.pageSize}
                                    onPageChange={commentsPage.setPage}
                                    onPageSizeChange={commentsPage.setPageSize}
                                />
                            ) : undefined
                        }
                    >
                        <div className="space-y-3 p-4">
                            {comments.length === 0 ? (
                                <p className="py-6 text-center text-sm text-gray-500">
                                    {t(`${baseKey}.emptyComments`)}
                                </p>
                            ) : (
                                commentsPage.slice.map((comment) => (
                                    <TumorPanel key={comment.id} className="p-4">
                                        <p className="text-sm leading-relaxed">{comment.content}</p>
                                        <p className="mt-2 text-xs text-gray-500">
                                            {new Date(comment.createdAt).toLocaleString()}
                                        </p>
                                    </TumorPanel>
                                ))
                            )}
                        </div>
                    </TumorTableCard>

                    <div className="mt-4">
                        <textarea
                            className={`mb-3 ${tumorTextareaClass}`}
                            value={commentText}
                            onChange={(e) => setCommentText(e.target.value)}
                            placeholder={t(`${baseKey}.commentPlaceholder`)}
                        />
                        <Button onClick={handleAddComment}>{t(`${baseKey}.actions.postComment`)}</Button>
                    </div>
                </ComponentCard>
            </div>
        </>
    );
}
