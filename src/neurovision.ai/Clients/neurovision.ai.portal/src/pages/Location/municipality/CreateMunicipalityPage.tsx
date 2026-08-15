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
    createNewMunicipality
} from "../../../features/location/municipality/municipality.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateMunicipalityPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        code: "",
        name: "",
        seatSettlementCode: "",

    });


    const handleChange = (
        field: string,
        value: string | number | undefined
    ) => {

        setForm(prev => ({
            ...prev,
            [field]: value ?? ""
        }));

    };


    const isValid =
        form.countryCode.trim() !== "" &&
        form.code.toString().trim() !== "" &&
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.municipalities.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewMunicipality({

                    countryCode: form.countryCode,
                    code: Number(form.code),
                    name: form.name,
                    seatSettlementCode: Number(form.seatSettlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.municipalities.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/municipalities"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.municipalities.messages.createError"),
                    type: "error"
                })
            );

        }
        finally {

            setLoading(false);

        }

    };



    return (
        <>

            <PageMeta
                title={`${t("location.municipalities.createTitle")} | NeuroVision.AI`}
                description={t("location.municipalities.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.municipalities.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.municipalities.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.municipalities.fields.countryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.countryCode}
                            disabled={false}
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
                            {t("location.municipalities.fields.code")}
                        </Label>
                        <Input
                            type="number"
                            value={form.code ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "code",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.municipalities.fields.name")}
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
                            {t("location.municipalities.fields.seatSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.seatSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "seatSettlementCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>
                    </div>



                    <div className="
                        flex
                        justify-end
                        gap-3
                        mt-8
                        pt-5
                        border-t
                    ">


                        <Button
                            variant="outline"
                            onClick={() =>
                                navigate(
                                    "/admin/location/municipalities"
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

                            {
                                loading
                                    ?
                                    t("common.creating")
                                    :
                                    t("location.municipalities.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
