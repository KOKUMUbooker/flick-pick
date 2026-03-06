import * as signalR from "@microsoft/signalr";
import type { AuthResponseData } from "../api";
import type { AppState } from "../types";
import { API_BASE_URL } from "../api/urls";

export const appState = $state<AppState>({
    user: undefined,
    hubConnection: new signalR.HubConnectionBuilder()
        .withUrl(API_BASE_URL + "/api/movieNightHub", { withCredentials: true })
        .withAutomaticReconnect()
        .build()
});

export function logIn(authData: AuthResponseData) {
    const { userDetails } = authData;
    appState.user = userDetails;
}

export async function logOut() {
    appState.user = undefined
}

export function isLoggedIn() {
    return appState.user != undefined
}

export function getAppUser() {
    return appState.user
}

export function hubIsConnected() {
    return appState.hubConnection.state === signalR.HubConnectionState.Connected
}

export function hubIsDisconnected() {
    return appState.hubConnection.state === signalR.HubConnectionState.Disconnected
}