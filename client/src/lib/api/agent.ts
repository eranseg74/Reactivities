import axios from "axios";
import { store } from "../stores/store";

// This file will concentrate all the axios functionallity so it can be used all over the application
const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

// Interceptors allow us to do something with the request before it goes to the server, and with the response before it reaches the client

// The great thing about MobX is that it is not only a react related package. We can use it also here in a regular JS or TS file to manage and update the properties in the store and any react component that observes these properties will re-render when they are changed.
agent.interceptors.request.use((config) => {
  // (parameter) config: InternalAxiosRequestConfig<any>. When the request is fired - switching the isLoading state to true
  store.uiStore.isBusy();
  return config;
});

agent.interceptors.response.use(async (response) => {
  try {
    await sleep(1000);
    return response;
  } catch (error) {
    console.log(error);
    return Promise.reject(error);
  } finally {
    // After getting the response - switching the isLoading state to false regardless success or error
    store.uiStore.isIdle();
  }
});

export default agent;
