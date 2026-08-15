import { get, post, put, del } from "../../../api/api";

import {
    CountryCompositionRequest,
    CountryCompositionKey,
    CountryCompositionResponse,
    CreateCountryCompositionResponse,
    PaginatedCountryCompositionResponse,
} from "./countryComposition.types";


export const getCountryCompositions = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedCountryCompositionResponse> => {


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
        `/countrycomposition?${query.toString()}`
    );

};


export const getCountryCompositionByKey = async (

    key: CountryCompositionKey

): Promise<CountryCompositionResponse> => {


    return await get(
        `/countrycomposition/${key.unionCountryCode}/${key.memberCountryCode}/${key.sequenceNumber}`
    );

};


export const createCountryComposition = async (

    data: CountryCompositionRequest

): Promise<CreateCountryCompositionResponse> => {


    return await post(
        "/countrycomposition",
        data
    );

};


export const updateCountryComposition = async (

    key: CountryCompositionKey,

    data: CountryCompositionRequest

): Promise<void> => {


    await put(
        `/countrycomposition/${key.unionCountryCode}/${key.memberCountryCode}/${key.sequenceNumber}`,
        data
    );

};

export const deleteCountryComposition = async (

    key: CountryCompositionKey

): Promise<void> => {


    await del(
        `/countrycomposition/${key.unionCountryCode}/${key.memberCountryCode}/${key.sequenceNumber}`
    );

};
