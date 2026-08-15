/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface MunicipalitySettlementCoverageForm {

    countryCode: string;

    municipalityCode: number;

    settlementCode: number;

}

export interface MunicipalitySettlementCoverageRequest extends MunicipalitySettlementCoverageForm { }

export interface MunicipalitySettlementCoverageKey {

    countryCode: string;
    municipalityCode: number;
    settlementCode: number;

}

export interface CreateMunicipalitySettlementCoverageResponse {

    countryCode: string;

    municipalityCode: number;

    settlementCode: number;

}

export interface MunicipalitySettlementCoverageResponse {

    countryCode: string;

    municipalityCode: number;

    settlementCode: number;

}

export interface PaginatedMunicipalitySettlementCoverageResponse {

    data: MunicipalitySettlementCoverageResponse[];

    count: number;

}
