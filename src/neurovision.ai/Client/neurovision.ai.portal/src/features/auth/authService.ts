import { post } from "../../api/api";

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

export const resend2FARequest = async (
    data: Resend2FADto
): Promise<{ message: string }> => {
    return await post("/Authentication/resend-2fa", data);
};

                                       
export const confirm2FARequest = async (
    data: TwoFADto
): Promise<Confirm2FAResponse> => {
    return await post("/Authentication/confirm-2fa", data);
};


export const loginRequest = async (
    data: LoginDto
): Promise<LoginResponse> => {
    return await post("/Authentication/login", data);
};