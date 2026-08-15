/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface RegionForm {

    typeCode: string;

    code: number;

    name: string;

    belongsToCountryCode?: string;

    headquartersCountryCode?: string;

    administrativeSeatSettlementCode?: number;

}

export interface RegionRequest extends RegionForm { }

export interface RegionKey {

    typeCode: string;
    code: number;

}

export interface CreateRegionResponse {

    typeCode: string;

    code: number;

    name: string;

}

export interface RegionResponse {

    typeCode: string;

    code: number;

    name: string;

    belongsToCountryCode?: string | null;

    headquartersCountryCode?: string | null;

    administrativeSeatSettlementCode?: number | null;

}

export interface PaginatedRegionResponse {

    data: RegionResponse[];

    count: number;

}
