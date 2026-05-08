import { apiClient } from './apiClient';
import type {
  AddressValidationRequest,
  AddressValidationResponse,
} from 'src/types/addressValidation';

export async function validateShippingAddress(
  request: AddressValidationRequest,
): Promise<AddressValidationResponse> {
  const response = await apiClient.post<AddressValidationResponse>(
    '/api/address-validation/validate',
    request,
  );
  return response.data;
}
