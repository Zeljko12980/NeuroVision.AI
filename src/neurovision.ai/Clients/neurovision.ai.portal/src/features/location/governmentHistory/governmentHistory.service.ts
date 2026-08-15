import { get, post, put, del } from "../../../api/api";

import {
    GovernmentHistoryRequest,
    GovernmentHistoryKey,
    GovernmentHistoryResponse,
    CreateGovernmentHistoryResponse,
    PaginatedGovernmentHistoryResponse,
} from "./governmentHistory.types";


export const getGovernmentHistories = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedGovernmentHistoryResponse> => {


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
        `/governmenthistory?${query.toString()}`
    );

};


export const getGovernmentHistoryByKey = async (

    key: GovernmentHistoryKey

): Promise<GovernmentHistoryResponse> => {


    return await get(
        `/governmenthistory/${key.countryCode}/${key.sequenceNumber}`
    );

};


export const createGovernmentHistory = async (

    data: GovernmentHistoryRequest

): Promise<CreateGovernmentHistoryResponse> => {


    return await post(
        "/governmenthistory",
        data
    );

};


export const updateGovernmentHistory = async (

    key: GovernmentHistoryKey,

    data: GovernmentHistoryRequest

): Promise<void> => {


    await put(
        `/governmenthistory/${key.countryCode}/${key.sequenceNumber}`,
        data
    );

};

export const deleteGovernmentHistory = async (

    key: GovernmentHistoryKey

): Promise<void> => {


    await del(
        `/governmenthistory/${key.countryCode}/${key.sequenceNumber}`
    );

};
