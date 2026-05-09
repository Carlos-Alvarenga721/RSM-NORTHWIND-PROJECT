import { apiClient } from './apiClient';
import type { OrdersReportResponse, ReportFilterRequest } from 'src/types/reports';

export async function getOrdersReport(filters: ReportFilterRequest): Promise<OrdersReportResponse> {
  const response = await apiClient.get<OrdersReportResponse>('/api/reports/orders', {
    params: buildReportParams(filters),
  });
  return normalizeReportResponse(response.data);
}

export async function exportOrdersToExcel(filters: ReportFilterRequest): Promise<Blob> {
  const response = await apiClient.get('/api/reports/orders/export/excel', {
    params: buildReportParams(filters),
    responseType: 'blob',
  });
  return response.data;
}

export async function exportOrdersToPdf(filters: ReportFilterRequest): Promise<Blob> {
  const response = await apiClient.get('/api/reports/orders/export/pdf', {
    params: buildReportParams(filters),
    responseType: 'blob',
  });
  return response.data;
}

function buildReportParams(filters: ReportFilterRequest): Record<string, number | string> {
  const params: Record<string, number | string> = {};

  if (filters.year !== null) {
    params.year = filters.year;
  }

  if (filters.month !== null) {
    params.month = filters.month;
  }

  if (filters.week !== null) {
    params.week = filters.week;
  }

  const region = filters.region?.trim();
  if (region) {
    params.region = region;
  }

  return params;
}

export function normalizeReportResponse(
  report: OrdersReportResponse | null | undefined,
): OrdersReportResponse {
  return {
    ordersOverTime: Array.isArray(report?.ordersOverTime) ? report.ordersOverTime : [],
    shipmentsByRegion: Array.isArray(report?.shipmentsByRegion) ? report.shipmentsByRegion : [],
    orders: Array.isArray(report?.orders) ? report.orders : [],
  };
}
