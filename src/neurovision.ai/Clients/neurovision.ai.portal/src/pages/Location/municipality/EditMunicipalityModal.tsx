import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    MunicipalityForm,
    MunicipalityResponse,
} from "../../../features/location/municipality/municipality.types";


interface Props {
    isOpen: boolean;
    item: MunicipalityResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: MunicipalityForm) => Promise<void>;
}


const emptyForm: MunicipalityForm = {
    countryCode: "",
    code: 0,
    name: "",
    seatSettlementCode: undefined,
};


export default function EditMunicipalityModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<MunicipalityForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            countryCode: item.countryCode,
            code: item.code,
            name: item.name,
            seatSettlementCode: item.seatSettlementCode ?? undefined,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.countryCode.toString().trim().length > 0 &&
        form.code !== undefined && form.code !== null &&
        form.name.toString().trim().length > 0;

    const handleChange = <
        K extends keyof MunicipalityForm
    >(
        key: K,
        value: MunicipalityForm[K]
    ) => {
        setForm((previous) => ({
            ...previous,
            [key]: value,
        }));
    };

    const handleSubmit = async () => {
        if (!isValid) return;
        await onSave(form);
        onClose();
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-2xl">
            <div className="bg-white dark:bg-gray-900 rounded-2xl p-6">
                <h2 className="text-xl font-semibold">
                    {t("location.municipalities.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.municipalities.fields.countryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.countryCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "countryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.municipalities.fields.code")}
                        </Label>
                        <Input
                            type="number"
                            value={form.code ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "code",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.municipalities.fields.name")}
                        </Label>
                        <Input
                            type="text"
                            value={form.name}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "name",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.municipalities.fields.seatSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.seatSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "seatSettlementCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3">
                    <Button variant="outline" onClick={onClose} disabled={loading}>
                        {t("common.cancel")}
                    </Button>
                    <Button onClick={handleSubmit} disabled={loading || !isValid}>
                        {loading ? t("common.saving") : t("common.saveChanges")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}
