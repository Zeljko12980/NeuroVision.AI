import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    GovernmentHistoryForm,
    GovernmentHistoryResponse,
} from "../../../features/location/governmentHistory/governmentHistory.types";


interface Props {
    isOpen: boolean;
    item: GovernmentHistoryResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: GovernmentHistoryForm) => Promise<void>;
}


const emptyForm: GovernmentHistoryForm = {
    countryCode: "",
    sequenceNumber: 0,
    governmentTypeCode: "",
    from: "",
    to: "",
};


export default function EditGovernmentHistoryModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<GovernmentHistoryForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            countryCode: item.countryCode,
            sequenceNumber: item.sequenceNumber,
            governmentTypeCode: item.governmentTypeCode,
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
        form.countryCode.toString().trim().length > 0 &&
        form.sequenceNumber !== undefined && form.sequenceNumber !== null &&
        form.governmentTypeCode.toString().trim().length > 0 &&
        form.from.toString().trim().length > 0;

    const handleChange = <
        K extends keyof GovernmentHistoryForm
    >(
        key: K,
        value: GovernmentHistoryForm[K]
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
                    {t("location.governmentHistories.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.countryCode")}
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
                            {t("location.governmentHistories.fields.sequenceNumber")}
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
                            {t("location.governmentHistories.fields.governmentTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.governmentTypeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "governmentTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.from")}
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
                            {t("location.governmentHistories.fields.to")}
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
