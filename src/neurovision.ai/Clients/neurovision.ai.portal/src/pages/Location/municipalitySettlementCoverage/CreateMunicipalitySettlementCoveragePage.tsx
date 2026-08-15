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
    createNewMunicipalitySettlementCoverage
} from "../../../features/location/municipalitySettlementCoverage/municipalitySettlementCoverage.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateMunicipalitySettlementCoveragePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        municipalityCode: "",
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
        form.municipalityCode.toString().trim() !== "" &&
        form.settlementCode.toString().trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.municipalitySettlementCoverages.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewMunicipalitySettlementCoverage({

                    countryCode: form.countryCode,
                    municipalityCode: Number(form.municipalityCode),
                    settlementCode: Number(form.settlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.municipalitySettlementCoverages.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/municipality-settlement-coverages"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.municipalitySettlementCoverages.messages.createError"),
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
                title={`${t("location.municipalitySettlementCoverages.createTitle")} | NeuroVision.AI`}
                description={t("location.municipalitySettlementCoverages.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.municipalitySettlementCoverages.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.municipalitySettlementCoverages.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.municipalitySettlementCoverages.fields.countryCode")}
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
                            {t("location.municipalitySettlementCoverages.fields.municipalityCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.municipalityCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "municipalityCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.municipalitySettlementCoverages.fields.settlementCode")}
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
                                    "/admin/location/municipality-settlement-coverages"
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
                                    t("location.municipalitySettlementCoverages.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
