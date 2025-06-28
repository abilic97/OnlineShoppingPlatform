import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../services/users.service';

@Component({
    selector: 'app-auth-callback',
    templateUrl: './auth-redirect.component.html'
})
export class AuthCallbackComponent implements OnInit {

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService
    ) { }

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            const token = params['token'];
            if (token) {
                this.userService.storeToken(token);
                  this.router.navigate(['/products']);            
            } else {
                this.router.navigate(['/login']);
            }
        });
    }
}
