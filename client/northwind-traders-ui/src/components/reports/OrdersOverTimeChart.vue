<template>
  <section class="panel chart-panel">
    <div class="panel-section chart-panel-section">
      <div class="chart-heading">Orders over Time</div>
      <div class="chart-body">
        <q-inner-loading :showing="isLoading" color="primary" />
        <div v-if="!isLoading && !hasData" class="chart-empty">
          <q-icon name="show_chart" size="32px" color="grey-6" />
          <div>No order activity found for the selected filters.</div>
        </div>
        <Line v-else :data="chartData" :options="chartOptions" class="chart-canvas" />
      </div>
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
  isLoading?: boolean;
}>();

const hasData = computed(() => props.points.length > 0);

const chartData = computed<ChartData<'line'>>(() => ({
  labels: props.points.map((point) => point.label),
  datasets: [
    {
      label: 'Orders',
      data: props.points.map((point) => point.orderCount),
      borderColor: '#28527a',
      backgroundColor: '#28527a',
      pointBackgroundColor: '#28527a',
      pointRadius: 3,
      pointHoverRadius: 5,
      tension: 0.25,
    },
  ],
}));

const chartOptions: ChartOptions<'line'> = {
  responsive: true,
  maintainAspectRatio: false,
  resizeDelay: 150,
  animation: false,
  layout: {
    padding: {
      top: 8,
      right: 8,
      bottom: 0,
      left: 0,
    },
  },
  plugins: {
    legend: {
      display: false,
    },
  },
  scales: {
    x: {
      grid: {
        display: false,
      },
      ticks: {
        autoSkip: true,
        maxRotation: 0,
      },
    },
    y: {
      beginAtZero: true,
      ticks: {
        precision: 0,
      },
    },
  },
};
</script>

<style scoped>
.chart-panel {
  min-height: 340px;
  overflow: hidden;
}

.chart-panel-section {
  display: flex;
  min-height: 338px;
  flex-direction: column;
}

.chart-heading {
  margin-bottom: 12px;
  font-size: 16px;
  font-weight: 700;
}

.chart-body {
  position: relative;
  height: 260px;
  min-height: 260px;
  max-height: 260px;
  overflow: hidden;
}

.chart-canvas {
  display: block;
  width: 100% !important;
  height: 260px !important;
  max-height: 260px !important;
}

.chart-empty {
  display: flex;
  height: 100%;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: var(--nw-muted);
  text-align: center;
  font-size: 13px;
}
</style>
