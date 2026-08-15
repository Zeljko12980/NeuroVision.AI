import { get, post, put, del } from "../../../api/api";

import {
    RegionTypeRequest,
    RegionTypeResponse,
    CreateRegionTypeResponse,
    PaginatedRegionTypeResponse,
} from "./regionType.types";


export const getRegionTypes = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedRegionTypeResponse> => {


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
        `/regiontype?${query.toString()}`
    );

};


export const getRegionTypeByCode = async (

    code: string

): Promise<RegionTypeResponse> => {


    return await get(
        `/regiontype/${code}`
    );

};


export const createRegionType = async (

    data: RegionTypeRequest

): Promise<CreateRegionTypeResponse> => {


    return await post(
        "/regiontype",
        data
    );

};


export const updateRegionType = async (

    code: string,

    data: RegionTypeRequest

): Promise<void> => {


    await put(
        `/regiontype/${code}`,
        data
    );

};

export const deleteRegionType = async (

    code: string

): Promise<void> => {


    await del(
        `/regiontype/${code}`
    );

};
