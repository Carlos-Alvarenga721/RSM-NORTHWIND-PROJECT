export interface ReportFilterRequest {
  year: number | null;
  month: number | null;
  week: number | null;
  region: string | null;
}

export interface OrdersOverTimePoint {
  label: string;
  orderCount: number;
  totalSales: number;
}

export interface ShipmentsByRegionPoint {
  region: string;
  shipmentCount: number;
}

export interface ReportOrderRow {
  orderId: number;
  customerName: string | null;
  employeeName: string | null;
  orderDate: string | null;
  shippedDate: string | null;
  shipRegion: string | null;
  shipCountry: string | null;
  orderTotal: number;
}

export interface OrdersReportResponse {
  ordersOverTime: OrdersOverTimePoint[];
  shipmentsByRegion: ShipmentsByRegionPoint[];
  orders: ReportOrderRow[];
}
