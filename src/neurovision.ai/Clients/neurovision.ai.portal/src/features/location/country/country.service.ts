import { get, post, put, del } from "../../../api/api";
import { appendFile } from "../../../utils/util";



import {
    CountryRequest
} from "./country.types";


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


export const getCountries = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedCountryResponse> => {


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
        `/country?${query.toString()}`
    );

};


export const getCountryByCode = async (

    code: string

): Promise<CountryResponse> => {


    return await get(
        `/country/${code}`
    );

};


export const createCountry = async (

    data: CountryRequest

) => {


    const formData = new FormData();



    formData.append(
        "Code",
        data.code
    );


    formData.append(
        "Name",
        data.name
    );


    formData.append(
        "FoundingDate",
        data.foundingDate
    );



    if (data.capitalSettlementCode !== undefined) {
        formData.append(
            "CapitalSettlementCode",
            data.capitalSettlementCode.toString()
        );
    }



    if (data.governmentTypeCode) {
        formData.append(
            "GovernmentTypeCode",
            data.governmentTypeCode
        );
    }



    if (data.callingCode !== undefined) {
        formData.append(
            "CallingCode",
            data.callingCode.toString()
        );
    }



    appendFile(
        formData,
        "Flag",
        data.flag
    );


    appendFile(
        formData,
        "CoatOfArms",
        data.coatOfArms
    );


    appendFile(
        formData,
        "Anthem",
        data.anthem
    );



    return await post(
        "/country",
        formData
    );

};


export const updateCountry = async (

    code: string,

    data: CountryRequest

): Promise<void> => {


    const formData = new FormData();



    formData.append(
        "Name",
        data.name
    );



    formData.append(
        "FoundingDate",
        data.foundingDate
    );



    if (data.capitalSettlementCode !== undefined) {
        formData.append(
            "CapitalSettlementCode",
            data.capitalSettlementCode.toString()
        );
    }



    if (data.governmentTypeCode) {
        formData.append(
            "GovernmentTypeCode",
            data.governmentTypeCode
        );
    }



    if (data.callingCode !== undefined) {
        formData.append(
            "CallingCode",
            data.callingCode.toString()
        );
    }



    appendFile(
        formData,
        "Flag",
        data.flag
    );


    appendFile(
        formData,
        "CoatOfArms",
        data.coatOfArms
    );


    appendFile(
        formData,
        "Anthem",
        data.anthem
    );



    await put(
        `/country/${code}`,
        formData
    );

};

export const deleteCountry = async (

    code: string

): Promise<void> => {


    await del(
        `/country/${code}`
    );

};
