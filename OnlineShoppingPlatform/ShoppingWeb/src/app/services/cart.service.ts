import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, catchError, forkJoin, map, Observable, of, tap } from 'rxjs';
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
        return this.http.get<Cart>(`${this.baseUrl}/${cartId}`);
    }

    getServerCart(): Observable<Cart> {
        return this.http.get<Cart>(`${this.baseUrl}/user`).pipe(
            tap((cart: Cart) => {
                this.updateCartItemCount(cart);
                this.storeCartId(cart);
            })
        );
    }

    private storeCartId(cart: Cart): void {
        if (cart && cart.cartId) {
            localStorage.setItem('server_cart_id', String(cart.cartId));
        }
    }

    getServerCartCount(): Observable<Cart> {
        return this.http.get<Cart>(`${this.baseUrl}/user`).pipe(
            tap((cart: Cart) => this.updateCartItemCount(cart))
        );
    }

    updateStatus(cartId: number, newStatus: string): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${cartId}/status`, { newStatus });
    }

    delete(cartId: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${cartId}`);
    }

    recalcTotals(cartId: number): Observable<Cart> {
        return this.http.post<Cart>(`${this.baseUrl}/${cartId}/recalculate`, {});
    }

    addItem(cartId: number, item: Partial<CartItem>): Observable<Cart> {
        let cartItemRequest = {
            cartItemId: item.cartItemId,
            cartId: cartId,
            productId: item.productId,
            quantity: item.quantity,
        }
        return this.http.post<Cart>(`${this.baseUrl}/${cartId}/items`, cartItemRequest).pipe(
            tap(cart => this.updateCartItemCount(cart))
        );
    }

    removeItem(cartId: number, item: Partial<CartItem>): Observable<Cart> {
        return this.http.delete<Cart>(`${this.baseUrl}/${cartId}/items/${item.cartItemId}`).pipe(
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
                quantity: item.quantity || 1,
                product: item.product!
            });
        }

        cart.total = this.getLocalCartTotal(cart);
        this.saveLocalCart(cart);
        this.updateCartItemCount(cart); // Add this
        return cart;
    }

    clearLocalCart(): void {
        localStorage.removeItem(this.localStorageKey);
    }

    mergeLocalCartToServer(serverCartId: number): Observable<Cart[]> {
        const local = this.getLocalCart();
        if (!local || !local.items.length) return of([]);

        const mergeRequests = local.items.map(item =>
            this.addItem(serverCartId, {
                productId: item.productId,
                quantity: item.quantity
            })
        );

        this.clearLocalCart();

        return forkJoin(mergeRequests);
    }

    getCartItemCount(): Observable<number> {
        if (this.userService.isLoggedIn()) {
            return this.getServerCart().pipe(
                map(cart => cart.items.reduce((total, item) => total + item.quantity, 0)),
                catchError(() => of(0))
            );
        } else {
            const localCart = this.getLocalCart();
            const count = localCart?.items.reduce((total, item) => total + item.quantity, 0) || 0;
            return of(count);
        }
    }

    createEmptyCart(): CartLocal {
        return {
            cartId: 0,
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

    private getLocalCartTotal(cart: Cart): number {
        return cart.items.reduce((sum, item) => sum + item.quantity * (item.product?.price || 0), 0);
    }
}

export interface CartLocal {
    cartId: number;
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