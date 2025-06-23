import { Component, OnInit } from '@angular/core';
import { Cart, CartItem } from '../models/cart';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  cartId = 1; // Static for demo — replace with dynamic logic if needed

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.cartService.getById(this.cartId).subscribe({
      next: (data) => {
        this.cart = data;
      },
      error: (err) => console.error('Error loading cart', err)
    });
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
    const newItem: Partial<CartItem> = { productId, quantity: 1 };
    this.cartService.addItem(this.cartId, newItem).subscribe({
      next: (updatedCart) => this.cart = updatedCart,
      error: (err) => console.error('Error adding item', err)
    });
  }

  clearCart(): void {
    // this.cartService.delete(this.cartId).subscribe({
    //   next: () => this.cart = { cartId: this.cartId, items: [], total: 0, userId : "null",  },
    //   error: (err) => console.error('Error clearing cart', err)
    // });
  }

  getTotal(): number {
    return this.cart?.items?.reduce((sum, item) => sum + item.quantity * item.product.price, 0) || 0;
  }

  removeItem(itemId: number): void {
    if (!this.cart) return;
    this.cart.items = this.cart.items.filter(item => item.cartItemId !== itemId);
  }
}
