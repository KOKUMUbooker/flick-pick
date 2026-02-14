import type { IAuthState, IAuthData } from "../types";

export const authState = $state<IAuthState>({
    accessToken : undefined,
    user : undefined
});

export function logIn(authData: IAuthData) {
    const {accessToken,user} = authData;
    authState.accessToken = accessToken;
    authState.user = user
}

export function logOut() {
    authState.accessToken = undefined;
    authState.user = undefined   
}