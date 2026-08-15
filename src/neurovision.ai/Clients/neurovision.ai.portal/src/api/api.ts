const BASE_URL = import.meta.env.VITE_API_URL;



const handleResponse = async (response: Response) => {

    if (response.status === 401) {

        localStorage.removeItem("token");

        throw new Error("Unauthorized");
    }



    if (!response.ok) {

        const errorText = await response.text();
        let message = errorText || "Request failed";

        try {
            const parsed = JSON.parse(errorText);
            message = parsed.detail || parsed.title || message;
        } catch {
            // plain text error
        }

        throw new Error(message);
    }



    if (response.status === 204) {
        return;
    }



    const contentType =
        response.headers.get("content-type");



    if (
        contentType &&
        contentType.includes("application/json")
    ) {

        return await response.json();

    }



    return await response.text();

};

export const fetchBlobUrl = async (url: string): Promise<string> => {
    const token = localStorage.getItem("token");

    const response = await fetch(`${BASE_URL}${url}`, {
        method: "GET",
        headers: {
            ...(token && { Authorization: `Bearer ${token}` }),
        },
    });

    if (!response.ok) {
        throw new Error("Failed to load image");
    }

    const blob = await response.blob();
    return URL.createObjectURL(blob);
};

export const getAnalysisImagePath = (
    analysisId: string,
    kind: "scan" | "annotated" | "detection" | "segmentation" | "mask"
) => `/tumor/analyses/${analysisId}/images/${kind}`;






const getHeaders = () => {

    const token =
        localStorage.getItem("token");


    return {

        "Content-Type": "application/json",

        ...(token && {
            Authorization: `Bearer ${token}`
        }),

    };

};





const getFormHeaders = () => {

    const token =
        localStorage.getItem("token");


    return {

        ...(token && {
            Authorization: `Bearer ${token}`
        }),

    };

};









export const get = async (
    url: string
) => {


    const response = await fetch(
        `${BASE_URL}${url}`,
        {
            method: "GET",
            headers: getHeaders(),
        }
    );


    return handleResponse(response);

};









export const post = async (
    url: string,
    data: any
) => {


    const isFormData =
        data instanceof FormData;



    const response = await fetch(
        `${BASE_URL}${url}`,
        {

            method: "POST",


            headers:
                isFormData
                    ?
                    getFormHeaders()
                    :
                    getHeaders(),


            body:
                isFormData
                    ?
                    data
                    :
                    JSON.stringify(data),

        }
    );



    return handleResponse(response);

};









export const put = async (
    url: string,
    data: any
) => {


    const isFormData =
        data instanceof FormData;



    const response = await fetch(
        `${BASE_URL}${url}`,
        {


            method: "PUT",



            headers:
                isFormData
                    ?
                    getFormHeaders()
                    :
                    getHeaders(),



            body:
                isFormData
                    ?
                    data
                    :
                    JSON.stringify(data),


        }
    );



    return handleResponse(response);

};









export const del = async (
    url: string
) => {


    const response = await fetch(
        `${BASE_URL}${url}`,
        {

            method: "DELETE",

            headers: getHeaders(),

        }
    );



    return handleResponse(response);

};