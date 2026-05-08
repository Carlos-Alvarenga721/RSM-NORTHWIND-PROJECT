import { defineStore } from 'pinia';
import { getOrdersReport } from 'src/services/reportService';
import type { OrdersReportResponse, ReportFilterRequest } from 'src/types/reports';

interface ReportState {
  filters: ReportFilterRequest;
  data: OrdersReportResponse;
  isLoading: boolean;
}

const emptyReport: OrdersReportResponse = {
  ordersOverTime: [],
  shipmentsByRegion: [],
  orders: [],
};

export const useReportStore = defineStore('reports', {
  state: (): ReportState => ({
    filters: {
      year: null,
      month: null,
      week: null,
      region: null,
    },
    data: emptyReport,
    isLoading: false,
  }),
  actions: {
    async loadReport(): Promise<void> {
      this.isLoading = true;
      try {
        this.data = await getOrdersReport(this.filters);
      } finally {
        this.isLoading = false;
      }
    },
    setFilters(filters: ReportFilterRequest): void {
      this.filters = filters;
    },
  },
});
