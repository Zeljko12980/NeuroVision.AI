import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    RegionTypeForm,
    RegionTypeResponse,
} from "../../../features/location/regionType/regionType.types";


interface Props {
    isOpen: boolean;
    item: RegionTypeResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: RegionTypeForm) => Promise<void>;
}


const emptyForm: RegionTypeForm = {
    code: "",
    name: "",
};


export default function EditRegionTypeModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<RegionTypeForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            code: item.code,
            name: item.name,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.code.toString().trim().length > 0 &&
        form.name.toString().trim().length > 0;

    const handleChange = <
        K extends keyof RegionTypeForm
    >(
        key: K,
        value: RegionTypeForm[K]
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
                    {t("location.regionTypes.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.regionTypes.fields.code")}
                        </Label>
                        <Input
                            type="text"
                            value={form.code}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "code",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionTypes.fields.name")}
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
