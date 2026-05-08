import axios from 'axios';

export const apiClient = axios.create({
  baseURL: process.env.API_BASE_URL || 'https://localhost:7001',
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
});
