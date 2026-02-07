import axios from "axios";

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
agent.interceptors.response.use(async (response) => {
  try {
    await sleep(1000);
    return response;
  } catch (error) {
    console.log(error);
    return Promise.reject(error);
  }
});

export default agent;
