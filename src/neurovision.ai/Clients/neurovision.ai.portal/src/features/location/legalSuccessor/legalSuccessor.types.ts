/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface LegalSuccessorForm {

    successorCountryCode: string;

    predecessorCountryCode: string;

}

export interface LegalSuccessorRequest extends LegalSuccessorForm { }

export interface LegalSuccessorKey {

    successorCountryCode: string;
    predecessorCountryCode: string;

}

export interface CreateLegalSuccessorResponse {

    successorCountryCode: string;

    predecessorCountryCode: string;

}

export interface LegalSuccessorResponse {

    successorCountryCode: string;

    predecessorCountryCode: string;

}

export interface PaginatedLegalSuccessorResponse {

    data: LegalSuccessorResponse[];

    count: number;

}
