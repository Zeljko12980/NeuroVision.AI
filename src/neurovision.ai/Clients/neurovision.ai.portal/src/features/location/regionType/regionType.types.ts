/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface RegionTypeForm {

    code: string;

    name: string;

}

export interface RegionTypeRequest extends RegionTypeForm { }

export interface RegionTypeKey {

    code: string;

}

export interface CreateRegionTypeResponse {

    code: string;

    name: string;

}

export interface RegionTypeResponse {

    code: string;

    name: string;

}

export interface PaginatedRegionTypeResponse {

    data: RegionTypeResponse[];

    count: number;

}
