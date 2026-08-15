import { get, post, put, del } from "../../../api/api";

import {
    RegionCompositionRequest,
    RegionCompositionKey,
    RegionCompositionResponse,
    CreateRegionCompositionResponse,
    PaginatedRegionCompositionResponse,
} from "./regionComposition.types";


export const getRegionCompositions = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedRegionCompositionResponse> => {


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
        `/regioncomposition?${query.toString()}`
    );

};


export const getRegionCompositionByKey = async (

    key: RegionCompositionKey

): Promise<RegionCompositionResponse> => {


    return await get(
        `/regioncomposition/${key.parentRegionTypeCode}/${key.parentRegionCode}/${key.memberRegionTypeCode}/${key.memberRegionCode}`
    );

};


export const createRegionComposition = async (

    data: RegionCompositionRequest

): Promise<CreateRegionCompositionResponse> => {


    return await post(
        "/regioncomposition",
        data
    );

};


export const updateRegionComposition = async (

    key: RegionCompositionKey,

    data: RegionCompositionRequest

): Promise<void> => {


    await put(
        `/regioncomposition/${key.parentRegionTypeCode}/${key.parentRegionCode}/${key.memberRegionTypeCode}/${key.memberRegionCode}`,
        data
    );

};

export const deleteRegionComposition = async (

    key: RegionCompositionKey

): Promise<void> => {


    await del(
        `/regioncomposition/${key.parentRegionTypeCode}/${key.parentRegionCode}/${key.memberRegionTypeCode}/${key.memberRegionCode}`
    );

};
