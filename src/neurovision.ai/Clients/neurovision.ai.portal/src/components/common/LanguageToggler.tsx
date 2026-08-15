import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Dropdown } from "../ui/dropdown/Dropdown";
import { SerbiaIcon, GermanyIcon, EnglandIcon } from "../../icons"; 

const LANGUAGES = [
  { code: "en", label: "English", icon: EnglandIcon },
  { code: "sr", label: "Srpski", icon: SerbiaIcon },
  { code: "de", label: "Deutsch", icon: GermanyIcon },
];

export default function LanguageToggler() {
  const { i18n } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  const handleChange = (code: string) => {
    i18n.changeLanguage(code);
    setIsOpen(false);
  };

  const currentLang = LANGUAGES.find((l) => l.code === i18n.language) || LANGUAGES[0];
  const CurrentIcon = currentLang.icon;

  return (
   <div className="relative inline-block text-left">
  <button
    className="dropdown-toggle inline-flex items-center justify-center w-40 rounded-full border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:hover:bg-gray-700"
    onClick={() => setIsOpen((prev) => !prev)}
  >
    <CurrentIcon className="w-5 h-5 mr-2" />
    <span className="truncate">{currentLang.label}</span>
    <svg
      className="w-4 h-4 ml-2"
      fill="none"
      stroke="currentColor"
      strokeWidth={2}
      viewBox="0 0 24 24"
    >
      <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
    </svg>
  </button>

  <Dropdown
    isOpen={isOpen}
    onClose={() => setIsOpen(false)}
    className="w-40" // isto kao dugme
  >
    <div className="flex flex-col">
      {LANGUAGES.map((lang) => {
        const LangIcon = lang.icon;
        return (
          <button
            key={lang.code}
            onClick={() => handleChange(lang.code)}
            className={`w-full px-4 py-2 text-left text-sm hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center ${
              i18n.language === lang.code
                ? "font-semibold text-brand-500"
                : "text-gray-700 dark:text-white"
            }`}
          >
            <LangIcon className="w-4 h-4 mr-2" />
            <span className="truncate">{lang.label}</span>
          </button>
        );
      })}
    </div>
  </Dropdown>
</div>
  );
}