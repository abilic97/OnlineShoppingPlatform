import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Cart, CartItem } from '../models/cart';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private baseUrl = `${environment.apiUrl}/api/cart`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<Cart[]> {
        return this.http.get<Cart[]>(this.baseUrl);
    }

    getById(cartId: number): Observable<Cart> {
        return this.http.get<Cart>(`${this.baseUrl}/${cartId}`);
    }

    create(cart: Partial<Cart>): Observable<Cart> {
        return this.http.post<Cart>(this.baseUrl, cart);
    }

    updateStatus(cartId: number, newStatus: string): Observable<void> {
        return this.http.put<void>(`${this.baseUrl} / ${cartId} / status `, newStatus);
    }

    delete(cartId: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl} / ${cartId} `);
    }

    recalcTotals(cartId: number): Observable<Cart> {
        return this.http.post<Cart>(`${this.baseUrl} / ${cartId} / recalculate `, {});
    }

    addItem(cartId: number, item: Partial<CartItem>): Observable<Cart> {
        return this.http.post<Cart>(`${this.baseUrl} / ${cartId} / items `, item);
    }
}
