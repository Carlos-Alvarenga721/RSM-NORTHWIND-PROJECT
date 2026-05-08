import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', redirect: '/orders' },
      { path: 'orders', name: 'orders', component: () => import('pages/OrdersPage.vue') },
      { path: 'orders/new', name: 'order-create', component: () => import('pages/OrderFormPage.vue') },
      { path: 'orders/:orderId', name: 'order-detail', component: () => import('pages/OrderDetailPage.vue') },
      { path: 'orders/:orderId/edit', name: 'order-edit', component: () => import('pages/OrderFormPage.vue') },
      { path: 'reports', name: 'reports', component: () => import('pages/ReportsDashboardPage.vue') },
    ],
  },
  {
    path: '/:catchAll(.*)*',
    redirect: '/orders',
  },
];

export default routes;
