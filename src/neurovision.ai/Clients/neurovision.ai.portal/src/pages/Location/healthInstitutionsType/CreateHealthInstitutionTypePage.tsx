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
    createNewHealthInstitutionType,
} from "../../../features/location/healthInstitutionsType/healthInstitutionType.slice";

import {
    showAlert,
} from "../../../features/ui/uiSlice";

import {
    useAppDispatch,
} from "../../../store/store";


export default function CreateHealthInstitutionTypePage() {


    const navigate = useNavigate();

    const dispatch = useAppDispatch();

    const { t } = useTranslation();



    const [loading, setLoading] =
        useState(false);



    const [form, setForm] = useState({

        code: "",

        name: ""

    });



    const handleChange = (
        field: string,
        value: string
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

                    type: "error",

                    message:
                        t(
                            "location.healthInstitutionTypes.messages.required"
                        )

                })
            );


            return;

        }



        try {


            setLoading(true);



            await dispatch(

                createNewHealthInstitutionType({

                    code:
                        form.code.trim(),


                    name:
                        form.name.trim()

                })

            ).unwrap();




            dispatch(
                showAlert({

                    type: "success",

                    message:
                        t(
                            "location.healthInstitutionTypes.messages.createSuccess"
                        )

                })
            );



            navigate(
                "/admin/location/health-institutions-types"
            );



        }
        catch (error: any) {


            dispatch(
                showAlert({

                    type: "error",

                    message:
                        error?.message ??
                        t(
                            "location.healthInstitutionTypes.messages.createError"
                        )

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

                title={
                    `${t(
                        "location.healthInstitutionTypes.createTitle"
                    )} | NeuroVision.AI`
                }


                description={
                    t(
                        "location.healthInstitutionTypes.pageDescription"
                    )
                }

            />




            <PageBreadcrumb

                pageTitle={
                    t(
                        "location.healthInstitutionTypes.createTitle"
                    )
                }

            />





            <div className="max-w-3xl mx-auto">



                <ComponentCard

                    title={
                        t(
                            "location.healthInstitutionTypes.createTitle"
                        )
                    }

                >



                    <div className="space-y-5">





                        <div>


                            <Label>

                                {
                                    t(
                                        "location.healthInstitutionTypes.fields.code"
                                    )
                                }

                                {" *"}

                            </Label>



                            <Input


                                value={
                                    form.code
                                }



                                placeholder={
                                    t(
                                        "location.healthInstitutionTypes.placeholders.code"
                                    )
                                }



                                onChange={
                                    e =>
                                        handleChange(
                                            "code",
                                            e.target.value
                                        )
                                }


                            />



                        </div>









                        <div>


                            <Label>


                                {
                                    t(
                                        "location.healthInstitutionTypes.fields.name"
                                    )
                                }

                                {" *"}


                            </Label>




                            <Input



                                value={
                                    form.name
                                }



                                placeholder={
                                    t(
                                        "location.healthInstitutionTypes.placeholders.name"
                                    )
                                }




                                onChange={
                                    e =>
                                        handleChange(
                                            "name",
                                            e.target.value
                                        )
                                }



                            />



                        </div>









                        <div
                            className="
                                flex
                                justify-end
                                gap-3
                                mt-8
                                pt-5
                                border-t
                            "
                        >





                            <Button


                                variant="outline"



                                onClick={() =>
                                    navigate(
                                        "/admin/location/health-institution-types"
                                    )
                                }


                            >


                                {
                                    t(
                                        "common.cancel"
                                    )
                                }



                            </Button>









                            <Button



                                disabled={
                                    loading ||
                                    !isValid
                                }



                                onClick={
                                    handleSubmit
                                }



                            >



                                {
                                    loading
                                        ?
                                        t(
                                            "common.creating"
                                        )
                                        :
                                        t(
                                            "location.healthInstitutionTypes.createButton"
                                        )
                                }



                            </Button>







                        </div>





                    </div>



                </ComponentCard>



            </div>


        </>


    );

}