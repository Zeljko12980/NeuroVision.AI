type BrandLogoProps = {
  variant?: "full" | "icon" | "onDark";
  className?: string;
};

const ICON_SRC = "/images/logo/neurovision-icon.svg";
const LIGHT_SRC = "/images/logo/neurovision-light-horizontal.svg";
const DARK_SRC = "/images/logo/neurovision-dark-horizontal.svg";

export default function BrandLogo({ variant = "full", className }: BrandLogoProps) {
  if (variant === "icon") {
    return (
      <img
        src={ICON_SRC}
        alt="NeuroVision.AI"
        className={className ?? "h-8 w-8"}
      />
    );
  }

  if (variant === "onDark") {
    return (
      <img
        src={DARK_SRC}
        alt="NeuroVision.AI"
        className={className ?? "h-10 w-auto"}
      />
    );
  }

  return (
    <>
      <img
        src={LIGHT_SRC}
        alt="NeuroVision.AI"
        className={`dark:hidden ${className ?? "h-8 w-auto"}`}
      />
      <img
        src={DARK_SRC}
        alt="NeuroVision.AI"
        className={`hidden dark:block ${className ?? "h-8 w-auto"}`}
      />
    </>
  );
}
