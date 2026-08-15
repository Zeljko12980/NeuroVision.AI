import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    RegionCompositionForm,
    RegionCompositionResponse,
} from "../../../features/location/regionComposition/regionComposition.types";


interface Props {
    isOpen: boolean;
    item: RegionCompositionResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: RegionCompositionForm) => Promise<void>;
}


const emptyForm: RegionCompositionForm = {
    parentRegionTypeCode: "",
    parentRegionCode: 0,
    memberRegionTypeCode: "",
    memberRegionCode: 0,
};


export default function EditRegionCompositionModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<RegionCompositionForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            parentRegionTypeCode: item.parentRegionTypeCode,
            parentRegionCode: item.parentRegionCode,
            memberRegionTypeCode: item.memberRegionTypeCode,
            memberRegionCode: item.memberRegionCode,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.parentRegionTypeCode.toString().trim().length > 0 &&
        form.parentRegionCode !== undefined && form.parentRegionCode !== null &&
        form.memberRegionTypeCode.toString().trim().length > 0 &&
        form.memberRegionCode !== undefined && form.memberRegionCode !== null;

    const handleChange = <
        K extends keyof RegionCompositionForm
    >(
        key: K,
        value: RegionCompositionForm[K]
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
                    {t("location.regionCompositions.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.parentRegionTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.parentRegionTypeCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "parentRegionTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.parentRegionCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.parentRegionCode ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "parentRegionCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.memberRegionTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.memberRegionTypeCode}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "memberRegionTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.memberRegionCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.memberRegionCode ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "memberRegionCode",
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
