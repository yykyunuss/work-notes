import axios from 'axios';
import { getCookie } from 'factor-shell';

const api = axios.create({
  baseURL: process.env.REACT_APP_GATEWAY_BFF + '/' + process.env.REACT_APP_API_NAME + '/fctr-cheque-bff',
  headers: {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Authorization': 'Bearer ' + getCookie('token'),
  },
});

api.interceptors.response.use(
  (res) => {
    if (res.data.error) {
      return Promise.reject(res);
    }
    return Promise.resolve(res);
  },
  (err) => {
    console.log(err);
    return Promise.reject(err);
  },
);

export default api;
