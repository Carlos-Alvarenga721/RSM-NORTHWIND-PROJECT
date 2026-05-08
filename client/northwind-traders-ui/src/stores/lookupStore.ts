import { defineStore } from 'pinia';
import {
  getCustomers,
  getEmployees,
  getProducts,
  getShippers,
} from 'src/services/lookupService';
import type {
  CustomerLookupResponse,
  EmployeeLookupResponse,
  ProductLookupResponse,
  ShipperLookupResponse,
} from 'src/types/lookups';

interface LookupState {
  customers: CustomerLookupResponse[];
  employees: EmployeeLookupResponse[];
  shippers: ShipperLookupResponse[];
  products: ProductLookupResponse[];
  isLoading: boolean;
  hasLoaded: boolean;
}

export const useLookupStore = defineStore('lookups', {
  state: (): LookupState => ({
    customers: [],
    employees: [],
    shippers: [],
    products: [],
    isLoading: false,
    hasLoaded: false,
  }),
  actions: {
    async loadLookups(force = false): Promise<void> {
      if (this.hasLoaded && !force) {
        return;
      }

      this.isLoading = true;
      try {
        const [customers, employees, shippers, products] = await Promise.all([
          getCustomers(),
          getEmployees(),
          getShippers(),
          getProducts(),
        ]);

        this.customers = customers;
        this.employees = employees;
        this.shippers = shippers;
        this.products = products;
        this.hasLoaded = true;
      } finally {
        this.isLoading = false;
      }
    },
  },
});
