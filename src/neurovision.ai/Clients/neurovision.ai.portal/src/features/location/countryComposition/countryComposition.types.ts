/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface CountryCompositionForm {

    unionCountryCode: string;

    memberCountryCode: string;

    sequenceNumber: number;

    from: string;

    to?: string;

}

export interface CountryCompositionRequest extends CountryCompositionForm { }

export interface CountryCompositionKey {

    unionCountryCode: string;
    memberCountryCode: string;
    sequenceNumber: number;

}

export interface CreateCountryCompositionResponse {

    unionCountryCode: string;

    memberCountryCode: string;

    sequenceNumber: number;

    from: string | null;

}

export interface CountryCompositionResponse {

    unionCountryCode: string;

    memberCountryCode: string;

    sequenceNumber: number;

    from: string | null;

    to?: string | null;

}

export interface PaginatedCountryCompositionResponse {

    data: CountryCompositionResponse[];

    count: number;

}
