import { post } from "../../api/api";

export interface WorkingSlotDto {
    start: string; 
    end: string;
}

export interface CreateDoctorRequest {
    firstName: string;
    lastName: string;
    licenseNumber: string;
    specialization: string;
    email: string;
    phoneNumber: string;
    languages: string;
    bio?: string;
    degrees?: string;
    hospital?: string;
    isAvailable: boolean;
    autoActivate: boolean;

    picture?: File;
}

export const createDoctorRequest = async (
    data: CreateDoctorRequest
): Promise<{ id: string }> => {
    const formData = new FormData();


    formData.append("FirstName", data.firstName);
    formData.append("LastName", data.lastName);
    formData.append("LicenseNumber", data.licenseNumber);
    formData.append("Specialization", data.specialization);

    formData.append("Email", data.email);
    formData.append("PhoneNumber", data.phoneNumber);
    formData.append("Languages", data.languages);

    formData.append("Bio", data.bio ?? "");
    formData.append("Degrees", data.degrees ?? "");
    formData.append("Hospital", data.hospital ?? "");

    formData.append("IsAvailable", String(data.isAvailable));
    formData.append("AutoActivate", String(data.autoActivate));


    if (data.picture) {
        formData.append("Picture", data.picture);
    }


    return await post(`/doctor`, formData);
};