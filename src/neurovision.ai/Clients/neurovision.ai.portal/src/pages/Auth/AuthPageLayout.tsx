import React from "react";
import GridShape from "../../components/common/GridShape";
import { Link } from "react-router";
import ThemeTogglerTwo from "../../components/common/ThemeTogglerTwo";
import LanguageToggler from "../../components/common/LanguageToggler";
import BrandLogo from "../../components/common/BrandLogo";

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="relative p-6 bg-white z-1 dark:bg-gray-900 sm:p-0">
      <div className="relative flex flex-col justify-center w-full h-screen lg:flex-row dark:bg-gray-900 sm:p-0">
        <div className="flex flex-col flex-1 w-full lg:w-1/2">
          <div className="flex justify-center pt-4 lg:hidden">
            <Link to="/">
              <BrandLogo className="h-9 w-auto" />
            </Link>
          </div>
          {children}
        </div>
        <div className="items-center hidden w-full h-full lg:w-1/2 bg-brand-950 dark:bg-white/5 lg:grid">
          <div className="relative flex items-center justify-center z-1">
            <GridShape />
            <div className="flex flex-col items-center max-w-xs">
              <Link to="/" className="block mb-4">
                <BrandLogo variant="onDark" className="h-11 w-auto" />
              </Link>
              <p className="text-center text-sm text-white/70 dark:text-white/60">
                AI brain tumor detection
              </p>
            </div>
          </div>
        </div>
        <div className="fixed z-50 hidden bottom-6 right-6 sm:block">
          <ThemeTogglerTwo />
        </div>
        <div className="absolute top-6 right-6 flex space-x-3">
          <LanguageToggler />
        </div>
      </div>
    </div>
  );
}
