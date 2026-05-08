import { Notify } from 'quasar';
import { AxiosError } from 'axios';
import type { ApiProblemDetails } from 'src/types/api';

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const problem = error.response?.data as ApiProblemDetails | undefined;

    if (problem?.errors) {
      return Object.values(problem.errors).flat().join(' ');
    }

    return problem?.detail || problem?.title || error.message;
  }

  return error instanceof Error ? error.message : 'An unexpected error occurred.';
}

export function notifyApiError(error: unknown): void {
  Notify.create({
    type: 'negative',
    message: getApiErrorMessage(error),
  });
}
