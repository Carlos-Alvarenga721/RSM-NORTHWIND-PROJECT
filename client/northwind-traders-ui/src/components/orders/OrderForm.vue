<template>
  <q-form class="order-form" @submit.prevent="submit">
    <section class="panel">
      <div class="panel-section">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <CustomerSelector v-model="draft.customerId" />
          </div>
          <div class="col-12 col-md-6">
            <EmployeeSelector v-model="draft.employeeId" />
          </div>
          <div class="col-12 col-md-4">
            <q-input v-model="draft.orderDate" outlined dense type="date" label="Order date" />
          </div>
          <div class="col-12 col-md-4">
            <q-input v-model="draft.requiredDate" outlined dense type="date" label="Required date" />
          </div>
          <div class="col-12 col-md-4">
            <q-input v-model="draft.shippedDate" outlined dense type="date" label="Shipped date" />
          </div>
          <div class="col-12 col-md-6">
            <ShipperSelector v-model="draft.shipVia" />
          </div>
          <div class="col-12 col-md-6">
            <q-input
              v-model.number="draft.freight"
              outlined
              dense
              type="number"
              min="0"
              step="0.01"
              label="Freight"
              :rules="[(value) => Number(value || 0) >= 0 || 'Freight must be zero or greater']"
            />
          </div>
          <div class="col-12">
            <q-input v-model="draft.shipName" outlined dense label="Ship name" />
          </div>
        </div>
      </div>
    </section>

    <ProductLineItemsTable v-model:details="draft.details" />

    <AddressValidationForm
      v-model:address="draft.shipAddress"
      v-model:city="draft.shipCity"
      v-model:region="draft.shipRegion"
      v-model:postal-code="draft.shipPostalCode"
      v-model:country="draft.shipCountry"
      @validated="handleAddressValidated"
    />

    <ValidatedAddressPanel
      v-if="validatedAddress"
      :response="validatedAddress"
    />

    <section v-if="validatedAddress && requiresReview" class="panel review-confirmation">
      <div class="panel-section">
        <q-banner dense class="bg-warning text-black q-mb-md">
          Google adjusted or inferred part of this address. Review the formatted address before saving.
        </q-banner>
        <q-checkbox
          v-model="hasReviewedAddress"
          label="I reviewed and accept the validated address"
        />
      </div>
    </section>

    <section class="panel">
      <div class="panel-section order-summary">
        <span>Order total</span>
        <strong>{{ formatCurrency(orderTotal) }}</strong>
      </div>
    </section>

    <div class="form-actions">
      <q-btn flat icon="close" label="Cancel" @click="emit('cancel')" />
      <q-btn color="primary" icon="save" label="Save Order" type="submit" :loading="isSubmitting" />
    </div>
  </q-form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { Notify } from 'quasar';
import CustomerSelector from './CustomerSelector.vue';
import EmployeeSelector from './EmployeeSelector.vue';
import ShipperSelector from './ShipperSelector.vue';
import ProductLineItemsTable from './ProductLineItemsTable.vue';
import AddressValidationForm from './AddressValidationForm.vue';
import ValidatedAddressPanel from './ValidatedAddressPanel.vue';
import { useLookupStore } from 'src/stores/lookupStore';
import type { AddressValidationResponse } from 'src/types/addressValidation';
import type { CreateOrderRequest, OrderFormModel } from 'src/types/orders';

const props = defineProps<{
  modelValue: OrderFormModel;
  isSubmitting?: boolean;
}>();

const emit = defineEmits<{
  submit: [value: CreateOrderRequest];
  cancel: [];
}>();

const lookupStore = useLookupStore();
const draft = ref<OrderFormModel>(cloneForm(props.modelValue));
const validatedAddress = ref<AddressValidationResponse | null>(null);
const validatedAddressSignature = ref<string | null>(null);
const hasReviewedAddress = ref(false);

const addressSignature = computed(() => getAddressSignature(draft.value));
const validationStatus = computed(() => validatedAddress.value?.validationStatus ?? null);
const requiresReview = computed(() => validationStatus.value === 'NeedsReview');
const isInvalidAddress = computed(() => validationStatus.value === 'Invalid');
const isValidationUnavailable = computed(() => validationStatus.value === 'ValidationUnavailable');
const isValidatedStatus = computed(() => validationStatus.value === 'Validated');
const isAddressAccepted = computed(() => {
  if (!validatedAddress.value) {
    return false;
  }

  if (validatedAddressSignature.value !== addressSignature.value) {
    return false;
  }

  if (isValidatedStatus.value || isValidationUnavailable.value) {
    return true;
  }

  return requiresReview.value && hasReviewedAddress.value;
});
const itemsTotal = computed(() =>
  draft.value.details.reduce(
    (sum, detail) => sum + detail.unitPrice * detail.quantity * (1 - detail.discount),
    0,
  ),
);
const orderTotal = computed(() => itemsTotal.value + Number(draft.value.freight || 0));

watch(
  () => props.modelValue,
  (value) => {
    draft.value = cloneForm(value);
    setValidationState(value.shippingValidation);
  },
  { deep: true },
);

watch(addressSignature, (value) => {
  if (validatedAddressSignature.value && validatedAddressSignature.value !== value) {
    clearValidationState();
  }
});

onMounted(async () => {
  setValidationState(draft.value.shippingValidation);
  await lookupStore.loadLookups();
});

function submit(): void {
  if (!validateBeforeSubmit()) {
    return;
  }

  const shipAddress = getPreferredShipAddress();

  emit('submit', {
    customerId: draft.value.customerId || '',
    employeeId: draft.value.employeeId || 0,
    orderDate: draft.value.orderDate,
    requiredDate: draft.value.requiredDate,
    shippedDate: draft.value.shippedDate,
    shipVia: draft.value.shipVia,
    freight: draft.value.freight,
    shipName: draft.value.shipName,
    shipAddress,
    shipCity: draft.value.shipCity,
    shipRegion: draft.value.shipRegion,
    shipPostalCode: draft.value.shipPostalCode,
    shipCountry: draft.value.shipCountry,
    shippingValidation: isAddressAccepted.value ? validatedAddress.value : null,
    details: draft.value.details,
  });
}

function validateBeforeSubmit(): boolean {
  const invalidDetail = draft.value.details.find(
    (detail) =>
      detail.productId <= 0 ||
      detail.quantity <= 0 ||
      detail.unitPrice < 0 ||
      detail.discount < 0 ||
      detail.discount > 1,
  );

  if (!draft.value.customerId) {
    Notify.create({ type: 'negative', message: 'Customer is required.' });
    return false;
  }

  if (!draft.value.employeeId || draft.value.employeeId <= 0) {
    Notify.create({ type: 'negative', message: 'Employee is required.' });
    return false;
  }

  if (!draft.value.shipAddress || !draft.value.shipCity || !draft.value.shipCountry) {
    Notify.create({ type: 'negative', message: 'Shipping address, city, and country are required.' });
    return false;
  }

  if (Number(draft.value.freight || 0) < 0) {
    Notify.create({ type: 'negative', message: 'Freight must be zero or greater.' });
    return false;
  }

  if (!validatedAddress.value || validatedAddressSignature.value !== addressSignature.value) {
    Notify.create({
      type: 'negative',
      message: 'Validate the shipping address successfully before saving.',
    });
    return false;
  }

  if (isInvalidAddress.value) {
    Notify.create({
      type: 'negative',
      message: validatedAddress.value?.validationMessage || 'Google could not validate this address.',
    });
    return false;
  }

  if (requiresReview.value && !hasReviewedAddress.value) {
    Notify.create({
      type: 'negative',
      message: 'Review and accept the validated address before saving.',
    });
    return false;
  }

  if (draft.value.details.length === 0) {
    Notify.create({ type: 'negative', message: 'Add at least one product line item.' });
    return false;
  }

  if (invalidDetail) {
    Notify.create({ type: 'negative', message: 'Each product must have a valid product, quantity, price, and discount.' });
    return false;
  }

  if (new Set(draft.value.details.map((detail) => detail.productId)).size !== draft.value.details.length) {
    Notify.create({ type: 'negative', message: 'An order cannot contain duplicate products.' });
    return false;
  }

  return true;
}

function handleAddressValidated(response: AddressValidationResponse): void {
  setValidationState(response);
  if (response.validationStatus === 'NeedsReview') {
    hasReviewedAddress.value = false;
  }
}

function getPreferredShipAddress(): string | null {
  if (!validatedAddress.value) {
    return draft.value.shipAddress;
  }

  if (!isAddressAccepted.value) {
    return draft.value.shipAddress;
  }

  return validatedAddress.value.formattedAddress || draft.value.shipAddress;
}

function getAddressSignature(value: OrderFormModel): string {
  return [
    value.shipAddress,
    value.shipCity,
    value.shipRegion,
    value.shipPostalCode,
    value.shipCountry,
  ]
    .map((part) => (part || '').trim().toLowerCase())
    .join('|');
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value);
}

function cloneForm(value: OrderFormModel): OrderFormModel {
  return {
    ...value,
    shippingValidation: value.shippingValidation ? { ...value.shippingValidation } : null,
    details: value.details.map((detail) => ({ ...detail })),
  };
}

function clearValidationState(): void {
  validatedAddress.value = null;
  validatedAddressSignature.value = null;
  hasReviewedAddress.value = false;
}

function setValidationState(response: AddressValidationResponse | null): void {
  if (!response) {
    clearValidationState();
    return;
  }

  validatedAddress.value = response;
  validatedAddressSignature.value = addressSignature.value;
  hasReviewedAddress.value = response.validationStatus === 'Validated' ||
    response.validationStatus === 'ValidationUnavailable';
}
</script>

<style scoped>
.order-form {
  display: grid;
  gap: 16px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.order-summary {
  display: flex;
  justify-content: flex-end;
  gap: 16px;
  font-size: 18px;
}

.review-confirmation {
  display: grid;
  gap: 12px;
}
</style>
