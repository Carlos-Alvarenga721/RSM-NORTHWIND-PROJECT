<template>
  <q-input
    :model-value="selectedShipperLabel"
    outlined
    dense
    readonly
    label="Shipper"
  >
    <template #append>
      <q-btn
        v-if="modelValue"
        flat
        dense
        round
        icon="close"
        @click.stop="emit('update:modelValue', null)"
      />
      <q-icon name="arrow_drop_down" />
    </template>
    <q-menu fit>
      <q-list class="lookup-list">
        <q-item
          v-for="shipper in lookupStore.shippers"
          :key="shipper.shipperId"
          v-close-popup
          clickable
          @click="selectShipper(shipper.shipperId)"
        >
          <q-item-section>
            <q-item-label>{{ shipper.companyName }}</q-item-label>
            <q-item-label caption>{{ shipper.phone || 'No phone' }}</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
    </q-menu>
  </q-input>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useLookupStore } from 'src/stores/lookupStore';

const props = defineProps<{
  modelValue: number | null;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: number | null];
}>();

const lookupStore = useLookupStore();

const selectedShipperLabel = computed(() => {
  const shipper = lookupStore.shippers.find((item) => item.shipperId === props.modelValue);

  return shipper ? shipper.companyName : '';
});

function selectShipper(shipperId: number): void {
  emit('update:modelValue', shipperId);
}
</script>

<style scoped>
.lookup-list {
  max-height: 360px;
  min-width: 420px;
  overflow-y: auto;
}
</style>
