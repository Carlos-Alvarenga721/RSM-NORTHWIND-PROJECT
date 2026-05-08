<template>
  <q-page class="page-shell">
    <div class="page-heading">
      <div>
        <h1 class="page-title">Reports</h1>
        <div class="page-subtitle">Analyze order activity, shipment regions, and filtered order details.</div>
      </div>
      <div class="row q-gutter-sm">
        <q-btn outline color="primary" icon="grid_on" label="Export Excel" @click="exportExcel" />
        <q-btn color="primary" icon="picture_as_pdf" label="Export PDF" @click="exportPdf" />
      </div>
    </div>

    <section class="panel q-mb-md">
      <div class="panel-section">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-3">
            <q-input v-model.number="filters.year" outlined dense type="number" label="Year" />
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model.number="filters.month" outlined dense type="number" min="1" max="12" label="Month" />
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model.number="filters.week" outlined dense type="number" min="1" max="53" label="Week" />
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model="filters.region" outlined dense label="Region" />
          </div>
          <div class="col-12 filter-actions">
            <q-btn color="primary" icon="filter_alt" label="Apply Filters" @click="loadReport" />
            <q-btn flat icon="restart_alt" label="Reset" @click="resetFilters" />
          </div>
        </div>
      </div>
    </section>

    <div class="metrics-grid q-mb-md">
      <section class="panel">
        <div class="panel-section">
          <div class="metric-label">Orders</div>
          <div class="metric-value">{{ reportStore.data.orders.length }}</div>
        </div>
      </section>
      <section class="panel">
        <div class="panel-section">
          <div class="metric-label">Regions</div>
          <div class="metric-value">{{ reportStore.data.shipmentsByRegion.length }}</div>
        </div>
      </section>
      <section class="panel">
        <div class="panel-section">
          <div class="metric-label">Filtered Total</div>
          <div class="metric-value">{{ formatCurrency(filteredTotal) }}</div>
        </div>
      </section>
    </div>

    <div class="charts-grid q-mb-md">
      <OrdersOverTimeChart :points="reportStore.data.ordersOverTime" />
      <ShipmentsByRegionChart :points="reportStore.data.shipmentsByRegion" />
    </div>

    <section class="panel">
      <div class="panel-section">
        <div class="text-subtitle1 text-weight-bold q-mb-md">Order Details</div>
        <q-table
          flat
          bordered
          :rows="reportStore.data.orders"
          :columns="columns"
          row-key="orderId"
          :loading="reportStore.isLoading"
          v-model:pagination="pagination"
        >
          <template #body-cell-orderDate="scope">
            <q-td :props="scope">{{ formatDate(scope.row.orderDate) }}</q-td>
          </template>
          <template #body-cell-shippedDate="scope">
            <q-td :props="scope">{{ formatDate(scope.row.shippedDate) }}</q-td>
          </template>
          <template #body-cell-orderTotal="scope">
            <q-td :props="scope">{{ formatCurrency(scope.row.orderTotal) }}</q-td>
          </template>
        </q-table>
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { Loading, Notify } from 'quasar';
import type { QTableColumn } from 'quasar';
import OrdersOverTimeChart from 'components/reports/OrdersOverTimeChart.vue';
import ShipmentsByRegionChart from 'components/reports/ShipmentsByRegionChart.vue';
import { exportOrdersToExcel, exportOrdersToPdf } from 'src/services/reportService';
import { notifyApiError } from 'src/services/errorHandler';
import { downloadFile } from 'src/utils/downloadFile';
import { useReportStore } from 'src/stores/reportStore';
import type { ReportFilterRequest, ReportOrderRow } from 'src/types/reports';

const reportStore = useReportStore();
const filters = ref<ReportFilterRequest>({ ...reportStore.filters });
const pagination = ref({
  page: 1,
  rowsPerPage: 10,
});

const columns: QTableColumn<ReportOrderRow>[] = [
  { name: 'orderId', label: 'Order ID', field: 'orderId', sortable: true, align: 'left' },
  { name: 'customerName', label: 'Customer', field: 'customerName', sortable: true, align: 'left' },
  { name: 'employeeName', label: 'Employee', field: 'employeeName', sortable: true, align: 'left' },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', sortable: true, align: 'left' },
  { name: 'shippedDate', label: 'Shipped Date', field: 'shippedDate', sortable: true, align: 'left' },
  { name: 'shipRegion', label: 'Region', field: 'shipRegion', sortable: true, align: 'left' },
  { name: 'shipCountry', label: 'Country', field: 'shipCountry', sortable: true, align: 'left' },
  { name: 'orderTotal', label: 'Total', field: 'orderTotal', sortable: true, align: 'right' },
];

const filteredTotal = computed(() =>
  reportStore.data.orders.reduce((sum, row) => sum + row.orderTotal, 0),
);

onMounted(async () => {
  await loadReport();
});

async function loadReport(): Promise<void> {
  Loading.show();
  try {
    reportStore.setFilters({ ...filters.value });
    await reportStore.loadReport();
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
  }
}

async function exportExcel(): Promise<void> {
  Loading.show();
  try {
    const file = await exportOrdersToExcel(reportStore.filters);
    downloadFile(file, 'northwind-orders-report.xlsx');
    Notify.create({ type: 'positive', message: 'Excel export generated.' });
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
  }
}

async function exportPdf(): Promise<void> {
  Loading.show();
  try {
    const file = await exportOrdersToPdf(reportStore.filters);
    downloadFile(file, 'northwind-orders-report.pdf');
    Notify.create({ type: 'positive', message: 'PDF export generated.' });
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
  }
}

function resetFilters(): void {
  filters.value = {
    year: null,
    month: null,
    week: null,
    region: null,
  };
}

function formatDate(value: string | null): string {
  return value ? new Date(value).toLocaleDateString() : 'Not set';
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
}
</script>

<style scoped>
.filter-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

@media (max-width: 900px) {
  .metrics-grid,
  .charts-grid {
    grid-template-columns: 1fr;
  }
}
</style>
