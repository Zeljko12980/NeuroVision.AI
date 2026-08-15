import { get, post, put, del } from "../../../api/api";

import {
    LegalSuccessorRequest,
    LegalSuccessorKey,
    LegalSuccessorResponse,
    CreateLegalSuccessorResponse,
    PaginatedLegalSuccessorResponse,
} from "./legalSuccessor.types";


export const getLegalSuccessors = async (

    pageIndex: number,

    pageSize: number,

    search?: string

): Promise<PaginatedLegalSuccessorResponse> => {


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
        `/legalsuccessor?${query.toString()}`
    );

};


export const getLegalSuccessorByKey = async (

    key: LegalSuccessorKey

): Promise<LegalSuccessorResponse> => {


    return await get(
        `/legalsuccessor/${key.successorCountryCode}/${key.predecessorCountryCode}`
    );

};


export const createLegalSuccessor = async (

    data: LegalSuccessorRequest

): Promise<CreateLegalSuccessorResponse> => {


    return await post(
        "/legalsuccessor",
        data
    );

};


export const updateLegalSuccessor = async (

    key: LegalSuccessorKey,

    data: LegalSuccessorRequest

): Promise<void> => {


    await put(
        `/legalsuccessor/${key.successorCountryCode}/${key.predecessorCountryCode}`,
        data
    );

};

export const deleteLegalSuccessor = async (

    key: LegalSuccessorKey

): Promise<void> => {


    await del(
        `/legalsuccessor/${key.successorCountryCode}/${key.predecessorCountryCode}`
    );

};
