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
    createNewRegion
} from "../../../features/location/region/region.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateRegionPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        typeCode: "",
        code: "",
        name: "",
        belongsToCountryCode: "",
        headquartersCountryCode: "",
        administrativeSeatSettlementCode: "",

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
        form.typeCode.trim() !== "" &&
        form.code.toString().trim() !== "" &&
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.regions.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewRegion({

                    typeCode: form.typeCode,
                    code: Number(form.code),
                    name: form.name,
                    belongsToCountryCode: form.belongsToCountryCode || undefined,
                    headquartersCountryCode: form.headquartersCountryCode || undefined,
                    administrativeSeatSettlementCode: Number(form.administrativeSeatSettlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.regions.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/regions"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.regions.messages.createError"),
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
                title={`${t("location.regions.createTitle")} | NeuroVision.AI`}
                description={t("location.regions.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.regions.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.regions.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.regions.fields.typeCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.typeCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "typeCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.code")}
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
                            {t("location.regions.fields.name")}
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
                            {t("location.regions.fields.belongsToCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.belongsToCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "belongsToCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.headquartersCountryCode")}
                        </Label>
                        <Input
                            type="text"
                            value={form.headquartersCountryCode}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "headquartersCountryCode",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regions.fields.administrativeSeatSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.administrativeSeatSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "administrativeSeatSettlementCode",
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
                                    "/admin/location/regions"
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
                                    t("location.regions.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
