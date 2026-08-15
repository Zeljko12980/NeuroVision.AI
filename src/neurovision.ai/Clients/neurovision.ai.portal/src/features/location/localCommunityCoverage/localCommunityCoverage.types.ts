/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface LocalCommunityCoverageForm {

    countryCode: string;

    municipalityCode: number;

    localCommunityIdentifier: number;

    settlementCode: number;

}

export interface LocalCommunityCoverageRequest extends LocalCommunityCoverageForm { }

export interface LocalCommunityCoverageKey {

    countryCode: string;
    municipalityCode: number;
    localCommunityIdentifier: number;
    settlementCode: number;

}

export interface CreateLocalCommunityCoverageResponse {

    countryCode: string;

    municipalityCode: number;

    localCommunityIdentifier: number;

    settlementCode: number;

}

export interface LocalCommunityCoverageResponse {

    countryCode: string;

    municipalityCode: number;

    localCommunityIdentifier: number;

    settlementCode: number;

}

export interface PaginatedLocalCommunityCoverageResponse {

    data: LocalCommunityCoverageResponse[];

    count: number;

}
