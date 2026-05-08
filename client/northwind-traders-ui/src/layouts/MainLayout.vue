<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="app-header">
      <q-toolbar>
        <q-btn flat dense round icon="menu" @click="drawerOpen = !drawerOpen" />
        <q-toolbar-title>Northwind Traders</q-toolbar-title>
        <q-badge outline color="white">Order Management</q-badge>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="drawerOpen" show-if-above bordered :width="260">
      <q-list padding>
        <q-item-label header>Operations</q-item-label>
        <q-item clickable :to="{ name: 'orders' }" exact>
          <q-item-section avatar>
            <q-icon name="receipt_long" />
          </q-item-section>
          <q-item-section>Orders</q-item-section>
        </q-item>
        <q-item clickable :to="{ name: 'order-create' }">
          <q-item-section avatar>
            <q-icon name="add_circle" />
          </q-item-section>
          <q-item-section>Create Order</q-item-section>
        </q-item>
        <q-separator spaced />
        <q-item clickable :to="{ name: 'reports' }">
          <q-item-section avatar>
            <q-icon name="analytics" />
          </q-item-section>
          <q-item-section>Reports</q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const drawerOpen = ref(true);
</script>

<style scoped>
.app-header {
  background: #20364f;
}
</style>
