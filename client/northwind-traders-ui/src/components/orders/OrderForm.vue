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
      @validated="validatedAddress = $event"
    />

    <ValidatedAddressPanel
      v-if="validatedAddress"
      :response="validatedAddress"
    />

    <div class="form-actions">
      <q-btn flat icon="close" label="Cancel" @click="emit('cancel')" />
      <q-btn color="primary" icon="save" label="Save Order" type="submit" :loading="isSubmitting" />
    </div>
  </q-form>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
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

watch(
  () => props.modelValue,
  (value) => {
    draft.value = cloneForm(value);
  },
  { deep: true },
);

onMounted(async () => {
  await lookupStore.loadLookups();
});

function submit(): void {
  emit('submit', {
    customerId: draft.value.customerId || '',
    employeeId: draft.value.employeeId || 0,
    orderDate: draft.value.orderDate,
    requiredDate: draft.value.requiredDate,
    shippedDate: draft.value.shippedDate,
    shipVia: draft.value.shipVia,
    freight: draft.value.freight,
    shipName: draft.value.shipName,
    shipAddress: draft.value.shipAddress,
    shipCity: draft.value.shipCity,
    shipRegion: draft.value.shipRegion,
    shipPostalCode: draft.value.shipPostalCode,
    shipCountry: draft.value.shipCountry,
    details: draft.value.details,
  });
}

function cloneForm(value: OrderFormModel): OrderFormModel {
  return {
    ...value,
    details: value.details.map((detail) => ({ ...detail })),
  };
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
</style>
