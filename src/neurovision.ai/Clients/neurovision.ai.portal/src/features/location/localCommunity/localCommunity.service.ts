import { get, post, put, del } from "../../../api/api";

import {
    LocalCommunityRequest,
    LocalCommunityKey,
    LocalCommunityResponse,
    CreateLocalCommunityResponse,
    PaginatedLocalCommunityResponse,
} from "./localCommunity.types";


export const getLocalCommunities = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedLocalCommunityResponse> => {


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
        `/localcommunity?${query.toString()}`
    );

};


export const getLocalCommunityByKey = async (

    key: LocalCommunityKey

): Promise<LocalCommunityResponse> => {


    return await get(
        `/localcommunity/${key.countryCode}/${key.municipalityCode}/${key.identifier}`
    );

};


export const createLocalCommunity = async (

    data: LocalCommunityRequest

): Promise<CreateLocalCommunityResponse> => {


    return await post(
        "/localcommunity",
        data
    );

};


export const updateLocalCommunity = async (

    key: LocalCommunityKey,

    data: LocalCommunityRequest

): Promise<void> => {


    await put(
        `/localcommunity/${key.countryCode}/${key.municipalityCode}/${key.identifier}`,
        data
    );

};

export const deleteLocalCommunity = async (

    key: LocalCommunityKey

): Promise<void> => {


    await del(
        `/localcommunity/${key.countryCode}/${key.municipalityCode}/${key.identifier}`
    );

};
