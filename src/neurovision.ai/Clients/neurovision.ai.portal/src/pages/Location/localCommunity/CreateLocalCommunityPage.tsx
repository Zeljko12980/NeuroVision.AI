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
    createNewLocalCommunity
} from "../../../features/location/localCommunity/localCommunity.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateLocalCommunityPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { t } = useTranslation();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        countryCode: "",
        municipalityCode: "",
        identifier: "",
        name: "",
        officeSettlementCode: "",

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
        form.identifier.toString().trim() !== "" &&
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message: t("location.localCommunities.messages.required"),
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewLocalCommunity({

                    countryCode: form.countryCode,
                    municipalityCode: Number(form.municipalityCode),
                    identifier: Number(form.identifier),
                    name: form.name,
                    officeSettlementCode: Number(form.officeSettlementCode),

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message: t("location.localCommunities.messages.createSuccess"),
                    type: "success"
                })
            );


            navigate(
                "/admin/location/local-communities"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        t("location.localCommunities.messages.createError"),
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
                title={`${t("location.localCommunities.createTitle")} | NeuroVision.AI`}
                description={t("location.localCommunities.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.localCommunities.createTitle")}
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title={t("location.localCommunities.createTitle")}>


                    <div className="space-y-5">

                    <div>
                        <Label>
                            {t("location.localCommunities.fields.countryCode")}
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
                            {t("location.localCommunities.fields.municipalityCode")}
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
                            {t("location.localCommunities.fields.identifier")}
                        </Label>
                        <Input
                            type="number"
                            value={form.identifier ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "identifier",
                                    e.target.value
                                        ? Number(e.target.value)
                                        : undefined
                                )
                            }
                        />
                    </div>

                    <div>
                        <Label>
                            {t("location.localCommunities.fields.name")}
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
                            {t("location.localCommunities.fields.officeSettlementCode")}
                        </Label>
                        <Input
                            type="number"
                            value={form.officeSettlementCode ?? ""}
                            disabled={false}
                            onChange={(e) =>
                                handleChange(
                                    "officeSettlementCode",
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
                                    "/admin/location/local-communities"
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
                                    t("location.localCommunities.createButton")
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}
