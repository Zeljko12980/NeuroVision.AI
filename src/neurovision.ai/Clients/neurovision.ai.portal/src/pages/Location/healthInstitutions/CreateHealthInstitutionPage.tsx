import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import ComponentCard from "../../../components/common/ComponentCard";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import PageMeta from "../../../components/common/PageMeta";

import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";
import CustomSelect from "../../../components/form/CustomSelect";
import SettlementSelect from "../../../components/form/SettlementSelect";

import {
    createNewHealthInstitution,
} from "../../../features/location/healthInstitutions/healthInstitution.slice";

import {
    fetchHealthInstitutionTypes,
} from "../../../features/location/healthInstitutionsType/healthInstitutionType.slice";

import {
    fetchCountries,
} from "../../../features/location/country/country.slice";

import {
    fetchSettlements,
} from "../../../features/location/settlement/settlement.slice";

import {
    showAlert,
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch,
    useAppSelector,
} from "../../../store/store";


export default function CreateHealthInstitutionPage() {


    const navigate = useNavigate();

    const dispatch = useAppDispatch();
    const { t } = useTranslation();



    const healthInstitutionTypes =
        useAppSelector(
            s => s.healthInstitutionTypes.items
        );


    const countries =
        useAppSelector(
            s => s.countries.items
        );


    const settlements =
        useAppSelector(
            s => s.settlements.items
        );



    const [loading, setLoading] =
        useState(false);



    const [form, setForm] = useState({

        name: "",

        typeCode: "",

        countryCode: "",


        settlement: null as {
            countryCode: string;
            code: number;
        } | null,


        address: "",

        bedCount: undefined as number | undefined,

        foundingDate: "",

        phone: "",

    });



    useEffect(() => {

        dispatch(
            fetchHealthInstitutionTypes({
                pageIndex: 0,
                pageSize: 1000
            })
        );


        dispatch(
            fetchCountries({
                pageIndex: 0,
                pageSize: 1000
            })
        );


        dispatch(
            fetchSettlements({
                pageIndex: 0,
                pageSize: 1000
            })
        );


    }, [dispatch]);




    const typeOptions =
        healthInstitutionTypes.map(x => ({
            value: x.code,
            label: `${x.name} (${x.code})`
        }));



    const countryOptions =
        countries.map(x => ({
            value: x.code,
            label: `${x.name} (${x.code})`
        }));




    const filteredSettlements =
        form.countryCode === ""
            ?
            settlements
            :
            settlements.filter(
                x =>
                    x.countryCode === form.countryCode
            );





    const handleChange = (
        field: string,
        value: any
    ) => {


        setForm(prev => ({
            ...prev,
            [field]: value
        }));

    };






    const isValid =

        form.name.trim() !== "" &&

        form.typeCode.trim() !== "" &&

        form.countryCode.trim() !== "" &&

        form.settlement !== null;




    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    type: "error",
                    message: t("location.healthInstitutions.messages.required")
                })
            );

            return;

        }



        try {


            setLoading(true);



            await dispatch(
                createNewHealthInstitution({

                    name: form.name,

                    typeCode: form.typeCode,

                    countryCode: form.countryCode,


                    settlementCode:
                        form.settlement!.code,


                    address:
                        form.address || null,


                    bedCount:
                        form.bedCount ?? null,


                    foundingDate:
                        form.foundingDate
                            ? form.foundingDate
                            : null,


                    phone:
                        form.phone || null

                })
            ).unwrap();





            dispatch(
                showAlert({

                    type: "success",

                    message: t("location.healthInstitutions.messages.createSuccess")

                })
            );



            navigate(
                "/admin/location/health-institutions"
            );



        }
        catch (error: any) {


            dispatch(
                showAlert({

                    type: "error",

                    message:
                        error?.message ??
                        t("location.healthInstitutions.messages.createError")

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
                title={`${t("location.healthInstitutions.createTitle")} | NeuroVision.AI`}
                description={t("location.healthInstitutions.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("location.healthInstitutions.createTitle")}
            />



            <div className="max-w-3xl mx-auto">


                <ComponentCard title={t("location.healthInstitutions.createTitle")}>


                    <div className="space-y-5">



                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.name")} *
                            </Label>

                            <Input

                                value={form.name}

                                placeholder={t("location.healthInstitutions.placeholders.name")}

                                onChange={
                                    e =>
                                        handleChange(
                                            "name",
                                            e.target.value
                                        )
                                }

                            />

                        </div>





                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.typeCode")} *
                            </Label>


                            <CustomSelect

                                options={typeOptions}

                                value={form.typeCode}

                                placeholder={t("location.healthInstitutions.placeholders.type")}

                                onChange={
                                    value =>
                                        handleChange(
                                            "typeCode",
                                            value
                                        )
                                }

                            />


                        </div>






                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.countryCode")} *
                            </Label>


                            <CustomSelect

                                options={countryOptions}

                                value={form.countryCode}

                                placeholder={t("location.healthInstitutions.placeholders.country")}

                                onChange={
                                    value =>
                                        handleChange(
                                            "countryCode",
                                            value
                                        )
                                }

                            />

                        </div>






                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.settlementCode")} *
                            </Label>


                            <SettlementSelect


                                settlements={
                                    filteredSettlements
                                }


                                value={
                                    form.settlement
                                }

                                placeholder={t("location.healthInstitutions.placeholders.settlement")}


                                onChange={
                                    (_, settlement) => {

                                        handleChange(
                                            "settlement",
                                            {
                                                countryCode:
                                                    settlement.countryCode,

                                                code:
                                                    settlement.code
                                            }
                                        );

                                    }
                                }


                            />

                        </div>





                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.address")}
                            </Label>


                            <Input

                                value={form.address}

                                placeholder={t("location.healthInstitutions.placeholders.address")}

                                onChange={
                                    e =>
                                        handleChange(
                                            "address",
                                            e.target.value
                                        )
                                }

                            />

                        </div>






                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.phone")}
                            </Label>


                            <Input

                                value={form.phone}

                                placeholder="+387 65 123 456"

                                onChange={
                                    e =>
                                        handleChange(
                                            "phone",
                                            e.target.value
                                        )
                                }

                            />

                        </div>






                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.bedCount")}
                            </Label>


                            <Input

                                type="number"

                                value={
                                    form.bedCount ?? ""
                                }

                                placeholder="100"

                                onChange={
                                    e =>
                                        handleChange(
                                            "bedCount",
                                            e.target.value
                                                ?
                                                Number(e.target.value)
                                                :
                                                undefined
                                        )
                                }

                            />

                        </div>







                        <div>

                            <Label>
                                {t("location.healthInstitutions.fields.foundingDate")}
                            </Label>


                            <Input

                                type="date"

                                value={form.foundingDate}

                                onChange={
                                    e =>
                                        handleChange(
                                            "foundingDate",
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
                                    "/admin/location/health-institutions"
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
                                    t("location.healthInstitutions.createButton")
                            }


                        </Button>



                    </div>



                </ComponentCard>


            </div>


        </>
    );
}