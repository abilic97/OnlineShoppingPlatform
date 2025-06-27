import { Component, OnInit } from '@angular/core';
import { Cart } from '../models/cart';
import { CartItemLocal, CartService } from '../services/cart.service';
import { UserService } from '../services/users.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;

  constructor(private cartService: CartService,
    private userService: UserService,
    private router: Router) { }

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    if (this.userService.isLoggedIn()) {
      this.cartService.getServerCart().subscribe({
        next: (serverCart) => {
          this.cart = serverCart;

          const local = this.cartService.getLocalCart();
          if (local?.items?.length) {
            for (const item of local.items) {
              this.cartService.addItem(serverCart.cartId, item).subscribe();
            }
            this.cartService.clearLocalCart();
          }
        },
        error: (err) => console.error('Failed to load server cart', err)
      });
    } else {
      this.cart = this.cartService.getLocalCart();
    }
  }

  addItem(productId: number): void {
    const newItem: Partial<CartItemLocal> = { productId, quantity: 1 };

    if (this.userService.isLoggedIn() && this.cart) {
      if (this.cart.cartId == 0) {
        let cartID = localStorage.getItem("server_cart_id");
        if (cartID != undefined)
          this.cart.cartId = + cartID;
      }
      this.cartService.addItem(this.cart.cartId, newItem).subscribe({
        next: (updatedCart) => this.cart = updatedCart,
        error: (err) => console.error('Error adding item to server cart', err)
      });
    } else {
      this.cart = this.cartService.addItemToLocalCart(newItem);
    }
  }

  clearCart(): void {
    let cartId = this.cart?.cartId;
    console.log(this.cart)
    if (this.userService.isLoggedIn() && cartId != null) {
      this.cartService.delete(cartId).subscribe({
        next: (updatedCart) => { this.cart = this.cartService.createEmptyCart(); },
        error: (err) => console.error('Error adding item to server cart', err)
      });
    } else {
      this.clearLocalCart();
    }
  }

  getTotal(): number {
    return this.cart?.total ?? 0;
  }

  removeItem(itemId: number): void {
    if (!this.cart) return;

    let itemToRemove = this.cart.items.find(x => x.cartItemId = itemId);
    if (itemToRemove == undefined) {
      return;
    }
    if (this.userService.isLoggedIn()) {
      console.log(itemToRemove);
      this.cartService.removeItem(this.cart.cartId, itemToRemove).subscribe({
        next: (updatedCart) => this.cart = updatedCart,
        error: (err: any) => console.error('Error removing item from server cart', err)
      });
    } else {
      this.cart.items = this.cart.items.filter(item => item.cartItemId !== itemId);
      this.cartService.saveLocalCart(this.cart);
      this.cartService.updateCartItemCount(this.cart);
    }
  }

  clearLocalCart(): void {
    localStorage.removeItem(this.cartService.localStorageKey);
    this.cart = this.cartService.createEmptyCart();
  }
}
