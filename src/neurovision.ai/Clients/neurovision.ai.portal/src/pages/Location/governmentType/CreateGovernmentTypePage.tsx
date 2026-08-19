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
    createNewGovernmentType
} from "../../../features/location/governmentTypeSlice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateGovernmentTypePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        code: "",

        name: ""

    });


    const handleChange = (
        field: string,
        value: any
    ) => {

        setForm(prev => ({
            ...prev,
            [field]: value
        }));

    };


    const isValid =
        form.code.trim() !== "" &&
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.governmentTypes.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewGovernmentType({

                    code: form.code,

                    name: form.name

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.governmentTypes.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/government-types"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.governmentTypes.messages.createError"),
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
                title={`${t("location.governmentTypes.createTitle")} | NeuroVision.AI`}
                description={t("location.governmentTypes.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.governmentTypes.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.governmentTypes.createTitle")}>


                    <div className="space-y-5">


                        <div>

                            <Label>
                                {t("location.governmentTypes.fields.code")} *
                            </Label>

                            <Input
                                value={form.code}
                                placeholder={t("location.governmentTypes.placeholders.code")}
                                onChange={e =>
                                    handleChange(
                                        "code",
                                        e.target.value.toUpperCase()
                                    )
                                }
                            />

                        </div>



                        <div>

                            <Label>
                                {t("location.governmentTypes.fields.name")} *
                            </Label>

                            <Input
                                value={form.name}
                                placeholder={t("location.governmentTypes.placeholders.name")}
                                onChange={e =>
                                    handleChange(
                                        "name",
                                        e.target.value
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
                                    "/admin/location/government-types"
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
                                    t("location.governmentTypes.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}