<template>
  <q-page class="page-shell reports-page">
    <div class="page-heading reports-heading">
      <div>
        <h1 class="page-title">Reports</h1>
        <div class="page-subtitle">Analyze order activity, shipment regions, and filtered order details.</div>
      </div>
      <div class="report-actions">
        <q-btn
          outline
          color="primary"
          icon="grid_on"
          label="Export Excel"
          :loading="isExportingExcel"
          :disable="isBusy"
          @click="exportExcel"
        />
        <q-btn
          color="primary"
          icon="picture_as_pdf"
          label="Export PDF"
          :loading="isExportingPdf"
          :disable="isBusy"
          @click="exportPdf"
        />
      </div>
    </div>

    <q-banner v-if="reportStore.errorMessage" rounded class="bg-red-1 text-negative q-mb-md">
      <template #avatar>
        <q-icon name="error_outline" color="negative" />
      </template>
      {{ reportStore.errorMessage }}
    </q-banner>

    <q-form ref="filterForm" class="panel q-mb-md" @submit.prevent="loadReport">
      <div class="panel-section">
        <div class="filters-header">
          <div>
            <div class="filters-title">Filters</div>
            <div class="filters-subtitle">Use one or more filters to refine the dashboard.</div>
          </div>
          <q-chip v-if="hasActiveFilters" dense color="primary" text-color="white" icon="filter_alt">
            Filtered
          </q-chip>
        </div>

        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3">
            <q-input
              v-model.number="filters.year"
              outlined
              dense
              clearable
              type="number"
              label="Year"
              min="1900"
              max="2100"
              :rules="yearRules"
              lazy-rules
            />
          </div>
          <div class="col-12 col-sm-6 col-lg-3">
            <q-select
              v-model="filters.month"
              outlined
              dense
              clearable
              emit-value
              map-options
              label="Month"
              :options="monthOptions"
            />
          </div>
          <div class="col-12 col-sm-6 col-lg-3">
            <q-input
              v-model.number="filters.week"
              outlined
              dense
              clearable
              type="number"
              label="ISO Week"
              min="1"
              max="53"
              :rules="weekRules"
              lazy-rules
            />
          </div>
          <div class="col-12 col-sm-6 col-lg-3">
            <q-input
              v-model="filters.region"
              outlined
              dense
              clearable
              label="Region"
              maxlength="50"
              :rules="regionRules"
              lazy-rules
            />
          </div>
          <div class="col-12 filter-actions">
            <q-btn
              type="submit"
              color="primary"
              icon="filter_alt"
              label="Apply Filters"
              :loading="reportStore.isLoading"
              :disable="isExporting"
            />
            <q-btn
              flat
              type="button"
              icon="restart_alt"
              label="Reset"
              :disable="isBusy && !reportStore.isLoading"
              @click="resetFilters"
            />
          </div>
        </div>
      </div>
    </q-form>

    <div class="metrics-grid q-mb-md">
      <section class="panel metric-panel">
        <div class="panel-section">
          <div class="metric-label">Orders</div>
          <q-skeleton v-if="reportStore.isLoading" type="text" width="72px" height="34px" />
          <div v-else class="metric-value">{{ reportRows.length }}</div>
        </div>
      </section>
      <section class="panel metric-panel">
        <div class="panel-section">
          <div class="metric-label">Regions</div>
          <q-skeleton v-if="reportStore.isLoading" type="text" width="72px" height="34px" />
          <div v-else class="metric-value">{{ reportStore.data.shipmentsByRegion.length }}</div>
        </div>
      </section>
      <section class="panel metric-panel">
        <div class="panel-section">
          <div class="metric-label">Filtered Total</div>
          <q-skeleton v-if="reportStore.isLoading" type="text" width="132px" height="34px" />
          <div v-else class="metric-value">{{ formatCurrency(filteredTotal) }}</div>
        </div>
      </section>
    </div>

    <div class="charts-grid q-mb-md">
      <OrdersOverTimeChart
        :points="reportStore.data.ordersOverTime"
        :is-loading="reportStore.isLoading"
      />
      <ShipmentsByRegionChart
        :points="reportStore.data.shipmentsByRegion"
        :is-loading="reportStore.isLoading"
      />
    </div>

    <section class="panel">
      <div class="panel-section">
        <div class="table-heading">
          <div>
            <div class="text-subtitle1 text-weight-bold">Order Details</div>
            <div class="table-subtitle">{{ tableSummary }}</div>
          </div>
        </div>
        <q-table
          flat
          bordered
          binary-state-sort
          class="reports-table"
          :rows="reportRows"
          :columns="columns"
          row-key="orderId"
          :loading="reportStore.isLoading"
          :rows-per-page-options="[10, 25, 50, 0]"
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
          <template #no-data>
            <div class="empty-table-state">
              <q-icon name="inventory_2" size="32px" color="grey-6" />
              <div>No orders match the selected filters.</div>
            </div>
          </template>
        </q-table>
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue';
import { Notify } from 'quasar';
import type { QTableColumn } from 'quasar';
import OrdersOverTimeChart from 'components/reports/OrdersOverTimeChart.vue';
import ShipmentsByRegionChart from 'components/reports/ShipmentsByRegionChart.vue';
import { exportOrdersToExcel, exportOrdersToPdf } from 'src/services/reportService';
import { notifyApiError } from 'src/services/errorHandler';
import { downloadFile } from 'src/utils/downloadFile';
import { useReportStore } from 'src/stores/reportStore';
import type { ReportFilterRequest, ReportOrderRow } from 'src/types/reports';

interface ReportFilterForm {
  year: number | null;
  month: number | null;
  week: number | null;
  region: string | null;
}

interface FilterFormRef {
  validate: () => Promise<boolean>;
  resetValidation: () => void;
}

const reportStore = useReportStore();
const filterForm = ref<FilterFormRef | null>(null);
const filters = ref<ReportFilterForm>({ ...reportStore.filters });
const isExportingExcel = ref(false);
const isExportingPdf = ref(false);
const pagination = ref({
  page: 1,
  rowsPerPage: 10,
});

const monthOptions = [
  { label: 'January', value: 1 },
  { label: 'February', value: 2 },
  { label: 'March', value: 3 },
  { label: 'April', value: 4 },
  { label: 'May', value: 5 },
  { label: 'June', value: 6 },
  { label: 'July', value: 7 },
  { label: 'August', value: 8 },
  { label: 'September', value: 9 },
  { label: 'October', value: 10 },
  { label: 'November', value: 11 },
  { label: 'December', value: 12 },
];

const yearRules = [
  (value: number | string | null) =>
    isEmpty(value) || isIntegerInRange(value, 1900, 2100) || 'Enter a year between 1900 and 2100.',
];

const weekRules = [
  (value: number | string | null) =>
    isEmpty(value) || isIntegerInRange(value, 1, 53) || 'Enter an ISO week between 1 and 53.',
];

const regionRules = [
  (value: string | null) =>
    !value || value.trim().length <= 50 || 'Region must be 50 characters or fewer.',
];

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

const reportRows = computed(() => reportStore.data.orders);
const isExporting = computed(() => isExportingExcel.value || isExportingPdf.value);
const isBusy = computed(() => reportStore.isLoading || isExporting.value);

const filteredTotal = computed(() =>
  reportRows.value.reduce((sum, row) => sum + safeNumber(row.orderTotal), 0),
);

const hasActiveFilters = computed(() => {
  const activeFilters = reportStore.filters;
  return Boolean(
    activeFilters.year !== null ||
      activeFilters.month !== null ||
      activeFilters.week !== null ||
      activeFilters.region,
  );
});

const tableSummary = computed(() => {
  if (reportStore.isLoading) {
    return 'Loading filtered order details.';
  }

  const count = reportRows.value.length;
  return count === 1 ? '1 order row found.' : `${count} order rows found.`;
});

onMounted(async () => {
  await loadReport();
});

async function loadReport(): Promise<void> {
  const isValid = await filterForm.value?.validate();
  if (isValid === false) {
    Notify.create({ type: 'warning', message: 'Review the report filters before applying them.' });
    return;
  }

  try {
    reportStore.setFilters(normalizeFilters(filters.value));
    await reportStore.loadReport();
    pagination.value.page = 1;
  } catch (error) {
    notifyApiError(error);
  }
}

async function exportExcel(): Promise<void> {
  isExportingExcel.value = true;
  try {
    const file = await exportOrdersToExcel(reportStore.filters);
    downloadFile(file, 'northwind-orders-report.xlsx');
    Notify.create({ type: 'positive', message: 'Excel export generated.' });
  } catch (error) {
    notifyApiError(error);
  } finally {
    isExportingExcel.value = false;
  }
}

async function exportPdf(): Promise<void> {
  isExportingPdf.value = true;
  try {
    const file = await exportOrdersToPdf(reportStore.filters);
    downloadFile(file, 'northwind-orders-report.pdf');
    Notify.create({ type: 'positive', message: 'PDF export generated.' });
  } catch (error) {
    notifyApiError(error);
  } finally {
    isExportingPdf.value = false;
  }
}

async function resetFilters(): Promise<void> {
  filters.value = {
    year: null,
    month: null,
    week: null,
    region: null,
  };

  await nextTick();
  filterForm.value?.resetValidation();
  await loadReport();
}

function normalizeFilters(formFilters: ReportFilterForm): ReportFilterRequest {
  const region = formFilters.region?.trim();

  return {
    year: toNullableInteger(formFilters.year),
    month: toNullableInteger(formFilters.month),
    week: toNullableInteger(formFilters.week),
    region: region || null,
  };
}

function isEmpty(value: number | string | null | undefined): boolean {
  return value === null || value === undefined || value === '';
}

function isIntegerInRange(value: number | string | null, min: number, max: number): boolean {
  const numberValue = Number(value);
  return Number.isInteger(numberValue) && numberValue >= min && numberValue <= max;
}

function toNullableInteger(value: number | string | null): number | null {
  if (isEmpty(value)) {
    return null;
  }

  const numberValue = Number(value);
  return Number.isFinite(numberValue) ? Math.trunc(numberValue) : null;
}

function safeNumber(value: number): number {
  return Number.isFinite(value) ? value : 0;
}

function formatDate(value: string | null): string {
  if (!value) {
    return 'Not set';
  }

  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? 'Not set' : date.toLocaleDateString();
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(
    safeNumber(value),
  );
}
</script>

<style scoped>
.reports-page {
  overflow-x: hidden;
}

.reports-heading {
  align-items: flex-start;
}

.report-actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-end;
  gap: 8px;
}

.filters-header,
.table-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 14px;
}

.filters-title {
  font-size: 16px;
  font-weight: 700;
}

.filters-subtitle,
.table-subtitle {
  color: var(--nw-muted);
  font-size: 13px;
}

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

.metric-panel {
  min-width: 0;
}

.metric-value {
  word-break: break-word;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
  align-items: stretch;
}

.reports-table {
  max-width: 100%;
}

.empty-table-state {
  display: flex;
  width: 100%;
  min-height: 112px;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: var(--nw-muted);
  text-align: center;
}

@media (max-width: 900px) {
  .reports-heading {
    flex-direction: column;
  }

  .report-actions {
    width: 100%;
    justify-content: flex-start;
  }

  .metrics-grid,
  .charts-grid {
    grid-template-columns: 1fr;
  }

  .filter-actions {
    justify-content: flex-start;
  }
}
</style>
