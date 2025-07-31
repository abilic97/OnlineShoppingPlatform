import { Component, OnInit } from '@angular/core';
import { Cart, CartItem } from '../models/cart';
import { CartItemLocal, CartService } from '../services/cart.service';
import { UserService } from '../services/users.service';
import { forkJoin } from 'rxjs';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  shippingCost: number = 25;

  constructor(private cartService: CartService,
    private userService: UserService,
    private orderService: OrderService) { }

  ngOnInit(): void {
    this.loadCart();
  }

  getShippingCost() {
    return this.shippingCost;
  }

  loadCart(): void {
    this.cartService.syncCart().subscribe({
      next: cart => {
        this.cart = cart;
        console.log('Cart loaded or created:', cart);
        this.cartService.updateCartItemCount(cart);
      },
      error: err => {
        console.error('Failed to load cart', err);
      }
    });
  }

  addItem(productId: number): void {
    const newItem: Partial<CartItemLocal> = { productId, quantity: 1 };

    if (this.userService.isLoggedIn() && this.cart) {
      if (this.cart.cartId == '') {
        let cartID = localStorage.getItem("server_cart_id");
        if (cartID != undefined)
          this.cart.cartId = cartID;
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
    if (this.userService.isLoggedIn() && cartId != null) {
      this.cartService.delete(cartId).subscribe({
        next: (updatedCart) => {
          this.cart = this.cartService.createEmptyCart();
          this.cartService.resetCartItemCount()
        },
        error: (err) => console.error('Error adding item to server cart', err)
      });
    } else {
      this.clearLocalCart();
    }
  }

  getTotal(): number {
    return this.cart?.subtotal ?? 0;
  }

  removeItem(itemId: number, productId: number): void {
    if (!this.cart) return;

    const item = this.findCartItem(itemId);
    if (!item) return;

    if (this.userService.isLoggedIn()) {
      this.removeItemFromServer(item);
    } else {
      this.removeItemFromLocal(productId);
    }
  }

  placeOrder() {
    if (!this.cart) return;

    this.orderService.place(this.cart).subscribe({
      next: (res: { message: any; }) => {
        alert(res.message || 'Order placed successfully!');
        this.cartService.delete(this.cart?.cartId ?? '').subscribe();
        console.log('Order success', res);
        this.cartService.removeServerToken();
      },
      error: (err: { error: { Error: any; }; }) => {
        alert(err.error?.Error || 'Failed to place order.');
        console.error('Order error', err);
      }
    });
  }

  clearLocalCart(): void {
    localStorage.removeItem(this.cartService.localStorageKey);
    this.cart = this.cartService.createEmptyCart();
  }

  private findCartItem(itemId: number): CartItem | undefined {
    return this.cart?.items.find(item => item.cartItemId === itemId);
  }

  private removeItemFromServer(item: CartItem): void {
    if (!this.cart) return;

    this.cartService.removeItem(this.cart.cartId, item).subscribe({
      next: updatedCart => this.cart = updatedCart,
      error: err => console.error('Error removing item from server cart', err)
    });
  }

  private removeItemFromLocal(productId: number): void {
    if (!this.cart) return;

    this.cart.items = this.cart.items.filter(item => item.productId !== productId);
    this.cartService.saveLocalCart(this.cart);
    this.cart.subtotal = this.cartService.getLocalCartTotal(this.cart);
    this.cartService.updateCartItemCount(this.cart);
  }
}
