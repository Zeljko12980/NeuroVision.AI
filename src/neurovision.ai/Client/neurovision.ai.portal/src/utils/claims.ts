import { JwtPayload } from "./jwt";

export const getUserInfoFromClaims = (claims: JwtPayload) => {
    if (!claims) return { name: "", email: "", role: "" };

    return {
        name: claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ,
        email: claims.email || "",
        role: claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "User",
    };
};