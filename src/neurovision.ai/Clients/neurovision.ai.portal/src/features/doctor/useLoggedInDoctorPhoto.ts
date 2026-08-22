import { useEffect, useState } from "react";

import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import {
    getDoctorByEmail,
    getDoctorById,
    resolveDoctorImageUrl,
} from "./doctorService";

export default function useLoggedInDoctorPhoto() {
    const claims = useAppSelector(selectUserClaims);
    const { role, userId, email } = getUserInfoFromClaims(claims || {});
    const [src, setSrc] = useState<string | undefined>(undefined);

    useEffect(() => {
        if (role.toLowerCase() !== "doctor") {
            setSrc(undefined);
            return;
        }

        let cancelled = false;

        const load = async () => {
            try {
                let pictureUrl: string | null = null;

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

                if (!cancelled) {
                    setSrc(resolveDoctorImageUrl(pictureUrl));
                }
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
