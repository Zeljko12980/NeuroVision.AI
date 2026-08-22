import { useState } from "react";
import { Dropdown } from "../ui/dropdown/Dropdown";

interface Option {
    value: string;
    label: string;
}

interface CatalogMultiSelectProps {
    options: Option[];
    values: string[];
    placeholder?: string;
    onChange: (values: string[]) => void;
}

export default function CatalogMultiSelect({
    options,
    values,
    placeholder = "Select option",
    onChange,
}: CatalogMultiSelectProps) {
    const [open, setOpen] = useState(false);

    const selectedLabels = options
        .filter((option) => values.includes(option.value))
        .map((option) => option.label);

    const toggle = (value: string) => {
        onChange(
            values.includes(value)
                ? values.filter((item) => item !== value)
                : [...values, value]
        );
    };

    return (
        <div className="relative w-full">
            <button
                type="button"
                onClick={() => setOpen((prev) => !prev)}
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
                {selectedLabels.length > 0 ? (
                    selectedLabels.join(", ")
                ) : (
                    <span className="text-gray-400">{placeholder}</span>
                )}
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
                    {options.map((option) => {
                        const selected = values.includes(option.value);

                        return (
                            <button
                                key={option.value}
                                type="button"
                                onClick={() => toggle(option.value)}
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
                                {selected ? "✓ " : ""}
                                {option.label}
                            </button>
                        );
                    })}
                </div>
            </Dropdown>
        </div>
    );
}
