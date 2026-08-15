import { useState } from "react";
import { useNavigate } from "react-router-dom";

import ComponentCard from "../../../components/common/ComponentCard";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import PageMeta from "../../../components/common/PageMeta";

import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";

import {
    createNewGovernmentType
} from "../../../features/location/governmentTypeSlice";

import {
    showAlert
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch
} from "../../../store/store";


export default function CreateGovernmentTypePage() {

    const navigate = useNavigate();
    const dispatch = useAppDispatch();


    const [loading, setLoading] =
        useState(false);


    const [form, setForm] = useState({

        code: "",

        name: ""

    });


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
        form.name.trim() !== "";


    const handleSubmit = async () => {


        if (!isValid) {

            dispatch(
                showAlert({
                    message:
                        "Government type code and name are required",
                    type: "error"
                })
            );

            return;
        }



        try {

            setLoading(true);


            await dispatch(
                createNewGovernmentType({

                    code: form.code,

                    name: form.name

                })
            ).unwrap();



            dispatch(
                showAlert({
                    message:
                        "Government type created successfully",
                    type: "success"
                })
            );


            navigate(
                "/admin/location/government-types"
            );


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    message:
                        error?.message ??
                        "Failed to create government type",
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
                title="Create Government Type | NeuroVision.AI"
                description="Create government type"
            />


            <PageBreadcrumb
                pageTitle="Create Government Type"
            />



            <div className="max-w-3xl mx-auto">

                <ComponentCard title="New Government Type">


                    <div className="space-y-5">


                        <div>

                            <Label>
                                Code *
                            </Label>

                            <Input
                                value={form.code}
                                placeholder="REP"
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
                                Name *
                            </Label>

                            <Input
                                value={form.name}
                                placeholder="Republic"
                                onChange={e =>
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
                                    "/admin/location/government-types"
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
                                    "Create Government Type"
                            }

                        </Button>


                    </div>


                </ComponentCard>


            </div>


        </>
    );
}