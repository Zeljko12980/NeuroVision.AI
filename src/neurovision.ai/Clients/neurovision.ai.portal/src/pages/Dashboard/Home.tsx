import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import GrafanaDashboardEmbed from "../../components/monitoring/GrafanaDashboardEmbed";

export default function Home() {
    const { t } = useTranslation();

    return (
        <>
            <PageMeta
                title={t("dashboard.title")}
                description={t("dashboard.description")}
            />
            <PageBreadcrumb pageTitle={t("dashboard.title")} />
            <GrafanaDashboardEmbed openInGrafanaLabel={t("dashboard.openGrafana")} />
        </>
    );
}
