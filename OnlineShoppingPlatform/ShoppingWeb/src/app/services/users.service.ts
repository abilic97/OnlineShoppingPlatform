import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  loginWithGoogle(): Promise<void> {
    return new Promise((resolve, reject) => {
      window.location.href = '/api/user/login/Google';
    });
  }

  storeToken(token: string) {
    localStorage.setItem('auth_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  removeToken() {
    localStorage.removeItem('auth_token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const payload = this.parseJwt(token);
    if (!payload) return false;

    const now = Math.floor(Date.now() / 1000);
    return payload.exp && payload.exp > now;
  }

  private parseJwt(token: string): any | null {
    try {
      const base64Payload = token.split('.')[1];
      const payloadJson = atob(base64Payload);
      return JSON.parse(payloadJson);
    } catch {
      console.log("Error happened while parsing JWT")
    }
  }
}