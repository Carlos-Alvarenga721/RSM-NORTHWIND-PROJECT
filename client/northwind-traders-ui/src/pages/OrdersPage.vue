<template>
  <q-page class="page-shell">
    <div class="page-heading">
      <div>
        <h1 class="page-title">Orders</h1>
        <div class="page-subtitle">Create, review, update, and delete customer orders.</div>
      </div>
      <q-btn color="primary" icon="add" label="Create Order" :to="{ name: 'order-create' }" />
    </div>

    <section class="panel">
      <div class="panel-section">
        <div class="row q-col-gutter-md q-mb-md">
          <div class="col-12 col-md-4">
            <q-input v-model="search" outlined dense debounce="250" label="Search orders">
              <template #prepend>
                <q-icon name="search" />
              </template>
            </q-input>
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="yearFilter"
              outlined
              dense
              clearable
              emit-value
              map-options
              :options="yearOptions"
              label="Year"
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="monthFilter"
              outlined
              dense
              clearable
              emit-value
              map-options
              :options="monthOptions"
              label="Month"
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="shipmentStatusFilter"
              outlined
              dense
              clearable
              emit-value
              map-options
              :options="shipmentStatusOptions"
              label="Shipment status"
            />
          </div>
          <div class="col-12 col-md-2">
            <q-input v-model="countryFilter" outlined dense debounce="250" label="Ship country" />
          </div>
        </div>

        <q-table
          flat
          bordered
          :rows="filteredOrders"
          :columns="columns"
          row-key="orderId"
          :loading="orderStore.isLoading"
          v-model:pagination="pagination"
        >
          <template #body-cell-orderDate="scope">
            <q-td :props="scope">{{ formatDate(scope.row.orderDate) }}</q-td>
          </template>
          <template #body-cell-shipmentStatus="scope">
            <q-td :props="scope">
              <q-badge
                :color="getShipmentStatusColor(scope.row)"
                :label="getShipmentStatusLabel(scope.row)"
              />
            </q-td>
          </template>
          <template #body-cell-orderTotal="scope">
            <q-td :props="scope">{{ formatCurrency(scope.row.orderTotal) }}</q-td>
          </template>
          <template #body-cell-freight="scope">
            <q-td :props="scope">{{ formatCurrency(scope.row.freight) }}</q-td>
          </template>
          <template #body-cell-actions="scope">
            <q-td :props="scope">
              <div class="table-actions">
                <q-btn flat round dense icon="visibility" color="primary" :to="{ name: 'order-detail', params: { orderId: scope.row.orderId } }">
                  <q-tooltip>View order</q-tooltip>
                </q-btn>
                <q-btn flat round dense icon="edit" color="primary" :to="{ name: 'order-edit', params: { orderId: scope.row.orderId } }">
                  <q-tooltip>Edit order</q-tooltip>
                </q-btn>
                <q-btn flat round dense icon="delete" color="negative" @click="confirmDelete(scope.row.orderId)">
                  <q-tooltip>Delete order</q-tooltip>
                </q-btn>
              </div>
            </q-td>
          </template>
        </q-table>
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { Dialog, Loading, Notify } from 'quasar';
import type { QTableColumn } from 'quasar';
import { useOrderStore } from 'src/stores/orderStore';
import { notifyApiError } from 'src/services/errorHandler';
import type { OrderSummaryResponse } from 'src/types/orders';

type ShipmentStatusFilter = 'shipped' | 'not-shipped';

const orderStore = useOrderStore();
const search = ref('');
const countryFilter = ref('');
const shipmentStatusFilter = ref<ShipmentStatusFilter | null>(null);
const yearFilter = ref<number | null>(null);
const monthFilter = ref<number | null>(null);
const pagination = ref({
  page: 1,
  rowsPerPage: 10,
  sortBy: 'orderDate',
  descending: true,
});

const columns: QTableColumn<OrderSummaryResponse>[] = [
  { name: 'orderId', label: 'Order ID', field: 'orderId', sortable: true, align: 'left' },
  { name: 'customerName', label: 'Customer', field: 'customerName', sortable: true, align: 'left' },
  { name: 'employeeName', label: 'Employee', field: 'employeeName', sortable: true, align: 'left' },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', sortable: true, align: 'left' },
  { name: 'shipperName', label: 'Shipper', field: 'shipperName', sortable: true, align: 'left' },
  {
    name: 'shipmentStatus',
    label: 'Status',
    field: (order) => getShipmentStatusLabel(order),
    sortable: true,
    align: 'left',
  },
  { name: 'shipCountry', label: 'Ship Country', field: 'shipCountry', sortable: true, align: 'left' },
  { name: 'freight', label: 'Freight', field: 'freight', sortable: true, align: 'right' },
  { name: 'detailCount', label: 'Items', field: 'detailCount', sortable: true, align: 'right' },
  { name: 'orderTotal', label: 'Total', field: 'orderTotal', sortable: true, align: 'right' },
  { name: 'actions', label: '', field: 'orderId', align: 'right' },
];

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

const shipmentStatusOptions: Array<{ label: string; value: ShipmentStatusFilter }> = [
  { label: 'Shipped', value: 'shipped' },
  { label: 'Not shipped', value: 'not-shipped' },
];

const yearOptions = computed(() =>
  [...new Set(orderStore.orders
    .map((order) => (order.orderDate ? new Date(order.orderDate).getFullYear() : null))
    .filter((year): year is number => year !== null))]
    .sort((left, right) => right - left)
    .map((year) => ({ label: String(year), value: year })),
);

const filteredOrders = computed(() => {
  const term = search.value.toLowerCase();
  const country = countryFilter.value.toLowerCase();

  return orderStore.orders.filter((order) => {
    const orderDate = order.orderDate ? new Date(order.orderDate) : null;
    const isShipped = Boolean(order.shippedDate);
    const text = [
      order.orderId,
      order.customerId,
      order.customerName,
      order.employeeName,
      order.shipperName,
      order.shipCity,
      order.shipRegion,
      order.shipCountry,
      getShipmentStatusLabel(order),
    ]
      .join(' ')
      .toLowerCase();

    return (
      (!term || text.includes(term)) &&
      (!country || (order.shipCountry || '').toLowerCase().includes(country)) &&
      (!shipmentStatusFilter.value ||
        (shipmentStatusFilter.value === 'shipped' ? isShipped : !isShipped)) &&
      (!yearFilter.value || orderDate?.getFullYear() === yearFilter.value) &&
      (!monthFilter.value || (orderDate?.getMonth() ?? -1) + 1 === monthFilter.value)
    );
  });
});

onMounted(async () => {
  Loading.show();
  try {
    await orderStore.loadOrders();
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
  }
});

function confirmDelete(orderId: number): void {
  Dialog.create({
    title: 'Delete Order',
    message: `Delete order ${orderId}?`,
    cancel: true,
    persistent: true,
    ok: {
      label: 'Delete',
      color: 'negative',
    },
  }).onOk(async () => {
    Loading.show();
    try {
      await orderStore.remove(orderId);
      Notify.create({ type: 'positive', message: 'Order deleted.' });
    } catch (error) {
      notifyApiError(error);
    } finally {
      Loading.hide();
    }
  });
}

function formatDate(value: string | null): string {
  return value ? new Date(value).toLocaleDateString() : 'Not set';
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
}

function getShipmentStatusLabel(order: OrderSummaryResponse): string {
  return order.shippedDate ? 'Shipped' : 'Not shipped';
}

function getShipmentStatusColor(order: OrderSummaryResponse): string {
  return order.shippedDate ? 'positive' : 'grey-7';
}
</script>
