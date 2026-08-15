import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const BASE_PATH = path.resolve(__dirname, "../src");

const ENTITIES = [
  {
    name: "region",
    api: "/region",
    storeKey: "regions",
    key: ["typeCode", "code"],
    fields: [
      { name: "typeCode", type: "string" },
      { name: "code", type: "number" },
      { name: "name", type: "string" },
      { name: "belongsToCountryCode", type: "string", optional: true },
      { name: "headquartersCountryCode", type: "string", optional: true },
      { name: "administrativeSeatSettlementCode", type: "number", optional: true },
    ],
    hasCreatePage: true,
    route: "regions",
  },
  {
    name: "municipality",
    api: "/municipality",
    storeKey: "municipalities",
    key: ["countryCode", "code"],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "code", type: "number" },
      { name: "name", type: "string" },
      { name: "seatSettlementCode", type: "number", optional: true },
    ],
    hasCreatePage: true,
    route: "municipalities",
  },
  {
    name: "capital",
    api: "/capital",
    storeKey: "capitals",
    key: ["countryCode"],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "settlementCode", type: "number" },
    ],
    hasCreatePage: true,
    route: "capitals",
  },
  {
    name: "regionType",
    api: "/regiontype",
    storeKey: "regionTypes",
    key: ["code"],
    fields: [
      { name: "code", type: "string" },
      { name: "name", type: "string" },
    ],
    hasCreatePage: true,
    route: "region-types",
  },
  {
    name: "localCommunity",
    api: "/localcommunity",
    storeKey: "localCommunities",
    key: ["countryCode", "municipalityCode", "identifier"],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "municipalityCode", type: "number" },
      { name: "identifier", type: "number" },
      { name: "name", type: "string" },
      { name: "officeSettlementCode", type: "number", optional: true },
    ],
    hasCreatePage: true,
    route: "local-communities",
  },
  {
    name: "countryComposition",
    api: "/countrycomposition",
    storeKey: "countryCompositions",
    key: ["unionCountryCode", "memberCountryCode", "sequenceNumber"],
    fields: [
      { name: "unionCountryCode", type: "string" },
      { name: "memberCountryCode", type: "string" },
      { name: "sequenceNumber", type: "number" },
      { name: "from", type: "date" },
      { name: "to", type: "date", optional: true },
    ],
    hasCreatePage: true,
    route: "country-compositions",
  },
  {
    name: "regionComposition",
    api: "/regioncomposition",
    storeKey: "regionCompositions",
    key: [
      "parentRegionTypeCode",
      "parentRegionCode",
      "memberRegionTypeCode",
      "memberRegionCode",
    ],
    fields: [
      { name: "parentRegionTypeCode", type: "string" },
      { name: "parentRegionCode", type: "number" },
      { name: "memberRegionTypeCode", type: "string" },
      { name: "memberRegionCode", type: "number" },
    ],
    hasCreatePage: true,
    route: "region-compositions",
  },
  {
    name: "regionSettlementCoverage",
    api: "/regionsettlementcoverage",
    storeKey: "regionSettlementCoverages",
    key: ["regionTypeCode", "regionCode", "countryCode", "settlementCode"],
    fields: [
      { name: "regionTypeCode", type: "string" },
      { name: "regionCode", type: "number" },
      { name: "countryCode", type: "string" },
      { name: "settlementCode", type: "number" },
    ],
    hasCreatePage: true,
    route: "region-settlement-coverages",
  },
  {
    name: "municipalitySettlementCoverage",
    api: "/municipalitysettlementcoverage",
    storeKey: "municipalitySettlementCoverages",
    key: ["countryCode", "municipalityCode", "settlementCode"],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "municipalityCode", type: "number" },
      { name: "settlementCode", type: "number" },
    ],
    hasCreatePage: true,
    route: "municipality-settlement-coverages",
  },
  {
    name: "localCommunityCoverage",
    api: "/localcommunitycoverage",
    storeKey: "localCommunityCoverages",
    key: [
      "countryCode",
      "municipalityCode",
      "localCommunityIdentifier",
      "settlementCode",
    ],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "municipalityCode", type: "number" },
      { name: "localCommunityIdentifier", type: "number" },
      { name: "settlementCode", type: "number" },
    ],
    hasCreatePage: true,
    route: "local-community-coverages",
  },
  {
    name: "legalSuccessor",
    api: "/legalsuccessor",
    storeKey: "legalSuccessors",
    key: ["successorCountryCode", "predecessorCountryCode"],
    fields: [
      { name: "successorCountryCode", type: "string" },
      { name: "predecessorCountryCode", type: "string" },
    ],
    hasCreatePage: true,
    route: "legal-successors",
  },
  {
    name: "governmentHistory",
    api: "/governmenthistory",
    storeKey: "governmentHistories",
    key: ["countryCode", "sequenceNumber"],
    fields: [
      { name: "countryCode", type: "string" },
      { name: "sequenceNumber", type: "number" },
      { name: "governmentTypeCode", type: "string" },
      { name: "from", type: "date" },
      { name: "to", type: "date", optional: true },
    ],
    hasCreatePage: true,
    route: "government-histories",
  },
];

function toPascalCase(str) {
  return str.charAt(0).toUpperCase() + str.slice(1);
}

function pluralPascal(storeKey) {
  return storeKey.charAt(0).toUpperCase() + storeKey.slice(1);
}

function tsType(field) {
  if (field.type === "date") return "string";
  return field.type;
}

function tsResponseType(field) {
  if (field.type === "date") {
    return field.optional ? "string | null" : "string | null";
  }
  if (field.optional && field.type === "number") return "number | null";
  if (field.optional && field.type === "string") return "string | null";
  return tsType(field);
}

function formFieldLine(field) {
  const opt = field.optional ? "?" : "";
  return `    ${field.name}${opt}: ${tsType(field)};`;
}

function responseFieldLine(field) {
  const opt = field.optional ? "?" : "";
  return `    ${field.name}${opt}: ${tsResponseType(field)};`;
}

function keyInterfaceFields(key, fields) {
  return key
    .map((k) => {
      const f = fields.find((x) => x.name === k);
      return `    ${k}: ${tsType(f)};`;
    })
    .join("\n");
}

function buildKeyPathExpr(key, prefix = "key") {
  return key.map((k) => `\${${prefix}.${k}}`).join("/");
}

function buildKeyPathFromVar(varName, key) {
  return `\`${varName}/${buildKeyPathExpr(key, "key")}\``;
}

function matchesKeyFilter(key) {
  if (key.length === 1) {
    const k = key[0];
    return `x.${k} !== action.payload.${k}`;
  }
  return key.map((k) => `x.${k} !== action.payload.${k}`).join(" || ");
}

function rowKeyExpr(key) {
  return key.map((k) => `item.${k}`).join(' + "-" + ');
}

function dropdownIdExpr(key) {
  if (key.length === 1) return `item.${key[0]}`;
  return `[${key.map((k) => `item.${k}`).join(", ")}].join("-")`;
}

function isKeyField(fieldName, key) {
  return key.includes(fieldName);
}

function requiredFields(fields) {
  return fields.filter((f) => !f.optional);
}

function writeFile(relativePath, content, created) {
  const fullPath = path.join(BASE_PATH, relativePath);
  fs.mkdirSync(path.dirname(fullPath), { recursive: true });
  fs.writeFileSync(fullPath, content, "utf8");
  created.push(relativePath);
}

function generateTypes(entity) {
  const { name, storeKey, key, fields } = entity;
  const Pascal = toPascalCase(name);

  const hasKeyInterface = key.length > 1 || key[0] !== fields[0]?.name;

  return `/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface ${Pascal}Form {

${fields.map(formFieldLine).join("\n\n")}

}

export interface ${Pascal}Request extends ${Pascal}Form { }

export interface ${Pascal}Key {

${keyInterfaceFields(key, fields)}

}

export interface Create${Pascal}Response {

${fields.filter((f) => !f.optional).map(responseFieldLine).join("\n\n")}

}

export interface ${Pascal}Response {

${fields.map(responseFieldLine).join("\n\n")}

}

export interface Paginated${Pascal}Response {

    data: ${Pascal}Response[];

    count: number;

}
`;
}

function generateService(entity) {
  const { name, api, key, fields } = entity;
  const Pascal = toPascalCase(name);
  const plural = pluralPascal(entity.storeKey);
  const camelPlural = entity.storeKey;

  const getByKeyParams =
    key.length === 1
      ? `${key[0]}: ${tsType(fields.find((f) => f.name === key[0]))}`
      : `key: ${Pascal}Key`;

  const getByKeyPath =
    key.length === 1
      ? `\`${api}/\${${key[0]}}\``
      : buildKeyPathFromVar(api, key);

  const updateParams =
    key.length === 1
      ? `${key[0]}: ${tsType(fields.find((f) => f.name === key[0]))},\n\n    data: ${Pascal}Request`
      : `key: ${Pascal}Key,\n\n    data: ${Pascal}Request`;

  const updatePath =
    key.length === 1
      ? `\`${api}/\${${key[0]}}\``
      : buildKeyPathFromVar(api, key);

  const deleteParams = getByKeyParams;
  const deletePath = getByKeyPath;

  const getByKeyFnName =
    key.length === 1
      ? `get${Pascal}By${toPascalCase(key[0])}`
      : `get${Pascal}ByKey`;

  return `import { get, post, put, del } from "../../../api/api";

import {
    ${Pascal}Request,
    ${Pascal}Key,
    ${Pascal}Response,
    Create${Pascal}Response,
    Paginated${Pascal}Response,
} from "./${name}.types";


export const get${plural} = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<Paginated${Pascal}Response> => {


    const query = new URLSearchParams({

        pageIndex:
            pageIndex.toString(),

        pageSize:
            pageSize.toString(),

    });



    if (search)
        query.append(
            "search",
            search
        );



    return await get(
        \`${api}?\${query.toString()}\`
    );

};


export const ${getByKeyFnName} = async (

    ${getByKeyParams}

): Promise<${Pascal}Response> => {


    return await get(
        ${getByKeyPath}
    );

};


export const create${Pascal} = async (

    data: ${Pascal}Request

): Promise<Create${Pascal}Response> => {


    return await post(
        "${api}",
        data
    );

};


export const update${Pascal} = async (

    ${updateParams}

): Promise<void> => {


    await put(
        ${updatePath},
        data
    );

};

export const delete${Pascal} = async (

    ${deleteParams}

): Promise<void> => {


    await del(
        ${deletePath}
    );

};
`;
}

function generateSlice(entity) {
  const { name, storeKey, key, fields } = entity;
  const Pascal = toPascalCase(name);
  const plural = pluralPascal(entity.storeKey);
  const camelPlural = storeKey;

  const getByKeyFnName =
    key.length === 1
      ? `get${Pascal}By${toPascalCase(key[0])}`
      : `get${Pascal}ByKey`;

  const fetchByKeyArg = key.length === 1 ? key[0] : "key";
  const fetchByKeyType =
    key.length === 1
      ? tsType(fields.find((f) => f.name === key[0]))
      : `${Pascal}Key`;

  const updatePayloadType =
    key.length === 1
      ? `{
        ${key[0]}: ${tsType(fields.find((f) => f.name === key[0]))};
        request: ${Pascal}Request;
    }`
      : `{
        key: ${Pascal}Key;
        request: ${Pascal}Request;
    }`;

  const updateCall =
    key.length === 1
      ? `await update${Pascal}(\n                ${key[0]},\n                request\n            );`
      : `await update${Pascal}(\n                key,\n                request\n            );`;

  const updateReturn = key.length === 1 ? key[0] : "key";

  const updateDestructure =
    key.length === 1 ? `{ ${key[0]}, request }` : `{ key, request }`;

  const deleteArgType =
    key.length === 1
      ? tsType(fields.find((f) => f.name === key[0]))
      : `${Pascal}Key`;

  const deleteFilter =
    key.length === 1
      ? `x.${key[0]} !== action.payload`
      : `!(${key.map((k) => `x.${k} === action.payload.${k}`).join(" && ")})`;

  const fetchByKeyServiceCall =
    key.length === 1
      ? `return await ${getByKeyFnName}(${fetchByKeyArg});`
      : `return await ${getByKeyFnName}(key);`;

  return `import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    ${Pascal}Request,
    ${Pascal}Response,
    ${Pascal}Key,
    Create${Pascal}Response,
    Paginated${Pascal}Response
} from "./${name}.types";


import {
    get${plural},
    ${getByKeyFnName},
    create${Pascal},
    update${Pascal},
    delete${Pascal},
} from "./${name}.service";





interface ${Pascal}State {

    items: ${Pascal}Response[];

    selected: ${Pascal}Response | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: ${Pascal}State = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetch${plural} = createAsyncThunk<
    Paginated${Pascal}Response,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "${camelPlural}/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await get${plural}(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch ${camelPlural}"
            );

        }

    }

);


export const fetch${Pascal}ByKey = createAsyncThunk<
    ${Pascal}Response,
    ${fetchByKeyType},
    {
        rejectValue: string;
    }
>(

    "${camelPlural}/getByKey",

    async (
        ${fetchByKeyArg},
        { rejectWithValue }
    ) => {

        try {

            ${fetchByKeyServiceCall}

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get ${name}"
            );

        }

    }

);


export const createNew${Pascal} = createAsyncThunk<
    Create${Pascal}Response,
    ${Pascal}Request,
    {
        rejectValue: string;
    }
>(

    "${camelPlural}/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await create${Pascal}(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create ${name}"
            );

        }

    }

);


export const updateExisting${Pascal} = createAsyncThunk<
    ${key.length === 1 ? tsType(fields.find((f) => f.name === key[0])) : `${Pascal}Key`},
    ${updatePayloadType},
    {
        rejectValue: string;
    }
>(

    "${camelPlural}/update",

    async (
        ${updateDestructure},

        { rejectWithValue }

    ) => {

        try {


            ${updateCall}


            return ${updateReturn};


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update ${name}"
            );

        }

    }

);


export const deleteExisting${Pascal} = createAsyncThunk<
    ${deleteArgType},
    ${deleteArgType},
    {
        rejectValue: string;
    }
>(

    "${camelPlural}/delete",

    async (
        ${key.length === 1 ? key[0] : "key"},
        { rejectWithValue }
    ) => {

        try {

            await delete${Pascal}(${key.length === 1 ? key[0] : "key"});


            return ${key.length === 1 ? key[0] : "key"};


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete ${name}"
            );

        }

    }

);


const ${name}Slice = createSlice({

    name: "${camelPlural}",

    initialState,


    reducers: {


        clearSelected(state) {

            state.selected = null;

        },


        clearError(state) {

            state.error = null;

        }


    },



    extraReducers: builder => {


        builder
            .addCase(
                fetch${plural}.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetch${plural}.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        action.payload.data;


                    state.totalCount =
                        action.payload.count;

                }
            )
            .addCase(
                fetch${plural}.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                fetch${Pascal}ByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetch${Pascal}ByKey.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.selected =
                        action.payload;

                }
            )
            .addCase(
                fetch${Pascal}ByKey.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                createNew${Pascal}.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNew${Pascal}.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items.push(
                        action.payload
                    );

                }
            )
            .addCase(
                createNew${Pascal}.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                updateExisting${Pascal}.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExisting${Pascal}.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExisting${Pascal}.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                deleteExisting${Pascal}.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExisting${Pascal}.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                ${deleteFilter}
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExisting${Pascal}.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            );


    }


});

export const select${plural} = (
    state: {
        ${camelPlural}: ${Pascal}State
    }
) => state.${camelPlural}.items;

export const {
    clearSelected,
    clearError

} = ${name}Slice.actions;



export default ${name}Slice.reducer;
`;
}

function cellDisplay(field) {
  if (field.type === "date") {
    return `{item.${field.name}
                                                        ? new Date(
                                                            item.${field.name}
                                                        ).toLocaleDateString()
                                                        : ""}`;
  }
  if (field.optional) {
    return `{item.${field.name} ?? "-"}`;
  }
  return `{item.${field.name}}`;
}

function generateTable(entity) {
  const { name, storeKey, key, fields } = entity;
  const Pascal = toPascalCase(name);
  const plural = pluralPascal(storeKey);
  const i18n = storeKey;

  const deleteKeyArg =
    key.length === 1
      ? `item.${key[0]}`
      : `{ ${key.map((k) => `${k}: item.${k}`).join(", ")} }`;

  const deleteHandlerParam = key.length === 1 ? key[0] : "key";
  const deleteHandlerType =
    key.length === 1
      ? tsType(fields.find((f) => f.name === key[0]))
      : `${Pascal}Key`;

  const selectedKeyState =
    key.length === 1
      ? `const [selectedKey, setSelectedKey] =
        useState<${deleteHandlerType} | null>(null);`
      : `const [selectedKey, setSelectedKey] =
        useState<${Pascal}Key | null>(null);`;

  const updatePayload =
    key.length === 1
      ? `{ ${key[0]}: updated.${key[0]}, request: updated }`
      : `{ key: { ${key.map((k) => `${k}: updated.${k}`).join(", ")} }, request: updated }`;

  const columnCells = fields
    .map(
      (f) => `
                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.${i18n}.columns.${f.name}")}
                                        </TableCell>`
    )
    .join("\n");

  const dataCells = fields
    .map(
      (f) => `
                                                <TableCell className="px-5 py-4 text-sm${f.name === key[0] ? " font-semibold" : ""}">
                                                    ${cellDisplay(f)}
                                                </TableCell>`
    )
    .join("\n");

  return `import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../../components/common/PageMeta";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import ComponentCard from "../../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../../components/ui/table";

import { RefreshIcon } from "../../../icons";

import Pagination from "../../../components/ui/pagination/Pagination";
import { Dropdown } from "../../../components/ui/dropdown/Dropdown";


import ConfirmDialog from "../../../components/ui/dialog/ConfirmDialog";
import Edit${Pascal}Modal from "./Edit${Pascal}Modal";

import { useAppDispatch, useAppSelector } from "../../../store/store";
import {
    fetch${plural},
    deleteExisting${Pascal},
    updateExisting${Pascal},
} from "../../../features/location/${name}/${name}.slice";

import { ${Pascal}Response, ${Pascal}Key } from "../../../features/location/${name}/${name}.types";
import { showAlert } from "../../../features/ui/uiSlice";
import ${Pascal}TableSkeleton from "./${Pascal}TableSkeleton";

export default function ${plural}Table() {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();

    const items = useAppSelector((s) => s.${storeKey}.items);
    const total = useAppSelector((s) => s.${storeKey}.totalCount);
    const loading = useAppSelector((s) => s.${storeKey}.loading);

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(5);

    const [openDropdownId, setOpenDropdownId] =
        useState<string | null>(null);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [editOpen, setEditOpen] = useState(false);
    const [selectedItem, setSelectedItem] = useState<${Pascal}Response | null>(null);
    const [editLoading, setEditLoading] = useState(false);

    const [confirmOpen, setConfirmOpen] = useState(false);
    ${selectedKeyState}
    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.ceil(total / pageSize);

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const getRowId = (item: ${Pascal}Response) =>
        ${dropdownIdExpr(key).replace(/item\./g, "item.")};

    const loadItems = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            const result = await dispatch(
                fetch${plural}({
                    pageIndex: page,
                    pageSize,
                })
            ).unwrap();


            const totalPages = Math.ceil(
                result.count / pageSize
            );

            if (
                page >= totalPages &&
                totalPages > 0
            ) {
                setPage(totalPages - 1);
                return;
            }


            if (
                totalPages === 0 &&
                page !== 0
            ) {
                setPage(0);
                return;
            }


            await delay(700);

        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadItems();
    }, [page, pageSize]);

    const toggleDropdown = (id: string) => {
        setOpenDropdownId((prev) =>
            prev === id ? null : id
        );
    };

    const closeDropdown = () => setOpenDropdownId(null);

    const handleDeleteClick = (item: ${Pascal}Response) => {
        setSelectedKey(${key.length === 1 ? `item.${key[0]}` : `{ ${key.map((k) => `${k}: item.${k}`).join(", ")} }`});
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {
        if (!selectedKey) return;

        try {
            setDeleting(true);

            await dispatch(
                deleteExisting${Pascal}(selectedKey)
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.${i18n}.messages.deleteSuccess"
                    ),
                })
            );

            setConfirmOpen(false);
            setSelectedKey(null);

            await loadItems();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err?.message ??
                        t(
                            "location.${i18n}.messages.deleteError"
                        ),
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("location.${i18n}.pageTitle")}
                description={t("location.${i18n}.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("location.${i18n}.pageTitle")}
            />

            <div className="space-y-6">
                <ComponentCard title={t("location.${i18n}.title")}>

                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadItems}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={\`w-5 h-5 bg-color-gray \${spinning ? "animate-spin" : "rotate-90"
                                    }\`}
                            />
                        </button>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">

                        <div className="flex-1 overflow-y-auto">

                            <Table>

                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">

                                    <TableRow>
${columnCells}

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("location.${i18n}.columns.actions")}
                                        </TableCell>

                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (
                                        <${Pascal}TableSkeleton rows={5} />
                                    ) : (
                                        items.map((item) => (
                                            <TableRow
                                                key={getRowId(item)}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
${dataCells}

                                                <TableCell className="px-5 py-4 relative">

                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            toggleDropdown(
                                                                getRowId(item)
                                                            )
                                                        }
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={
                                                            openDropdownId ===
                                                            getRowId(item)
                                                        }
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedItem(
                                                                        item
                                                                    );
                                                                    setEditOpen(
                                                                        true
                                                                    );
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.${i18n}.actions.edit"
                                                                )}
                                                            </button>

                                                            <button
                                                                onClick={() =>
                                                                    handleDeleteClick(
                                                                        item
                                                                    )
                                                                }
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t(
                                                                    "location.${i18n}.actions.delete"
                                                                )}
                                                            </button>

                                                        </div>
                                                    </Dropdown>

                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}

                                </TableBody>

                            </Table>

                        </div>

                        <div className="border-t border-gray-100 dark:border-white/[0.05] shrink-0">
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

            <ConfirmDialog
                isOpen={confirmOpen}
                title={t("location.${i18n}.messages.deleteTitle")}
                description={t(
                    "location.${i18n}.messages.deleteDescription"
                )}
                onConfirm={handleConfirmDelete}
                onCancel={() => setConfirmOpen(false)}
                loading={deleting}
            />

            <Edit${Pascal}Modal
                isOpen={editOpen}
                item={selectedItem}
                loading={editLoading}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedItem(null);
                }}
                onSave={async (updated) => {
                    try {
                        setEditLoading(true);

                        await dispatch(
                            updateExisting${Pascal}(${updatePayload})
                        ).unwrap();

                        dispatch(
                            showAlert({
                                type: "success",
                                message: t(
                                    "location.${i18n}.messages.updateSuccess"
                                ),
                            })
                        );

                        setEditOpen(false);
                        setSelectedItem(null);

                        await loadItems();
                    } catch (err: any) {
                        dispatch(
                            showAlert({
                                type: "error",
                                message:
                                    err?.message ??
                                    t(
                                        "location.${i18n}.messages.updateError"
                                    ),
                            })
                        );
                    } finally {
                        setEditLoading(false);
                    }
                }}
            />
        </>
    );
}
`;
}

function generateSkeleton(entity) {
  const { name, fields } = entity;
  const Pascal = toPascalCase(name);
  const colCount = fields.length + 1;

  const cells = Array.from({ length: colCount })
    .map(
      (_, idx) => `
                    <TableCell className="px-5 py-4">
                        <div className="h-4 w-${idx === colCount - 1 ? "8" : idx === 0 ? "16" : "28"} bg-gray-200 dark:bg-white/10 rounded${idx === colCount - 1 ? " h-8" : ""}" />
                    </TableCell>`
    )
    .join("\n");

  return `import { TableRow, TableCell } from "../../../components/ui/table";


interface ${Pascal}TableSkeletonProps {
    rows?: number;
}

export default function ${Pascal}TableSkeleton({
    rows = 5,
}: ${Pascal}TableSkeletonProps) {
    return (
        <>
            {Array.from({ length: rows }).map((_, idx) => (
                <TableRow key={idx} className="animate-pulse">
${cells}
                </TableRow>
            ))}
        </>
    );
}
`;
}

function emptyFormValue(field) {
  if (field.type === "number") return "0";
  return '""';
}

function emptyFormObject(fields) {
  return fields
    .map((f) => `    ${f.name}: ${emptyFormValue(f)}`)
    .join(",\n");
}

function formInputField(field, i18n, disabledExpr = "false") {
  const inputType =
    field.type === "number"
      ? "number"
      : field.type === "date"
        ? "date"
        : "text";

  const valueExpr =
    field.type === "number"
      ? `form.${field.name} ?? ""`
      : field.type === "date"
        ? `form.${field.name} ? form.${field.name}.split("T")[0] : ""`
        : `form.${field.name}`;

  const onChangeExpr =
    field.type === "number"
      ? `handleChange(
                                    "${field.name}",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )`
      : `handleChange(
                                    "${field.name}",
                                    e.target.value
                                )`;

  return `
                    <div>
                        <Label>
                            {t("location.${i18n}.fields.${field.name}")}
                        </Label>
                        <Input
                            type="${inputType}"
                            value={${valueExpr}}
                            disabled={${disabledExpr}}
                            onChange={(e) =>
                                ${onChangeExpr}
                            }
                        />
                    </div>`;
}

function generateEditModal(entity) {
  const { name, storeKey, key, fields } = entity;
  const Pascal = toPascalCase(name);
  const i18n = storeKey;

  const formFields = fields
    .map((f) => {
      const disabled = isKeyField(f.name, key) ? "!!item" : "false";
      return formInputField(f, i18n, disabled);
    })
    .join("\n");

  const setFormFromItem = fields
    .map((f) => {
      if (f.type === "date") {
        return `            ${f.name}:\n                item.${f.name}\n                    ? item.${f.name}.split("T")[0]\n                    : "",`;
      }
      if (f.type === "number") {
        return `            ${f.name}: item.${f.name} ?? 0,`;
      }
      if (f.optional) {
        return `            ${f.name}: item.${f.name} ?? undefined,`;
      }
      return `            ${f.name}: item.${f.name} ?? "",`;
    })
    .join("\n");

  const emptyForm = fields
    .map((f) => `    ${f.name}: ${emptyFormValue(f)},`)
    .join("\n");

  const validation = requiredFields(fields)
    .map((f) => {
      if (f.type === "number") {
        return `form.${f.name} !== undefined && form.${f.name} !== null`;
      }
      return `form.${f.name}.toString().trim().length > 0`;
    })
    .join(" &&\n        ");

  return `import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    ${Pascal}Form,
    ${Pascal}Response,
} from "../../../features/location/${name}/${name}.types";


interface Props {
    isOpen: boolean;
    item: ${Pascal}Response | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: ${Pascal}Form) => Promise<void>;
}


const emptyForm: ${Pascal}Form = {
${emptyForm}
};


export default function Edit${Pascal}Modal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<${Pascal}Form>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
${setFormFromItem}
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        ${validation};

    const handleChange = <
        K extends keyof ${Pascal}Form
    >(
        key: K,
        value: ${Pascal}Form[K]
    ) => {
        setForm((previous) => ({
            ...previous,
            [key]: value,
        }));
    };

    const handleSubmit = async () => {
        if (!isValid) return;
        await onSave(form);
        onClose();
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-2xl">
            <div className="bg-white dark:bg-gray-900 rounded-2xl p-6">
                <h2 className="text-xl font-semibold">
                    {t("location.${i18n}.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">
${formFields}
                </div>

                <div className="mt-6 flex justify-end gap-3">
                    <Button variant="outline" onClick={onClose} disabled={loading}>
                        {t("common.cancel")}
                    </Button>
                    <Button onClick={handleSubmit} disabled={loading || !isValid}>
                        {loading ? t("common.saving") : t("common.saveChanges")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}
`;
}

function generateCreatePage(entity) {
  const { name, storeKey, fields, route } = entity;
  const Pascal = toPascalCase(name);
  const i18n = storeKey;

  const formState = fields
    .map((f) => {
      if (f.type === "number") return `        ${f.name}: "",`;
      return `        ${f.name}: "",`;
    })
    .join("\n");

  const formFields = fields
    .map((f) => formInputField(f, i18n, "false"))
    .join("\n");

  const validation = requiredFields(fields)
    .map((f) => {
      if (f.type === "number") return `form.${f.name}.toString().trim() !== ""`;
      return `form.${f.name}.trim() !== ""`;
    })
    .join(" &&\n        ");

  const dispatchPayload = fields
    .map((f) => {
      if (f.type === "number") {
        return `                    ${f.name}: Number(form.${f.name}),`;
      }
      if (f.optional) {
        return `                    ${f.name}: form.${f.name} || undefined,`;
      }
      return `                    ${f.name}: form.${f.name},`;
    })
    .join("\n");

  return `import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import ComponentCard from "../../../components/common/ComponentCard";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import PageMeta from "../../../components/common/PageMeta";

import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";

import {
    createNew${Pascal}
} from "../../../features/location/${name}/${name}.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function Create${Pascal}Page() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

${formState}

    });


    const handleChange = (
        field: string,
        value: string | number | undefined
    ) => {

        setForm(prev => ({
            ...prev,
            [field]: value ?? ""
        }));

    };


    const isValid =
        ${validation};


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.${i18n}.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNew${Pascal}({

${dispatchPayload}

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.${i18n}.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/${route}"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.${i18n}.messages.createError"),
                    type: "error"
                })
            );

        }
        finally {

            setLoading(false);

        }

    };



    return (
        <>

            <PageMeta
                title={\`\${t("location.${i18n}.createTitle")} | NeuroVision.AI\`}
                description={t("location.${i18n}.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.${i18n}.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.${i18n}.createTitle")}>


                    <div className="space-y-5">
${formFields}
                    </div>



                    <div className="
                        flex
                        justify-end
                        gap-3
                        mt-8
                        pt-5
                        border-t
                    ">


                        <Button
                            variant="outline"
                            onClick={() =>
                                navigate(
                                    "/admin/location/${route}"
                                )
                            }
                        >
                            {t("common.cancel")}
                        </Button>



                        <Button

                            disabled={
                                loading ||
                                !isValid
                            }

                            onClick={handleSubmit}

                        >

                            {
                                loading
                                    ?
                                    t("common.creating")
                                    :
                                    t("location.${i18n}.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
`;
}

function generateEntity(entity, created) {
  const { name, hasCreatePage } = entity;
  const Pascal = toPascalCase(name);
  const plural = pluralPascal(entity.storeKey);

  writeFile(
    `features/location/${name}/${name}.types.ts`,
    generateTypes(entity),
    created
  );
  writeFile(
    `features/location/${name}/${name}.service.ts`,
    generateService(entity),
    created
  );
  writeFile(
    `features/location/${name}/${name}.slice.ts`,
    generateSlice(entity),
    created
  );
  writeFile(
    `pages/Location/${name}/${plural}Table.tsx`,
    generateTable(entity),
    created
  );
  writeFile(
    `pages/Location/${name}/${Pascal}TableSkeleton.tsx`,
    generateSkeleton(entity),
    created
  );
  writeFile(
    `pages/Location/${name}/Edit${Pascal}Modal.tsx`,
    generateEditModal(entity),
    created
  );

  if (hasCreatePage) {
    writeFile(
      `pages/Location/${name}/Create${Pascal}Page.tsx`,
      generateCreatePage(entity),
      created
    );
  }
}

const created = [];

for (const entity of ENTITIES) {
  generateEntity(entity, created);
}

console.log(`Generated ${created.length} files:\n`);
created.forEach((f) => console.log(`  ${f}`));
