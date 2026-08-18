import { get, post, put } from "../../api/api";

export interface AdminUserDto {
    id: string;
    userName: string;
    email: string;
    phoneNumber?: string | null;
    emailConfirmed: boolean;
    roles: string[];
    isLockedOut: boolean;
    lockoutEnd?: string | null;
}

export interface PagedUsersResponse {
    pageIndex: number;
    pageSize: number;
    count: number;
    data: AdminUserDto[];
}

export const getUsersRequest = async (
    pageIndex: number,
    pageSize: number,
    search?: string
): Promise<PagedUsersResponse> => {
    const params = new URLSearchParams({
        PageIndex: String(pageIndex),
        PageSize: String(pageSize),
    });
    if (search?.trim()) params.set("Search", search.trim());
    return await get(`/user?${params.toString()}`);
};

export const createAdministratorRequest = async (data: {
    userName: string;
    email: string;
}): Promise<AdminUserDto> => {
    return await post("/user", {
        userName: data.userName,
        email: data.email,
        roles: ["Administrator"],
    });
};

export const unlockUserRequest = async (userId: string): Promise<void> => {
    await post(`/user/${userId}/unlock`, {});
};

export const lockUserRequest = async (userId: string): Promise<void> => {
    await post(`/user/${userId}/lock`, {});
};

export const updateUserRolesRequest = async (data: {
    userId: string;
    roles: string[];
}): Promise<void> => {
    await put("/role/update-user-roles", data);
};
