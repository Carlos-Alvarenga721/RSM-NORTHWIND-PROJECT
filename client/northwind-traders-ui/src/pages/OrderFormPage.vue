<template>
  <q-page class="page-shell">
    <div class="page-heading">
      <div>
        <h1 class="page-title">{{ isEditMode ? 'Edit Order' : 'Create Order' }}</h1>
        <div class="page-subtitle">Manage customer, shipment, and product line details.</div>
      </div>
    </div>

    <OrderForm
      :model-value="form"
      :is-submitting="isSubmitting"
      @submit="saveOrder"
      @cancel="router.push({ name: 'orders' })"
    />
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { Loading, Notify } from 'quasar';
import OrderForm from 'components/orders/OrderForm.vue';
import { useOrderStore } from 'src/stores/orderStore';
import { notifyApiError } from 'src/services/errorHandler';
import type { CreateOrderRequest, OrderFormModel, OrderResponse } from 'src/types/orders';

const route = useRoute();
const router = useRouter();
const orderStore = useOrderStore();
const isSubmitting = ref(false);

const form = ref<OrderFormModel>({
  customerId: null,
  employeeId: null,
  orderDate: toDateInputValue(new Date()),
  requiredDate: null,
  shippedDate: null,
  shipVia: null,
  freight: 0,
  shipName: null,
  shipAddress: null,
  shipCity: null,
  shipRegion: null,
  shipPostalCode: null,
  shipCountry: null,
  shippingValidation: null,
  details: [],
});

const orderId = computed(() => Number(route.params.orderId));
const isEditMode = computed(() => Number.isFinite(orderId.value) && orderId.value > 0);

onMounted(async () => {
  if (!isEditMode.value) {
    return;
  }

  Loading.show();
  try {
    const order = await orderStore.loadOrder(orderId.value);
    form.value = mapOrderToForm(order);
  } catch (error) {
    notifyApiError(error);
    await router.push({ name: 'orders' });
  } finally {
    Loading.hide();
  }
});

async function saveOrder(request: CreateOrderRequest): Promise<void> {
  isSubmitting.value = true;
  Loading.show();
  try {
    const order = isEditMode.value
      ? await orderStore.update(orderId.value, request)
      : await orderStore.create(request);

    Notify.create({
      type: 'positive',
      message: isEditMode.value ? 'Order updated.' : 'Order created.',
    });
    await router.push({ name: 'order-detail', params: { orderId: order.orderId } });
  } catch (error) {
    notifyApiError(error);
  } finally {
    Loading.hide();
    isSubmitting.value = false;
  }
}

function mapOrderToForm(order: OrderResponse): OrderFormModel {
  return {
    orderId: order.orderId,
    customerId: order.customerId,
    employeeId: order.employeeId,
    orderDate: toDateInputValue(order.orderDate),
    requiredDate: toDateInputValue(order.requiredDate),
    shippedDate: toDateInputValue(order.shippedDate),
    shipVia: order.shipVia,
    freight: order.freight,
    shipName: order.shipName,
    shipAddress: order.shipAddress,
    shipCity: order.shipCity,
    shipRegion: order.shipRegion,
    shipPostalCode: order.shipPostalCode,
    shipCountry: order.shipCountry,
    shippingValidation: order.shippingValidation,
    details: order.details.map((detail) => ({
      productId: detail.productId,
      unitPrice: detail.unitPrice,
      quantity: detail.quantity,
      discount: detail.discount,
    })),
  };
}

function toDateInputValue(value: Date | string | null): string | null {
  if (!value) {
    return null;
  }

  const date = value instanceof Date ? value : new Date(value);
  return date.toISOString().slice(0, 10);
}
</script>
