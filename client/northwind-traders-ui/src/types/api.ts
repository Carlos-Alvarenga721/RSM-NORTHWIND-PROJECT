export interface ApiValidationErrors {
  [field: string]: string[];
}

export interface ApiProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errors?: ApiValidationErrors;
}
