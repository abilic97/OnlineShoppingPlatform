export interface CartItem {
  cartItemId: number;
  productId: number;
  quantity: number;
  productName: string;
  price: number;
  product?: {
    productId: number;
    name: string;
    price: number;
  };
}

export interface Cart {
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
