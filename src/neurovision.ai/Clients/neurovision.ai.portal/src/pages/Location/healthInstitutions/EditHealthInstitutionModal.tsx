import { useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import CustomSelect from "../../../components/form/CustomSelect";
import SettlementSelect from "../../../components/form/SettlementSelect";

import {
    HealthInstitutionTypeResponse,
} from "../../../features/location/healthInstitutionsType/healthInstitutionType.type";

import {
    CountryResponse,
} from "../../../features/location/country/country.service";

import {
    SettlementResponse,
} from "../../../features/location/settlement/settlement.service";

export interface HealthInstitutionForm {
    name: string;
    typeCode: string;
    countryCode: string;
    settlementCode: number;
    address?: string;
    bedCount?: number;
    foundingDate?: string;
    phone?: string;
}

interface HealthInstitutionItem extends HealthInstitutionForm {
    id: number;
}

interface Props {
    isOpen: boolean;
    healthInstitution: HealthInstitutionItem | null;

    loading: boolean;

    healthInstitutionTypes: HealthInstitutionTypeResponse[];
    countries: CountryResponse[];
    settlements: SettlementResponse[];

    onClose: () => void;
    onSave: (form: HealthInstitutionForm) => Promise<void>;
}

type Tab =
    | "general"
    | "location"
    | "contact";

const emptyForm: HealthInstitutionForm = {
    name: "",
    typeCode: "",
    countryCode: "",
    settlementCode: 0,
    address: "",
    bedCount: undefined,
    foundingDate: "",
    phone: "",
};

export default function EditHealthInstitutionModal({
    isOpen,
    healthInstitution,
    loading,
    healthInstitutionTypes,
    countries,
    settlements,
    onClose,
    onSave,
}: Props) {

    const { t } = useTranslation();

    const [activeTab, setActiveTab] =
        useState<Tab>("general");

    const [form, setForm] =
        useState<HealthInstitutionForm>(emptyForm);

    const typeOptions = useMemo(
        () =>
            healthInstitutionTypes.map((x) => ({
                value: x.code,
                label: `${x.name} (${x.code})`,
            })),
        [healthInstitutionTypes]
    );

    const countryOptions = useMemo(
        () =>
            countries.map((x) => ({
                value: x.code,
                label: `${x.name} (${x.code})`,
            })),
        [countries]
    );

    const filteredSettlements = useMemo(() => {
        if (!form.countryCode) return settlements;

        return settlements.filter(
            (x: any) => x.countryCode === form.countryCode
        );
    }, [settlements, form.countryCode]);


    useEffect(() => {
        if (!isOpen) return;

        console.log(settlements);

        setActiveTab("general");
    }, [isOpen]);

    useEffect(() => {
        if (healthInstitution) {
            setForm({
                name: healthInstitution.name,
                typeCode: healthInstitution.typeCode,
                countryCode: healthInstitution.countryCode,
                settlementCode: healthInstitution.settlementCode,
                address: healthInstitution.address ?? "",
                bedCount: healthInstitution.bedCount,
                foundingDate: healthInstitution.foundingDate?.split("T")[0] ?? "",
                phone: healthInstitution.phone ?? "",
            });
        } else {
            setForm(emptyForm);
        }
    }, [healthInstitution, isOpen]);

    if (!isOpen) {
        return null;
    }

    const handleChange = (
        field: keyof HealthInstitutionForm,
        value: string | number | undefined
    ) => {
        setForm((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const isValid =
        form.name.trim().length > 0 &&
        form.typeCode.trim().length > 0 &&
        form.countryCode.trim().length > 0 &&
        form.settlementCode > 0;

    const handleSubmit = async () => {
        if (!isValid) return;

        try {
            await onSave(form);
        } catch {
            // Parent prikazuje grešku.
        }
    };

    const handleCountryChange = (countryCode: string) => {
        setForm((prev) => ({
            ...prev,
            countryCode,
            settlementCode: 0,
        }));
    };

    const handleSettlementChange = (
        _: string,
        settlement: SettlementResponse
    ) => {
        setForm((prev) => ({
            ...prev,
            settlementCode: settlement.code,
        }));
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            className="max-w-lg"
        >
            <div className="bg-white dark:bg-gray-900 rounded-2xl p-6">
                <h2 className="text-xl font-semibold">
                    {healthInstitution
                        ? t("location.healthInstitutions.editTitle")
                        : t("location.healthInstitutions.createTitle")}
                </h2>

                <div className="flex gap-8 border-b mt-4 mb-6">
                    {[
                        {
                            key: "general",
                            label: t("location.healthInstitutions.tabs.general"),
                        },
                        {
                            key: "location",
                            label: t("location.healthInstitutions.tabs.location"),
                        },
                        {
                            key: "contact",
                            label: t("location.healthInstitutions.tabs.contact"),
                        },
                    ].map((tab) => (
                        <button
                            key={tab.key}
                            type="button"
                            onClick={() => setActiveTab(tab.key as Tab)}
                            className={`pb-3 text-sm font-medium ${activeTab === tab.key
                                    ? "border-b-2 border-blue-600 text-blue-600"
                                    : "text-gray-500"
                                }`}
                        >
                            {tab.label}
                        </button>
                    ))}
                </div>

                <div className="h-[350px] overflow-y-auto pr-1">
                    {activeTab === "general" && (
                        <div className="space-y-4">
                            <div>
                                <Label>
                                    {t("location.healthInstitutions.fields.name")}
                                </Label>

                                <Input
                                    value={form.name}
                                    onChange={(e) =>
                                        handleChange("name", e.target.value)
                                    }
                                />
                            </div>

                            <div>
                                <Label>
                                    {t("location.healthInstitutions.fields.typeCode")}
                                </Label>

                                <CustomSelect
                                    options={typeOptions}
                                    value={form.typeCode}
                                    placeholder={t(
                                        "location.healthInstitutions.fields.typeCode"
                                    )}
                                    onChange={(value) =>
                                        handleChange("typeCode", value)
                                    }
                                />
                            </div>

                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.foundingDate"
                                    )}
                                </Label>

                                <Input
                                    type="date"
                                    value={form.foundingDate}
                                    onChange={(e) =>
                                        handleChange(
                                            "foundingDate",
                                            e.target.value
                                        )
                                    }
                                />
                            </div>

                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.bedCount"
                                    )}
                                </Label>

                                <Input
                                    type="number"
                                    value={form.bedCount ?? ""}
                                    onChange={(e) =>
                                        handleChange(
                                            "bedCount",
                                            e.target.value === ""
                                                ? undefined
                                                : Number(e.target.value)
                                        )
                                    }
                                />
                            </div>
                        </div>
                    )}

                    {activeTab === "location" && (
                        <div className="space-y-4">
                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.countryCode"
                                    )}
                                </Label>

                                <CustomSelect
                                    options={countryOptions}
                                    value={form.countryCode}
                                    placeholder={t(
                                        "location.healthInstitutions.fields.countryCode"
                                    )}
                                    onChange={handleCountryChange}
                                />
                            </div>

                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.settlementCode"
                                    )}
                                </Label>

                                <SettlementSelect
                                    settlements={filteredSettlements}
                                    value={
                                        form.settlementCode
                                            ? {
                                                countryCode: form.countryCode,
                                                code: form.settlementCode,
                                            }
                                            : null
                                    }
                                    placeholder={t(
                                        "location.healthInstitutions.fields.settlementCode"
                                    )}
                                    onChange={handleSettlementChange}
                                />
                            </div>

                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.address"
                                    )}
                                </Label>

                                <Input
                                    value={form.address ?? ""}
                                    onChange={(e) =>
                                        handleChange(
                                            "address",
                                            e.target.value
                                        )
                                    }
                                />
                            </div>
                        </div>
                    )}

                    {activeTab === "contact" && (
                        <div className="space-y-4">
                            <div>
                                <Label>
                                    {t(
                                        "location.healthInstitutions.fields.phone"
                                    )}
                                </Label>

                                <Input
                                    value={form.phone ?? ""}
                                    onChange={(e) =>
                                        handleChange(
                                            "phone",
                                            e.target.value
                                        )
                                    }
                                />
                            </div>
                        </div>
                    )}
                </div>

                <div className="mt-6 flex justify-end gap-3 pt-5 border-t">
                    <Button
                        variant="ghost"
                        onClick={onClose}
                    >
                        {t("common.cancel")}
                    </Button>

                    <Button
                        onClick={handleSubmit}
                        disabled={loading || !isValid}
                    >
                        {loading
                            ? t("common.saving")
                            : t("common.save")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}