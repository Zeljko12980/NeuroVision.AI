import { get, post, put, del } from "../../../api/api";

import {
    LocalCommunityCoverageRequest,
    LocalCommunityCoverageKey,
    LocalCommunityCoverageResponse,
    CreateLocalCommunityCoverageResponse,
    PaginatedLocalCommunityCoverageResponse,
} from "./localCommunityCoverage.types";


export const getLocalCommunityCoverages = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedLocalCommunityCoverageResponse> => {


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
        `/localcommunitycoverage?${query.toString()}`
    );

};


export const getLocalCommunityCoverageByKey = async (

    key: LocalCommunityCoverageKey

): Promise<LocalCommunityCoverageResponse> => {


    return await get(
        `/localcommunitycoverage/${key.countryCode}/${key.municipalityCode}/${key.localCommunityIdentifier}/${key.settlementCode}`
    );

};


export const createLocalCommunityCoverage = async (

    data: LocalCommunityCoverageRequest

): Promise<CreateLocalCommunityCoverageResponse> => {


    return await post(
        "/localcommunitycoverage",
        data
    );

};


export const updateLocalCommunityCoverage = async (

    key: LocalCommunityCoverageKey,

    data: LocalCommunityCoverageRequest

): Promise<void> => {


    await put(
        `/localcommunitycoverage/${key.countryCode}/${key.municipalityCode}/${key.localCommunityIdentifier}/${key.settlementCode}`,
        data
    );

};

export const deleteLocalCommunityCoverage = async (

    key: LocalCommunityCoverageKey

): Promise<void> => {


    await del(
        `/localcommunitycoverage/${key.countryCode}/${key.municipalityCode}/${key.localCommunityIdentifier}/${key.settlementCode}`
    );

};
