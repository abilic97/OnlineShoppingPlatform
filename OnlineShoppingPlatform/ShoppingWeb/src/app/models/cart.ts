import { Product } from "./product";

export interface CartItem {
    cartItemId: number;
    cartId: number;
    productId: number;
    quantity: number;
    product: Product;
}

export interface Cart {
    cartId: number;
    userId: string;
    status: string;
    subtotal: number;
    shippingCost: number;
    total: number;
    items: CartItem[];
}