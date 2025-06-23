export interface Product {
    productId: number;
    name: string;
    description?: string;
    price: number;
    stockQuantity: number;
    isInStock: boolean;
}