<template>
  <q-page class="page-shell">
    <div class="page-heading">
      <div>
        <h1 class="page-title">Order {{ order?.orderId }}</h1>
        <div class="page-subtitle">{{ order?.customerName || 'Customer not available' }}</div>
      </div>
      <div class="row q-gutter-sm">
        <q-btn outline color="primary" icon="edit" label="Edit" :to="{ name: 'order-edit', params: { orderId } }" />
        <q-btn color="primary" icon="picture_as_pdf" label="Generate PDF" @click="generatePdf" />
      </div>
    </div>

    <div v-if="order" class="detail-grid">
      <section class="panel">
        <div class="panel-section">
          <div class="text-subtitle1 text-weight-bold q-mb-md">Order Information</div>
          <q-list dense>
            <q-item>
              <q-item-section>
                <q-item-label caption>Order ID</q-item-label>
                <q-item-label>{{ order.orderId }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Customer</q-item-label>
                <q-item-label>{{ order.customerName || order.customerId }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Employee</q-item-label>
                <q-item-label>{{ order.employeeName || 'Not assigned' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Shipper</q-item-label>
                <q-item-label>{{ order.shipperName || 'Not assigned' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Order Date</q-item-label>
                <q-item-label>{{ formatDate(order.orderDate) }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Ship Country</q-item-label>
                <q-item-label>{{ order.shipCountry || 'Not set' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Freight</q-item-label>
                <q-item-label>{{ formatCurrency(order.freight) }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </section>

      <section class="panel">
        <div class="panel-section">
          <div class="text-subtitle1 text-weight-bold q-mb-md">Shipping Address</div>
          <div>{{ order.shipName || order.customerName }}</div>
          <div>{{ order.shipAddress }}</div>
          <div>{{ addressLine }}</div>
          <div v-if="validatedAddress" class="validated-address-inline q-mt-md">
            <q-list dense>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Validated Address</q-item-label>
                  <q-item-label>{{ validatedAddress.formattedAddress || fullAddress }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Coordinates</q-item-label>
                  <q-item-label>{{ coordinatesLabel }}</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
            <GoogleMapPreview
              :latitude="validatedAddress.latitude"
              :longitude="validatedAddress.longitude"
              :address="validatedAddress.formattedAddress || fullAddress"
            />
          </div>
          <GoogleMapPreview
            v-else
            class="q-mt-md"
            :latitude="null"
            :longitude="null"
            :address="fullAddress"
          />
        </div>
      </section>

      <section class="panel detail-wide">
        <div class="panel-section">
          <div class="text-subtitle1 text-weight-bold q-mb-md">Line Items</div>
          <q-table
            flat
            bordered
            :rows="order.details"
            :columns="columns"
            row-key="productId"
            hide-pagination
            :pagination="{ rowsPerPage: 0 }"
          >
            <template #body-cell-unitPrice="scope">
              <q-td :props="scope">{{ formatCurrency(scope.row.unitPrice) }}</q-td>
            </template>
            <template #body-cell-lineTotal="scope">
              <q-td :props="scope">{{ formatCurrency(scope.row.lineTotal) }}</q-td>
            </template>
          </q-table>
          <div class="detail-total q-mt-md">
            <span>Freight: {{ formatCurrency(order.freight) }}</span>
            <strong>Total: {{ formatCurrency(order.orderTotal) }}</strong>
          </div>
        </div>
      </section>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { Loading, Notify } from 'quasar';
import type { QTableColumn } from 'quasar';
import GoogleMapPreview from 'components/maps/GoogleMapPreview.vue';
import { downloadOrderPdf } from 'src/services/orderService';
import { notifyApiError } from 'src/services/errorHandler';
import { downloadFile } from 'src/utils/downloadFile';
import { useOrderStore } from 'src/stores/orderStore';
import type { OrderDetailResponse } from 'src/types/orders';

const route = useRoute();
const router = useRouter();
const orderStore = useOrderStore();
const orderId = Number(route.params.orderId);

const order = computed(() => orderStore.selectedOrder);
const validatedAddress = computed(() => order.value?.shippingValidation ?? null);
const addressLine = computed(() =>
  [order.value?.shipCity, order.value?.shipRegion, order.value?.shipPostalCode, order.value?.shipCountry]
    .filter(Boolean)
    .join(', '),
);
const fullAddress = computed(() =>
  [order.value?.shipAddress, addressLine.value].filter(Boolean).join(', '),
);
const coordinatesLabel = computed(() => {
  const latitude = validatedAddress.value?.latitude;
  const longitude = validatedAddress.value?.longitude;

  if (latitude === null || latitude === undefined || longitude === null || longitude === undefined) {
    return 'Coordinates unavailable';
  }

  return `${latitude.toFixed(6)}, ${longitude.toFixed(6)}`;
});

const columns: QTableColumn<OrderDetailResponse>[] = [
  { name: 'productName', label: 'Product', field: 'productName', align: 'left' },
  { name: 'unitPrice', label: 'Unit Price', field: 'unitPrice', align: 'right' },
  { name: 'quantity', label: 'Quantity', field: 'quantity', align: 'right' },
  { name: 'discount', label: 'Discount', field: 'discount', align: 'right' },
  { name: 'lineTotal', label: 'Line Total', field: 'lineTotal', align: 'right' },
];

onMounted(async () => {
  Loading.show();
  try {
    await orderStore.loadOrder(orderId);
  } catch (error) {
    notifyApiError(error);
    await router.push({ name: 'orders' });
    return;
  } finally {
    Loading.hide();
  }

});

async function generatePdf(): Promise<void> {
  Loading.show();
  try {
    const pdf = await downloadOrderPdf(orderId);
    downloadFile(pdf, `northwind-order-${orderId}.pdf`);
    Notify.create({ type: 'positive', message: 'PDF report generated.' });
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
  }
}

function formatDate(value: string | null): string {
  return value ? new Date(value).toLocaleDateString() : 'Not set';
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
}
</script>

<style scoped>
.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
  gap: 16px;
}

.detail-wide {
  grid-column: 1 / -1;
}

.detail-total {
  display: flex;
  justify-content: flex-end;
  gap: 24px;
}

.validated-address-inline {
  display: grid;
  gap: 12px;
}

@media (max-width: 900px) {
  .detail-grid {
    grid-template-columns: 1fr;
  }
}
</style>
