import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    RegionForm,
    RegionResponse,
} from "../../../features/location/region/region.types";


interface Props {
    isOpen: boolean;
    item: RegionResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: RegionForm) => Promise<void>;
}


const emptyForm: RegionForm = {
    typeCode: "",
    code: 0,
    name: "",
    belongsToCountryCode: "",
    headquartersCountryCode: "",
    administrativeSeatSettlementCode: undefined,
};


export default function EditRegionModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<RegionForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            typeCode: item.typeCode,
            code: item.code,
            name: item.name,
            belongsToCountryCode: item.belongsToCountryCode ?? undefined,
            headquartersCountryCode: item.headquartersCountryCode ?? undefined,
            administrativeSeatSettlementCode: item.administrativeSeatSettlementCode ?? undefined,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.typeCode.toString().trim().length > 0 &&
        form.code !== undefined && form.code !== null &&
        form.name.toString().trim().length > 0;

    const handleChange = <
        K extends keyof RegionForm
    >(
        key: K,
        value: RegionForm[K]
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
                    {t("location.regions.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.regions.fields.typeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.typeCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "typeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.code")}
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
                            {t("location.regions.fields.name")}
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
                            {t("location.regions.fields.belongsToCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.belongsToCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "belongsToCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.headquartersCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.headquartersCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "headquartersCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.administrativeSeatSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.administrativeSeatSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "administrativeSeatSettlementCode",
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
