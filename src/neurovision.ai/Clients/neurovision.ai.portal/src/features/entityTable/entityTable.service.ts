import { get, post } from "../../api/api";
import { EntityTableField } from "./tables.config";
import { EntityTablePageResponse } from "./entityTable.types";

const coerceValue = (kind: EntityTableField["kind"], raw: string): unknown => {
    switch (kind) {
        case "bool":
            return raw === "true";
        case "int": {
            const parsed = Number.parseInt(raw, 10);
            return Number.isNaN(parsed) ? raw : parsed;
        }
        case "decimal": {
            const parsed = Number.parseFloat(raw);
            return Number.isNaN(parsed) ? raw : parsed;
        }
        case "datetime": {
            if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$/.test(raw)) {
                return `${raw}:00`;
            }

            if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$/.test(raw)) {
                return raw;
            }

            const parsed = new Date(raw);
            return Number.isNaN(parsed.getTime()) ? raw : parsed.toISOString();
        }
        case "time":
            return /^\d{2}:\d{2}$/.test(raw) ? `${raw}:00` : raw;
        default:
            return raw;
    }
};

const toPayload = (
    values: Record<string, string>,
    fields: EntityTableField[]
): Record<string, unknown> => {
    const payload: Record<string, unknown> = {};

    for (const field of fields) {
        const raw = values[field.key];
        if (raw === undefined) continue;
        if (field.kind !== "bool" && raw.trim() === "") continue;
        payload[field.key] = coerceValue(field.kind, raw);
    }

    return payload;
};

export const getEntityTable = async (
    apiPath: string,
    pageIndex: number,
    pageSize: number,
    search?: string
): Promise<EntityTablePageResponse> => {
    const query = new URLSearchParams({
        pageIndex: pageIndex.toString(),
        pageSize: pageSize.toString(),
    });

    if (search) query.append("search", search);

    return await get(`${apiPath}?${query.toString()}`);
};

export const createEntityTableRow = async (
    apiPath: string,
    values: Record<string, string>,
    fields: EntityTableField[]
): Promise<void> => {
    await post(apiPath, toPayload(values, fields));
};
