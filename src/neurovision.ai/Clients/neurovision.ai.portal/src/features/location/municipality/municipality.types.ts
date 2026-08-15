/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface MunicipalityForm {

    countryCode: string;

    code: number;

    name: string;

    seatSettlementCode?: number;

}

export interface MunicipalityRequest extends MunicipalityForm { }

export interface MunicipalityKey {

    countryCode: string;
    code: number;

}

export interface CreateMunicipalityResponse {

    countryCode: string;

    code: number;

    name: string;

}

export interface MunicipalityResponse {

    countryCode: string;

    code: number;

    name: string;

    seatSettlementCode?: number | null;

}

export interface PaginatedMunicipalityResponse {

    data: MunicipalityResponse[];

    count: number;

}
