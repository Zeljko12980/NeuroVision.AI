import { get, post, put, del } from "../../../api/api";

import {
    MunicipalityRequest,
    MunicipalityKey,
    MunicipalityResponse,
    CreateMunicipalityResponse,
    PaginatedMunicipalityResponse,
} from "./municipality.types";


export const getMunicipalities = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedMunicipalityResponse> => {


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
        `/municipality?${query.toString()}`
    );

};


export const getMunicipalityByKey = async (

    key: MunicipalityKey

): Promise<MunicipalityResponse> => {


    return await get(
        `/municipality/${key.countryCode}/${key.code}`
    );

};


export const createMunicipality = async (

    data: MunicipalityRequest

): Promise<CreateMunicipalityResponse> => {


    return await post(
        "/municipality",
        data
    );

};


export const updateMunicipality = async (

    key: MunicipalityKey,

    data: MunicipalityRequest

): Promise<void> => {


    await put(
        `/municipality/${key.countryCode}/${key.code}`,
        data
    );

};

export const deleteMunicipality = async (

    key: MunicipalityKey

): Promise<void> => {


    await del(
        `/municipality/${key.countryCode}/${key.code}`
    );

};
