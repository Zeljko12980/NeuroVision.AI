import { jwtDecode } from "jwt-decode";

export interface JwtPayload {
    sub?: string;
    email?: string;
    role?: string;
    exp?: number;
    [key: string]: any;
}

export const getClaimsFromToken = (token: string): JwtPayload | null => {
    try {
        return jwtDecode<JwtPayload>(token);
    } catch {
        return null;
    }
};

export const isTokenExpired = (token: string): boolean => {
    const decoded = getClaimsFromToken(token);
    if (!decoded?.exp) return true;

    return decoded.exp * 1000 < Date.now();
};