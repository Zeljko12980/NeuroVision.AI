import { createSelector } from "@reduxjs/toolkit";
import { RootState } from "../store/store";
import { getClaimsFromToken } from "../utils/jwt";

const selectToken = (state: RootState) => state.auth.token;

export const selectUserClaims = createSelector(
    [selectToken],
    (token) => {
        if (!token) return null;
        return getClaimsFromToken(token);
    }
);