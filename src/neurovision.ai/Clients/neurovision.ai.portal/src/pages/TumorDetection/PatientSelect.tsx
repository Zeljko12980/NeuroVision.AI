import { useEffect } from "react";
import { useTranslation } from "react-i18next";

import { fetchPatients } from "../../features/patient/patientSlice";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { tumorSelectClass } from "./tumorUtils";

interface PatientSelectProps {
    value: string;
    onChange: (patientId: string) => void;
    allowAll?: boolean;
    disabled?: boolean;
}

export default function PatientSelect({
    value,
    onChange,
    allowAll = false,
    disabled = false,
}: PatientSelectProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const patients = useAppSelector((s) => s.patient.items);
    const loading = useAppSelector((s) => s.patient.loading);

    useEffect(() => {
        void dispatch(fetchPatients({ pageIndex: 0, pageSize: 200 }));
    }, [dispatch]);

    return (
        <select
            className={tumorSelectClass}
            value={value}
            disabled={disabled || loading}
            onChange={(e) => onChange(e.target.value)}
        >
            <option value="">
                {allowAll ? t("tumor.patient.all") : t("tumor.patient.select")}
            </option>
            {patients.map((patient) => (
                <option key={patient.id} value={patient.id}>
                    {patient.firstName} {patient.lastName}
                </option>
            ))}
        </select>
    );
}
