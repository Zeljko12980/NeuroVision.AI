import { useState } from "react";
import { Dropdown } from "../ui/dropdown/Dropdown";

interface Option {
    value: string;
    label: string;
}

interface CustomSelectProps {
    options: Option[];
    placeholder?: string;
    value?: string;
    onChange: (value: string) => void;
}


export default function CustomSelect({
    options,
    placeholder = "Select option",
    value,
    onChange
}: CustomSelectProps) {


    const [open, setOpen] = useState(false);


    const selected =
        options.find(
            x => x.value === value
        );



    return (

        <div className="relative w-full">


            <button

                type="button"

                onClick={() => setOpen(prev => !prev)}

                className="
                    h-11
                    w-full
                    rounded-lg
                    border
                    border-gray-300
                    bg-transparent
                    px-4
                    text-left
                    text-sm
                    dark:border-gray-700
                    dark:bg-gray-900
                    dark:text-white
                "

            >

                {
                    selected
                        ? selected.label
                        :
                        <span className="text-gray-400">
                            {placeholder}
                        </span>
                }


            </button>



            <Dropdown

                isOpen={open}

                onClose={() => setOpen(false)}

                className="
                    top-full
                    left-0
                    mt-2
                    w-full
                    max-h-60
                    overflow-y-auto
                "

            >


                <div className="py-2">


                    {
                        options.map(option => (

                            <button

                                key={option.value}

                                type="button"

                                onClick={() => {

                                    onChange(option.value);

                                    setOpen(false);

                                }}

                                className="
                                    w-full
                                    px-4
                                    py-2
                                    text-left
                                    text-sm
                                    hover:bg-gray-100
                                    dark:hover:bg-white/[0.05]
                                "

                            >

                                {option.label}


                            </button>


                        ))
                    }


                </div>


            </Dropdown>


        </div>

    );
}