export interface AddressValidationRequest {
  addressLine: string | null;
  city: string | null;
  region: string | null;
  postalCode: string | null;
  country: string | null;
}

export interface AddressValidationResponse {
  originalAddress: string | null;
  formattedAddress: string | null;
  latitude: number | null;
  longitude: number | null;
  validationStatus: string;
  googlePlaceId: string | null;
}
