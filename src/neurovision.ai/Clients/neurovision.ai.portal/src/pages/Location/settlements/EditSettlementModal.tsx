import { useEffect, useState } from "react";

import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import { SettlementForm } from "./SettlementsTable";

export interface SettlementItem {
    countryCode: string;
    code: number;
    name: string;
    postalCode?: string | null;
}


interface Props {
    isOpen: boolean;
    item?: SettlementItem | null;
    loading?: boolean;
    onClose: () => void;
    onSave: (data: SettlementForm) => Promise<void>;
}

export default function EditSettlementModal({
    isOpen,
    item,
    loading = false,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<SettlementForm>({
        countryCode: "",
        code: 0,
        name: "",
        postalCode: "",
    });

    useEffect(() => {
        if (item) {
            setForm({
                countryCode: item.countryCode,
                code: item.code,
                name: item.name,
                postalCode: item.postalCode ?? "",
            });
        } else {
            setForm({
                countryCode: "",
                code: 0,
                name: "",
                postalCode: "",
            });
        }
    }, [item]);

    if (!isOpen) return null;

    const handleChange = (
        field: keyof SettlementForm,
        value: string | number | undefined
    ) => {
        setForm((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const handleSave = async () => {
        await onSave(form);
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            className="max-w-lg"
        >
            <div className="bg-white dark:bg-gray-900 rounded-2xl p-6">

                <h2 className="text-xl font-semibold">
                    {item
                        ? t("location.settlements.editTitle")
                        : t("location.settlements.createTitle")}
                </h2>

                <div className="mt-4 space-y-4">

                    <div>
                        <Label>
                            {t("location.settlements.fields.countryCode")}
                        </Label>

                        <Input
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
                            {t("location.settlements.fields.code")}
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
                            {t("location.settlements.fields.name")}
                        </Label>

                        <Input
                            value={form.name}
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
                            {t("location.settlements.fields.postalCode")}
                        </Label>

                        <Input
                            value={form.postalCode ?? ""}
                            onChange={(e) =>
                                handleChange(
                                    "postalCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                </div>

                <div className="mt-6 flex justify-end gap-3">

                    <Button
                        variant="ghost"
                        onClick={onClose}
                    >
                        {t("common.cancel")}
                    </Button>

                    <Button
                        onClick={handleSave}
                        disabled={loading}
                    >
                        {loading
                            ? "Saving..."
                            : t("common.actions.save")}
                    </Button>

                </div>

            </div>
        </Modal>
    );
}