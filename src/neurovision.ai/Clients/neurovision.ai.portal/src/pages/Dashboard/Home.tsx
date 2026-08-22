import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import GrafanaDashboardEmbed from "../../components/monitoring/GrafanaDashboardEmbed";
import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";

const GRAFANA_ROLES = new Set(["superadministrator", "administrator"]);

export default function Home() {
    const { t } = useTranslation();
    const claims = useAppSelector(selectUserClaims);
    const { role } = getUserInfoFromClaims(claims || {});
    const canViewGrafana = GRAFANA_ROLES.has(role.toLowerCase());

    return (
        <>
            <PageMeta
                title={t(canViewGrafana ? "dashboard.title" : "dashboard.welcomeTitle")}
                description={t(
                    canViewGrafana
                        ? "dashboard.description"
                        : "dashboard.welcomeDescription"
                )}
            />
            <PageBreadcrumb
                pageTitle={t(canViewGrafana ? "dashboard.title" : "dashboard.welcomeTitle")}
            />
            {canViewGrafana ? (
                <GrafanaDashboardEmbed openInGrafanaLabel={t("dashboard.openGrafana")} />
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
