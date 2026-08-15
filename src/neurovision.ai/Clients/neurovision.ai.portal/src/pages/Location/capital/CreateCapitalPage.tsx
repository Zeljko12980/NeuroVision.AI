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
    createNewCapital
} from "../../../features/location/capital/capital.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateCapitalPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        settlementCode: "",

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
        form.settlementCode.toString().trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.capitals.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewCapital({

                    countryCode: form.countryCode,
                    settlementCode: Number(form.settlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.capitals.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/capitals"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.capitals.messages.createError"),
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
                title={`${t("location.capitals.createTitle")} | NeuroVision.AI`}
                description={t("location.capitals.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.capitals.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.capitals.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.capitals.fields.countryCode")}
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
                            {t("location.capitals.fields.settlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.settlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "settlementCode",
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
                                    "/admin/location/capitals"
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
                                    t("location.capitals.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
