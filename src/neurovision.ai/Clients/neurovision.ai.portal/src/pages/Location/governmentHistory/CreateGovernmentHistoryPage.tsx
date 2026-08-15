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
    createNewGovernmentHistory
} from "../../../features/location/governmentHistory/governmentHistory.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateGovernmentHistoryPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        sequenceNumber: "",
        governmentTypeCode: "",
        from: "",
        to: "",

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
        form.sequenceNumber.toString().trim() !== "" &&
        form.governmentTypeCode.trim() !== "" &&
        form.from.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.governmentHistories.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewGovernmentHistory({

                    countryCode: form.countryCode,
                    sequenceNumber: Number(form.sequenceNumber),
                    governmentTypeCode: form.governmentTypeCode,
                    from: form.from,
                    to: form.to || undefined,

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.governmentHistories.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/government-histories"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.governmentHistories.messages.createError"),
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
                title={`${t("location.governmentHistories.createTitle")} | NeuroVision.AI`}
                description={t("location.governmentHistories.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.governmentHistories.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.governmentHistories.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.countryCode")}
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
                            {t("location.governmentHistories.fields.sequenceNumber")}
                        </Label>
                        <Input
                            type="number"
                            value={form.sequenceNumber ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "sequenceNumber",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.governmentTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.governmentTypeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "governmentTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.from")}
                        </Label>
                        <Input
                            type="date"
                            value={form.from ? form.from.split("T")[0] : ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "from",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.governmentHistories.fields.to")}
                        </Label>
                        <Input
                            type="date"
                            value={form.to ? form.to.split("T")[0] : ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "to",
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
                                    "/admin/location/government-histories"
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
                                    t("location.governmentHistories.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
