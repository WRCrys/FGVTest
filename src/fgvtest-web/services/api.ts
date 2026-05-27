import axios from 'axios';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const mensagem =
      error.response?.data?.mensagem ??
      error.response?.data?.title ??
      error.response?.data?.detail ??
      'Ocorreu um erro inesperado.';
    return Promise.reject(new Error(mensagem));
  }
);

export default api;
