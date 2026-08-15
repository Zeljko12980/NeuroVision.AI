export const appendFile = (

    formData: FormData,

    key: string,

    file?: File | null

) => {


    if (file) {
        formData.append(
            key,
            file
        );
    }

};