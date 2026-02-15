export interface User {
  id: string;
  email: string;
  role: string;
  fullName: string;
}

export interface IAuthData {
  user: User
}

export interface IAuthState {
  user?: User
}