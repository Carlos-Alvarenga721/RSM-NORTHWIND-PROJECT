/// <reference types="node" />

declare module '*.vue' {
  import type { DefineComponent } from 'vue';

  const component: DefineComponent<object, object, unknown>;
  export default component;
}

declare namespace NodeJS {
  interface ProcessEnv {
    API_BASE_URL: string;
    VUE_ROUTER_MODE: 'hash' | 'history';
    VUE_ROUTER_BASE: string;
    SERVER?: string;
  }
}
