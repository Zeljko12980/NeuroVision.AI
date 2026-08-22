import { useEffect, useState } from "react";

import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import {
    getDoctorByEmail,
    getDoctorById,
    resolveDoctorImageUrl,
} from "../doctor/doctorService";
import {
    getPatientByEmail,
    getPatientById,
    resolvePatientImageUrl,
} from "./patientService";

export default function useLoggedInProfilePhoto() {
    const claims = useAppSelector(selectUserClaims);
    const { role, userId, email } = getUserInfoFromClaims(claims || {});
    const [src, setSrc] = useState<string | undefined>(undefined);

    useEffect(() => {
        const normalizedRole = role.toLowerCase();
        if (normalizedRole !== "doctor" && normalizedRole !== "patient") {
            setSrc(undefined);
            return;
        }

        let cancelled = false;

        const load = async () => {
            try {
                let pictureUrl: string | null = null;

                if (normalizedRole === "doctor") {
                    if (userId) {
                        try {
                            const doctor = await getDoctorById(userId);
                            pictureUrl = doctor.profilePictureUrl;
                        } catch {
                            pictureUrl = null;
                        }
                    }

                    if (!pictureUrl && email) {
                        const doctor = await getDoctorByEmail(email);
                        pictureUrl = doctor?.profilePictureUrl ?? null;
                    }

                    if (!cancelled) setSrc(resolveDoctorImageUrl(pictureUrl));
                    return;
                }

                if (userId) {
                    try {
                        const patient = await getPatientById(userId);
                        pictureUrl = patient.profilePictureUrl;
                    } catch {
                        pictureUrl = null;
                    }
                }

                if (!pictureUrl && email) {
                    const patient = await getPatientByEmail(email);
                    pictureUrl = patient?.profilePictureUrl ?? null;
                }

                if (!cancelled) setSrc(resolvePatientImageUrl(pictureUrl));
            } catch {
                if (!cancelled) setSrc(undefined);
            }
        };

        void load();

        return () => {
            cancelled = true;
        };
    }, [role, userId, email]);

    return src;
}
