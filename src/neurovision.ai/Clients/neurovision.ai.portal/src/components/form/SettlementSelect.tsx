import { useState } from "react";

import { Dropdown } from "../ui/dropdown/Dropdown";

import { SettlementResponse } from "../../features/location/settlement/settlement.service";


interface SettlementSelectProps {

    settlements: SettlementResponse[];

    placeholder?: string;

    value?: {
        countryCode: string;
        code: number;
    } | null;

    onChange: (
        value: string,
        settlement: SettlementResponse
    ) => void;

}



export default function SettlementSelect({

    settlements,

    placeholder = "Select settlement",

    value,

    onChange

}: SettlementSelectProps) {



    const [open, setOpen] = useState(false);



    const selected = settlements.find(
        x =>
            x.countryCode === value?.countryCode &&
            x.code === value?.code
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

                        ?

                        `${selected.name} (${selected.countryCode})`

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
                        settlements.map(settlement => (


                            <button

                                key={`${settlement.countryCode}-${settlement.code}`}

                                type="button"


                                onClick={() => {


                                    onChange(

                                        `${settlement.countryCode}-${settlement.code}`,

                                        settlement

                                    );


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


                                <div className="flex justify-between">


                                    <span>

                                        {settlement.name}

                                    </span>



                                    <span className="text-gray-400">

                                        {settlement.countryCode}

                                    </span>


                                </div>


                            </button>


                        ))
                    }



                </div>



            </Dropdown>



        </div>


    );

}