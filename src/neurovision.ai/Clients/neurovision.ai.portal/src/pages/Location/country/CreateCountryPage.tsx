import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import ComponentCard from "../../../components/common/ComponentCard";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import PageMeta from "../../../components/common/PageMeta";

import CustomSelect from "../../../components/form/CustomSelect";
import SettlementSelect from "../../../components/form/SettlementSelect";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";

import {
    createNewCountry
} from "../../../features/location/country/country.slice";

import {
    fetchGovernmentTypes
} from "../../../features/location/governmentTypeSlice";

import {
    fetchSettlements
} from "../../../features/location/settlement/settlement.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch,
    useAppSelector
} from "../../../store/store";


type Tab =
    | "general"
    | "administrative"
    | "media";


export default function CreateCountryPage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();


    const settlements =
        useAppSelector(
            state => state.settlements.items
        );


    const governmentTypes =
        useAppSelector(
            state => state.governmentTypes.items
        );


    const governmentTypeOptions =
        governmentTypes.map(x => ({
            value: x.code,
            label: `${x.name} (${x.code})`
        }));


    const [activeTab, setActiveTab] =
        useState<Tab>("general");


    const [loading, setLoading] =
        useState(false);



    const [form, setForm] = useState({

        code: "",

        name: "",

        foundingDate: "",

        capitalSettlementCode: "",

        governmentTypeCode: "",

        callingCode: "0",

        flag: null as File | null,

        coatOfArms: null as File | null,

        anthem: null as File | null

    });



    useEffect(() => {

        dispatch(
            fetchGovernmentTypes({
                pageIndex: 0,
                pageSize: 100
            })
        );


        dispatch(
            fetchSettlements({
                pageIndex: 0,
                pageSize: 100
            })
        );

    }, [dispatch]);



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
        form.code.trim() !== "" &&
        form.name.trim() !== "" &&
        form.foundingDate !== "";




    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message:
                        "Country code, name and founding date are required",
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewCountry({

                    code: form.code,

                    name: form.name,

                    foundingDate:
                        form.foundingDate,

                    capitalSettlementCode:
                        form.capitalSettlementCode ||
                        undefined,

                    governmentTypeCode:
                        form.governmentTypeCode ||
                        undefined,

                    callingCode:
                        form.callingCode
                            ? Number(form.callingCode)
                            : undefined,


                    flag: form.flag,

                    coatOfArms:
                        form.coatOfArms,

                    anthem:
                        form.anthem

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message:
                        "Country created successfully",
                    type: "success"
                })
            );


            navigate(
                "/admin/location/countries"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        "Failed to create country",
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
                title="Create Country | NeuroVision.AI"
                description="Create country"
            />


            <PageBreadcrumb
                pageTitle="Create Country"
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title="New Country">


                    <div className="
                        flex
                        gap-8
                        border-b
                        mb-6
                    ">

                        {
                            [
                                {
                                    key: "general",
                                    label: "General"
                                },
                                {
                                    key: "administrative",
                                    label: "Administrative"
                                },
                                {
                                    key: "media",
                                    label: "Media"
                                }
                            ]
                                .map(tab => (

                                    <button
                                        key={tab.key}
                                        onClick={() =>
                                            setActiveTab(
                                                tab.key as Tab
                                            )
                                        }
                                        className={`
                                        pb-3
                                        text-sm
                                        font-medium
                                        ${activeTab === tab.key
                                                ?
                                                "border-b-2 border-blue-600 text-blue-600"
                                                :
                                                "text-gray-500"
                                            }
                                    `}
                                    >
                                        {tab.label}
                                    </button>

                                ))
                        }

                    </div>



                    <div className="
                        h-[450px]
                        overflow-y-auto
                    ">


                        {
                            activeTab === "general" &&

                            <div className="space-y-5">


                                <div>

                                    <Label>
                                        Country Code *
                                    </Label>

                                    <Input
                                        value={form.code}
                                        placeholder="BA"
                                        onChange={e =>
                                            handleChange(
                                                "code",
                                                e.target.value.toUpperCase()
                                            )
                                        }
                                    />

                                </div>



                                <div>

                                    <Label>
                                        Country Name *
                                    </Label>

                                    <Input
                                        value={form.name}
                                        placeholder="Bosnia and Herzegovina"
                                        onChange={e =>
                                            handleChange(
                                                "name",
                                                e.target.value
                                            )
                                        }
                                    />

                                </div>



                                <div>

                                    <Label>
                                        Founding Date *
                                    </Label>

                                    <Input
                                        type="date"
                                        value={form.foundingDate}
                                        onChange={e =>
                                            handleChange(
                                                "foundingDate",
                                                e.target.value
                                            )
                                        }
                                    />

                                </div>


                            </div>

                        }




                        {
                            activeTab === "administrative" &&

                            <div className="space-y-5">


                                <div>

                                    <Label>
                                        Government Type
                                    </Label>

                                    <CustomSelect

                                        options={
                                            governmentTypeOptions
                                        }

                                        value={
                                            form.governmentTypeCode
                                        }

                                        placeholder="Select government type"

                                        onChange={value =>
                                            handleChange(
                                                "governmentTypeCode",
                                                value
                                            )
                                        }

                                    />

                                </div>



                                <div>

                                    <Label>
                                        Capital Settlement
                                    </Label>


                                    <SettlementSelect

                                        settlements={
                                            settlements
                                        }

                                        placeholder="Select capital settlement"

                                        onChange={
                                            (_, settlement) => {

                                                handleChange(
                                                    "capitalSettlementCode",
                                                    settlement.code.toString()
                                                );

                                            }
                                        }

                                    />


                                </div>



                                <div>

                                    <Label>
                                        Calling Code
                                    </Label>

                                    <Input

                                        type="number"

                                        value={
                                            form.callingCode
                                        }

                                        onChange={e =>
                                            handleChange(
                                                "callingCode",
                                                e.target.value
                                            )
                                        }

                                    />

                                </div>


                            </div>

                        }




                        {
                            activeTab === "media" &&

                            <div className="space-y-6">


                                <div>

                                    <Label>
                                        Flag
                                    </Label>


                                    <Input
                                        type="file"
                                        accept="image/*"
                                        onChange={e =>
                                            handleChange(
                                                "flag",
                                                e.target.files?.[0] ?? null
                                            )
                                        }
                                    />


                                    {
                                        form.flag && (

                                            <img

                                                src={
                                                    URL.createObjectURL(
                                                        form.flag
                                                    )
                                                }

                                                alt="Flag preview"

                                                className="
                            mt-3
                            h-32
                            w-48
                            rounded-lg
                            border
                            object-contain
                        "

                                            />

                                        )
                                    }


                                </div>





                                <div>

                                    <Label>
                                        Coat Of Arms
                                    </Label>


                                    <Input
                                        type="file"
                                        accept="image/*"
                                        onChange={e =>
                                            handleChange(
                                                "coatOfArms",
                                                e.target.files?.[0] ?? null
                                            )
                                        }
                                    />


                                    {
                                        form.coatOfArms && (

                                            <img

                                                src={
                                                    URL.createObjectURL(
                                                        form.coatOfArms
                                                    )
                                                }

                                                alt="Coat of arms preview"

                                                className="
                            mt-3
                            h-32
                            w-32
                            rounded-lg
                            border
                            object-contain
                        "

                                            />

                                        )
                                    }


                                </div>





                                <div>

                                    <Label>
                                        Anthem
                                    </Label>


                                    <Input
                                        type="file"
                                        accept=".mp3,.wav,.ogg"
                                        onChange={e =>
                                            handleChange(
                                                "anthem",
                                                e.target.files?.[0] ?? null
                                            )
                                        }
                                    />


                                    {
                                        form.anthem && (

                                            <audio

                                                controls

                                                className="
                            mt-3
                            w-full
                        "

                                                src={
                                                    URL.createObjectURL(
                                                        form.anthem
                                                    )
                                                }

                                            />

                                        )
                                    }


                                </div>


                            </div>
                        }


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
                                    "/admin/location/countries"
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
                                    "Create Country"
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}