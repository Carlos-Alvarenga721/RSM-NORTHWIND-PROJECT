<template>
  <section class="panel">
    <div class="panel-section">
      <div class="row q-col-gutter-md">
        <div class="col-12">
          <q-input
            :model-value="address"
            outlined
            dense
            label="Ship address"
            @update:model-value="emit('update:address', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="city"
            outlined
            dense
            label="Ship city"
            @update:model-value="emit('update:city', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="region"
            outlined
            dense
            label="Ship region"
            @update:model-value="emit('update:region', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="postalCode"
            outlined
            dense
            label="Postal code"
            @update:model-value="emit('update:postalCode', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-6">
          <q-input
            :model-value="country"
            outlined
            dense
            label="Ship country"
            @update:model-value="emit('update:country', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-6 address-actions">
          <q-btn
            color="primary"
            icon="task_alt"
            label="Validate Address"
            :loading="isValidating"
            @click="validateAddress"
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Notify } from 'quasar';
import { validateShippingAddress } from 'src/services/addressValidationService';
import { notifyApiError } from 'src/services/errorHandler';
import type { AddressValidationResponse } from 'src/types/addressValidation';

const props = defineProps<{
  address: string | null;
  city: string | null;
  region: string | null;
  postalCode: string | null;
  country: string | null;
}>();

const emit = defineEmits<{
  'update:address': [value: string | null];
  'update:city': [value: string | null];
  'update:region': [value: string | null];
  'update:postalCode': [value: string | null];
  'update:country': [value: string | null];
  validated: [value: AddressValidationResponse];
}>();

const isValidating = ref(false);

async function validateAddress(): Promise<void> {
  if (!props.address || !props.city || !props.country) {
    Notify.create({ type: 'negative', message: 'Enter shipping address, city, and country before validation.' });
    return;
  }

  isValidating.value = true;
  try {
    const response = await validateShippingAddress({
      addressLine: props.address,
      city: props.city,
      region: props.region,
      postalCode: props.postalCode,
      country: props.country,
    });
    emit('validated', response);
    const isAccepted = ['Validated', 'ValidationUnavailable'].includes(response.validationStatus);
    Notify.create({
      type: isAccepted ? (response.validationStatus === 'ValidationUnavailable' ? 'warning' : 'positive') : 'negative',
      message: response.validationMessage || getValidationMessage(response.validationStatus),
    });
  } catch (error) {
    notifyApiError(error);
  } finally {
    isValidating.value = false;
  }
}

function getValidationMessage(status: string): string {
  if (status === 'ValidationUnavailable') {
    return 'Address accepted. Google validation is not configured.';
  }

  if (status === 'NeedsReview') {
    return 'Google found this address but it needs review before saving.';
  }

  if (status === 'Invalid') {
    return 'Google could not validate this address. Please review it.';
  }

  return 'Shipping address validated.';
}
</script>

<style scoped>
.address-actions {
  display: flex;
  align-items: flex-end;
  justify-content: flex-end;
}
</style>
