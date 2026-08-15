import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    LocalCommunityForm,
    LocalCommunityResponse,
} from "../../../features/location/localCommunity/localCommunity.types";


interface Props {
    isOpen: boolean;
    item: LocalCommunityResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: LocalCommunityForm) => Promise<void>;
}


const emptyForm: LocalCommunityForm = {
    countryCode: "",
    municipalityCode: 0,
    identifier: 0,
    name: "",
    officeSettlementCode: undefined,
};


export default function EditLocalCommunityModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<LocalCommunityForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            countryCode: item.countryCode,
            municipalityCode: item.municipalityCode,
            identifier: item.identifier,
            name: item.name,
            officeSettlementCode: item.officeSettlementCode ?? undefined,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.countryCode.toString().trim().length > 0 &&
        form.municipalityCode !== undefined && form.municipalityCode !== null &&
        form.identifier !== undefined && form.identifier !== null &&
        form.name.toString().trim().length > 0;

    const handleChange = <
        K extends keyof LocalCommunityForm
    >(
        key: K,
        value: LocalCommunityForm[K]
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
                    {t("location.localCommunities.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.localCommunities.fields.countryCode")}
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
                            {t("location.localCommunities.fields.municipalityCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.municipalityCode ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "municipalityCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.localCommunities.fields.identifier")}
                        </Label>
                        <Input
                            type="number"
                            value={form.identifier ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "identifier",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.localCommunities.fields.name")}
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
                            {t("location.localCommunities.fields.officeSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.officeSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "officeSettlementCode",
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
