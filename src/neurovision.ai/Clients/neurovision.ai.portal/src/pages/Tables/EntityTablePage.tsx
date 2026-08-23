import { useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";
import { RefreshIcon } from "../../icons";
import Pagination from "../../components/ui/pagination/Pagination";
import Input from "../../components/form/input/InputField";
import { useAppDispatch } from "../../store/store";
import { showAlert } from "../../features/ui/uiSlice";
import { getEntityTable } from "../../features/entityTable/entityTable.service";
import {
    EntityService,
    getTablesForService,
} from "../../features/entityTable/tables.config";
import EntityTableSkeleton from "./EntityTableSkeleton";

const humanize = (key: string) =>
    key
        .replace(/([A-Z])/g, " $1")
        .replace(/^./, (char) => char.toUpperCase())
        .trim();

const formatCell = (value: unknown, yes: string, no: string) => {
    if (value == null || value === "") return "—";
    if (typeof value === "boolean") return value ? yes : no;
    if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}T/.test(value)) {
        const parsed = new Date(value);
        return Number.isNaN(parsed.getTime()) ? value : parsed.toLocaleString();
    }
    return String(value);
};

export default function EntityTablePage({ service }: { service: EntityService }) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const { table = "" } = useParams();

    const definition = useMemo(
        () => getTablesForService(service).find((item) => item.key === table),
        [service, table]
    );

    const title = definition
        ? t(definition.nameKey)
        : t("entityTables.unknown");

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);
    const [search, setSearch] = useState("");
    const [appliedSearch, setAppliedSearch] = useState("");
    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [rows, setRows] = useState<Record<string, unknown>[]>([]);
    const [columns, setColumns] = useState<string[]>([]);
    const [total, setTotal] = useState(0);
    const [reloadToken, setReloadToken] = useState(0);

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    useEffect(() => {
        setPage(0);
        setSearch("");
        setAppliedSearch("");
    }, [service, table]);

    useEffect(() => {
        if (!definition) {
            setRows([]);
            setColumns([]);
            setTotal(0);
            return;
        }

        let cancelled = false;
        const loadRows = async () => {
            setSpinning(true);
            setFetching(true);
            try {
                const result = await getEntityTable(
                    definition.apiPath,
                    page,
                    pageSize,
                    appliedSearch || undefined
                );

                if (cancelled) return;

                const nextRows = result.data ?? [];
                setRows(nextRows);
                setColumns(
                    definition.fields.map((field) => field.key)
                );
                setTotal(result.count ?? 0);

                const pages = Math.ceil((result.count ?? 0) / pageSize);
                if (page >= pages && pages > 0) {
                    setPage(pages - 1);
                } else if (pages === 0 && page !== 0) {
                    setPage(0);
                }
            } catch (err: unknown) {
                if (cancelled) return;
                dispatch(
                    showAlert({
                        type: "error",
                        message:
                            typeof err === "string"
                                ? err
                                : err instanceof Error
                                    ? err.message
                                    : t("entityTables.messages.loadError"),
                    })
                );
            } finally {
                if (!cancelled) {
                    setSpinning(false);
                    setFetching(false);
                }
            }
        };

        loadRows();
        return () => {
            cancelled = true;
        };
    }, [service, table, page, pageSize, appliedSearch, definition, dispatch, t, reloadToken]);

    return (
        <>
            <PageMeta
                title={title}
                description={t("entityTables.pageDescription", { table: title })}
            />

            <PageBreadcrumb pageTitle={title} />

            <div className="space-y-6">
                <ComponentCard title={title}>
                    <div className="mb-3 flex items-center justify-between gap-3">
                        <form
                            className="w-full max-w-xs"
                            onSubmit={(e) => {
                                e.preventDefault();
                                setPage(0);
                                setAppliedSearch(search);
                            }}
                        >
                            <Input
                                value={search}
                                placeholder={t("entityTables.searchPlaceholder")}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </form>

                        <button
                            type="button"
                            onClick={() => setReloadToken((value) => value + 1)}
                            disabled={fetching || spinning || !definition}
                            className="flex h-9 w-9 items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`h-5 w-5 ${spinning ? "animate-spin" : "rotate-90"}`}
                            />
                        </button>
                    </div>

                    <div className="flex h-[520px] flex-col rounded-xl border border-gray-200 dark:border-white/[0.05]">
                        <div className="flex-1 overflow-auto">
                            <Table>
                                <TableHeader className="sticky top-0 z-10 border-b border-gray-100 bg-white dark:border-white/[0.05] dark:bg-gray-900">
                                    <TableRow>
                                        {(columns.length ? columns : ["empty"]).map((column) => (
                                            <TableCell
                                                key={column}
                                                isHeader
                                                className="px-5 py-3 text-xs font-semibold uppercase"
                                            >
                                                {column === "empty"
                                                    ? t("entityTables.columns.value")
                                                    : t(`entityTables.columns.${column}`, {
                                                          defaultValue: humanize(column),
                                                      })}
                                            </TableCell>
                                        ))}
                                    </TableRow>
                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {!definition ? (
                                        <TableRow>
                                            <TableCell className="px-5 py-8 text-sm text-gray-500">
                                                {t("entityTables.unknown")}
                                            </TableCell>
                                        </TableRow>
                                    ) : fetching ? (
                                        <EntityTableSkeleton
                                            rows={5}
                                            columns={Math.max(columns.length, 3)}
                                        />
                                    ) : rows.length === 0 ? (
                                        <TableRow>
                                            <TableCell
                                                className="px-5 py-8 text-sm text-gray-500"
                                                colSpan={Math.max(columns.length, 1)}
                                            >
                                                {t("entityTables.empty")}
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        rows.map((row, index) => (
                                            <TableRow
                                                key={index}
                                                className="transition hover:bg-gray-50 dark:hover:bg-white/[0.03]"
                                            >
                                                {columns.map((column) => (
                                                    <TableCell
                                                        key={column}
                                                        className="whitespace-nowrap px-5 py-4 text-sm text-gray-600 dark:text-gray-300"
                                                    >
                                                        {formatCell(
                                                            row[column],
                                                            t("entityTables.boolean.yes"),
                                                            t("entityTables.boolean.no")
                                                        )}
                                                    </TableCell>
                                                ))}
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                        </div>

                        <div className="shrink-0 border-t border-gray-100 dark:border-white/[0.05]">
                            <Pagination
                                currentPage={page + 1}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={(p) => setPage(p - 1)}
                                onPageSizeChange={(size) => {
                                    setPageSize(size);
                                    setPage(0);
                                }}
                            />
                        </div>
                    </div>
                </ComponentCard>
            </div>

        </>
    );
}
