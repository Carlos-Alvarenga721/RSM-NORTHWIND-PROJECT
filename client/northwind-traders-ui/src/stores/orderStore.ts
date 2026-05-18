import { defineStore } from 'pinia';
import {
  createOrder,
  deleteOrder,
  getOrder,
  getOrders,
  updateOrder,
} from 'src/services/orderService';
import type {
  CreateOrderRequest,
  OrderResponse,
  OrderSummaryResponse,
  UpdateOrderRequest,
} from 'src/types/orders';

interface OrderState {
  orders: OrderSummaryResponse[];
  selectedOrder: OrderResponse | null;
  isLoading: boolean;
}

export const useOrderStore = defineStore('orders', {
  state: (): OrderState => ({
    orders: [],
    selectedOrder: null,
    isLoading: false,
  }),
  actions: {
    // Centralize order API calls so pages stay focused on presentation and navigation.
    async loadOrders(): Promise<void> {
      this.isLoading = true;
      try {
        this.orders = await getOrders();
      } finally {
        this.isLoading = false;
      }
    },
    async loadOrder(orderId: number): Promise<OrderResponse> {
      this.isLoading = true;
      try {
        this.selectedOrder = await getOrder(orderId);
        return this.selectedOrder;
      } finally {
        this.isLoading = false;
      }
    },
    async create(request: CreateOrderRequest): Promise<OrderResponse> {
      const order = await createOrder(request);
      await this.loadOrders();
      return order;
    },
    async update(orderId: number, request: UpdateOrderRequest): Promise<OrderResponse> {
      const order = await updateOrder(orderId, request);
      this.selectedOrder = order;
      await this.loadOrders();
      return order;
    },
    async remove(orderId: number): Promise<void> {
      await deleteOrder(orderId);
      this.orders = this.orders.filter((order) => order.orderId !== orderId);
      if (this.selectedOrder?.orderId === orderId) {
        this.selectedOrder = null;
      }
    },
  },
});
