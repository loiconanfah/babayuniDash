// frontend/src/main.ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

// 🔥 très important : on charge Tailwind + nos styles
import './assets/main.css'

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.mount('#app')
