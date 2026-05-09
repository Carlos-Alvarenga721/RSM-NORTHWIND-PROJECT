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
      </q-list>
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
const validationMessage = computed(
  () => props.response.validationMessage || getValidationMessage(props.response.validationStatus),
);
const chipColor = computed(() => {
  if (props.response.validationStatus === 'Validated') {
    return 'positive';
  }

  if (props.response.validationStatus === 'ValidationUnavailable') {
    return 'warning';
  }

  return 'negative';
});
const chipIcon = computed(() => {
  if (props.response.validationStatus === 'Validated') {
    return 'verified';
  }

  if (props.response.validationStatus === 'ValidationUnavailable') {
    return 'warning';
  }

  return 'error_outline';
});
const chipLabel = computed(() => {
  if (props.response.validationStatus === 'Validated') {
    return 'Validated';
  }

  if (props.response.validationStatus === 'ValidationUnavailable') {
    return 'Accepted';
  }

  return 'Blocked';
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
