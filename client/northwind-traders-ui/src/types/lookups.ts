export interface CustomerLookupResponse {
  customerId: string;
  companyName: string;
  contactName: string | null;
  city: string | null;
  country: string | null;
}

export interface EmployeeLookupResponse {
  employeeId: number;
  fullName: string;
  title: string | null;
  city: string | null;
  country: string | null;
}

export interface ShipperLookupResponse {
  shipperId: number;
  companyName: string;
  phone: string | null;
}

export interface ProductLookupResponse {
  productId: number;
  productName: string;
  unitPrice: number | null;
  unitsInStock: number | null;
  discontinued: boolean;
  categoryId: number | null;
  categoryName: string | null;
}
