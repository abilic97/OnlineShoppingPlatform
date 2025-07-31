import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, catchError, forkJoin, map, Observable, of, switchMap, tap } from 'rxjs';
import { Cart, CartItem } from '../models/cart';
import { UserService } from './users.service';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private baseUrl = `${environment.apiUrl}/api/cart`;
    public localStorageKey = 'local_cart';
    private cartItemCountSubject = new BehaviorSubject<number>(0);
    cartItemCount$ = this.cartItemCountSubject.asObservable();

    constructor(
        private http: HttpClient,
        private userService: UserService
    ) { }

    getById(cartId: number): Observable<Cart> {
        let encodedCartId = encodeURIComponent(cartId)
        return this.http.get<Cart>(`${this.baseUrl}/${encodedCartId}`);
    }

    getServerCart(): Observable<Cart> {
        return this.http.get<Cart>(`${this.baseUrl}/user`).pipe(
            tap((cart: Cart) => {
                this.updateCartItemCount(cart);
                this.storeCartId(cart);
            })
        );
    }

    removeServerToken() {
        localStorage.removeItem("server_cart_id");
    }

    delete(cartId: string): Observable<void> {
        let encodedCartId = encodeURIComponent(cartId)
        return this.http.delete<void>(`${this.baseUrl}/${encodedCartId}`);
    }

    addItem(cartId: string, item: Partial<CartItem>): Observable<Cart> {
        let cartItemRequest = {
            cartItemId: item.cartItemId,
            cartId: 0,
            productId: item.productId,
            quantity: item.quantity,
            productName: item.product?.name
        }
        let encodedCartId = encodeURIComponent(cartId)
        return this.http.post<Cart>(`${this.baseUrl}/${encodedCartId}/items`, cartItemRequest).pipe(
            tap(cart => this.updateCartItemCount(cart))
        );
    }

    addLocalItemToServer(cartId: string, item: CartItem): Observable<Cart> {
        let cartItemRequest = {
            cartItemId: 0,
            cartId: 0,
            productId: item.productId,
            quantity: item.quantity,
            productName: item.product?.name
        }
        return this.http.post<Cart>(`${this.baseUrl}/${cartId}/items`, cartItemRequest);
    }

    removeItem(cartId: string, item: Partial<CartItem>): Observable<Cart> {
        let encodedCartId = encodeURIComponent(cartId)
        return this.http.delete<Cart>(`${this.baseUrl}/${encodedCartId}/items/${item.cartItemId}`).pipe(
            tap(cart => this.updateCartItemCount(cart))
        );
    }

    getLocalCart(): Cart | null {
        const raw = localStorage.getItem(this.localStorageKey);
        return raw ? JSON.parse(raw) : null;
    }

    saveLocalCart(cart: Cart): void {
        localStorage.setItem(this.localStorageKey, JSON.stringify(cart));
    }

    addItemToLocalCart(item: Partial<CartItemLocal>): CartLocal {
        const cart = this.getLocalCart() || this.createEmptyCart();

        const existing = cart.items.find(i => i.productId === item.productId);
        if (existing) {
            existing.quantity += item.quantity || 1;
        } else {
            cart.items.push({
                cartItemId: Math.random(),
                productId: item.productId!,
                productName: item.product?.name ?? "",
                quantity: item.quantity || 1,
                price: item.product?.price ?? 0,
                product: item.product!
            });
        }

        cart.subtotal = this.getLocalCartTotal(cart);
        this.saveLocalCart(cart);
        this.updateCartItemCount(cart);
        return cart;
    }

    clearLocalCart(): void {
        localStorage.removeItem(this.localStorageKey);
    }

    getCartItemCount(): Observable<number> {
        if (this.userService.isLoggedIn()) {
            return this.getServerCart().pipe(
                map(cart => {
                    this.updateCartItemCount(cart);
                    return cart.items.reduce((total, item) => total + item.quantity, 0);
                }),
                catchError(() => of(0))
            );
        } else {
            const localCart = this.getLocalCart();
            if (localCart != null)
                this.updateCartItemCount(localCart);
            const count = localCart?.items.reduce((total, item) => total + item.quantity, 0) || 0;
            return of(count);
        }
    }

    syncCart(): Observable<Cart> {
        if (!this.userService.isLoggedIn()) {
            const localCart = this.getLocalCart();
            return of(localCart ?? this.createEmptyCart());
        }
        return this.getServerCart().pipe(
            switchMap(serverCart => {
                const local = this.getLocalCart();
                if (!local?.items?.length) return of(serverCart);

                const addRequests = local.items
                    .filter(item => item?.productId)
                    .map(item => this.addLocalItemToServer(serverCart.cartId, item));

                if (addRequests.length === 0) {
                    this.clearLocalCart();
                    return of(serverCart);
                }

                return forkJoin(addRequests).pipe(
                    map(updatedCarts => {
                        const merged = updatedCarts[updatedCarts.length - 1];
                        this.clearLocalCart();
                        return merged;
                    }),
                    catchError(mergeErr => {
                        console.error('Error during cart merge', mergeErr);
                        return of(serverCart);
                    })
                );
            }),
            catchError(err => {
                console.error('Failed to load server cart', err);
                const fallback = this.getLocalCart() ?? this.createEmptyCart();
                return of(fallback);
            })
        );
    }

    createEmptyCart(): CartLocal {
        return {
            cartId: '',
            userId: '',
            cartNumber: '',
            status: 'Open',
            subtotal: 0,
            shippingCost: 0,
            total: 0,
            expiresAt: undefined,
            notes: undefined,
            items: []
        };
    }

    updateCartItemCount(cart: Cart) {
        const count = cart.items.reduce((sum, item) => sum + item.quantity, 0);
        this.cartItemCountSubject.next(count);
    }

    resetCartItemCount() {
        this.cartItemCountSubject.next(0);
    }

    getLocalCartTotal(cart: Cart): number {
        return cart.items.reduce((sum, item) => sum + item.quantity * (item.product?.price || 0), 0);
    }

    private storeCartId(cart: Cart): void {
        if (cart && cart.cartId) {
            localStorage.setItem('server_cart_id', String(cart.cartId));
        }
    }
}

export interface CartLocal {
    cartId: string;
    userId: string;
    cartNumber: string;
    status: string;
    subtotal: number;
    shippingCost: number;
    total: number;
    expiresAt?: string;
    notes?: string;
    items: CartItem[];
}

export interface CartItemLocal {
    cartItemId: number;
    productId: number;
    quantity: number;
    product: {
        productId: number;
        name: string;
        price: number;
    };
}