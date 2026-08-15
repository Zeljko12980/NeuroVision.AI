/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface CountryForm {

    code: string;

    name: string;

    foundingDate: string;

    capitalSettlementCode?: string;

    governmentTypeCode?: string;

    callingCode?: number;

    flag?: File | string | null;

    coatOfArms?: File | string | null;

    anthem?: File | string | null;

}


export interface CountryRequest extends CountryForm { }


export interface CreateCountryResponse {

    code: string;

    name: string;

    foundingDate: string;

    callingCode?: number | null;

    governmentTypeCode?: string | null;

}


export interface CountryResponse {

    code: string;

    name: string;

    foundingDate: string | null;

    callingCode?: number | null;

    governmentTypeCode?: string | null;

}


export interface PaginatedCountryResponse {

    data: CountryResponse[];

    count: number;

}