const BASE_URL = import.meta.env.VITE_API_URL

const handleResponse = async (response: Response) => {
  
    if (response.status === 401) {
        localStorage.removeItem("token");

        throw new Error("Unauthorized");
    }

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || "Request failed");
    }

    return response.json();
};


const getHeaders = () => {
    const token = localStorage.getItem("token");

    return {
        "Content-Type": "application/json",
        ...(token && { Authorization: `Bearer ${token}` }),
    };
};


export const get = async (url: string) => {
    const response = await fetch(`${BASE_URL}${url}`, {
        method: "GET",
        headers: getHeaders(),
    });

    return handleResponse(response);
};


export const post = async (url: string, data: any) => {
    const response = await fetch(`${BASE_URL}${url}`, {
        method: "POST",
        headers: getHeaders(),
        body: JSON.stringify(data),
    });

    return handleResponse(response);
};


export const put = async (url: string, data: any) => {
    const response = await fetch(`${BASE_URL}${url}`, {
        method: "PUT",
        headers: getHeaders(),
        body: JSON.stringify(data),
    });

    return handleResponse(response);
};


export const del = async (url: string) => {
    const response = await fetch(`${BASE_URL}${url}`, {
        method: "DELETE",
        headers: getHeaders(),
    });

    return handleResponse(response);
};