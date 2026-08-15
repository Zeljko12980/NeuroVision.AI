import { get, post, put, del } from "../../../api/api";

import {
    RegionSettlementCoverageRequest,
    RegionSettlementCoverageKey,
    RegionSettlementCoverageResponse,
    CreateRegionSettlementCoverageResponse,
    PaginatedRegionSettlementCoverageResponse,
} from "./regionSettlementCoverage.types";


export const getRegionSettlementCoverages = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedRegionSettlementCoverageResponse> => {


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
        `/regionsettlementcoverage?${query.toString()}`
    );

};


export const getRegionSettlementCoverageByKey = async (

    key: RegionSettlementCoverageKey

): Promise<RegionSettlementCoverageResponse> => {


    return await get(
        `/regionsettlementcoverage/${key.regionTypeCode}/${key.regionCode}/${key.countryCode}/${key.settlementCode}`
    );

};


export const createRegionSettlementCoverage = async (

    data: RegionSettlementCoverageRequest

): Promise<CreateRegionSettlementCoverageResponse> => {


    return await post(
        "/regionsettlementcoverage",
        data
    );

};


export const updateRegionSettlementCoverage = async (

    key: RegionSettlementCoverageKey,

    data: RegionSettlementCoverageRequest

): Promise<void> => {


    await put(
        `/regionsettlementcoverage/${key.regionTypeCode}/${key.regionCode}/${key.countryCode}/${key.settlementCode}`,
        data
    );

};

export const deleteRegionSettlementCoverage = async (

    key: RegionSettlementCoverageKey

): Promise<void> => {


    await del(
        `/regionsettlementcoverage/${key.regionTypeCode}/${key.regionCode}/${key.countryCode}/${key.settlementCode}`
    );

};
