import axios from "axios";

const api = axios.create({
    baseURL: window.location.origin + "/api/",
    headers: {
        'Content-Type': 'application/json',
        // 'Access-Control-Allow-Origin': true
    }
});

export { api };

// export const GetAuthorizationHeader = () => {
//     return {
//       headers: {
//         Authorization: `Bearer ${localStorage.getItem('accesstoken') || ''}`
//       }
//     } as AxiosRequestConfig;
// }
