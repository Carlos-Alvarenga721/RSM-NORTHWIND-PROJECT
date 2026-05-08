import { apiClient } from './apiClient';
import type {
  CustomerLookupResponse,
  EmployeeLookupResponse,
  ProductLookupResponse,
  ShipperLookupResponse,
} from 'src/types/lookups';

export async function getCustomers(): Promise<CustomerLookupResponse[]> {
  const response = await apiClient.get<CustomerLookupResponse[]>('/api/customers');
  return response.data;
}

export async function getEmployees(): Promise<EmployeeLookupResponse[]> {
  const response = await apiClient.get<EmployeeLookupResponse[]>('/api/employees');
  return response.data;
}

export async function getShippers(): Promise<ShipperLookupResponse[]> {
  const response = await apiClient.get<ShipperLookupResponse[]>('/api/shippers');
  return response.data;
}

export async function getProducts(): Promise<ProductLookupResponse[]> {
  const response = await apiClient.get<ProductLookupResponse[]>('/api/products');
  return response.data;
}
