import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import ComponentCard from "../../../components/common/ComponentCard";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import PageMeta from "../../../components/common/PageMeta";

import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";

import {
    createNewSettlement,
} from "../../../features/location/settlement/settlement.slice";

import { showAlert } from "../../../features/ui/uiSlice";
import { useAppDispatch } from "../../../store/store";
import {
    useEffect
} from "react";



import Select from "../../../components/form/Select";

import {
    useAppSelector
} from "../../../store/store";
import { selectCountries, fetchCountries } from "../../../features/location/country/country.slice";
import CustomSelect from "../../../components/form/CustomSelect";
export default function CreateSettlementPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();
    const countries = useAppSelector(selectCountries);

    const [loading, setLoading] = useState(false);

    const [form, setForm] = useState({
        countryCode: "",
        code: "",
        name: "",
        postalCode: "",
    });

    const handleChange = (
        field: string,
        value: string
    ) => {
        setForm(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const isValid =
        form.countryCode.trim() !== "" &&
        form.code.trim() !== "" &&
        form.name.trim() !== "";

    useEffect(() => {

        dispatch(
            fetchCountries({
                pageIndex: 0,
                pageSize: 250
            })
        );

    }, [dispatch]);


    const handleSubmit = async () => {

        if (!isValid) {

            dispatch(
                showAlert({
                    type: "error",
                    message: t(
                        "common.messages.requiredField"
                    ),
                })
            );

            return;
        }

        try {

            setLoading(true);

            await dispatch(
                createNewSettlement({
                    countryCode: form.countryCode.trim(),
                    code: Number(form.code),
                    name: form.name.trim(),
                    postalCode:
                        form.postalCode.trim() || null,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.settlements.messages.createSuccess"
                    ),
                })
            );

            navigate("/admin/location/settlements");

        } catch (error: any) {

            dispatch(
                showAlert({
                    type: "error",
                    message:
                        error?.message ??
                        t(
                            "location.settlements.messages.createError"
                        ),
                })
            );

        } finally {

            setLoading(false);

        }
    };

    const countryOptions = countries.map(country => ({
        value: country.code,
        label: `${country.name} (${country.code})`
    }));

    return (
        <>
            <PageMeta
                title={`${t("location.settlements.createTitle")} | NeuroVision.AI`}
                description={t("location.settlements.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("location.settlements.createTitle")}
            />

            <div className="max-w-3xl mx-auto">

                <ComponentCard
                    title={t("location.settlements.createTitle")}
                >

                    <div className="space-y-5">

                        <div>
                            <Label>
                                {t("location.settlements.fields.country")}
                                {" *"}
                            </Label>

                            <CustomSelect

                                options={countryOptions}

                                value={form.countryCode}

                                placeholder={t(
                                    "location.settlements.fields.country"
                                )}

                                onChange={
                                    value =>
                                        handleChange(
                                            "countryCode",
                                            value
                                        )
                                }

                            />
                        </div>

                        <div>
                            <Label>
                                {t("location.settlements.fields.code")}
                                {" *"}
                            </Label>

                            <Input
                                type="number"
                                value={form.code}
                                placeholder="1001"
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
                                {t("location.settlements.fields.name")}
                                {" *"}
                            </Label>

                            <Input
                                value={form.name}
                                placeholder="Sarajevo"
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
                                value={form.postalCode}
                                placeholder="71000"
                                onChange={(e) =>
                                    handleChange(
                                        "postalCode",
                                        e.target.value
                                    )
                                }
                            />
                        </div>

                        <div
                            className="
                            flex
                            justify-end
                            gap-3
                            mt-8
                            pt-5
                            border-t
                        "
                        >

                            <Button
                                variant="outline"
                                onClick={() =>
                                    navigate(
                                        "/admin/location/settlements"
                                    )
                                }
                            >
                                {t("common.cancel")}
                            </Button>

                            <Button
                                disabled={
                                    loading ||
                                    !isValid
                                }
                                onClick={handleSubmit}
                            >
                                {loading
                                    ? t("common.creating")
                                    : t(
                                        "location.settlements.createTitle"
                                    )}
                            </Button>

                        </div>

                    </div>

                </ComponentCard>

            </div>
        </>
    );
}


