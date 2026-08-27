import { useEffect, useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import Badge from "../../components/ui/badge/Badge";
import Button from "../../components/ui/button/Button";
import Pagination from "../../components/ui/pagination/Pagination";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import FileInput from "../../components/form/input/FileInput";
import CustomSelect from "../../components/form/CustomSelect";
import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { loadModels, loadStatistics } from "../../features/tumorDetection/tumorDetection.slice";
import {
    fetchAnalysisErrors,
    fetchModelTypes,
    activateModelVersion,
    uploadModelVersion,
} from "../../features/tumorDetection/tumorDetection.service";
import type {
    AnalysisErrorLogResponse,
    AiModelTypeResponse,
    AiModelVersionResponse,
} from "../../features/tumorDetection/tumorDetection.types";
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
    const [uploading, setUploading] = useState(false);
    const [modelTypes, setModelTypes] = useState<AiModelTypeResponse[]>([]);
    const [taskType, setTaskType] = useState("");
    const [versionLabel, setVersionLabel] = useState("");
    const [runId, setRunId] = useState("");
    const [setActive, setSetActive] = useState(true);
    const [weightsFile, setWeightsFile] = useState<File | null>(null);
    const [activatingId, setActivatingId] = useState<string | null>(null);
    const [selectedByTask, setSelectedByTask] = useState<Record<string, string>>({});
    const [errors, setErrors] = useState<AnalysisErrorLogResponse[]>([]);
    const [errorsTotal, setErrorsTotal] = useState(0);
    const [errorsPage, setErrorsPage] = useState(0);
    const [errorsPageSize, setErrorsPageSize] = useState(5);
    const [expandedErrorId, setExpandedErrorId] = useState<string | null>(null);

    const errorPages = Math.max(1, Math.ceil(errorsTotal / errorsPageSize));
    const typeName = (code: string) =>
        modelTypes.find((item) => item.code.toLowerCase() === code.toLowerCase())?.name ?? code;

    const pipelineTasks = ["Detection", "Classification", "Segmentation"];
    const modelsForTask = (task: string) =>
        models.filter((model) => model.taskType.toLowerCase() === task.toLowerCase());

    const syncSelectedFromModels = (items: AiModelVersionResponse[]) => {
        setSelectedByTask((current) => {
            const next = { ...current };
            for (const task of pipelineTasks) {
                const taskModels = items.filter((model) => model.taskType.toLowerCase() === task.toLowerCase());
                const active = taskModels.find((model) => model.isActive);
                if (active) next[task] = active.id;
                else if (!next[task] || !taskModels.some((model) => model.id === next[task]))
                    next[task] = taskModels[0]?.id ?? "";
            }
            return next;
        });
    };

    const loadErrors = async (page = errorsPage, pageSize = errorsPageSize) => {
        const result = await fetchAnalysisErrors(page + 1, pageSize);
        setErrors(result.items);
        setErrorsTotal(result.total);
    };

    const load = async () => {
        setSpinning(true);
        setFetching(true);
        try {
            const [, loadedModels] = await Promise.all([
                dispatch(loadStatistics()).unwrap(),
                dispatch(loadModels()).unwrap(),
                loadErrors(),
                fetchModelTypes().then((types) => {
                    setModelTypes(types);
                    setTaskType((current) => current || types[0]?.code || "");
                }),
            ]);
            syncSelectedFromModels(loadedModels);
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
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    useEffect(() => {
        loadErrors().catch((err: unknown) => {
            dispatch(
                showAlert({
                    type: "error",
                    message: err instanceof Error ? err.message : t("tumor.monitoring.messages.loadError"),
                })
            );
        });
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [errorsPage, errorsPageSize]);

    const handleUpload = async (event: FormEvent) => {
        event.preventDefault();
        if (!weightsFile || !versionLabel.trim() || !taskType) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("tumor.monitoring.messages.uploadRequired"),
                })
            );
            return;
        }

        setUploading(true);
        try {
            await uploadModelVersion({
                taskType,
                versionLabel: versionLabel.trim(),
                runId: runId.trim() || undefined,
                setActive,
                file: weightsFile,
            });
            setVersionLabel("");
            setRunId("");
            setWeightsFile(null);
            dispatch(showAlert({ type: "success", message: t("tumor.monitoring.messages.uploadSuccess") }));
            const loaded = await dispatch(loadModels()).unwrap();
            syncSelectedFromModels(loaded);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t("tumor.monitoring.messages.uploadError"),
                })
            );
        } finally {
            setUploading(false);
        }
    };

    const handleActivate = async (id: string) => {
        if (!id || activatingId) return;
        const current = models.find((model) => model.id === id);
        if (current?.isActive) return;

        setActivatingId(id);
        try {
            await activateModelVersion(id);
            dispatch(showAlert({ type: "success", message: t("tumor.monitoring.messages.activateSuccess") }));
            const loaded = await dispatch(loadModels()).unwrap();
            syncSelectedFromModels(loaded);
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: err?.message ?? t("tumor.monitoring.messages.activateError"),
                })
            );
        } finally {
            setActivatingId(null);
        }
    };

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

                <ComponentCard title={t("tumor.monitoring.activeTitle")} desc={t("tumor.monitoring.activeDescription")}>
                    <div className="grid gap-4 md:grid-cols-3">
                        {pipelineTasks.map((task) => {
                            const options = modelsForTask(task).map((model) => ({
                                value: model.id,
                                label: `${model.versionLabel} (${model.runId})${model.isActive ? ` · ${t("common.status.active")}` : ""}`,
                            }));
                            return (
                                <div key={task}>
                                    <Label>{typeName(task)}</Label>
                                    <CustomSelect
                                        value={selectedByTask[task] ?? ""}
                                        placeholder={t("tumor.monitoring.fields.modelPlaceholder")}
                                        disabled={options.length === 0 || activatingId !== null}
                                        onChange={(id) => {
                                            setSelectedByTask((current) => ({ ...current, [task]: id }));
                                            void handleActivate(id);
                                        }}
                                        options={options}
                                    />
                                </div>
                            );
                        })}
                    </div>
                </ComponentCard>

                <ComponentCard title={t("tumor.monitoring.uploadTitle")}>
                    <form className="grid gap-4 md:grid-cols-2" onSubmit={handleUpload}>
                        <div>
                            <Label>{t("tumor.monitoring.fields.task")}</Label>
                            <CustomSelect
                                value={taskType}
                                placeholder={t("tumor.monitoring.fields.taskPlaceholder")}
                                onChange={setTaskType}
                                options={modelTypes.map((item) => ({
                                    value: item.code,
                                    label: item.name,
                                }))}
                            />
                        </div>
                        <div>
                            <Label>{t("tumor.monitoring.fields.version")}</Label>
                            <Input
                                value={versionLabel}
                                placeholder={t("tumor.monitoring.fields.versionPlaceholder")}
                                onChange={(e) => setVersionLabel(e.target.value)}
                            />
                        </div>
                        <div>
                            <Label>{t("tumor.monitoring.fields.runId")}</Label>
                            <Input
                                value={runId}
                                placeholder={t("tumor.monitoring.fields.runIdPlaceholder")}
                                onChange={(e) => setRunId(e.target.value)}
                            />
                        </div>
                        <div>
                            <Label>{t("tumor.monitoring.fields.file")}</Label>
                            <FileInput
                                accept=".pt,.pth"
                                onChange={(e) => setWeightsFile(e.target.files?.[0] ?? null)}
                            />
                            {weightsFile && (
                                <p className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                    {weightsFile.name} ({Math.round(weightsFile.size / 1024 / 1024)} MB)
                                </p>
                            )}
                        </div>
                        <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300 md:col-span-2">
                            <input
                                type="checkbox"
                                checked={setActive}
                                onChange={(e) => setSetActive(e.target.checked)}
                            />
                            {t("tumor.monitoring.fields.setActive")}
                        </label>
                        <div>
                            <Button type="submit" disabled={uploading}>
                                {uploading
                                    ? t("tumor.monitoring.actions.uploading")
                                    : t("tumor.monitoring.actions.upload")}
                            </Button>
                        </div>
                    </form>
                </ComponentCard>

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
                                    currentPage={modelsPage.uiPage}
                                    totalPages={modelsPage.totalPages}
                                    pageSize={modelsPage.pageSize}
                                    onPageChange={modelsPage.setUiPage}
                                    onPageSizeChange={modelsPage.setPageSize}
                                />
                            ) : undefined
                        }
                    >
                        {fetching ? (
                            <TumorTableSkeleton rows={5} columns={6} />
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
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.columns.actions")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {models.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={6} className="px-5 py-10 text-center text-sm text-gray-500">
                                                {t("tumor.monitoring.emptyModels")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        modelsPage.slice.map((model) => (
                                            <TableRow
                                                key={model.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                <TableCell className="px-5 py-4">{typeName(model.taskType)}</TableCell>
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
                                                <TableCell className="px-5 py-4">
                                                    <Button
                                                        size="sm"
                                                        variant={model.isActive ? "outline" : "primary"}
                                                        disabled={model.isActive || activatingId !== null}
                                                        onClick={() => void handleActivate(model.id)}
                                                    >
                                                        {activatingId === model.id
                                                            ? t("tumor.monitoring.actions.activating")
                                                            : model.isActive
                                                              ? t("common.status.active")
                                                              : t("tumor.monitoring.actions.use")}
                                                    </Button>
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                        )}
                    </TumorTableCard>
                </ComponentCard>

                <ComponentCard title={t("tumor.monitoring.errorsTitle")}>
                    <TumorTableCard
                        footer={
                            errorsTotal > 0 ? (
                                <Pagination
                                    currentPage={errorsPage + 1}
                                    totalPages={errorPages}
                                    pageSize={errorsPageSize}
                                    onPageChange={(nextPage) => setErrorsPage(nextPage - 1)}
                                    onPageSizeChange={(size) => {
                                        setErrorsPageSize(size);
                                        setErrorsPage(0);
                                    }}
                                />
                            ) : undefined
                        }
                    >
                        {fetching ? (
                            <TumorTableSkeleton rows={5} columns={3} />
                        ) : (
                            <Table>
                                <TableHeader className={tumorTableHeaderClass}>
                                    <TableRow>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.errorColumns.occurred")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.errorColumns.analysis")}
                                        </TableCell>
                                        <TableCell isHeader className="px-5 py-3 text-xs font-semibold uppercase">
                                            {t("tumor.monitoring.errorColumns.message")}
                                        </TableCell>
                                    </TableRow>
                                </TableHeader>
                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {errors.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={3} className="px-5 py-10 text-center text-sm text-gray-500">
                                                {t("tumor.monitoring.emptyErrors")}
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
                                                    {item.tumorAnalysisId ?? "—"}
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
                                                                ? t("tumor.monitoring.actions.hideDetails")
                                                                : t("tumor.monitoring.actions.showDetails")}
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
