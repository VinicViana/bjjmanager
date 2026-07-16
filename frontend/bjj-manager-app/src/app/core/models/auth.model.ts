export interface RegisterResponse {
  id: string;
  name: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  userName: string;
}
