/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface LocalCommunityForm {

    countryCode: string;

    municipalityCode: number;

    identifier: number;

    name: string;

    officeSettlementCode?: number;

}

export interface LocalCommunityRequest extends LocalCommunityForm { }

export interface LocalCommunityKey {

    countryCode: string;
    municipalityCode: number;
    identifier: number;

}

export interface CreateLocalCommunityResponse {

    countryCode: string;

    municipalityCode: number;

    identifier: number;

    name: string;

}

export interface LocalCommunityResponse {

    countryCode: string;

    municipalityCode: number;

    identifier: number;

    name: string;

    officeSettlementCode?: number | null;

}

export interface PaginatedLocalCommunityResponse {

    data: LocalCommunityResponse[];

    count: number;

}
