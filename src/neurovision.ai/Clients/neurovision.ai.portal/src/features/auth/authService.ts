import { post,get } from "../../api/api";

interface LoginDto {
    email: string;
    password: string;
}

interface LoginResponse {
    email: string;
    message: string;
}

interface TwoFADto {
    email: string;
    code: string; 
}

interface Confirm2FAResponse {
    token: string;   
    message: string; 
} 


interface Resend2FADto {
    email: string;
}

export interface ConfirmEmailDto {
    email: string;
    token: string;
}

export interface ConfirmEmailResponse {
    isConfirmed: boolean;
}

export const resend2FARequest = async (
    data: Resend2FADto
): Promise<{ message: string }> => {
    return await post("/authentication/resend-2fa", data);
};

export const confirmEmailRequest = async (
    email: string,
    token: string
): Promise<ConfirmEmailResponse> => {
    return await get(
        `/user/confirm-email?Email=${encodeURIComponent(email)}&Token=${encodeURIComponent(token)}`
    );
};
                                       
export const confirm2FARequest = async (
    data: TwoFADto
): Promise<Confirm2FAResponse> => {
    return await post("/authentication/confirm-2fa", data);
};
export interface SetPasswordDto {
    email: string;
    token: string;
    password: string;
}
export interface SetPasswordResponse {
    success: boolean;
    message: string;
}

export const setPasswordRequest = async (
    data: SetPasswordDto
): Promise<SetPasswordResponse> => {
    return await post("/authentication/set-password", data);
};

export const loginRequest = async (
    data: LoginDto
): Promise<LoginResponse> => {
    return await post("/authentication/login", data);
};