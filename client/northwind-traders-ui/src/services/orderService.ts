import { apiClient } from './apiClient';
import type {
  CreateOrderRequest,
  OrderResponse,
  OrderSummaryResponse,
  UpdateOrderRequest,
} from 'src/types/orders';

export async function getOrders(): Promise<OrderSummaryResponse[]> {
  const response = await apiClient.get<OrderSummaryResponse[]>('/api/orders');
  return response.data;
}

export async function getOrder(orderId: number): Promise<OrderResponse> {
  const response = await apiClient.get<OrderResponse>(`/api/orders/${orderId}`);
  return response.data;
}

export async function createOrder(request: CreateOrderRequest): Promise<OrderResponse> {
  const response = await apiClient.post<OrderResponse>('/api/orders', request);
  return response.data;
}

export async function updateOrder(orderId: number, request: UpdateOrderRequest): Promise<OrderResponse> {
  const response = await apiClient.put<OrderResponse>(`/api/orders/${orderId}`, request);
  return response.data;
}

export async function deleteOrder(orderId: number): Promise<void> {
  await apiClient.delete(`/api/orders/${orderId}`);
}

export async function downloadOrderPdf(orderId: number): Promise<Blob> {
  const response = await apiClient.get(`/api/orders/${orderId}/report/pdf`, {
    responseType: 'blob',
  });
  return response.data;
}
