import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    CountryCompositionForm,
    CountryCompositionResponse,
} from "../../../features/location/countryComposition/countryComposition.types";


interface Props {
    isOpen: boolean;
    item: CountryCompositionResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: CountryCompositionForm) => Promise<void>;
}


const emptyForm: CountryCompositionForm = {
    unionCountryCode: "",
    memberCountryCode: "",
    sequenceNumber: 0,
    from: "",
    to: "",
};


export default function EditCountryCompositionModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<CountryCompositionForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            unionCountryCode: item.unionCountryCode,
            memberCountryCode: item.memberCountryCode,
            sequenceNumber: item.sequenceNumber,
            from:
                item.from
                    ? item.from.split("T")[0]
                    : "",
            to:
                item.to
                    ? item.to.split("T")[0]
                    : undefined,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.unionCountryCode.toString().trim().length > 0 &&
        form.memberCountryCode.toString().trim().length > 0 &&
        form.sequenceNumber !== undefined && form.sequenceNumber !== null &&
        form.from.toString().trim().length > 0;

    const handleChange = <
        K extends keyof CountryCompositionForm
    >(
        key: K,
        value: CountryCompositionForm[K]
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
                    {t("location.countryCompositions.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.unionCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.unionCountryCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "unionCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.memberCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.memberCountryCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "memberCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.sequenceNumber")}
                        </Label>
                        <Input
                            type="number"
                            value={form.sequenceNumber ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "sequenceNumber",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.from")}
                        </Label>
                        <Input
                            type="date"
                            value={form.from ? form.from.split("T")[0] : ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "from",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.to")}
                        </Label>
                        <Input
                            type="date"
                            value={form.to ? form.to.split("T")[0] : ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "to",
                                    e.target.value
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
