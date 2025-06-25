import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  loginWithGoogle(): Promise<void> {
    return new Promise((resolve, reject) => {
      // Redirect to Google OAuth endpoint
      window.location.href = '/api/user/login/Google';

      // Handling OAuth success/failure elsewhere (e.g., server-side)
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
}