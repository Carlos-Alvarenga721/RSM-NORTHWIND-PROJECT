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
          <div class="col-12 col-md-6">
            <q-input v-model="search" outlined dense debounce="250" label="Search orders">
              <template #prepend>
                <q-icon name="search" />
              </template>
            </q-input>
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model="countryFilter" outlined dense debounce="250" label="Ship country" />
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model="cityFilter" outlined dense debounce="250" label="Ship city" />
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
          <template #body-cell-orderTotal="scope">
            <q-td :props="scope">{{ formatCurrency(scope.row.orderTotal) }}</q-td>
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

const orderStore = useOrderStore();
const search = ref('');
const countryFilter = ref('');
const cityFilter = ref('');
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
  { name: 'shipCity', label: 'Ship City', field: 'shipCity', sortable: true, align: 'left' },
  { name: 'shipCountry', label: 'Ship Country', field: 'shipCountry', sortable: true, align: 'left' },
  { name: 'detailCount', label: 'Items', field: 'detailCount', sortable: true, align: 'right' },
  { name: 'orderTotal', label: 'Total', field: 'orderTotal', sortable: true, align: 'right' },
  { name: 'actions', label: '', field: 'orderId', align: 'right' },
];

const filteredOrders = computed(() => {
  const term = search.value.toLowerCase();
  const country = countryFilter.value.toLowerCase();
  const city = cityFilter.value.toLowerCase();

  return orderStore.orders.filter((order) => {
    const text = [
      order.orderId,
      order.customerId,
      order.customerName,
      order.employeeName,
      order.shipperName,
      order.shipCity,
      order.shipCountry,
    ]
      .join(' ')
      .toLowerCase();

    return (
      (!term || text.includes(term)) &&
      (!country || (order.shipCountry || '').toLowerCase().includes(country)) &&
      (!city || (order.shipCity || '').toLowerCase().includes(city))
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
</script>
