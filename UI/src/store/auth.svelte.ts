import { SvelteDate } from "svelte/reactivity";
import type { AuthResponseData } from "../api";
import type { IAuthState } from "../types";

export const authState = $state<IAuthState>({
    user: undefined,
    accessToken: undefined,
    accessTokenExpiresAt: undefined,
    refreshToken: undefined
});

export function logIn(authData: AuthResponseData) {
    const { userDetails, accessToken, accessTokenExpiresAt, refreshToken } = authData;
    const tknExpiration = new SvelteDate(accessTokenExpiresAt).getTime()

    authState.user = userDetails;
    authState.accessToken = accessToken;
    authState.refreshToken = refreshToken;
    authState.accessTokenExpiresAt = tknExpiration;

    // Set values in localStorage
    localStorage.setItem(ACCESS_TOKEN_STORE_NAME, accessToken)
    localStorage.setItem(REFRESH_TOKEN_STORE_NAME, refreshToken)
    localStorage.setItem(ACCESS_TOKEN_EXP_STORE_NAME, tknExpiration.toString())
}

export async function logOut() {
    authState.user = undefined
    authState.refreshToken = undefined
    authState.accessToken = undefined
    authState.accessTokenExpiresAt = undefined

    localStorage.removeItem(ACCESS_TOKEN_STORE_NAME)
    localStorage.removeItem(ACCESS_TOKEN_EXP_STORE_NAME)
    localStorage.removeItem(REFRESH_TOKEN_STORE_NAME)
}

export function isLoggedIn() {
    return authState.user != undefined
}

export const ACCESS_TOKEN_STORE_NAME = "Atkn"
export const REFRESH_TOKEN_STORE_NAME = "Rtkn"
export const ACCESS_TOKEN_EXP_STORE_NAME = "ATknExp"
