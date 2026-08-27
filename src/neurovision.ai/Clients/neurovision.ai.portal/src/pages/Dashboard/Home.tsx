import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import GrafanaDashboardEmbed from "../../components/monitoring/GrafanaDashboardEmbed";
import ClinicalHome from "./ClinicalHome";
import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";

const GRAFANA_ROLES = new Set([
    "superadministrator",
    "superadministrator",
    "administrator",
]);

export default function Home() {
    const { t } = useTranslation();
    const claims = useAppSelector(selectUserClaims);
    const { role, name, userId } = getUserInfoFromClaims(claims || {});
    const normalizedRole = role.toLowerCase();
    const canViewGrafana = GRAFANA_ROLES.has(normalizedRole);
    const clinicalRole =
        normalizedRole === "doctor" || normalizedRole === "patient"
            ? (normalizedRole as "doctor" | "patient")
            : null;
    const pageTitle = canViewGrafana
        ? t("dashboard.title")
        : clinicalRole
          ? t("dashboard.homeTitle")
          : t("dashboard.welcomeTitle");

    return (
        <>
            <PageMeta
                title={pageTitle}
                description={t(
                    canViewGrafana
                        ? "dashboard.description"
                        : clinicalRole
                          ? "dashboard.homeDescription"
                          : "dashboard.welcomeDescription"
                )}
            />
            <PageBreadcrumb pageTitle={pageTitle} />
            {canViewGrafana ? (
                <GrafanaDashboardEmbed openInGrafanaLabel={t("dashboard.openGrafana")} />
            ) : clinicalRole ? (
                <ClinicalHome role={clinicalRole} userId={userId} displayName={name} />
            ) : (
                <div className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                    <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                        {t("dashboard.welcomeTitle")}
                    </h3>
                    <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                        {t("dashboard.welcomeDescription")}
                    </p>
                </div>
            )}
        </>
    );
}
