export interface OrderDetailRequest {
  productId: number;
  unitPrice: number;
  quantity: number;
  discount: number;
}

export interface CreateOrderRequest {
  customerId: string;
  employeeId: number;
  orderDate: string | null;
  requiredDate: string | null;
  shippedDate: string | null;
  shipVia: number | null;
  freight: number | null;
  shipName: string | null;
  shipAddress: string | null;
  shipCity: string | null;
  shipRegion: string | null;
  shipPostalCode: string | null;
  shipCountry: string | null;
  details: OrderDetailRequest[];
}

export type UpdateOrderRequest = CreateOrderRequest;

export interface OrderDetailResponse {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  discount: number;
  lineTotal: number;
}

export interface OrderResponse {
  orderId: number;
  customerId: string;
  customerName: string | null;
  employeeId: number | null;
  employeeName: string | null;
  orderDate: string | null;
  requiredDate: string | null;
  shippedDate: string | null;
  shipVia: number | null;
  shipperName: string | null;
  freight: number;
  shipName: string | null;
  shipAddress: string | null;
  shipCity: string | null;
  shipRegion: string | null;
  shipPostalCode: string | null;
  shipCountry: string | null;
  orderTotal: number;
  details: OrderDetailResponse[];
}

export interface OrderSummaryResponse {
  orderId: number;
  customerId: string;
  customerName: string | null;
  employeeId: number | null;
  employeeName: string | null;
  orderDate: string | null;
  requiredDate: string | null;
  shippedDate: string | null;
  shipVia: number | null;
  shipperName: string | null;
  freight: number;
  shipCity: string | null;
  shipRegion: string | null;
  shipCountry: string | null;
  detailCount: number;
  orderTotal: number;
}

export interface OrderFormModel {
  orderId?: number;
  customerId: string | null;
  employeeId: number | null;
  orderDate: string | null;
  requiredDate: string | null;
  shippedDate: string | null;
  shipVia: number | null;
  freight: number | null;
  shipName: string | null;
  shipAddress: string | null;
  shipCity: string | null;
  shipRegion: string | null;
  shipPostalCode: string | null;
  shipCountry: string | null;
  details: OrderDetailRequest[];
}
