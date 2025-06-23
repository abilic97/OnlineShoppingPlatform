import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Product } from '../models/product';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private baseUrl = `${environment.apiUrl}/api/product`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<Product[]> {
        return this.http.get<Product[]>(this.baseUrl);
    }

    getById(productId: number): Observable<Product> {
        return this.http.get<Product>(`${this.baseUrl} / ${productId} `);
    }

    create(product: Product): Observable<Product> {
        return this.http.post<Product>(this.baseUrl, product);
    }

    update(product: Product): Observable<void> {
        return this.http.put<void>(`${this.baseUrl} / ${product.productId} `, product);
    }

    delete(productId: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl} / ${productId} `);
    }
}