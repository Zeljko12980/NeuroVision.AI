import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import {
    LocalCommunityCoverageForm,
    LocalCommunityCoverageResponse,
} from "../../../features/location/localCommunityCoverage/localCommunityCoverage.types";


interface Props {
    isOpen: boolean;
    item: LocalCommunityCoverageResponse | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: LocalCommunityCoverageForm) => Promise<void>;
}


const emptyForm: LocalCommunityCoverageForm = {
    countryCode: "",
    municipalityCode: 0,
    localCommunityIdentifier: 0,
    settlementCode: 0,
};


export default function EditLocalCommunityCoverageModal({
    isOpen,
    item,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<LocalCommunityCoverageForm>(emptyForm);

    useEffect(() => {
        if (!item) {
            setForm(emptyForm);
            return;
        }

        setForm({
            countryCode: item.countryCode,
            municipalityCode: item.municipalityCode,
            localCommunityIdentifier: item.localCommunityIdentifier,
            settlementCode: item.settlementCode,
        });
    }, [item]);

    if (!isOpen) return null;

    const isValid =
        form.countryCode.toString().trim().length > 0 &&
        form.municipalityCode !== undefined && form.municipalityCode !== null &&
        form.localCommunityIdentifier !== undefined && form.localCommunityIdentifier !== null &&
        form.settlementCode !== undefined && form.settlementCode !== null;

    const handleChange = <
        K extends keyof LocalCommunityCoverageForm
    >(
        key: K,
        value: LocalCommunityCoverageForm[K]
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
                    {t("location.localCommunityCoverages.editTitle")}
                </h2>

                <div className="mt-4 grid grid-cols-2 gap-4">

                    <div>
                        <Label>
                            {t("location.localCommunityCoverages.fields.countryCode")}
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
                            {t("location.localCommunityCoverages.fields.municipalityCode")}
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
                            {t("location.localCommunityCoverages.fields.localCommunityIdentifier")}
                        </Label>
                        <Input
                            type="number"
                            value={form.localCommunityIdentifier ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "localCommunityIdentifier",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : 0
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.localCommunityCoverages.fields.settlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.settlementCode ?? ""}
                            disabled={!!item}
                            onChange={(e) =>
                                handleChange(
                                    "settlementCode",
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
