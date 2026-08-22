import { JwtPayload } from "./jwt";

const toStringValue = (value: unknown): string => {
    if (!value) return "";

    if (Array.isArray(value)) {
        return value.join(",");
    }

    return String(value);
};

export const getUserInfoFromClaims = (claims: JwtPayload) => {
    if (!claims) {
        return { name: "", email: "", role: "", userId: "" };
    }

    const userId =
        toStringValue(claims.sub) ||
        toStringValue(
            claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
        );

    return {
        name: toStringValue(
            claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
        ),
        email: toStringValue(claims.email) ||
            toStringValue(
                claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]
            ),
        role: toStringValue(
            claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
        ) || toStringValue(claims.role) || "User",
        userId,
    };
};