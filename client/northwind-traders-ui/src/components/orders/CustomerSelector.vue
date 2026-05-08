<template>
  <q-input
    :model-value="selectedCustomerLabel"
    outlined
    dense
    readonly
    label="Customer"
    :rules="[(value) => Boolean(value) || 'Customer is required']"
  >
    <template #append>
      <q-icon name="arrow_drop_down" />
    </template>
    <q-menu fit>
      <q-list class="lookup-list">
        <q-item>
          <q-item-section>
            <q-input v-model="search" dense outlined autofocus label="Search customers" />
          </q-item-section>
        </q-item>
        <q-separator />
        <q-item
          v-for="customer in filteredCustomers"
          :key="customer.customerId"
          v-close-popup
          clickable
          @click="selectCustomer(customer.customerId)"
        >
          <q-item-section>
            <q-item-label>{{ customer.customerId }} - {{ customer.companyName }}</q-item-label>
            <q-item-label caption>{{ customer.city || 'No city' }} / {{ customer.country || 'No country' }}</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
    </q-menu>
  </q-input>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useLookupStore } from 'src/stores/lookupStore';

const props = defineProps<{
  modelValue: string | null;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: string | null];
}>();

const lookupStore = useLookupStore();
const search = ref('');

const selectedCustomerLabel = computed(() => {
  const customer = lookupStore.customers.find((item) => item.customerId === props.modelValue);

  return customer ? `${customer.customerId} - ${customer.companyName}` : '';
});

const filteredCustomers = computed(() => {
  const term = search.value.toLowerCase();

  return lookupStore.customers.filter(
    (customer) =>
      !term ||
      customer.customerId.toLowerCase().includes(term) ||
      customer.companyName.toLowerCase().includes(term),
  );
});

function selectCustomer(customerId: string): void {
  emit('update:modelValue', customerId);
}
</script>

<style scoped>
.lookup-list {
  max-height: 360px;
  min-width: 420px;
  overflow-y: auto;
}
</style>
