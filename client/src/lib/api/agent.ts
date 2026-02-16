import axios from 'axios';
import { store } from '../stores/store';
import { toast } from 'react-toastify';
import { router } from '../../app/router/Routes';

// This file will concentrate all the axios functionallity so it can be used all over the application
// We will create an axios instance with the base URL of our API and we will use interceptors to switch the isLoading state in the store before and after the request is fired. This way we can show a loading spinner in the UI when the request is being processed and hide it when the response is received.
const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true, // This is to allow sending cookies with the request. We need this because we are using cookie-based authentication in our API, and we want to include the authentication cookie in the requests from the client to the API. By setting withCredentials to true, we can ensure that the cookies are included in the requests, allowing us to maintain the user's authenticated session and access protected resources in the API.
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

// The response interceptor is used to switch the isLoading state to false after the response is received, regardless of whether it is a success or an error. This way we can ensure that the loading spinner is hidden when the response is received, even if there is an error. The use funnction gets two parameters - the first one is the response object that we get when the request is successful, and the second one is the error object that we get when the request fails. We can use these parameters to switch the isLoading state to false in both cases.
agent.interceptors.response.use(
  async (response) => {
    if (import.meta.env.DEV) await sleep(1000);
    // After getting the response - switching the isLoading state to false regardless success or error
    store.uiStore.isIdle();
    return response;
  },
  async (error) => {
    if (import.meta.env.DEV) await sleep(1000);
    // After getting the response - switching the isLoading state to false regardless success or error
    store.uiStore.isIdle();
    const { status, data } = error.response;
    switch (status) {
      case 400:
        if (data.errors) {
          const modelStateErrors = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              modelStateErrors.push(data.errors[key]);
            }
          }
          throw modelStateErrors.flat();
        } else {
          toast.error(data);
        }
        break;
      case 401:
        toast.error('Unauthorized');
        break;
      case 404:
        router.navigate('/not-found');
        break;
      case 500:
        // We can pass as a second parameter to the navigate function an object with the state property that contains the error data. This way we can access the error data in the ServerError component and show it in the UI.
        router.navigate('/server-error', { state: { error: data } });
        break;
      default:
        break;
    }
    // Rethrowing the error to React Query so we can catch it in the components and show it in the UI
    return Promise.reject(error);
  },
);

export default agent;
