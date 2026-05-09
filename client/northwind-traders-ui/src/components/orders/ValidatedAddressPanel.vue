<template>
  <section class="validated-address panel">
    <div class="panel-section">
      <div class="row items-center justify-between q-mb-md">
        <div>
          <div class="text-subtitle1 text-weight-bold">Validated Address</div>
          <div class="text-caption text-grey-7">{{ response.validationStatus }}</div>
        </div>
        <q-chip :color="chipColor" text-color="white" :icon="chipIcon">
          {{ chipLabel }}
        </q-chip>
      </div>

      <q-list dense>
        <q-item>
          <q-item-section>
            <q-item-label caption>Formatted address</q-item-label>
            <q-item-label>{{ response.formattedAddress || 'Not provided' }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item>
          <q-item-section>
            <q-item-label caption>Coordinates</q-item-label>
            <q-item-label>{{ coordinatesLabel }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item>
          <q-item-section>
            <q-item-label caption>Validation result</q-item-label>
            <q-item-label>{{ validationMessage }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item>
          <q-item-section>
            <q-item-label caption>Validation granularity</q-item-label>
            <q-item-label>{{ response.validationGranularity || 'Not provided' }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item>
          <q-item-section>
            <q-item-label caption>Geocode granularity</q-item-label>
            <q-item-label>{{ response.geocodeGranularity || 'Not provided' }}</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>

      <q-banner v-if="isNeedsReview" dense class="bg-warning text-black q-mt-md">
        Google adjusted or inferred part of this address. Review the formatted address before saving.
      </q-banner>
      <q-banner v-if="isInvalid" dense class="bg-negative text-white q-mt-md">
        This address could not be validated. Please update the shipping address.
      </q-banner>
    </div>
    <GoogleMapPreview
      :latitude="response.latitude"
      :longitude="response.longitude"
      :address="response.formattedAddress"
    />
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import GoogleMapPreview from 'components/maps/GoogleMapPreview.vue';
import type { AddressValidationResponse } from 'src/types/addressValidation';

const props = defineProps<{
  response: AddressValidationResponse;
}>();

const coordinatesLabel = computed(() => {
  if (props.response.latitude === null || props.response.longitude === null) {
    return 'Coordinates unavailable';
  }

  return `${props.response.latitude.toFixed(6)}, ${props.response.longitude.toFixed(6)}`;
});
const isValidated = computed(() => props.response.validationStatus === 'Validated');
const isNeedsReview = computed(() => props.response.validationStatus === 'NeedsReview');
const isInvalid = computed(() => props.response.validationStatus === 'Invalid');
const isValidationUnavailable = computed(
  () => props.response.validationStatus === 'ValidationUnavailable',
);
const validationMessage = computed(
  () => props.response.validationMessage || getValidationMessage(props.response.validationStatus),
);
const chipColor = computed(() => {
  if (isValidated.value) {
    return 'positive';
  }

  if (isNeedsReview.value || isValidationUnavailable.value) {
    return 'warning';
  }

  return 'negative';
});
const chipIcon = computed(() => {
  if (isValidated.value) {
    return 'verified';
  }

  if (isNeedsReview.value || isValidationUnavailable.value) {
    return 'warning';
  }

  return 'error_outline';
});
const chipLabel = computed(() => {
  if (isValidated.value) {
    return 'Validated';
  }

  if (isNeedsReview.value) {
    return 'Needs Review';
  }

  if (isValidationUnavailable.value) {
    return 'Unconfigured';
  }

  return 'Invalid';
});

function getValidationMessage(status: string): string {
  if (status === 'NeedsReview') {
    return 'Google found the address but it needs review before saving.';
  }

  if (status === 'Invalid') {
    return 'Google could not validate this as a complete deliverable address.';
  }

  if (status === 'ValidationUnavailable') {
    return 'Google address validation is not configured.';
  }

  return 'Google validated this address.';
}
</script>

<style scoped>
.validated-address {
  overflow: hidden;
}
</style>
