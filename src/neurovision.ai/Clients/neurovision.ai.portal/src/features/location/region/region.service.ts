import { get, post, put, del } from "../../../api/api";

import {
    RegionRequest,
    RegionKey,
    RegionResponse,
    CreateRegionResponse,
    PaginatedRegionResponse,
} from "./region.types";


export const getRegions = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedRegionResponse> => {


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
        `/region?${query.toString()}`
    );

};


export const getRegionByKey = async (

    key: RegionKey

): Promise<RegionResponse> => {


    return await get(
        `/region/${key.typeCode}/${key.code}`
    );

};


export const createRegion = async (

    data: RegionRequest

): Promise<CreateRegionResponse> => {


    return await post(
        "/region",
        data
    );

};


export const updateRegion = async (

    key: RegionKey,

    data: RegionRequest

): Promise<void> => {


    await put(
        `/region/${key.typeCode}/${key.code}`,
        data
    );

};

export const deleteRegion = async (

    key: RegionKey

): Promise<void> => {


    await del(
        `/region/${key.typeCode}/${key.code}`
    );

};
