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
    createNewCountryComposition
} from "../../../features/location/countryComposition/countryComposition.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateCountryCompositionPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        unionCountryCode: "",
        memberCountryCode: "",
        sequenceNumber: "",
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
        form.unionCountryCode.trim() !== "" &&
        form.memberCountryCode.trim() !== "" &&
        form.sequenceNumber.toString().trim() !== "" &&
        form.from.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.countryCompositions.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewCountryComposition({

                    unionCountryCode: form.unionCountryCode,
                    memberCountryCode: form.memberCountryCode,
                    sequenceNumber: Number(form.sequenceNumber),
                    from: form.from,
                    to: form.to || undefined,

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.countryCompositions.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/country-compositions"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.countryCompositions.messages.createError"),
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
                title={`${t("location.countryCompositions.createTitle")} | NeuroVision.AI`}
                description={t("location.countryCompositions.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.countryCompositions.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.countryCompositions.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.unionCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.unionCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "unionCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.memberCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.memberCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "memberCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.countryCompositions.fields.sequenceNumber")}
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
                            {t("location.countryCompositions.fields.from")}
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
                            {t("location.countryCompositions.fields.to")}
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
                                    "/admin/location/country-compositions"
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
                                    t("location.countryCompositions.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
