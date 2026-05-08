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
const chipColor = computed(() => (props.response.validationStatus === 'ValidationUnavailable' ? 'warning' : 'positive'));
const chipIcon = computed(() => (props.response.validationStatus === 'ValidationUnavailable' ? 'warning' : 'verified'));
const chipLabel = computed(() => (props.response.validationStatus === 'ValidationUnavailable' ? 'Accepted' : 'Validated'));
</script>

<style scoped>
.validated-address {
  overflow: hidden;
}
</style>
