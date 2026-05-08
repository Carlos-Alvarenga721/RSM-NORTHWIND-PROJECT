<template>
  <q-input
    :model-value="selectedEmployeeLabel"
    outlined
    dense
    readonly
    label="Employee"
    :rules="[(value) => Boolean(value) || 'Employee is required']"
  >
    <template #append>
      <q-icon name="arrow_drop_down" />
    </template>
    <q-menu fit>
      <q-list class="lookup-list">
        <q-item
          v-for="employee in lookupStore.employees"
          :key="employee.employeeId"
          v-close-popup
          clickable
          @click="selectEmployee(employee.employeeId)"
        >
          <q-item-section>
            <q-item-label>{{ employee.fullName }}</q-item-label>
            <q-item-label caption>{{ employee.title || 'No title' }}</q-item-label>
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

const selectedEmployeeLabel = computed(() => {
  const employee = lookupStore.employees.find((item) => item.employeeId === props.modelValue);

  if (!employee) {
    return '';
  }

  return employee.title ? `${employee.fullName} - ${employee.title}` : employee.fullName;
});

function selectEmployee(employeeId: number): void {
  emit('update:modelValue', employeeId);
}
</script>

<style scoped>
.lookup-list {
  max-height: 360px;
  min-width: 420px;
  overflow-y: auto;
}
</style>
