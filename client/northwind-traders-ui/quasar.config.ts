import { configure } from 'quasar/wrappers';

export default configure((context) => ({
  boot: ['axios', 'pinia'],
  css: ['app.scss'],
  extras: ['material-icons'],
  build: {
    target: {
      browser: ['es2022', 'firefox115', 'chrome115', 'safari14'],
      node: 'node20',
    },
    vueRouterMode: 'history',
    env: {
      API_BASE_URL: context.dev ? 'http://localhost:5083' : '',
    },
  },
  devServer: {
    open: false,
    port: 9000,
  },
  framework: {
    config: {
      notify: {
        position: 'top-right',
        timeout: 3500,
      },
    },
    plugins: ['Dialog', 'Loading', 'Notify'],
  },
}));
