<template>
  <section class="panel chart-panel">
    <div class="panel-section">
      <div class="text-subtitle1 text-weight-bold q-mb-md">Shipments by Region</div>
      <Bar :data="chartData" :options="chartOptions" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import {
  BarElement,
  CategoryScale,
  Chart as ChartJS,
  Legend,
  LinearScale,
  Title,
  Tooltip,
} from 'chart.js';
import { Bar } from 'vue-chartjs';
import type { ChartData, ChartOptions } from 'chart.js';
import type { ShipmentsByRegionPoint } from 'src/types/reports';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const props = defineProps<{
  points: ShipmentsByRegionPoint[];
}>();

const chartData = computed<ChartData<'bar'>>(() => ({
  labels: props.points.map((point) => point.region || 'Unassigned'),
  datasets: [
    {
      label: 'Shipments',
      data: props.points.map((point) => point.shipmentCount),
      backgroundColor: '#2f7d6d',
    },
  ],
}));

const chartOptions: ChartOptions<'bar'> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false,
    },
  },
};
</script>

<style scoped>
.chart-panel {
  height: 340px;
}
</style>
