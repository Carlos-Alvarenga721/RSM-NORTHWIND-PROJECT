<template>
  <section class="panel chart-panel">
    <div class="panel-section">
      <div class="text-subtitle1 text-weight-bold q-mb-md">Orders over Time</div>
      <Line :data="chartData" :options="chartOptions" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import {
  CategoryScale,
  Chart as ChartJS,
  Legend,
  LinearScale,
  LineElement,
  PointElement,
  Title,
  Tooltip,
} from 'chart.js';
import { Line } from 'vue-chartjs';
import type { ChartData, ChartOptions } from 'chart.js';
import type { OrdersOverTimePoint } from 'src/types/reports';

ChartJS.register(CategoryScale, LinearScale, LineElement, PointElement, Title, Tooltip, Legend);

const props = defineProps<{
  points: OrdersOverTimePoint[];
}>();

const chartData = computed<ChartData<'line'>>(() => ({
  labels: props.points.map((point) => point.label),
  datasets: [
    {
      label: 'Orders',
      data: props.points.map((point) => point.orderCount),
      borderColor: '#28527a',
      backgroundColor: '#28527a',
      tension: 0.25,
    },
  ],
}));

const chartOptions: ChartOptions<'line'> = {
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
