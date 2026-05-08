<template>
  <section class="panel">
    <div class="panel-section">
      <div class="row items-center justify-between q-mb-md">
        <div>
          <div class="text-subtitle1 text-weight-bold">Order Items</div>
          <div class="text-caption text-grey-7">Products, quantities, pricing, and discounts</div>
        </div>
        <q-btn color="primary" icon="add" label="Add Item" @click="addItem" />
      </div>

      <q-table
        flat
        bordered
        :rows="details"
        :columns="columns"
        row-key="productId"
        hide-pagination
        :pagination="{ rowsPerPage: 0 }"
      >
        <template #body-cell-productId="scope">
          <q-td :props="scope">
            <q-select
              :model-value="scope.row.productId || null"
              :options="productOptions"
              option-label="label"
              option-value="value"
              emit-value
              map-options
              outlined
              dense
              label="Product"
              :rules="[(value) => Number(value) > 0 || 'Product is required']"
              @update:model-value="updateProduct(scope.rowIndex, Number($event))"
            />
          </q-td>
        </template>

        <template #body-cell-unitPrice="scope">
          <q-td :props="scope">
            <q-input
              :model-value="scope.row.unitPrice"
              outlined
              dense
              type="number"
              min="0"
              step="0.01"
              :rules="[(value) => Number(value) >= 0 || 'Price must be zero or greater']"
              @update:model-value="updateDetail(scope.rowIndex, 'unitPrice', Number($event || 0))"
            />
          </q-td>
        </template>

        <template #body-cell-quantity="scope">
          <q-td :props="scope">
            <q-input
              :model-value="scope.row.quantity"
              outlined
              dense
              type="number"
              min="1"
              :rules="[(value) => Number(value) > 0 || 'Quantity must be greater than zero']"
              @update:model-value="updateDetail(scope.rowIndex, 'quantity', Number($event || 1))"
            />
          </q-td>
        </template>

        <template #body-cell-discount="scope">
          <q-td :props="scope">
            <q-input
              :model-value="scope.row.discount"
              outlined
              dense
              type="number"
              min="0"
              max="1"
              step="0.01"
              :rules="[(value) => (Number(value) >= 0 && Number(value) <= 1) || 'Discount must be between 0 and 1']"
              @update:model-value="updateDetail(scope.rowIndex, 'discount', Number($event || 0))"
            />
          </q-td>
        </template>

        <template #body-cell-lineTotal="scope">
          <q-td :props="scope">
            {{ formatCurrency(getLineTotal(scope.row)) }}
          </q-td>
        </template>

        <template #body-cell-actions="scope">
          <q-td :props="scope">
            <q-btn
              flat
              round
              dense
              color="negative"
              icon="delete"
              @click="removeItem(scope.rowIndex)"
            >
              <q-tooltip>Remove item</q-tooltip>
            </q-btn>
          </q-td>
        </template>
      </q-table>

      <div class="order-total q-mt-md">
        <span>Items total</span>
        <strong>{{ formatCurrency(itemsTotal) }}</strong>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { QTableColumn } from 'quasar';
import { useLookupStore } from 'src/stores/lookupStore';
import type { OrderDetailRequest } from 'src/types/orders';

const props = defineProps<{
  details: OrderDetailRequest[];
}>();

const emit = defineEmits<{
  'update:details': [value: OrderDetailRequest[]];
}>();

const lookupStore = useLookupStore();

const columns: QTableColumn<OrderDetailRequest>[] = [
  { name: 'productId', label: 'Product', field: 'productId', align: 'left' },
  { name: 'unitPrice', label: 'Unit Price', field: 'unitPrice', align: 'right' },
  { name: 'quantity', label: 'Quantity', field: 'quantity', align: 'right' },
  { name: 'discount', label: 'Discount', field: 'discount', align: 'right' },
  { name: 'lineTotal', label: 'Line Total', field: 'productId', align: 'right' },
  { name: 'actions', label: '', field: 'productId', align: 'right' },
];

const productOptions = computed(() =>
  lookupStore.products
    .filter((product) => !product.discontinued)
    .map((product) => ({
      label: `${product.productName} - ${formatCurrency(product.unitPrice || 0)}`,
      value: product.productId,
      unitPrice: product.unitPrice || 0,
    })),
);

const itemsTotal = computed(() => props.details.reduce((sum, detail) => sum + getLineTotal(detail), 0));

function addItem(): void {
  emit('update:details', [
    ...props.details,
    {
      productId: 0,
      unitPrice: 0,
      quantity: 1,
      discount: 0,
    },
  ]);
}

function removeItem(index: number): void {
  emit(
    'update:details',
    props.details.filter((_, detailIndex) => detailIndex !== index),
  );
}

function updateProduct(index: number, productId: number): void {
  const product = lookupStore.products.find((item) => item.productId === productId);
  updateRow(index, {
    productId,
    unitPrice: product?.unitPrice || 0,
  });
}

function updateDetail(
  index: number,
  field: keyof Pick<OrderDetailRequest, 'unitPrice' | 'quantity' | 'discount'>,
  value: number,
): void {
  updateRow(index, { [field]: value });
}

function updateRow(index: number, value: Partial<OrderDetailRequest>): void {
  emit(
    'update:details',
    props.details.map((detail, detailIndex) =>
      detailIndex === index ? { ...detail, ...value } : detail,
    ),
  );
}

function getLineTotal(detail: OrderDetailRequest): number {
  return detail.unitPrice * detail.quantity * (1 - detail.discount);
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value);
}
</script>

<style scoped>
.order-total {
  display: flex;
  justify-content: flex-end;
  gap: 16px;
  font-size: 16px;
}
</style>
