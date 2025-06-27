import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../services/users.service';
import { CartService } from '../services/cart.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  cartItemCount$: number = 0;
  constructor(private userService: UserService,
    private cartService: CartService,
    private router: Router) {
    this.cartService.getCartItemCount();
    this.cartService.cartItemCount$.subscribe(value => { this.cartItemCount$ = value });
  }

  get loggedIn(): boolean {
    return !!this.userService.getToken();
  }

  toggleLogin() {
    if (this.loggedIn) {
      this.userService.removeToken();
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}