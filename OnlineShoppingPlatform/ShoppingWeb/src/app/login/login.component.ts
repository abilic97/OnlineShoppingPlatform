import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../services/users.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {

    constructor(private userService: UserService, private router: Router) { }

    onGoogleLogin() {
        this.userService.loginWithGoogle().then(() => {
            this.router.navigate(['/products']);
        }).catch((error) => {
            console.error('OAuth login error:', error);
        });
    }
}