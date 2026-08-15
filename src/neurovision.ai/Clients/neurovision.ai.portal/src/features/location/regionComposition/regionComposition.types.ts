/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface RegionCompositionForm {

    parentRegionTypeCode: string;

    parentRegionCode: number;

    memberRegionTypeCode: string;

    memberRegionCode: number;

}

export interface RegionCompositionRequest extends RegionCompositionForm { }

export interface RegionCompositionKey {

    parentRegionTypeCode: string;
    parentRegionCode: number;
    memberRegionTypeCode: string;
    memberRegionCode: number;

}

export interface CreateRegionCompositionResponse {

    parentRegionTypeCode: string;

    parentRegionCode: number;

    memberRegionTypeCode: string;

    memberRegionCode: number;

}

export interface RegionCompositionResponse {

    parentRegionTypeCode: string;

    parentRegionCode: number;

    memberRegionTypeCode: string;

    memberRegionCode: number;

}

export interface PaginatedRegionCompositionResponse {

    data: RegionCompositionResponse[];

    count: number;

}
