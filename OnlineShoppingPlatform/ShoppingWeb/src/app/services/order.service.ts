import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Cart, CartItem } from '../models/cart';

@Injectable({
    providedIn: 'root'
})
export class OrderService {
    private baseUrl = `${environment.apiUrl}/api/order`;

    constructor(private http: HttpClient) { }

    place(item: Cart): Observable<OrderResponse> {   
        console.log("here")
        return this.http.post<OrderResponse>(`${this.baseUrl}/place`, item)
    }
}

export interface OrderResponse {
    orderId: number;
    message: string;
}