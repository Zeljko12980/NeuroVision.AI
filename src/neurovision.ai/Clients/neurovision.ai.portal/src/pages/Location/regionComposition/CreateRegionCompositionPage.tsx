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
    createNewRegionComposition
} from "../../../features/location/regionComposition/regionComposition.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateRegionCompositionPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        parentRegionTypeCode: "",
        parentRegionCode: "",
        memberRegionTypeCode: "",
        memberRegionCode: "",

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
        form.parentRegionTypeCode.trim() !== "" &&
        form.parentRegionCode.toString().trim() !== "" &&
        form.memberRegionTypeCode.trim() !== "" &&
        form.memberRegionCode.toString().trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.regionCompositions.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewRegionComposition({

                    parentRegionTypeCode: form.parentRegionTypeCode,
                    parentRegionCode: Number(form.parentRegionCode),
                    memberRegionTypeCode: form.memberRegionTypeCode,
                    memberRegionCode: Number(form.memberRegionCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.regionCompositions.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/region-compositions"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.regionCompositions.messages.createError"),
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
                title={`${t("location.regionCompositions.createTitle")} | NeuroVision.AI`}
                description={t("location.regionCompositions.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.regionCompositions.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.regionCompositions.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.parentRegionTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.parentRegionTypeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "parentRegionTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.parentRegionCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.parentRegionCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "parentRegionCode",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.memberRegionTypeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.memberRegionTypeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "memberRegionTypeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionCompositions.fields.memberRegionCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.memberRegionCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "memberRegionCode",
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
                                    "/admin/location/region-compositions"
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
                                    t("location.regionCompositions.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
