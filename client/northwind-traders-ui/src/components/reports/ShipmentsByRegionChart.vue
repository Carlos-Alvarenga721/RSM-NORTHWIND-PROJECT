<template>
  <section class="panel chart-panel">
    <div class="panel-section chart-panel-section">
      <div class="chart-heading">Shipments by Region</div>
      <div class="chart-body">
        <q-inner-loading :showing="isLoading" color="primary" />
        <div v-if="!isLoading && !hasData" class="chart-empty">
          <q-icon name="bar_chart" size="32px" color="grey-6" />
          <div>No shipment regions found for the selected filters.</div>
        </div>
        <Bar v-else :data="chartData" :options="chartOptions" class="chart-canvas" />
      </div>
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
  isLoading?: boolean;
}>();

const hasData = computed(() => props.points.length > 0);

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
