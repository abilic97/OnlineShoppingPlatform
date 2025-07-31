import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../services/users.service';
import { CartService } from '../services/cart.service';

@Component({
    selector: 'app-auth-callback',
    templateUrl: './auth-redirect.component.html'
})
export class AuthCallbackComponent implements OnInit {

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly cartService: CartService,
        private readonly userService: UserService
    ) { }

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            const token = params['token'];
            if (token) {
                this.userService.storeToken(token);

                this.cartService.syncCart().subscribe({
                    next: cart => {
                        console.log('Cart loaded or created:', cart);
                        this.cartService.updateCartItemCount(cart);

                        this.router.navigate(['/products']);
                    },
                    error: err => {
                        console.error('Failed to load cart', err);
                        this.router.navigate(['/products']);
                    }
                });

            } else {
                this.router.navigate(['/login']);
            }
        });
    }
}
