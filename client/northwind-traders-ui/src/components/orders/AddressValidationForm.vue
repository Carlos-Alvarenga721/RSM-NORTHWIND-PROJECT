<template>
  <section class="panel">
    <div class="panel-section">
      <div class="address-form-heading">
        <div>
          <div class="text-subtitle1 text-weight-bold">Shipping Address</div>
          <div class="text-caption text-grey-7">Enter the delivery location as it would appear on a shipment label.</div>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12">
          <q-input
            :model-value="address"
            outlined
            dense
            label="Ship address"
            hint="Street name and number, building, suite, or apartment."
            @update:model-value="emit('update:address', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="city"
            outlined
            dense
            label="Ship city"
            hint="City, town, or municipality."
            @update:model-value="emit('update:city', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="region"
            outlined
            dense
            label="State / Province / Region"
            :hint="regionHint"
            @update:model-value="emit('update:region', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            :model-value="postalCode"
            outlined
            dense
            label="Postal code"
            hint="Optional when unknown, but validation may require it in some countries."
            @update:model-value="emit('update:postalCode', String($event || ''))"
          />
        </div>
        <div class="col-12 col-md-6">
          <q-select
            :model-value="selectedCountryCode"
            outlined
            dense
            use-input
            fill-input
            hide-selected
            input-debounce="0"
            clearable
            emit-value
            map-options
            label="Ship country"
            hint="Select a country by name. The system sends the correct country code to Google."
            :options="filteredCountryOptions"
            @filter="filterCountries"
            @update:model-value="updateCountry"
            @new-value="createCountryValue"
          />
        </div>
        <div class="col-12 col-md-6 address-actions">
          <q-btn
            color="primary"
            icon="task_alt"
            label="Validate Address"
            :loading="isValidating"
            @click="validateAddress"
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { Notify } from 'quasar';
import { validateShippingAddress } from 'src/services/addressValidationService';
import { notifyApiError } from 'src/services/errorHandler';
import type { AddressValidationResponse } from 'src/types/addressValidation';

interface CountryOption {
  label: string;
  value: string;
  aliases: string[];
  regionHint: string;
}

const props = defineProps<{
  address: string | null;
  city: string | null;
  region: string | null;
  postalCode: string | null;
  country: string | null;
}>();

const emit = defineEmits<{
  'update:address': [value: string | null];
  'update:city': [value: string | null];
  'update:region': [value: string | null];
  'update:postalCode': [value: string | null];
  'update:country': [value: string | null];
  validated: [value: AddressValidationResponse];
}>();

const isValidating = ref(false);
const filteredCountryOptions = ref<CountryOption[]>([]);

const countryOptions: CountryOption[] = [
  {
    label: 'United States',
    value: 'US',
    aliases: ['usa', 'united states of america', 'eeuu'],
    regionHint: 'Use a state abbreviation or name, for example CA, NY, or Texas.',
  },
  {
    label: 'El Salvador',
    value: 'SV',
    aliases: ['salvador'],
    regionHint: 'Use the department, for example San Salvador, La Libertad, or Santa Ana.',
  },
  {
    label: 'Mexico',
    value: 'MX',
    aliases: ['méxico'],
    regionHint: 'Use the state, for example CDMX, Jalisco, Nuevo León, or Yucatán.',
  },
  {
    label: 'Canada',
    value: 'CA',
    aliases: [],
    regionHint: 'Use the province or territory, for example ON, BC, Québec, or Alberta.',
  },
  {
    label: 'United Kingdom',
    value: 'GB',
    aliases: ['uk', 'great britain', 'england'],
    regionHint: 'Use the county or nation when known, for example England, Scotland, or Essex.',
  },
  {
    label: 'Germany',
    value: 'DE',
    aliases: ['deutschland'],
    regionHint: 'Use the state when known, for example Bavaria, Berlin, or Hesse.',
  },
  {
    label: 'France',
    value: 'FR',
    aliases: [],
    regionHint: 'Use the region or department when known.',
  },
  {
    label: 'Brazil',
    value: 'BR',
    aliases: ['brasil'],
    regionHint: 'Use the state abbreviation or name, for example SP, RJ, or Paraná.',
  },
  {
    label: 'Venezuela',
    value: 'VE',
    aliases: [],
    regionHint: 'Use the state, for example Lara, Táchira, or Nueva Esparta.',
  },
  {
    label: 'Ireland',
    value: 'IE',
    aliases: [],
    regionHint: 'Use the county when known, for example County Cork or Dublin.',
  },
];

filteredCountryOptions.value = countryOptions;

const selectedCountry = computed(() => findCountryOption(props.country));
const selectedCountryCode = computed(() => selectedCountry.value?.value ?? props.country);
const regionHint = computed(
  () => selectedCountry.value?.regionHint ?? 'Use the state, province, department, county, or region when known.',
);

async function validateAddress(): Promise<void> {
  if (!props.address || !props.city || !props.country) {
    Notify.create({ type: 'negative', message: 'Enter shipping address, city, and country before validation.' });
    return;
  }

  isValidating.value = true;
  try {
    const response = await validateShippingAddress({
      addressLine: props.address,
      city: props.city,
      region: props.region,
      postalCode: props.postalCode,
      country: normalizeCountryCode(props.country),
    });
    emit('validated', response);
    Notify.create({
      type: getNotificationType(response.validationStatus),
      message: response.validationMessage || getValidationMessage(response.validationStatus),
    });
  } catch (error) {
    notifyApiError(error);
  } finally {
    isValidating.value = false;
  }
}

function getNotificationType(status: string): 'positive' | 'warning' | 'negative' {
  if (status === 'Validated') {
    return 'positive';
  }

  if (status === 'NeedsReview' || status === 'ValidationUnavailable') {
    return 'warning';
  }

  return 'negative';
}

function getValidationMessage(status: string): string {
  if (status === 'ValidationUnavailable') {
    return 'Address accepted. Google validation is not configured.';
  }

  if (status === 'NeedsReview') {
    return 'Google adjusted or inferred part of this address. Review it before saving.';
  }

  if (status === 'Invalid') {
    return 'Google could not validate this address. Please review it.';
  }

  return 'Shipping address validated.';
}

function filterCountries(value: string, update: (callback: () => void) => void): void {
  update(() => {
    const needle = value.trim().toLowerCase();
    filteredCountryOptions.value = needle
      ? countryOptions.filter((option) =>
          [option.label, option.value, ...option.aliases].some((candidate) =>
            candidate.toLowerCase().includes(needle),
          ),
        )
      : countryOptions;
  });
}

function updateCountry(value: string | null): void {
  emit('update:country', value);
}

function createCountryValue(
  value: string,
  done: (item?: string, mode?: 'add' | 'add-unique' | 'toggle') => void,
): void {
  const normalizedValue = normalizeCountryCode(value) ?? value.trim();
  emit('update:country', normalizedValue);
  done(normalizedValue, 'add-unique');
}

function normalizeCountryCode(value: string | null): string | null {
  if (!value) {
    return null;
  }

  return findCountryOption(value)?.value ?? value.trim();
}

function findCountryOption(value: string | null): CountryOption | null {
  if (!value) {
    return null;
  }

  const normalizedValue = value.trim().toLowerCase();
  return countryOptions.find(
    (option) =>
      option.value.toLowerCase() === normalizedValue ||
      option.label.toLowerCase() === normalizedValue ||
      option.aliases.includes(normalizedValue),
  ) ?? null;
}
</script>

<style scoped>
.address-form-heading {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 14px;
}

.address-actions {
  display: flex;
  align-items: flex-end;
  justify-content: flex-end;
}
</style>
