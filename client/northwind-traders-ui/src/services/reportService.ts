import { apiClient } from './apiClient';
import type { OrdersReportResponse, ReportFilterRequest } from 'src/types/reports';

export async function getOrdersReport(filters: ReportFilterRequest): Promise<OrdersReportResponse> {
  const response = await apiClient.get<OrdersReportResponse>('/api/reports/orders', {
    params: filters,
  });
  return response.data;
}

export async function exportOrdersToExcel(filters: ReportFilterRequest): Promise<Blob> {
  const response = await apiClient.get('/api/reports/orders/export/excel', {
    params: filters,
    responseType: 'blob',
  });
  return response.data;
}

export async function exportOrdersToPdf(filters: ReportFilterRequest): Promise<Blob> {
  const response = await apiClient.get('/api/reports/orders/export/pdf', {
    params: filters,
    responseType: 'blob',
  });
  return response.data;
}
