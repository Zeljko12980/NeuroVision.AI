/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface GovernmentHistoryForm {

    countryCode: string;

    sequenceNumber: number;

    governmentTypeCode: string;

    from: string;

    to?: string;

}

export interface GovernmentHistoryRequest extends GovernmentHistoryForm { }

export interface GovernmentHistoryKey {

    countryCode: string;
    sequenceNumber: number;

}

export interface CreateGovernmentHistoryResponse {

    countryCode: string;

    sequenceNumber: number;

    governmentTypeCode: string;

    from: string | null;

}

export interface GovernmentHistoryResponse {

    countryCode: string;

    sequenceNumber: number;

    governmentTypeCode: string;

    from: string | null;

    to?: string | null;

}

export interface PaginatedGovernmentHistoryResponse {

    data: GovernmentHistoryResponse[];

    count: number;

}
