/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface CapitalForm {

    countryCode: string;

    settlementCode: number;

}

export interface CapitalRequest extends CapitalForm { }

export interface CapitalKey {

    countryCode: string;

}

export interface CreateCapitalResponse {

    countryCode: string;

    settlementCode: number;

}

export interface CapitalResponse {

    countryCode: string;

    settlementCode: number;

}

export interface PaginatedCapitalResponse {

    data: CapitalResponse[];

    count: number;

}
