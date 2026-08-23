import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import { useTranslation } from "react-i18next";

import ComponentCard from "../../components/common/ComponentCard";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";
import Input from "../../components/form/input/InputField";
import Label from "../../components/form/Label";
import Button from "../../components/ui/button/Button";
import { useAppDispatch } from "../../store/store";
import { showAlert } from "../../features/ui/uiSlice";
import { createEntityTableRow } from "../../features/entityTable/entityTable.service";
import {
    EntityService,
    EntityTableField,
    getEntityTableBasePath,
    getTablesForService,
} from "../../features/entityTable/tables.config";

const humanize = (key: string) =>
    key
        .replace(/([A-Z])/g, " $1")
        .replace(/^./, (char) => char.toUpperCase())
        .trim();

const emptyForm = (fields: EntityTableField[]) =>
    Object.fromEntries(
        fields.map((field) => [field.key, field.kind === "bool" ? "false" : ""])
    );

export default function CreateEntityTablePage({ service }: { service: EntityService }) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { table = "" } = useParams();

    const definition = useMemo(
        () => getTablesForService(service).find((item) => item.key === table),
        [service, table]
    );

    const tableName = definition
        ? t(definition.nameKey)
        : t("entityTables.unknown");
    const title = t("entityTables.createTitle", { table: tableName });
    const listPath = `${getEntityTableBasePath(service)}/${table}`;

    const [form, setForm] = useState<Record<string, string>>(() =>
        emptyForm(definition?.fields ?? [])
    );
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        setForm(emptyForm(definition?.fields ?? []));
    }, [definition]);

    const isValid = (definition?.fields ?? [])
        .filter((field) => field.required && field.kind !== "bool")
        .every((field) => form[field.key]?.trim());

    const handleSubmit = async () => {
        if (!definition) return;

        if (!isValid) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("entityTables.messages.required"),
                })
            );
            return;
        }

        try {
            setLoading(true);
            await createEntityTableRow(definition.apiPath, form, definition.fields);
            dispatch(
                showAlert({
                    type: "success",
                    message: t("entityTables.messages.createSuccess"),
                })
            );
            navigate(listPath);
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        typeof err === "string"
                            ? err
                            : err instanceof Error
                                ? err.message
                                : t("entityTables.messages.createError"),
                })
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <PageMeta
                title={title}
                description={t("entityTables.pageDescription", { table: tableName })}
            />
            <PageBreadcrumb pageTitle={title} />

            <div className="mx-auto max-w-3xl">
                <ComponentCard title={title}>
                    {!definition ? (
                        <p className="text-sm text-gray-500">{t("entityTables.unknown")}</p>
                    ) : (
                        <>
                            <div className="space-y-5">
                                {definition.fields.map((field) => (
                                    <div key={field.key}>
                                        <Label htmlFor={field.key}>
                                            {t(`entityTables.columns.${field.key}`, {
                                                defaultValue: humanize(field.key),
                                            })}
                                            {field.required ? " *" : ""}
                                        </Label>
                                        {field.kind === "bool" ? (
                                            <label className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-300">
                                                <input
                                                    id={field.key}
                                                    type="checkbox"
                                                    checked={field.kind === "bool" && form[field.key] === "true"}
                                                    onChange={(e) =>
                                                        setForm((prev) => ({
                                                            ...prev,
                                                            [field.key]: e.target.checked ? "true" : "false",
                                                        }))
                                                    }
                                                />
                                                {t("entityTables.boolean.yes")}
                                            </label>
                                        ) : (
                                            <Input
                                                id={field.key}
                                                type={
                                                    field.kind === "datetime"
                                                        ? "datetime-local"
                                                        : field.kind === "time"
                                                            ? "time"
                                                            : field.kind === "int" || field.kind === "decimal"
                                                                ? "number"
                                                                : "text"
                                                }
                                                step={
                                                    field.kind === "decimal"
                                                        ? 0.1
                                                        : field.kind === "datetime"
                                                            ? 1
                                                            : undefined
                                                }
                                                value={form[field.key] ?? ""}
                                                onChange={(e) =>
                                                    setForm((prev) => ({
                                                        ...prev,
                                                        [field.key]: e.target.value,
                                                    }))
                                                }
                                            />
                                        )}
                                    </div>
                                ))}
                            </div>

                            <div className="mt-8 flex justify-end gap-3 border-t pt-5">
                                <Button
                                    type="button"
                                    variant="outline"
                                    onClick={() => navigate(listPath)}
                                    disabled={loading}
                                >
                                    {t("entityTables.cancel")}
                                </Button>
                                <Button
                                    type="button"
                                    onClick={handleSubmit}
                                    disabled={!isValid || loading}
                                >
                                    {loading ? t("entityTables.saving") : t("entityTables.save")}
                                </Button>
                            </div>
                        </>
                    )}
                </ComponentCard>
            </div>
        </>
    );
}
