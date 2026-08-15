import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

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
                    message:
                        "Name, type, country and settlement are required"
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

                    message:
                        "Health institution created successfully"

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
                        "Failed to create health institution"

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
                title="Create Health Institution | NeuroVision.AI"
                description="Create health institution"
            />


            <PageBreadcrumb
                pageTitle="Create Health Institution"
            />



            <div className="max-w-3xl mx-auto">


                <ComponentCard title="New Health Institution">


                    <div className="space-y-5">



                        <div>

                            <Label>
                                Name *
                            </Label>

                            <Input

                                value={form.name}

                                placeholder="General Hospital"

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
                                Type *
                            </Label>


                            <CustomSelect

                                options={typeOptions}

                                value={form.typeCode}

                                placeholder="Select type"

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
                                Country *
                            </Label>


                            <CustomSelect

                                options={countryOptions}

                                value={form.countryCode}

                                placeholder="Select country"

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
                                Settlement *
                            </Label>


                            <SettlementSelect


                                settlements={
                                    filteredSettlements
                                }


                                value={
                                    form.settlement
                                }


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
                                Address
                            </Label>


                            <Input

                                value={form.address}

                                placeholder="Main Street 25, Building A"

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
                                Phone
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
                                Bed Count
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
                                Founding Date
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
                            Cancel

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
                                    "Creating..."
                                    :
                                    "Create Health Institution"
                            }


                        </Button>



                    </div>



                </ComponentCard>


            </div>


        </>
    );
}