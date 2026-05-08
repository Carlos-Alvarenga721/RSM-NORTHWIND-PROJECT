import { Notify } from 'quasar';
import { AxiosError } from 'axios';
import type { ApiProblemDetails } from 'src/types/api';

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const problem = error.response?.data as ApiProblemDetails | undefined;

    if (problem?.errors) {
      return Object.values(problem.errors).flat().join(' ');
    }

    if (problem?.detail || problem?.title) {
      return problem.detail || problem.title || 'The request could not be completed.';
    }

    if (error.response?.status === 404) {
      return 'The requested feature or resource is not available.';
    }

    if (error.response?.status && error.response.status >= 500) {
      return 'The server could not complete the request. Please try again.';
    }

    return 'The request could not be completed. Please review the information and try again.';
  }

  return error instanceof Error ? error.message : 'An unexpected error occurred.';
}

export function notifyApiError(error: unknown): void {
  Notify.create({
    type: 'negative',
    message: getApiErrorMessage(error),
  });
}
