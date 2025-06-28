import { Component, OnInit } from '@angular/core';
import { Cart } from '../models/cart';
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
    if (!this.userService.isLoggedIn()) {
      this.cart = this.cartService.getLocalCart();
      return;
    }
    this.cartService.getServerCart().subscribe({
      next: (serverCart) => {
        this.cart = serverCart;

        const local = this.cartService.getLocalCart();

        if (!local?.items?.length) return;

        const addRequests = local.items
          .filter(item => item && item.productId)
          .map(item => this.cartService.addLocalItemToServer(serverCart.cartId, item));

        if (addRequests.length === 0) {
          this.cartService.clearLocalCart();
          return;
        }
        forkJoin(addRequests).subscribe({
          next: (updatedCarts) => {
            this.cart = updatedCarts[updatedCarts.length - 1];
            this.cartService.clearLocalCart();
          },
          error: (mergeErr) => {
            console.error('Error during cart merge', mergeErr);
          }
        });
      },
      error: (err) => {
        console.error('Failed to load server cart', err);
      }
    });
    this.cartService.getCartItemCount();
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
    return this.cart?.subtotal ?? 0;
  }

  removeItem(itemId: number, productId: number): void {
    if (!this.cart) return;

    let itemToRemove = this.cart.items.find(x => x.cartItemId == itemId);
    if (itemToRemove == undefined) {
      return;
    }
    if (this.userService.isLoggedIn()) {
      this.cartService.removeItem(this.cart.cartId, itemToRemove).subscribe({
        next: (updatedCart) => this.cart = updatedCart,
        error: (err: any) => console.error('Error removing item from server cart', err)
      });
    } else {
      this.cart.items = this.cart.items.filter(item => item.productId !== productId);
      this.cartService.saveLocalCart(this.cart);
      this.cart.subtotal = this.cartService.getLocalCartTotal(this.cart);
      this.cartService.updateCartItemCount(this.cart);
    }
  }

  placeOrder() {
    if (!this.cart) return;

    this.orderService.place(this.cart).subscribe({
      next: (res: { message: any; }) => {
        alert(res.message || 'Order placed successfully!');
        this.cartService.delete(this.cart?.cartId ?? 0).subscribe();
        console.log('Order success', res);
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
}
