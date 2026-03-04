import { HubConnection } from "@microsoft/signalr"

export interface User {
  id: string;
  email: string;
  role: string;
  fullName: string;
}

export interface IAuthState {
  user?: User;
  accessToken?: string;
  refreshToken?: string;
  accessTokenExpiresAt?: number;
}

export interface AppState {
  user?: User;
  hubConnection?: HubConnection
}