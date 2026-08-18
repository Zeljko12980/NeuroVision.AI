import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "./store/store";
import { isTokenExpired } from "./utils/jwt";
import { logout } from "./features/auth/authSlice";

import GlobalAlert from "./components/ui/alert/GlobalAlert";

import SignIn from "./pages/Auth/SignIn";
import TwoFA from "./pages/Auth/TwoFA";
import NotFound from "./pages/OtherPage/NotFound";
import Home from "./pages/Dashboard/Home";
import UserProfiles from "./pages/UserProfiles";
import Calendar from "./pages/Calendar";
import Blank from "./pages/Blank";
import FormElements from "./pages/Forms/FormElements";
import RolesTable from "./pages/Tables/RolesTable";
import Videos from "./pages/UiElements/Videos";
import Images from "./pages/UiElements/Images";
import Alerts from "./pages/UiElements/Alerts";
import Badges from "./pages/UiElements/Badges";
import Avatars from "./pages/UiElements/Avatars";
import Buttons from "./pages/UiElements/Buttons";
import LineChart from "./pages/Charts/LineChart";
import BarChart from "./pages/Charts/BarChart";
import ConfirmEmailPage from "./pages/Auth/ConfirmEmail";
import CreateRolePage from "./pages/CreateRolePage";
import CreateDoctorPage from "./pages/Doctor/CreateDoctorPage";
import SetPasswordPage from "./pages/Auth/SetPasswordPage";
import ResetPasswordPage from "./pages/Auth/ResetPasswordPage";

import AppLayout from "./layout/AppLayout";
import PrivateRoute from "./components/common/PrivateRoute";
import RoleRoute from "./components/common/RoleRoute";
import { ScrollToTop } from "./components/common/ScrollToTop";
import DoctorsTable from "./pages/Tables/DoctorsTable";
import PdfTable from "./pages/Pdf/PdfTable";
import CreatePdfTemplatePage from "./pages/Pdf/CreatePdfTemplatePage";

import CountriesTable from "./pages/Location/country/CountriesTable";
import CreateCountryPage from "./pages/Location/country/CreateCountryPage";
import SettlementsTable from "./pages/Location/settlements/SettlementsTable";
import CreateSettlementPage from "./pages/Location/settlements/CreateSettlementPage";
import CapitalsTable from "./pages/Location/capital/CapitalsTable";
import CreateCapitalPage from "./pages/Location/capital/CreateCapitalPage";
import MunicipalitiesTable from "./pages/Location/municipality/MunicipalitiesTable";
import CreateMunicipalityPage from "./pages/Location/municipality/CreateMunicipalityPage";
import RegionsTable from "./pages/Location/region/RegionsTable";
import CreateRegionPage from "./pages/Location/region/CreateRegionPage";
import GovernmentTypesTable from "./pages/Location/governmentType/GovernmentTypesTable";
import CreateGovernmentTypePage from "./pages/Location/governmentType/CreateGovernmentTypePage";
import RegionTypesTable from "./pages/Location/regionType/RegionTypesTable";
import CreateRegionTypePage from "./pages/Location/regionType/CreateRegionTypePage";
import LocalCommunitiesTable from "./pages/Location/localCommunity/LocalCommunitiesTable";
import CreateLocalCommunityPage from "./pages/Location/localCommunity/CreateLocalCommunityPage";
import HealthInstitutionsTable from "./pages/Location/healthInstitutions/HealthInstitutionsTable";
import CreateHealthInstitutionPage from "./pages/Location/healthInstitutions/CreateHealthInstitutionPage";
import HealthInstitutionTypesTable from "./pages/Location/healthInstitutionsType/HealthInstitutionTypesTable";
import CreateHealthInstitutionTypePage from "./pages/Location/healthInstitutionsType/CreateHealthInstitutionTypePage";
import CountryCompositionsTable from "./pages/Location/countryComposition/CountryCompositionsTable";
import CreateCountryCompositionPage from "./pages/Location/countryComposition/CreateCountryCompositionPage";
import RegionCompositionsTable from "./pages/Location/regionComposition/RegionCompositionsTable";
import CreateRegionCompositionPage from "./pages/Location/regionComposition/CreateRegionCompositionPage";
import RegionSettlementCoveragesTable from "./pages/Location/regionSettlementCoverage/RegionSettlementCoveragesTable";
import CreateRegionSettlementCoveragePage from "./pages/Location/regionSettlementCoverage/CreateRegionSettlementCoveragePage";
import MunicipalitySettlementCoveragesTable from "./pages/Location/municipalitySettlementCoverage/MunicipalitySettlementCoveragesTable";
import CreateMunicipalitySettlementCoveragePage from "./pages/Location/municipalitySettlementCoverage/CreateMunicipalitySettlementCoveragePage";
import LocalCommunityCoveragesTable from "./pages/Location/localCommunityCoverage/LocalCommunityCoveragesTable";
import CreateLocalCommunityCoveragePage from "./pages/Location/localCommunityCoverage/CreateLocalCommunityCoveragePage";
import LegalSuccessorsTable from "./pages/Location/legalSuccessor/LegalSuccessorsTable";
import CreateLegalSuccessorPage from "./pages/Location/legalSuccessor/CreateLegalSuccessorPage";
import GovernmentHistoriesTable from "./pages/Location/governmentHistory/GovernmentHistoriesTable";
import CreateGovernmentHistoryPage from "./pages/Location/governmentHistory/CreateGovernmentHistoryPage";
import HealthPage from "./pages/Health/HealthPage";
import ScansTable from "./pages/TumorDetection/ScansTable";
import UploadScanPage from "./pages/TumorDetection/UploadScanPage";
import AnalysesTable from "./pages/TumorDetection/AnalysesTable";
import AnalysisDetailPage from "./pages/TumorDetection/AnalysisDetailPage";
import AiMonitoringPage from "./pages/TumorDetection/AiMonitoringPage";
import ReportsTable from "./pages/TumorDetection/ReportsTable";

export default function App() {
    const token = useAppSelector((state) => state.auth.token);
    const dispatch = useAppDispatch();

    useEffect(() => {
        if (token && isTokenExpired(token)) {
            dispatch(logout());
        }
    }, [token, dispatch]);

    return (
        <>
            <GlobalAlert />

            <Router>
                <ScrollToTop />

                <Routes>
                    <Route path="/signin" element={<SignIn />} />
                    <Route path="/confirm-2fa" element={<TwoFA />} />
                    <Route path="/confirm-email" element={<ConfirmEmailPage />} />
                    <Route path="/set-password" element={<SetPasswordPage />} />
                    <Route path="/reset-password" element={<ResetPasswordPage />} />
                    <Route path="*" element={<NotFound />} />

                    <Route
                        element={
                            <PrivateRoute>
                                <AppLayout />
                            </PrivateRoute>
                        }
                    >
                        <Route index path="/" element={<Home />} />

                        <Route element={<RoleRoute allowedRoles={["doctor", "superadministrator"]} />}>
                            <Route path="/scans/list" element={<ScansTable translationKey="doctor" />} />
                            <Route path="/scans/add" element={<UploadScanPage redirectPath="/scans/list" translationKey="doctor" />} />
                            <Route path="/analysis/new" element={<AnalysesTable detailPathPrefix="/analysis" translationKey="doctor" />} />
                            <Route path="/analysis/archive" element={<AnalysesTable detailPathPrefix="/analysis" translationKey="doctor" archived />} />
                            <Route path="/analysis/:analysisId" element={<AnalysisDetailPage detailPathPrefix="/analysis" translationKey="doctor" />} />
                            <Route path="/reports" element={<ReportsTable detailPathPrefix="/analysis" translationKey="doctor" />} />
                            <Route path="/ai-monitoring" element={<AiMonitoringPage />} />
                        </Route>

                        <Route path="/my-scans" element={<ScansTable translationKey="patient" />} />
                        <Route path="/my-scans/upload" element={<UploadScanPage redirectPath="/my-scans" translationKey="patient" />} />
                        <Route path="/my-analysis/new" element={<AnalysesTable detailPathPrefix="/my-analysis" translationKey="patient" />} />
                        <Route path="/my-analysis/archive" element={<AnalysesTable detailPathPrefix="/my-analysis" translationKey="patient" archived />} />
                        <Route path="/my-analysis/:analysisId" element={<AnalysisDetailPage detailPathPrefix="/my-analysis" translationKey="patient" />} />
                        <Route path="/my-reports" element={<ReportsTable detailPathPrefix="/my-analysis" translationKey="patient" />} />

                        <Route
                            element={<RoleRoute allowedRoles={["superadministrator"]} />}
                        >
                            <Route path="/admin/roles" element={<RolesTable />} />
                            <Route path="/admin/roles/create" element={<CreateRolePage />} />
                            <Route path="/admin/doctors" element={<DoctorsTable />} />
                            <Route path="/admin/doctors/add" element={<CreateDoctorPage />} />
                            <Route path="/admin/pdfs" element={<PdfTable />} />
                            <Route path="/admin/pdfs/create" element={<CreatePdfTemplatePage />} />

                            <Route path="/admin/location/countries" element={<CountriesTable />} />
                            <Route path="/admin/location/countries/create" element={<CreateCountryPage />} />
                            <Route path="/admin/location/settlements" element={<SettlementsTable />} />
                            <Route path="/admin/location/settlements/create" element={<CreateSettlementPage />} />
                            <Route path="/admin/location/capitals" element={<CapitalsTable />} />
                            <Route path="/admin/location/capitals/create" element={<CreateCapitalPage />} />
                            <Route path="/admin/location/municipalities" element={<MunicipalitiesTable />} />
                            <Route path="/admin/location/municipalities/create" element={<CreateMunicipalityPage />} />
                            <Route path="/admin/location/regions" element={<RegionsTable />} />
                            <Route path="/admin/location/regions/create" element={<CreateRegionPage />} />
                            <Route path="/admin/location/government-types" element={<GovernmentTypesTable />} />
                            <Route path="/admin/location/gov/create" element={<CreateGovernmentTypePage />} />
                            <Route path="/admin/location/region-types" element={<RegionTypesTable />} />
                            <Route path="/admin/location/region-types/create" element={<CreateRegionTypePage />} />
                            <Route path="/admin/location/local-communities" element={<LocalCommunitiesTable />} />
                            <Route path="/admin/location/local-communities/create" element={<CreateLocalCommunityPage />} />
                            <Route path="/admin/location/health-institutions" element={<HealthInstitutionsTable />} />
                            <Route path="/admin/location/health-institutions/create" element={<CreateHealthInstitutionPage />} />
                            <Route path="/admin/location/health-institutions-types" element={<HealthInstitutionTypesTable />} />
                            <Route path="/admin/location/health-institutions-types/create" element={<CreateHealthInstitutionTypePage />} />
                            <Route path="/admin/location/country-compositions" element={<CountryCompositionsTable />} />
                            <Route path="/admin/location/country-compositions/create" element={<CreateCountryCompositionPage />} />
                            <Route path="/admin/location/region-compositions" element={<RegionCompositionsTable />} />
                            <Route path="/admin/location/region-compositions/create" element={<CreateRegionCompositionPage />} />
                            <Route path="/admin/location/region-settlement-coverages" element={<RegionSettlementCoveragesTable />} />
                            <Route path="/admin/location/region-settlement-coverages/create" element={<CreateRegionSettlementCoveragePage />} />
                            <Route path="/admin/location/municipality-settlement-coverages" element={<MunicipalitySettlementCoveragesTable />} />
                            <Route path="/admin/location/municipality-settlement-coverages/create" element={<CreateMunicipalitySettlementCoveragePage />} />
                            <Route path="/admin/location/local-community-coverages" element={<LocalCommunityCoveragesTable />} />
                            <Route path="/admin/location/local-community-coverages/create" element={<CreateLocalCommunityCoveragePage />} />
                            <Route path="/admin/location/legal-successors" element={<LegalSuccessorsTable />} />
                            <Route path="/admin/location/legal-successors/create" element={<CreateLegalSuccessorPage />} />
                            <Route path="/admin/location/government-histories" element={<GovernmentHistoriesTable />} />
                            <Route path="/admin/location/government-histories/create" element={<CreateGovernmentHistoryPage />} />

                            <Route path="/admin/health" element={<HealthPage />} />
                            <Route path="/admin/ai-monitoring" element={<AiMonitoringPage />} />
                            <Route path="/admin/reports" element={<ReportsTable detailPathPrefix="/analysis" translationKey="doctor" />} />
                        </Route>

                        <Route path="/profile" element={<UserProfiles />} />
                        <Route path="/calendar" element={<Calendar />} />
                        <Route path="/blank" element={<Blank />} />
                        <Route path="/form-elements" element={<FormElements />} />

                        <Route path="/alerts" element={<Alerts />} />
                        <Route path="/avatars" element={<Avatars />} />
                        <Route path="/badge" element={<Badges />} />
                        <Route path="/buttons" element={<Buttons />} />
                        <Route path="/images" element={<Images />} />
                        <Route path="/videos" element={<Videos />} />

                        <Route path="/line-chart" element={<LineChart />} />
                        <Route path="/bar-chart" element={<BarChart />} />
                    </Route>
                </Routes>
            </Router>
        </>
    );
}
