import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    LegalSuccessorForm,
    LegalSuccessorResponse,
} from "../../../features/location/legalSuccessor/legalSuccessor.types";


interface Props {
    isOpen: boolean;
    item: LegalSuccessorResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: LegalSuccessorForm) => Promise<void>;
}


const emptyForm: LegalSuccessorForm = {
    successorCountryCode: "",
    predecessorCountryCode: "",
};


export default function EditLegalSuccessorModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<LegalSuccessorForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            successorCountryCode: item.successorCountryCode,
            predecessorCountryCode: item.predecessorCountryCode,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.successorCountryCode.toString().trim().length > 0 &&
        form.predecessorCountryCode.toString().trim().length > 0;

    const handleChange = <
        K extends keyof LegalSuccessorForm
    >(
        key: K,
        value: LegalSuccessorForm[K]
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
                    {t("location.legalSuccessors.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.legalSuccessors.fields.successorCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.successorCountryCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "successorCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.legalSuccessors.fields.predecessorCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.predecessorCountryCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "predecessorCountryCode",
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
