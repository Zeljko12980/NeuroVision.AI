import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
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
import GrafanaLogsEmbed from "../../components/monitoring/GrafanaLogsEmbed";
import { useAppDispatch } from "../../store/store";
import { showAlert } from "../../features/ui/uiSlice";
import { fetchAnalysisErrors } from "../../features/tumorDetection/tumorDetection.service";
import type { AnalysisErrorLogResponse } from "../../features/tumorDetection/tumorDetection.types";
import { getSystemHealth, type HealthStatus } from "../../features/health/healthService";
import TumorTableSkeleton from "../TumorDetection/TumorTableSkeleton";
import TumorRefreshButton from "../TumorDetection/TumorRefreshButton";
import TumorStatCard from "../TumorDetection/TumorStatCard";
import TumorTableCard, { tumorTableHeaderClass } from "../TumorDetection/TumorTableCard";

const statusBadgeColor: Record<HealthStatus, "success" | "error" | "warning"> = {
    Healthy: "success",
    Unhealthy: "error",
    Degraded: "warning",
};

export default function LogsPage() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [systemStatus, setSystemStatus] = useState<HealthStatus | null>(null);
    const [errors, setErrors] = useState<AnalysisErrorLogResponse[]>([]);
    const [errorsTotal, setErrorsTotal] = useState(0);
    const [errorsPage, setErrorsPage] = useState(0);
    const [errorsPageSize, setErrorsPageSize] = useState(10);
    const [expandedErrorId, setExpandedErrorId] = useState<string | null>(null);

    const errorPages = Math.max(1, Math.ceil(errorsTotal / errorsPageSize));

    const load = async (page = errorsPage, pageSize = errorsPageSize) => {
        setSpinning(true);
        setFetching(true);
        try {
            const [health, result] = await Promise.all([
                getSystemHealth(0, 1),
                fetchAnalysisErrors(page + 1, pageSize),
            ]);
            setSystemStatus(health.status);
            setErrors(result.items);
            setErrorsTotal(result.total);
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err instanceof Error ? err.message : t("logs.messages.loadError"),
                })
            );
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        load();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [errorsPage, errorsPageSize]);

    return (
        <>
            <PageMeta title={t("logs.pageTitle")} description={t("logs.pageDescription")} />
            <PageBreadcrumb pageTitle={t("logs.pageTitle")} />

            <div className="space-y-6">
                <div className="flex justify-end">
                    <TumorRefreshButton
                        label={t("common.actions.refresh")}
                        spinning={spinning}
                        onClick={() => load()}
                    />
                </div>

                <div className="grid gap-4 sm:grid-cols-3">
                    <TumorStatCard
                        label={t("logs.stats.systemStatus")}
                        value={
                            systemStatus ? (
                                <Badge color={statusBadgeColor[systemStatus]} size="sm">
                                    {t(`health.status.${systemStatus.toLowerCase()}`)}
                                </Badge>
                            ) : (
                                "—"
                            )
                        }
                    />
                    <TumorStatCard
                        label={t("logs.stats.analysisErrors")}
                        value={errorsTotal}
                        accent="text-orange-600 dark:text-orange-400"
                    />
                    <TumorStatCard
                        label={t("logs.stats.healthPage")}
                        value={
                            <Link
                                to="/admin/health"
                                className="text-sm font-medium text-brand-600 hover:underline dark:text-brand-400"
                            >
                                {t("logs.actions.openHealth")}
                            </Link>
                        }
                    />
                </div>

                <ComponentCard title={t("logs.grafanaTitle")} desc={t("logs.grafanaDescription")}>
                    <GrafanaLogsEmbed openInGrafanaLabel={t("dashboard.openGrafana")} />
                </ComponentCard>

                <ComponentCard title={t("logs.errorsTitle")} desc={t("logs.errorsDescription")}>
                    <TumorTableCard
                        footer={
                            errorsTotal > 0 ? (
                                <Pagination
                                    currentPage={errorsPage + 1}
                                    totalPages={errorPages}
                                    pageSize={errorsPageSize}
                                    onPageChange={(p) => setErrorsPage(p - 1)}
                                    onPageSizeChange={(size) => {
                                        setErrorsPageSize(size);
                                        setErrorsPage(0);
                                    }}
                                />
                            ) : undefined
                        }
                    >
                        {fetching ? (
                            <TumorTableSkeleton rows={8} columns={3} />
                        ) : (
                            <Table>
                                <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("logs.columns.occurred")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("logs.columns.analysis")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("logs.columns.message")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {errors.length === 0 ? (
                                        <TableRow>
                                            <TableCell
                                                colSpan={3}
                                                className="px-5 py-10 text-center text-sm text-gray-500"
                                            >
                                                {t("logs.empty")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        errors.map((item) => (
                                            <TableRow
                                                key={item.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {new Date(item.occurredAt).toLocaleString()}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 font-mono text-xs">
                                                    {item.tumorAnalysisId ? (
                                                        <Link
                                                            to={`/analysis/${item.tumorAnalysisId}`}
                                                            className="text-brand-600 hover:underline dark:text-brand-400"
                                                        >
                                                            {item.tumorAnalysisId}
                                                        </Link>
                                                    ) : (
                                                        "—"
                                                    )}
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm">
                                                    <p>{item.message}</p>
                                                    {item.details && (
                                                        <button
                                                            type="button"
                                                            className="mt-2 text-xs font-medium text-brand-500 hover:underline"
                                                            onClick={() =>
                                                                setExpandedErrorId((prev) =>
                                                                    prev === item.id ? null : item.id
                                                                )
                                                            }
                                                        >
                                                            {expandedErrorId === item.id
                                                                ? t("logs.actions.hideDetails")
                                                                : t("logs.actions.showDetails")}
                                                        </button>
                                                    )}
                                                    {expandedErrorId === item.id && item.details && (
                                                        <pre className="mt-2 max-h-48 overflow-auto rounded-lg bg-gray-50 p-3 text-xs dark:bg-white/5">
                                                            {item.details}
                                                        </pre>
                                                    )}
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
