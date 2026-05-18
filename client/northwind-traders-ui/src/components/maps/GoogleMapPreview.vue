<template>
  <div class="map-preview">
    <iframe
      v-if="mapUrl"
      title="Validated shipping address map"
      :src="mapUrl"
      loading="lazy"
      referrerpolicy="no-referrer-when-downgrade"
    />
    <div v-else class="map-preview__empty">
      <q-icon name="location_on" size="32px" />
      <span>Map preview unavailable</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  latitude: number | null;
  longitude: number | null;
  address?: string | null;
}>();

const mapUrl = computed(() => {
  if (props.latitude !== null && props.longitude !== null) {
    // Prefer coordinates from validation because they are less ambiguous than free-form address text.
    return `https://maps.google.com/maps?q=${props.latitude},${props.longitude}&z=14&output=embed`;
  }

  if (props.address) {
    // Address text is a fallback for older orders or validation responses without coordinates.
    return `https://maps.google.com/maps?q=${encodeURIComponent(props.address)}&z=14&output=embed`;
  }

  return null;
});
</script>

<style scoped>
.map-preview {
  min-height: 260px;
  border: 1px solid var(--nw-border);
  border-radius: 8px;
  overflow: hidden;
  background: #eef2f7;
}

.map-preview iframe {
  width: 100%;
  min-height: 260px;
  border: 0;
}

.map-preview__empty {
  min-height: 260px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: var(--nw-muted);
}
</style>
