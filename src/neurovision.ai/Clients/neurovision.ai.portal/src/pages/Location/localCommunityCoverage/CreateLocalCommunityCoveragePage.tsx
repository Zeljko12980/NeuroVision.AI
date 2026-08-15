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
    createNewLocalCommunityCoverage
} from "../../../features/location/localCommunityCoverage/localCommunityCoverage.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateLocalCommunityCoveragePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        municipalityCode: "",
        localCommunityIdentifier: "",
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
        form.localCommunityIdentifier.toString().trim() !== "" &&
        form.settlementCode.toString().trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.localCommunityCoverages.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewLocalCommunityCoverage({

                    countryCode: form.countryCode,
                    municipalityCode: Number(form.municipalityCode),
                    localCommunityIdentifier: Number(form.localCommunityIdentifier),
                    settlementCode: Number(form.settlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.localCommunityCoverages.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/local-community-coverages"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.localCommunityCoverages.messages.createError"),
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
                title={`${t("location.localCommunityCoverages.createTitle")} | NeuroVision.AI`}
                description={t("location.localCommunityCoverages.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.localCommunityCoverages.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.localCommunityCoverages.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.localCommunityCoverages.fields.countryCode")}
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
                            {t("location.localCommunityCoverages.fields.municipalityCode")}
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
                            {t("location.localCommunityCoverages.fields.localCommunityIdentifier")}
                        </Label>
                        <Input
                            type="number"
                            value={form.localCommunityIdentifier ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "localCommunityIdentifier",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.localCommunityCoverages.fields.settlementCode")}
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
                                    "/admin/location/local-community-coverages"
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
                                    t("location.localCommunityCoverages.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
