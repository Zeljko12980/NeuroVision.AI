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
    createNewLegalSuccessor
} from "../../../features/location/legalSuccessor/legalSuccessor.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateLegalSuccessorPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        successorCountryCode: "",
        predecessorCountryCode: "",

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
        form.successorCountryCode.trim() !== "" &&
        form.predecessorCountryCode.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.legalSuccessors.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewLegalSuccessor({

                    successorCountryCode: form.successorCountryCode,
                    predecessorCountryCode: form.predecessorCountryCode,

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.legalSuccessors.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/legal-successors"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.legalSuccessors.messages.createError"),
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
                title={`${t("location.legalSuccessors.createTitle")} | NeuroVision.AI`}
                description={t("location.legalSuccessors.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.legalSuccessors.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.legalSuccessors.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.legalSuccessors.fields.successorCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.successorCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "successorCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.legalSuccessors.fields.predecessorCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.predecessorCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "predecessorCountryCode",
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
                                    "/admin/location/legal-successors"
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
                                    t("location.legalSuccessors.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
