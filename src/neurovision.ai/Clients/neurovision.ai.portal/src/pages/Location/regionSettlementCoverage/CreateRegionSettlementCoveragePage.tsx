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
    createNewRegionSettlementCoverage
} from "../../../features/location/regionSettlementCoverage/regionSettlementCoverage.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateRegionSettlementCoveragePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        regionTypeCode: "",
        regionCode: "",
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
        form.regionTypeCode.trim() !== "" &&
        form.regionCode.toString().trim() !== "" &&
        form.countryCode.trim() !== "" &&
        form.settlementCode.toString().trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.regionSettlementCoverages.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewRegionSettlementCoverage({

                    regionTypeCode: form.regionTypeCode,
                    regionCode: Number(form.regionCode),
                    countryCode: form.countryCode,
                    settlementCode: Number(form.settlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.regionSettlementCoverages.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/region-settlement-coverages"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.regionSettlementCoverages.messages.createError"),
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
                title={`${t("location.regionSettlementCoverages.createTitle")} | NeuroVision.AI`}
                description={t("location.regionSettlementCoverages.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.regionSettlementCoverages.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.regionSettlementCoverages.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.regionSettlementCoverages.fields.regionTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.regionTypeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "regionTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionSettlementCoverages.fields.regionCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.regionCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "regionCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionSettlementCoverages.fields.countryCode")}
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
                            {t("location.regionSettlementCoverages.fields.settlementCode")}
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
                                    "/admin/location/region-settlement-coverages"
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
                                    t("location.regionSettlementCoverages.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
