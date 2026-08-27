import { useEffect, useMemo, useState, type ReactNode } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Badge from "../../components/ui/badge/Badge";
import { getAppointmentCatalogs, getAppointments } from "../../features/appointment/appointmentService";
import type { AppointmentResponse, CatalogItem } from "../../features/appointment/appointment.types";
import { getDoctorById } from "../../features/doctor/doctorService";
import type { DoctorResponse } from "../../features/doctor/doctor.types";
import { getNotificationInbox } from "../../features/notification/notificationService";
import type { NotificationResponse } from "../../features/notification/notification.types";
import { getPatientById, getPatients } from "../../features/patient/patientService";
import type { PatientResponse } from "../../features/patient/patient.types";
import {
    fetchScans,
    fetchStatistics,
    searchAnalyses,
    searchAnalysisReports,
} from "../../features/tumorDetection/tumorDetection.service";
import type {
    AnalysisResponse,
    AnalysisStatisticsResponse,
    BrainScanResponse,
} from "../../features/tumorDetection/tumorDetection.types";
import { formatPatientName, formatTumorClass, primaryFindingClass, tumorStatusColor } from "../TumorDetection/tumorUtils";
import { CalenderIcon, DocsIcon, FolderIcon, GroupIcon, TimeIcon, UserIcon } from "../../icons";

type RoleKind = "doctor" | "patient";

type ClinicalHomeProps = {
    role: RoleKind;
    userId: string;
    displayName: string;
};

const SCHEDULED = new Set(["SCHD"]);
const ACTIVE_ANALYSIS = new Set(["Pending", "Processing"]);

const settle = async <T,>(promise: Promise<T>, fallback: T): Promise<T> => {
    try {
        return await promise;
    } catch {
        return fallback;
    }
};

const startOfDay = (value: Date) => {
    const date = new Date(value);
    date.setHours(0, 0, 0, 0);
    return date;
};

const addDays = (value: Date, days: number) => {
    const date = new Date(value);
    date.setDate(date.getDate() + days);
    return date;
};

const catalogName = (items: CatalogItem[], code: string) =>
    items.find((item) => item.code === code)?.name ?? code;

const personName = (first?: string, last?: string) =>
    [first, last].filter(Boolean).join(" ").trim();

function StatCard({
    label,
    value,
    hint,
    icon,
}: {
    label: string;
    value: string | number;
    hint?: string;
    icon: ReactNode;
}) {
    return (
        <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
            <div className="flex items-center justify-between">
                <p className="text-sm text-gray-500 dark:text-gray-400">{label}</p>
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gray-100 text-gray-700 dark:bg-white/[0.06] dark:text-white/80">
                    {icon}
                </div>
            </div>
            <p className="mt-3 text-2xl font-semibold text-gray-800 dark:text-white/90">{value}</p>
            {hint ? <p className="mt-1 text-xs text-gray-400">{hint}</p> : null}
        </div>
    );
}

function QuickLink({ to, label }: { to: string; label: string }) {
    return (
        <Link
            to={to}
            className="inline-flex items-center rounded-lg border border-gray-200 px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.04]"
        >
            {label}
        </Link>
    );
}

export default function ClinicalHome({ role, userId, displayName }: ClinicalHomeProps) {
    const { t, i18n } = useTranslation();
    const isDoctor = role === "doctor";
    const locale = i18n.language || "sr";

    const [loading, setLoading] = useState(true);
    const [appointments, setAppointments] = useState<AppointmentResponse[]>([]);
    const [types, setTypes] = useState<CatalogItem[]>([]);
    const [patients, setPatients] = useState<PatientResponse[]>([]);
    const [patientCount, setPatientCount] = useState(0);
    const [profile, setProfile] = useState<PatientResponse | null>(null);
    const [doctor, setDoctor] = useState<DoctorResponse | null>(null);
    const [analyses, setAnalyses] = useState<AnalysisResponse[]>([]);
    const [scans, setScans] = useState<BrainScanResponse[]>([]);
    const [scanTotal, setScanTotal] = useState(0);
    const [reportTotal, setReportTotal] = useState(0);
    const [stats, setStats] = useState<AnalysisStatisticsResponse | null>(null);
    const [notifications, setNotifications] = useState<NotificationResponse[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);

    const formatDateTime = (value: string) => {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return value;
        return date.toLocaleString(locale, {
            day: "2-digit",
            month: "short",
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    useEffect(() => {
        if (!userId) return;

        const now = new Date();
        const from = startOfDay(now).toISOString();
        const to = addDays(now, 14).toISOString();
        const patientId = isDoctor ? undefined : userId;
        const doctorId = isDoctor ? userId : undefined;

        let cancelled = false;
        setLoading(true);

        void (async () => {
            const [
                appointmentItems,
                catalogs,
                patientPage,
                analysisPage,
                scanPage,
                reportPage,
                statistics,
                inbox,
                ownProfile,
            ] = await Promise.all([
                settle(getAppointments({ from, to, patientId, doctorId }), []),
                settle(getAppointmentCatalogs(), { types: [], statuses: [] }),
                isDoctor
                    ? settle(getPatients(0, 50), { data: [], count: 0, pageIndex: 0, pageSize: 50 })
                    : Promise.resolve({ data: [], count: 0, pageIndex: 0, pageSize: 50 }),
                settle(searchAnalyses({ patientId, page: 1, pageSize: 8, archived: false }), {
                    items: [],
                    total: 0,
                    page: 1,
                    pageSize: 8,
                }),
                settle(fetchScans(patientId, 1, 5), { items: [], total: 0, page: 1, pageSize: 5 }),
                settle(searchAnalysisReports({ patientId, page: 1, pageSize: 1 }), {
                    items: [],
                    total: 0,
                    page: 1,
                    pageSize: 1,
                }),
                settle(fetchStatistics(), { totalCompletedAnalyses: 0, totalScans: 0 }),
                settle(getNotificationInbox(userId, 5), { items: [], unreadCount: 0 }),
                isDoctor ? Promise.resolve(null) : settle(getPatientById(userId), null),
            ]);

            let assignedDoctor: DoctorResponse | null = null;
            if (ownProfile?.assignedDoctorId) {
                assignedDoctor = await settle(getDoctorById(ownProfile.assignedDoctorId), null);
            }

            if (cancelled) return;

            setAppointments(appointmentItems);
            setTypes(catalogs.types);
            setPatients(patientPage.data);
            setPatientCount(patientPage.count);
            setAnalyses(analysisPage.items);
            setScans(scanPage.items);
            setScanTotal(scanPage.total);
            setReportTotal(reportPage.total);
            setStats(statistics);
            setNotifications(inbox.items);
            setUnreadCount(inbox.unreadCount);
            setProfile(ownProfile);
            setDoctor(assignedDoctor);
            setLoading(false);
        })();

        return () => {
            cancelled = true;
        };
    }, [isDoctor, userId]);

    const greetingKey = useMemo(() => {
        const hour = new Date().getHours();
        if (hour < 12) return "dashboard.greetingMorning";
        if (hour < 18) return "dashboard.greetingAfternoon";
        return "dashboard.greetingEvening";
    }, []);

    const now = Date.now();
    const todayStart = startOfDay(new Date()).getTime();
    const tomorrowStart = addDays(startOfDay(new Date()), 1).getTime();

    const upcoming = appointments
        .filter((item) => SCHEDULED.has(item.statusCode) && new Date(item.startsAt).getTime() >= now)
        .sort((a, b) => new Date(a.startsAt).getTime() - new Date(b.startsAt).getTime())
        .slice(0, 5);

    const todayCount = appointments.filter((item) => {
        const start = new Date(item.startsAt).getTime();
        return SCHEDULED.has(item.statusCode) && start >= todayStart && start < tomorrowStart;
    }).length;

    const inProgress = analyses.filter((item) => ACTIVE_ANALYSIS.has(item.status)).length;
    const latestAnalyses = analyses.slice(0, 5);
    const analysisBase = isDoctor ? "/analysis" : "/my-analysis";
    const firstName = displayName.split(" ")[0] || displayName;

    return (
        <div className="space-y-6">
            <div className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                <p className="text-sm text-gray-500 dark:text-gray-400">{t(greetingKey)}</p>
                <h3 className="mt-1 text-2xl font-semibold text-gray-800 dark:text-white/90">
                    {t(isDoctor ? "dashboard.doctorHeadline" : "dashboard.patientHeadline", {
                        name: firstName || t("dashboard.colleague"),
                    })}
                </h3>
                <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                    {t(isDoctor ? "dashboard.doctorSubtitle" : "dashboard.patientSubtitle")}
                </p>
                <div className="mt-4 flex flex-wrap gap-2">
                    {isDoctor ? (
                        <>
                            <QuickLink to="/calendar" label={t("dashboard.actions.calendar")} />
                            <QuickLink to="/patients/list" label={t("dashboard.actions.patients")} />
                            <QuickLink to="/scans/add" label={t("dashboard.actions.addScan")} />
                            <QuickLink to="/analysis/new" label={t("dashboard.actions.analyses")} />
                            <QuickLink to="/reports" label={t("dashboard.actions.reports")} />
                        </>
                    ) : (
                        <>
                            <QuickLink to="/calendar" label={t("dashboard.actions.calendar")} />
                            <QuickLink to="/my-scans/upload" label={t("dashboard.actions.uploadScan")} />
                            <QuickLink to="/my-analysis/new" label={t("dashboard.actions.myAnalyses")} />
                            <QuickLink to="/my-reports" label={t("dashboard.actions.reports")} />
                            <QuickLink to="/notifications" label={t("dashboard.actions.notifications")} />
                        </>
                    )}
                </div>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
                <StatCard
                    label={t("dashboard.stats.todayAppointments")}
                    value={loading ? "—" : todayCount}
                    hint={t("dashboard.stats.upcomingHint", { count: upcoming.length })}
                    icon={<CalenderIcon className="h-5 w-5" />}
                />
                <StatCard
                    label={t(isDoctor ? "dashboard.stats.patients" : "dashboard.stats.scans")}
                    value={loading ? "—" : isDoctor ? patientCount : scanTotal}
                    hint={isDoctor ? t("dashboard.stats.patientsHint") : t("dashboard.stats.scansHint")}
                    icon={isDoctor ? <GroupIcon className="h-5 w-5" /> : <FolderIcon className="h-5 w-5" />}
                />
                <StatCard
                    label={t("dashboard.stats.inProgress")}
                    value={loading ? "—" : inProgress}
                    hint={t("dashboard.stats.completedAnalyses", {
                        count: stats?.totalCompletedAnalyses ?? 0,
                    })}
                    icon={<TimeIcon className="h-5 w-5" />}
                />
                <StatCard
                    label={t("dashboard.stats.unread")}
                    value={loading ? "—" : unreadCount}
                    hint={t("dashboard.stats.reportsHint", { count: reportTotal })}
                    icon={<DocsIcon className="h-5 w-5" />}
                />
            </div>

            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
                <section className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                    <div className="mb-4 flex items-center justify-between">
                        <h4 className="text-base font-semibold text-gray-800 dark:text-white/90">
                            {t("dashboard.upcomingTitle")}
                        </h4>
                        <Link to="/calendar" className="text-sm text-brand-600 hover:underline dark:text-brand-400">
                            {t("dashboard.viewAll")}
                        </Link>
                    </div>
                    {loading ? (
                        <p className="text-sm text-gray-400">{t("dashboard.loading")}</p>
                    ) : upcoming.length === 0 ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">{t("dashboard.emptyAppointments")}</p>
                    ) : (
                        <ul className="divide-y divide-gray-100 dark:divide-gray-800">
                            {upcoming.map((item) => (
                                <li key={item.id} className="flex items-start justify-between gap-3 py-3">
                                    <div>
                                        <p className="font-medium text-gray-800 dark:text-white/90">{item.title}</p>
                                        <p className="text-sm text-gray-500">
                                            {catalogName(types, item.typeCode)}
                                            {isDoctor
                                                ? ` · ${formatPatientName(patients, item.patientId)}`
                                                : doctor
                                                  ? ` · ${personName(doctor.firstName, doctor.lastName)}`
                                                  : ""}
                                        </p>
                                    </div>
                                    <span className="shrink-0 text-sm text-gray-500">
                                        {formatDateTime(item.startsAt)}
                                    </span>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>

                <section className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                    <div className="mb-4 flex items-center justify-between">
                        <h4 className="text-base font-semibold text-gray-800 dark:text-white/90">
                            {t("dashboard.analysesTitle")}
                        </h4>
                        <Link
                            to={`${analysisBase}/new`}
                            className="text-sm text-brand-600 hover:underline dark:text-brand-400"
                        >
                            {t("dashboard.viewAll")}
                        </Link>
                    </div>
                    {loading ? (
                        <p className="text-sm text-gray-400">{t("dashboard.loading")}</p>
                    ) : latestAnalyses.length === 0 ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">{t("dashboard.emptyAnalyses")}</p>
                    ) : (
                        <ul className="divide-y divide-gray-100 dark:divide-gray-800">
                            {latestAnalyses.map((item) => (
                                <li key={item.id} className="py-3">
                                    <Link
                                        to={`${analysisBase}/${item.id}`}
                                        className="flex items-start justify-between gap-3"
                                    >
                                        <div>
                                            <p className="font-medium text-gray-800 dark:text-white/90">
                                                {item.scanFileName}
                                            </p>
                                            <p className="text-sm text-gray-500">
                                                {formatTumorClass(primaryFindingClass(item), t)}
                                                {item.overallConfidence != null
                                                    ? ` · ${(item.overallConfidence * 100).toFixed(0)}%`
                                                    : ""}
                                                {isDoctor ? ` · ${formatPatientName(patients, item.patientId)}` : ""}
                                            </p>
                                        </div>
                                        <Badge color={tumorStatusColor(item.status)} size="sm">
                                            {t(`tumor.status.${item.status}`, item.status)}
                                        </Badge>
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>
            </div>

            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
                <section className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                    <div className="mb-4 flex items-center justify-between">
                        <h4 className="text-base font-semibold text-gray-800 dark:text-white/90">
                            {t("dashboard.scansTitle")}
                        </h4>
                        <Link
                            to={isDoctor ? "/scans/list" : "/my-scans"}
                            className="text-sm text-brand-600 hover:underline dark:text-brand-400"
                        >
                            {t("dashboard.viewAll")}
                        </Link>
                    </div>
                    {loading ? (
                        <p className="text-sm text-gray-400">{t("dashboard.loading")}</p>
                    ) : scans.length === 0 ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">{t("dashboard.emptyScans")}</p>
                    ) : (
                        <ul className="divide-y divide-gray-100 dark:divide-gray-800">
                            {scans.map((scan) => (
                                <li key={scan.id} className="flex items-center justify-between py-3 text-sm">
                                    <div>
                                        <p className="font-medium text-gray-800 dark:text-white/90">{scan.fileName}</p>
                                        <p className="text-gray-500">
                                            {t(`tumor.scanTypes.${scan.scanType.toLowerCase()}`, scan.scanType)}
                                            {` · ${t("dashboard.analysisCount", { count: scan.analysisCount })}`}
                                        </p>
                                    </div>
                                    <span className="text-gray-500">{formatDateTime(scan.uploadedAt)}</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>

                <section className="rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
                    {isDoctor ? (
                        <>
                            <div className="mb-4 flex items-center justify-between">
                                <h4 className="text-base font-semibold text-gray-800 dark:text-white/90">
                                    {t("dashboard.notificationsTitle")}
                                </h4>
                                <Link
                                    to="/notifications"
                                    className="text-sm text-brand-600 hover:underline dark:text-brand-400"
                                >
                                    {t("dashboard.viewAll")}
                                </Link>
                            </div>
                            {loading ? (
                                <p className="text-sm text-gray-400">{t("dashboard.loading")}</p>
                            ) : notifications.length === 0 ? (
                                <p className="text-sm text-gray-500 dark:text-gray-400">
                                    {t("dashboard.emptyNotifications")}
                                </p>
                            ) : (
                                <ul className="divide-y divide-gray-100 dark:divide-gray-800">
                                    {notifications.map((item) => (
                                        <li key={item.id} className="py-3">
                                            <p className="font-medium text-gray-800 dark:text-white/90">{item.title}</p>
                                            <p className="line-clamp-2 text-sm text-gray-500">{item.message}</p>
                                            <p className="mt-1 text-xs text-gray-400">
                                                {formatDateTime(item.createdAt)}
                                            </p>
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </>
                    ) : (
                        <>
                            <h4 className="mb-4 text-base font-semibold text-gray-800 dark:text-white/90">
                                {t("dashboard.careTeamTitle")}
                            </h4>
                            {loading ? (
                                <p className="text-sm text-gray-400">{t("dashboard.loading")}</p>
                            ) : (
                                <div className="space-y-4 text-sm">
                                    <div className="flex items-center gap-3">
                                        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gray-100 dark:bg-white/[0.06]">
                                            <UserIcon className="h-5 w-5 text-gray-600 dark:text-white/80" />
                                        </div>
                                        <div>
                                            <p className="font-medium text-gray-800 dark:text-white/90">
                                                {doctor
                                                    ? personName(doctor.firstName, doctor.lastName)
                                                    : t("dashboard.noAssignedDoctor")}
                                            </p>
                                            <p className="text-gray-500">
                                                {doctor?.currentSpecializationCode ||
                                                    doctor?.currentInstitutionName ||
                                                    t("dashboard.assignedDoctorHint")}
                                            </p>
                                        </div>
                                    </div>
                                    <div className="grid grid-cols-2 gap-3">
                                        <div className="rounded-xl bg-gray-50 p-3 dark:bg-white/[0.04]">
                                            <p className="text-xs text-gray-400">{t("dashboard.profile.institution")}</p>
                                            <p className="mt-1 font-medium text-gray-800 dark:text-white/90">
                                                {profile?.currentInstitutionName || "—"}
                                            </p>
                                        </div>
                                        <div className="rounded-xl bg-gray-50 p-3 dark:bg-white/[0.04]">
                                            <p className="text-xs text-gray-400">{t("dashboard.profile.bloodType")}</p>
                                            <p className="mt-1 font-medium text-gray-800 dark:text-white/90">
                                                {profile?.bloodTypeCode || "—"}
                                            </p>
                                        </div>
                                    </div>
                                    {unreadCount > 0 ? (
                                        <Link to="/notifications" className="text-sm text-brand-600 hover:underline">
                                            {t("dashboard.unreadLink", { count: unreadCount })}
                                        </Link>
                                    ) : null}
                                </div>
                            )}
                        </>
                    )}
                </section>
            </div>
        </div>
    );
}
