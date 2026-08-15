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
    createNewRegionType
} from "../../../features/location/regionType/regionType.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateRegionTypePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        code: "",
        name: "",

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
        form.code.trim() !== "" &&
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.regionTypes.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewRegionType({

                    code: form.code,
                    name: form.name,

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.regionTypes.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/region-types"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.regionTypes.messages.createError"),
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
                title={`${t("location.regionTypes.createTitle")} | NeuroVision.AI`}
                description={t("location.regionTypes.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.regionTypes.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.regionTypes.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.regionTypes.fields.code")}
                        </Label>
                        <Input
                            type="text"
                            value={form.code}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "code",
                                    e.target.value
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.regionTypes.fields.name")}
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
                                    "/admin/location/region-types"
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
                                    t("location.regionTypes.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
