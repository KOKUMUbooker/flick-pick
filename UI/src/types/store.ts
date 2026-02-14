export interface User {
    id : string;
    email : string;
    role : string;
}

export interface IAuthData {
  accessToken :string,
  user: User
}

export interface IAuthState {
  accessToken? :string,
  user?: User
}