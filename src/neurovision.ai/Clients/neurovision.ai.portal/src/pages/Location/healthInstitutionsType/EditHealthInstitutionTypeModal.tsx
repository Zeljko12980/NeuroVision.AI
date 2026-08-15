import { useEffect, useState } from "react";

import { Modal } from "../../../components/ui/modal";
import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";

import { useTranslation } from "react-i18next";

import { HealthInstitutionTypeItem } from "../../../pages/Location/healthInstitutionsType/HealthInstitutionTypesTable";



interface Props {

    isOpen: boolean;

    item?: HealthInstitutionTypeItem | null;

    loading?: boolean;

    onClose: () => void;

    onSave: (
        data: HealthInstitutionTypeItem
    ) => Promise<void>;

}



export default function EditHealthInstitutionTypeModal({

    isOpen,

    item,

    loading = false,

    onClose,

    onSave

}: Props) {


    const { t } = useTranslation();



    const [form, setForm] = useState<HealthInstitutionTypeForm>({

        code: "",

        name: ""

    });



    useEffect(() => {

        if (item) {

            setForm({

                code: item.code,

                name: item.name

            });

        }
        else {

            setForm({

                code: "",

                name: ""

            });

        }


    }, [item]);




    if (!isOpen)
        return null;




    const handleChange = (
        field: keyof HealthInstitutionTypeForm,
        value: string
    ) => {


        setForm(prev => ({

            ...prev,

            [field]: value

        }));

    };




    const handleSave = async () => {


        await onSave(form);


    };




    return (

        <Modal

            isOpen={isOpen}

            onClose={onClose}

            className="max-w-lg"

        >


            <div className="
                bg-white
                dark:bg-gray-900
                rounded-2xl
                p-6
            ">


                <h2 className="text-xl font-semibold">

                    {
                        item
                            ?
                            t(
                                "location.healthInstitutionTypes.editTitle"
                            )
                            :
                            t(
                                "location.healthInstitutionTypes.createTitle"
                            )
                    }

                </h2>




                <div className="mt-4 space-y-4">


                    <div>

                        <Label>

                            {
                                t(
                                    "location.healthInstitutionTypes.fields.code"
                                )
                            }

                        </Label>


                        <Input

                            value={form.code}

                            disabled={!!item}

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

                        </Label>


                        <Input

                            value={form.name}

                            onChange={
                                e =>
                                    handleChange(
                                        "name",
                                        e.target.value
                                    )
                            }

                        />

                    </div>



                </div>





                <div className="
                    mt-6
                    flex
                    justify-end
                    gap-3
                ">


                    <Button

                        variant="ghost"

                        onClick={onClose}

                    >

                        {t("common.cancel")}

                    </Button>




                    <Button

                        onClick={handleSave}

                        disabled={loading}

                    >

                        {
                            loading
                                ?
                                "Saving..."
                                :
                                t("common.save")
                        }

                    </Button>



                </div>



            </div>



        </Modal>


    );

}