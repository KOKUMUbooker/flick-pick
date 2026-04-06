import * as signalR from "@microsoft/signalr";
import type { AuthResponseData } from "../api";
import { API_BASE_URL } from "../api/urls";
import { chatListeners, suggestionListeners } from "../hubs/listeners";
import type { AppState } from "../types";

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

let listenersRegistered = false;

export async function startHubConnection() {
    if (!listenersRegistered) {
        suggestionListeners();
        chatListeners();
        listenersRegistered = true;
    }

    if (hubIsDisconnected()) {
        await appState.hubConnection.start();
        // console.log("SignalR connected");
    }
}

export function hubIsConnected() {
    return appState.hubConnection.state === signalR.HubConnectionState.Connected
}

export function hubIsDisconnected() {
    return appState.hubConnection.state === signalR.HubConnectionState.Disconnected
}