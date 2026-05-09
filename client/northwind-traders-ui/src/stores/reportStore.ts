import { defineStore } from 'pinia';
import { getApiErrorMessage } from 'src/services/errorHandler';
import { getOrdersReport, normalizeReportResponse } from 'src/services/reportService';
import type { OrdersReportResponse, ReportFilterRequest } from 'src/types/reports';

interface ReportState {
  filters: ReportFilterRequest;
  data: OrdersReportResponse;
  isLoading: boolean;
  errorMessage: string | null;
}

let activeReportRequest = 0;

function createEmptyReport(): OrdersReportResponse {
  return {
    ordersOverTime: [],
    shipmentsByRegion: [],
    orders: [],
  };
}

const defaultFilters: ReportFilterRequest = {
  year: null,
  month: null,
  week: null,
  region: null,
};

const emptyReport: OrdersReportResponse = {
  ordersOverTime: [],
  shipmentsByRegion: [],
  orders: [],
};

export const useReportStore = defineStore('reports', {
  state: (): ReportState => ({
    filters: {
      ...defaultFilters,
    },
    data: { ...emptyReport },
    isLoading: false,
    errorMessage: null,
  }),
  actions: {
    async loadReport(): Promise<void> {
      const requestId = ++activeReportRequest;
      this.isLoading = true;
      this.errorMessage = null;

      try {
        const report = normalizeReportResponse(await getOrdersReport(this.filters));

        if (requestId === activeReportRequest) {
          this.data = report;
        }
      } catch (error) {
        if (requestId === activeReportRequest) {
          this.data = createEmptyReport();
          this.errorMessage = getApiErrorMessage(error);
        }

        throw error;
      } finally {
        if (requestId === activeReportRequest) {
          this.isLoading = false;
        }
      }
    },
    setFilters(filters: ReportFilterRequest): void {
      this.filters = { ...filters };
    },
    reset(): void {
      this.filters = { ...defaultFilters };
      this.data = createEmptyReport();
      this.errorMessage = null;
    },
  },
});
