import { get, post, put, del } from "../../../api/api";

import {
    MunicipalitySettlementCoverageRequest,
    MunicipalitySettlementCoverageKey,
    MunicipalitySettlementCoverageResponse,
    CreateMunicipalitySettlementCoverageResponse,
    PaginatedMunicipalitySettlementCoverageResponse,
} from "./municipalitySettlementCoverage.types";


export const getMunicipalitySettlementCoverages = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedMunicipalitySettlementCoverageResponse> => {


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
        `/municipalitysettlementcoverage?${query.toString()}`
    );

};


export const getMunicipalitySettlementCoverageByKey = async (

    key: MunicipalitySettlementCoverageKey

): Promise<MunicipalitySettlementCoverageResponse> => {


    return await get(
        `/municipalitysettlementcoverage/${key.countryCode}/${key.municipalityCode}/${key.settlementCode}`
    );

};


export const createMunicipalitySettlementCoverage = async (

    data: MunicipalitySettlementCoverageRequest

): Promise<CreateMunicipalitySettlementCoverageResponse> => {


    return await post(
        "/municipalitysettlementcoverage",
        data
    );

};


export const updateMunicipalitySettlementCoverage = async (

    key: MunicipalitySettlementCoverageKey,

    data: MunicipalitySettlementCoverageRequest

): Promise<void> => {


    await put(
        `/municipalitysettlementcoverage/${key.countryCode}/${key.municipalityCode}/${key.settlementCode}`,
        data
    );

};

export const deleteMunicipalitySettlementCoverage = async (

    key: MunicipalitySettlementCoverageKey

): Promise<void> => {


    await del(
        `/municipalitysettlementcoverage/${key.countryCode}/${key.municipalityCode}/${key.settlementCode}`
    );

};
