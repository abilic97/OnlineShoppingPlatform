import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CartService } from '../services/cart.service';
import { Product } from '../models/product';
import { Cart, CartItem } from '../models/cart'
@Component({
    selector: 'app-product-list',
    templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
    products: Product[] = [];
    cartId: number | null = null;

    constructor(
        private productService: ProductService,
        private cartService: CartService
    ) { }

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

    // addToCart(product: Product): void {
    //     const cartItem: Partial<CartItem> = {
    //         productId: product.productId,
    //         quantity: 1,
    //         product: product
    //     };

    //     this.cartService.addItem(this.cartId, cartItem).subscribe({
    //         next: () => alert(`${product.name} added to cart!`),
    //         error: () => alert(`Could not add ${product.name} to cart.`)
    //     });
    // }

    addToCart(product: Product): void {
        const cartItem: CartItem = {
            productId: product.productId,
            quantity: 1,
            cartId: 0,
            cartItemId: 0,
            product: product
        };

        if (this.cartId) {
            // Existing cart — add item
            this.cartService.addItem(this.cartId, cartItem).subscribe({
                next: () => alert(`${product.name} added to cart!`),
                error: (err) => {
                    console.error('Error adding to cart', err);
                    alert(`Failed to add ${product.name} to cart.`);
                }
            });
        } else {
            // No cart yet — create it first, then add item
            const newCart: Partial<Cart> = {
                items: [cartItem],
                total: product.price
            };

            this.cartService.create(newCart).subscribe({
                next: (createdCart) => {
                    this.cartId = createdCart.cartId;
                    alert(`${product.name} added to a new cart!`);
                },
                error: (err) => {
                    console.error('Error creating cart', err);
                    alert('Failed to create cart.');
                }
            });
        }
    }
}