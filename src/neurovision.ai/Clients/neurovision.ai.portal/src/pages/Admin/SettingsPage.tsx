import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";
import LanguageToggler from "../../components/common/LanguageToggler";
import { ThemeToggleButton } from "../../components/common/ThemeToggleButton";
import { useTheme } from "../../context/ThemeContext";
import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5000/api";
const GRAFANA_URL = (import.meta.env.VITE_GRAFANA_URL ?? "http://localhost:3000").replace(
    /\/$/,
    ""
);

export default function SettingsPage() {
    const { t } = useTranslation();
    const { theme } = useTheme();
    const claims = useAppSelector(selectUserClaims);
    const { name, email, role } = getUserInfoFromClaims(claims || {});

    const links = [
        { to: "/admin/health", label: t("settings.links.health") },
        { to: "/admin/ai-monitoring", label: t("settings.links.aiMonitoring") },
        { to: "/admin/logs", label: t("settings.links.logs") },
        { to: "/profile", label: t("settings.links.profile") },
        { to: "/admin/certificates", label: t("settings.links.certificates") },
    ];

    return (
        <>
            <PageMeta title={t("settings.pageTitle")} description={t("settings.pageDescription")} />
            <PageBreadcrumb pageTitle={t("settings.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("settings.appearanceTitle")} desc={t("settings.appearanceDescription")}>
                    <div className="grid gap-6 sm:grid-cols-2">
                        <div>
                            <p className="mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                {t("settings.theme")}
                            </p>
                            <div className="flex items-center gap-3">
                                <ThemeToggleButton />
                                <span className="text-sm text-gray-600 dark:text-gray-400">
                                    {theme === "dark" ? t("settings.themeDark") : t("settings.themeLight")}
                                </span>
                            </div>
                        </div>
                        <div>
                            <p className="mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                {t("settings.language")}
                            </p>
                            <LanguageToggler />
                        </div>
                    </div>
                </ComponentCard>

                <ComponentCard title={t("settings.accountTitle")}>
                    <dl className="grid gap-4 sm:grid-cols-3">
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.account.name")}
                            </dt>
                            <dd className="mt-1 text-sm text-gray-800 dark:text-white/90">{name || "—"}</dd>
                        </div>
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.account.email")}
                            </dt>
                            <dd className="mt-1 text-sm text-gray-800 dark:text-white/90">{email || "—"}</dd>
                        </div>
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.account.role")}
                            </dt>
                            <dd className="mt-1 text-sm text-gray-800 dark:text-white/90">{role || "—"}</dd>
                        </div>
                    </dl>
                    <Link
                        to="/profile"
                        className="inline-flex text-sm font-medium text-brand-600 hover:underline dark:text-brand-400"
                    >
                        {t("settings.links.editProfile")}
                    </Link>
                </ComponentCard>

                <ComponentCard title={t("settings.environmentTitle")} desc={t("settings.environmentDescription")}>
                    <dl className="grid gap-4 sm:grid-cols-3">
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.env.api")}
                            </dt>
                            <dd className="mt-1 break-all font-mono text-sm text-gray-800 dark:text-white/90">
                                {API_URL}
                            </dd>
                        </div>
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.env.grafana")}
                            </dt>
                            <dd className="mt-1">
                                <a
                                    href={GRAFANA_URL}
                                    target="_blank"
                                    rel="noreferrer"
                                    className="break-all font-mono text-sm text-brand-600 hover:underline dark:text-brand-400"
                                >
                                    {GRAFANA_URL}
                                </a>
                            </dd>
                        </div>
                        <div>
                            <dt className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                                {t("settings.env.mode")}
                            </dt>
                            <dd className="mt-1 font-mono text-sm text-gray-800 dark:text-white/90">
                                {import.meta.env.MODE}
                            </dd>
                        </div>
                    </dl>
                </ComponentCard>

                <ComponentCard title={t("settings.linksTitle")}>
                    <ul className="grid gap-3 sm:grid-cols-2">
                        {links.map((item) => (
                            <li key={item.to}>
                                <Link
                                    to={item.to}
                                    className="flex rounded-xl border border-gray-200 px-4 py-3 text-sm font-medium text-gray-700 hover:bg-gray-50 dark:border-gray-800 dark:text-gray-200 dark:hover:bg-white/[0.03]"
                                >
                                    {item.label}
                                </Link>
                            </li>
                        ))}
                    </ul>
                </ComponentCard>
            </div>
        </>
    );
}
