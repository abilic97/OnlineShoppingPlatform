import { Component, OnInit } from '@angular/core';
import { Cart, CartItem } from '../models/cart';
import { CartItemLocal, CartService } from '../services/cart.service';
import { UserService } from '../services/users.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  cartId = 1; // Static for demo — replace with dynamic logic if needed

  constructor(private cartService: CartService, private userService: UserService) {}

  ngOnInit(): void {
    this.loadCart();
  }

 loadCart(): void {
    if (this.userService.isLoggedIn()) {
      this.cartService.getServerCart().subscribe({
        next: (serverCart) => {
          this.cart = serverCart;

          // Merge any local cart (optional)
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

  recalc(): void {
    if (!this.cart) return;
    this.cartService.recalcTotals(this.cartId).subscribe({
      next: (updatedCart) => {
        this.cart = updatedCart;
      },
      error: (err) => console.error('Error recalculating', err)
    });
  }

addItem(productId: number): void {
    const newItem: Partial<CartItemLocal> = { productId, quantity: 1 };

    if (this.userService.isLoggedIn() && this.cart) {
      this.cartService.addItem(this.cart.cartId, newItem).subscribe({
        next: (updatedCart) => this.cart = updatedCart,
        error: (err) => console.error('Error adding item to server cart', err)
      });
    } else {
      this.cart = this.cartService.addItemToLocalCart(newItem);
    }
  }

  clearCart(): void {
    // this.cartService.delete(this.cartId).subscribe({
    //   next: () => this.cart = { cartId: this.cartId, items: [], total: 0, userId : "null",  },
    //   error: (err) => console.error('Error clearing cart', err)
    // });
  }

  getTotal(): number {
    return 100;
  }

  removeItem(itemId: number): void {
    if (!this.cart) return;
    this.cart.items = this.cart.items.filter(item => item.cartItemId !== itemId);
  }
}
