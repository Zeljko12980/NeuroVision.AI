/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface RegionSettlementCoverageForm {

    regionTypeCode: string;

    regionCode: number;

    countryCode: string;

    settlementCode: number;

}

export interface RegionSettlementCoverageRequest extends RegionSettlementCoverageForm { }

export interface RegionSettlementCoverageKey {

    regionTypeCode: string;
    regionCode: number;
    countryCode: string;
    settlementCode: number;

}

export interface CreateRegionSettlementCoverageResponse {

    regionTypeCode: string;

    regionCode: number;

    countryCode: string;

    settlementCode: number;

}

export interface RegionSettlementCoverageResponse {

    regionTypeCode: string;

    regionCode: number;

    countryCode: string;

    settlementCode: number;

}

export interface PaginatedRegionSettlementCoverageResponse {

    data: RegionSettlementCoverageResponse[];

    count: number;

}
