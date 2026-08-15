import { get, del, post, put } from "../../api/api";

export interface RoleDto {
    id: string;
    name: string;
    description: string;
    userCount?: number;
    status?: "Active" | "Inactive";
}

export interface PagedRolesResponse {
    pageIndex: number;
    pageSize: number;
    count: number;
    data: RoleDto[];
}


export const getRolesRequest = async (
    pageIndex: number,
    pageSize: number
): Promise<PagedRolesResponse> => {
    return await get(
        `/role?PageIndex=${pageIndex}&PageSize=${pageSize}`
    );
};

export const getRoleByIdRequest = async (
    id: string
): Promise<RoleDto> => {
    return await get(`/role/${id}`);
};

export const createRoleRequest = async (data: {
    roleName: string;
    description?: string;
}): Promise<RoleDto> => {
    return await post(`/role`, data);
};

export const updateRoleRequest = async (data: {
    id: string;
    roleName: string;
    description?: string;
}): Promise<RoleDto> => {
    return await put(`/role/${data.id}`, {
        roleName: data.roleName,
        description: data.description,
    });
};

export const deleteRoleRequest = async (id: string): Promise<void> => {
    return await del(`/role/${id}`);
};

export const getUserRolesRequest = async (
    userId: string
): Promise<RoleDto[]> => {
    return await get(`/role/user/${userId}`);
};

export const assignRolesRequest = async (data: {
    userId: string;
    roles: string[];
}): Promise<void> => {
    return await post(`/role/assign`, data);
};


export const updateUserRolesRequest = async (data: {
    userId: string;
    roles: string[];
}): Promise<void> => {
    return await put(`/role/update-user-roles`, data);
};