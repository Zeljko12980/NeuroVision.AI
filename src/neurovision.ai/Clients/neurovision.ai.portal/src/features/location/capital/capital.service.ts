import { get, post, put, del } from "../../../api/api";

import {
    CapitalRequest,
    CapitalResponse,
    CreateCapitalResponse,
    PaginatedCapitalResponse,
} from "./capital.types";


export const getCapitals = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedCapitalResponse> => {


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
        `/capital?${query.toString()}`
    );

};


export const getCapitalByCountryCode = async (

    countryCode: string

): Promise<CapitalResponse> => {


    return await get(
        `/capital/${countryCode}`
    );

};


export const createCapital = async (

    data: CapitalRequest

): Promise<CreateCapitalResponse> => {


    return await post(
        "/capital",
        data
    );

};


export const updateCapital = async (

    countryCode: string,

    data: CapitalRequest

): Promise<void> => {


    await put(
        `/capital/${countryCode}`,
        data
    );

};

export const deleteCapital = async (

    countryCode: string

): Promise<void> => {


    await del(
        `/capital/${countryCode}`
    );

};
