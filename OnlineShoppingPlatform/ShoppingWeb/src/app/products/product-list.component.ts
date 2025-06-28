import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CartService } from '../services/cart.service';
import { Product } from '../models/product';
import { UserService } from '../services/users.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-product-list',
    templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
    products: Product[] = [];
    cartId: number | null = null;

    constructor(
        private productService: ProductService,
        private cartService: CartService,
        private userService: UserService,
        private router: Router
    ) {
        this.cartService.getCartItemCount();
    }

    ngOnInit(): void {
        this.loadProducts();
    }

    loadProducts(): void {
        this.productService.getAll().subscribe({
            next: (data) => {
                this.products = data;
            },
            error: (err) => {
                console.error('Error loading products', err);
            }
        });
    }

    addToCart(product: Product): void {
        const cartItem = {
            productId: product.productId,
            quantity: 1,
            product
        };

        if (this.userService.isLoggedIn()) {
            const cartId = +localStorage.getItem('server_cart_id')!;

            this.cartService.addItem(cartId, cartItem).subscribe({
                next: updatedCart => {
                    console.log('Item added to server cart', updatedCart);
                },
                error: err => {
                    console.error('Failed to add to server cart', err);
                }
            });
        } else {
            const updatedCart = this.cartService.addItemToLocalCart(cartItem);
            console.log('Item added to local cart', updatedCart);
        }
    }
}