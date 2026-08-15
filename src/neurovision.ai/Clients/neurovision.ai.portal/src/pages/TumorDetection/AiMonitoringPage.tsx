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
import { loadModels, loadStatistics } from "../../features/tumorDetection/tumorDetection.slice";
import { showAlert } from "../../features/ui/uiSlice";
import TumorTableSkeleton from "./TumorTableSkeleton";
import TumorRefreshButton from "./TumorRefreshButton";
import TumorStatCard from "./TumorStatCard";
import TumorTableCard, { tumorTableHeaderClass } from "./TumorTableCard";
import { useClientPagination } from "./useClientPagination";

export default function AiMonitoringPage() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const statistics = useAppSelector((s) => s.tumorDetection.statistics);
    const models = useAppSelector((s) => s.tumorDetection.models);
    const modelsPage = useClientPagination(models, 5);
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const load = async () => {
        setSpinning(true);
        setFetching(true);
        try {
            await Promise.all([
                dispatch(loadStatistics()).unwrap(),
                dispatch(loadModels()).unwrap(),
            ]);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t("tumor.monitoring.messages.loadError"),
                })
            );
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        load();
    }, []);

    return (
        <>
            <PageMeta
                title={t("tumor.monitoring.pageTitle")}
                description={t("tumor.monitoring.pageDescription")}
            />
            <PageBreadcrumb pageTitle={t("tumor.monitoring.pageTitle")} />

            <div className="space-y-6">
                <div className="grid gap-4 sm:grid-cols-2">
                    <TumorStatCard
                        label={t("tumor.monitoring.stats.completed")}
                        value={statistics?.totalCompletedAnalyses ?? 0}
                        accent="text-brand-600 dark:text-brand-400"
                    />
                    <TumorStatCard
                        label={t("tumor.monitoring.stats.scans")}
                        value={statistics?.totalScans ?? 0}
                        accent="text-orange-600 dark:text-orange-400"
                    />
                </div>

                <ComponentCard title={t("tumor.monitoring.modelsTitle")}>
                    <div className="mb-3 flex justify-end">
                        <TumorRefreshButton
                            label={t("common.actions.refresh")}
                            spinning={spinning}
                            onClick={load}
                        />
                    </div>

                    <TumorTableCard
                        footer={
                            models.length > 0 ? (
                                <Pagination
                                    currentPage={modelsPage.page}
                                    totalPages={modelsPage.totalPages}
                                    pageSize={modelsPage.pageSize}
                                    onPageChange={modelsPage.setPage}
                                    onPageSizeChange={modelsPage.setPageSize}
                                />
                            ) : undefined
                        }
                    >
                        {fetching ? (
                            <TumorTableSkeleton rows={5} columns={5} />
                        ) : (
                            <Table>
                                <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.task")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.version")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.runId")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.status")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.registered")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {models.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={5} className="px-5 py-10 text-center text-sm text-gray-500">
                                                {t("tumor.monitoring.emptyModels")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        modelsPage.slice.map((model) => (
                                            <TableRow
                                                key={model.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                <TableCell className="px-5 py-4">{model.taskType}</TableCell>
                                                <TableCell className="px-5 py-4">{model.versionLabel}</TableCell>
                                                <TableCell className="px-5 py-4 font-mono text-sm">
                                                    {model.runId}
                                                </TableCell>
                                                <TableCell className="px-5 py-4">
                                                    <Badge
                                                        color={model.isActive ? "success" : "light"}
                                                        size="sm"
                                                    >
                                                        {model.isActive
                                                            ? t("common.status.active")
                                                            : t("common.status.inactive")}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell className="px-5 py-4 text-sm text-gray-500">
                                                    {new Date(model.registeredAt).toLocaleString()}
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
