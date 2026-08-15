 import { t } from "i18next";
import ComponentCard from "../../components/common/ComponentCard";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";

export default function DoctorsTable() {
    return (
        <>
            <PageMeta
                title={t("doctors.pageTitle")}
                description={t("doctors.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("doctors.pageTitle")} />

            <div className="space-y-6">
                <ComponentCard title={t("doctors.title")}>
                   <></>
                </ComponentCard>
            </div>
        </>
    );
}