import {
    useEffect,
    useState
} from "react";

import {
    useTranslation
} from "react-i18next";

import {
    Modal
} from "../../../components/ui/modal";

import Button from "../../../components/ui/button/Button";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import CustomSelect from "../../../components/form/CustomSelect";

import {
    useAppDispatch,
    useAppSelector
} from "../../../store/store";

import {
    fetchGovernmentTypes
} from "../../../features/location/governmentTypeSlice";

import {
    CountryForm
} from "../../../features/location/country/country.types";


interface Props {
    isOpen: boolean;
    country: CountryForm | null;
    loading: boolean;
    onClose: () => void;
    onSave: (country: CountryForm) => Promise<void>;
}


type TabKey =
    | "general"
    | "government"
    | "assets";


const TABS = [
    {
        key: "general",
        labelKey: "location.countries.editModal.tabs.general"
    },
    {
        key: "government",
        labelKey: "location.countries.editModal.tabs.government"
    },
    {
        key: "assets",
        labelKey: "location.countries.editModal.tabs.assets"
    }
] as const;


const emptyCountry: CountryForm = {
    code: "",
    name: "",
    foundingDate: "",
    capitalSettlementCode: undefined,
    governmentTypeCode: "",
    callingCode: undefined,
    anthem: null,
    coatOfArms: null,
    flag: null
};


export default function EditCountryModal({
    isOpen,
    country,
    loading,
    onClose,
    onSave
}: Props) {

    const {
        t
    } = useTranslation();


    const dispatch =
        useAppDispatch();


    const governmentTypes =
        useAppSelector(
            state =>
                state.governmentTypes.items
        );


    const governmentOptions =
        governmentTypes.map(type => ({
            value: type.code,
            label: `${type.name} (${type.code})`
        }));


    const [form, setForm] =
        useState<CountryForm>(
            emptyCountry
        );


    const [activeTab, setActiveTab] =
        useState<TabKey>(
            "general"
        );


    useEffect(() => {

        dispatch(
            fetchGovernmentTypes({
                pageIndex: 0,
                pageSize: 100
            })
        );

    }, [dispatch]);


    useEffect(() => {

        if (!country) {

            setForm(emptyCountry);

            setActiveTab("general");

            return;
        }


        setForm({
            ...country,
            foundingDate:
                country.foundingDate
                    ? country.foundingDate.split("T")[0]
                    : "",
            flag: null,
            coatOfArms: null,
            anthem: null
        });


        setActiveTab("general");


    }, [country]);


    const handleChange = <
        K extends keyof CountryForm
    >(
        key: K,
        value: CountryForm[K]
    ) => {

        setForm(previous => ({
            ...previous,
            [key]: value
        }));

    };


    const handleSubmit = async () => {

        await onSave(form);

        onClose();

    };


    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            className="max-w-4xl"
        >

            <div
                className="
                    flex
                    h-[600px]
                    flex-col
                    overflow-hidden
                    rounded-2xl
                    bg-white
                    dark:bg-gray-900
                "
            >

                <div
                    className="
                        border-b
                        border-gray-200
                        px-6
                        py-5
                        dark:border-gray-800
                    "
                >

                    <div className="flex items-center justify-between">

                        <div>

                            <h2 className="text-xl font-semibold">

                                {t(
                                    "location.countries.editTitle"
                                )}

                            </h2>


                            <p className="mt-1 text-sm text-gray-500">

                                {t(
                                    "location.countries.editDescription"
                                )}

                            </p>

                        </div>


                        <span
                            className="
                                rounded-full
                                bg-blue-100
                                px-3
                                py-1
                                text-xs
                                font-semibold
                                text-blue-600
                            "
                        >

                            {form.code}

                        </span>

                    </div>

                </div>


                <div
                    className="
                        flex
                        border-b
                        border-gray-200
                        dark:border-gray-800
                    "
                >

                    {
                        TABS.map(tab => (

                            <button
                                key={tab.key}
                                onClick={() =>
                                    setActiveTab(tab.key)
                                }
                                className={`
                                    px-5
                                    py-3
                                    text-sm
                                    font-medium
                                    ${activeTab === tab.key
                                        ?
                                        "border-b-2 border-blue-600 text-blue-600"
                                        :
                                        "text-gray-500 hover:text-gray-900"
                                    }
                                `}
                            >

                                {
                                    t(tab.labelKey)
                                }

                            </button>

                        ))
                    }

                </div>


                <div className="flex-1 overflow-hidden p-6">

                    <div className="h-full overflow-y-auto">
                        {
                            activeTab === "general" && (

                                <div className="grid grid-cols-2 gap-5">

                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.code"
                                            )}
                                        </Label>


                                        <Input
                                            value={form.code}
                                            disabled
                                            onChange={() => { }}
                                        />

                                    </div>


                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.name"
                                            )}
                                        </Label>


                                        <Input
                                            value={form.name}
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
                                            {t(
                                                "location.countries.fields.foundingDate"
                                            )}
                                        </Label>


                                        <Input
                                            type="date"
                                            value={form.foundingDate ?? ""}
                                            onChange={e =>
                                                handleChange(
                                                    "foundingDate",
                                                    e.target.value
                                                )
                                            }
                                        />

                                    </div>

                                </div>

                            )
                        }



                        {
                            activeTab === "government" && (

                                <div className="grid grid-cols-2 gap-5">

                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.callingCode"
                                            )}
                                        </Label>


                                        <Input
                                            type="number"
                                            value={
                                                form.callingCode ?? ""
                                            }
                                            onChange={e =>
                                                handleChange(
                                                    "callingCode",
                                                    e.target.value
                                                        ? Number(e.target.value)
                                                        : undefined
                                                )
                                            }
                                        />

                                    </div>



                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.capitalSettlementCode"
                                            )}
                                        </Label>


                                        <Input
                                            type="number"
                                            value={
                                                form.capitalSettlementCode ?? ""
                                            }
                                            onChange={e =>
                                                handleChange(
                                                    "capitalSettlementCode",
                                                    e.target.value
                                                        ? Number(e.target.value)
                                                        : undefined
                                                )
                                            }
                                        />

                                    </div>



                                    <div className="col-span-2">

                                        <Label>
                                            {t(
                                                "location.countries.fields.governmentTypeCode"
                                            )}
                                        </Label>


                                        <CustomSelect
                                            options={
                                                governmentOptions
                                            }
                                            value={
                                                form.governmentTypeCode ?? ""
                                            }
                                            placeholder={
                                                t(
                                                    "location.countries.placeholders.governmentType"
                                                )
                                            }
                                            onChange={value =>
                                                handleChange(
                                                    "governmentTypeCode",
                                                    value
                                                )
                                            }
                                        />

                                    </div>

                                </div>

                            )
                        }



                        {
                            activeTab === "assets" && (

                                <div className="grid gap-6">


                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.flag"
                                            )}
                                        </Label>


                                        {
                                            country?.flag && (

                                                <img
                                                    src={`data:image/webp;base64,${country.flag}`}
                                                    alt="Flag"
                                                    className="
                                                        mb-3
                                                        h-32
                                                        w-48
                                                        rounded-lg
                                                        border
                                                        object-cover
                                                    "
                                                />

                                            )
                                        }


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

                                    </div>




                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.coatOfArms"
                                            )}
                                        </Label>


                                        {
                                            country?.coatOfArms && (

                                                <img
                                                    src={`data:image/png;base64,${country.coatOfArms}`}
                                                    alt="Coat of arms"
                                                    className="
                                                        mb-3
                                                        h-32
                                                        w-32
                                                        rounded-lg
                                                        border
                                                        object-contain
                                                    "
                                                />

                                            )
                                        }


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

                                    </div>




                                    <div>

                                        <Label>
                                            {t(
                                                "location.countries.fields.anthem"
                                            )}
                                        </Label>


                                        {
                                            country?.anthem && (

                                                <audio
                                                    controls
                                                    className="mb-3 w-full"
                                                >

                                                    <source
                                                        src={`data:audio/mpeg;base64,${country.anthem}`}
                                                        type="audio/mpeg"
                                                    />

                                                </audio>

                                            )
                                        }


                                        <Input
                                            type="file"
                                            accept=".mp3,.wav"
                                            onChange={e =>
                                                handleChange(
                                                    "anthem",
                                                    e.target.files?.[0] ?? null
                                                )
                                            }
                                        />

                                    </div>


                                </div>

                            )
                        }

                    </div>

                </div>



                <div
                    className="
                        flex
                        justify-end
                        gap-3
                        border-t
                        border-gray-200
                        px-6
                        py-4
                        dark:border-gray-800
                    "
                >

                    <Button
                        variant="outline"
                        onClick={onClose}
                    >

                        {t(
                            "common.cancel"
                        )}

                    </Button>



                    <Button
                        onClick={handleSubmit}
                        disabled={loading}
                    >

                        {
                            loading
                                ?
                                t(
                                    "common.saving"
                                )
                                :
                                t(
                                    "common.saveChanges"
                                )
                        }

                    </Button>


                </div>


            </div>

        </Modal>
    );

}