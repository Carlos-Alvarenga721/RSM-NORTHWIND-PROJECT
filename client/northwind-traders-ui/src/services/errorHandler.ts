import { Notify } from 'quasar';
import axios from 'axios';
import type { ApiProblemDetails } from 'src/types/api';

const fallbackMessage = 'The request could not be completed. Please review the information and try again.';

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (!error.response) {
      if (error.code === 'ECONNABORTED') {
        return 'The backend API did not respond in time. Please try again.';
      }

      return 'Cannot reach the backend API. Check that the backend is running and try again.';
    }

    const status = error.response.status;
    const problem = getProblemDetails(error.response.data);

    if (problem?.errors) {
      const validationMessages = Object.values(problem.errors).flat().filter(Boolean);
      if (validationMessages.length > 0) {
        return validationMessages.join(' ');
      }
    }

    if (problem?.detail || problem?.title) {
      return problem.detail || problem.title || fallbackMessage;
    }

    if (status === 400) {
      return 'Some information is missing or invalid. Please review the form and try again.';
    }

    if (status === 403) {
      return 'The request was rejected by the backend. Please check the configuration and try again.';
    }

    if (status === 404) {
      return 'The requested record was not found. It may have been removed or is no longer available.';
    }

    if (status === 409) {
      return 'The request conflicts with the current data. Refresh the page and try again.';
    }

    if (status >= 500) {
      return 'The server could not complete the request. Please try again or check the backend logs.';
    }

    return fallbackMessage;
  }

  return error instanceof Error ? error.message : 'An unexpected error occurred.';
}

export function notifyApiError(error: unknown): void {
  Notify.create({
    type: 'negative',
    message: getApiErrorMessage(error),
    timeout: 7000,
    closeBtn: true,
    multiLine: true,
  });
}

function getProblemDetails(data: unknown): ApiProblemDetails | null {
  if (!data || typeof data !== 'object' || data instanceof Blob) {
    return null;
  }

  return data as ApiProblemDetails;
}
