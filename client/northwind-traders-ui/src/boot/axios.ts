import { boot } from 'quasar/wrappers';
import { apiClient } from 'src/services/apiClient';

export default boot(({ app }) => {
  app.config.globalProperties.$api = apiClient;
});
