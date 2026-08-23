import { useEffect, useMemo, useRef, useState } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import { DateSelectArg, DatesSetArg, EventClickArg, EventContentArg } from "@fullcalendar/core";
import { useTranslation } from "react-i18next";
import { Modal } from "../components/ui/modal";
import { useModal } from "../hooks/useModal";
import PageMeta from "../components/common/PageMeta";
import { useAppDispatch, useAppSelector } from "../store/store";
import { selectUserClaims } from "../selectors/authSelectors";
import { getUserInfoFromClaims } from "../utils/claims";
import {
    cancelAppointment,
    createAppointment,
    fetchAppointmentCatalogs,
    fetchAppointments,
    rescheduleAppointment,
} from "../features/appointment/appointmentSlice";
import { fetchPatients } from "../features/patient/patientSlice";
import { fetchDoctors } from "../features/doctor/doctorSlice";
import { getPatientById } from "../features/patient/patientService";
import { AppointmentResponse } from "../features/appointment/appointment.types";
import {
    defaultSlotFromSelect,
    fromDateTimeLocal,
    toCalendarEvent,
    toDateTimeLocal,
} from "../features/appointment/appointment.utils";

const Calendar: React.FC = () => {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId, role } = getUserInfoFromClaims(claims || {});
    const userRole = role?.toLowerCase() ?? "patient";
    const isPatient = userRole === "patient";
    const isDoctor = userRole === "doctor";
    const isAdmin = userRole === "superadministrator";

    const { items, catalogs, loading, saving, error } = useAppSelector((state) => state.appointment);
    const patients = useAppSelector((state) => state.patient.items);
    const doctors = useAppSelector((state) => state.doctor.items);

    const [selected, setSelected] = useState<AppointmentResponse | null>(null);
    const [title, setTitle] = useState("");
    const [typeCode, setTypeCode] = useState("CONS");
    const [startsAt, setStartsAt] = useState("");
    const [endsAt, setEndsAt] = useState("");
    const [notes, setNotes] = useState("");
    const [patientId, setPatientId] = useState("");
    const [doctorId, setDoctorId] = useState("");
    const [assignedDoctorId, setAssignedDoctorId] = useState("");
    const [range, setRange] = useState<{ from: string; to: string } | null>(null);
    const calendarRef = useRef<FullCalendar>(null);
    const { isOpen, openModal, closeModal } = useModal();

    useEffect(() => {
        void dispatch(fetchAppointmentCatalogs());
        if (isDoctor || isAdmin) {
            void dispatch(fetchPatients({ pageIndex: 0, pageSize: 50 }));
        }
        if (isAdmin) {
            void dispatch(fetchDoctors({ pageIndex: 0, pageSize: 50 }));
        }
    }, [dispatch, isAdmin, isDoctor]);

    useEffect(() => {
        if (!isPatient || !userId) return;
        void getPatientById(userId)
            .then((patient) => setAssignedDoctorId(patient.assignedDoctorId ?? ""))
            .catch(() => setAssignedDoctorId(""));
    }, [isPatient, userId]);

    useEffect(() => {
        if (!range) return;
        void dispatch(
            fetchAppointments({
                from: range.from,
                to: range.to,
                patientId: isPatient ? userId : undefined,
                doctorId: isDoctor ? userId : undefined,
            })
        );
    }, [dispatch, isDoctor, isPatient, range, userId]);

    const events = useMemo(() => items.map(toCalendarEvent), [items]);
    const canEdit = selected ? selected.statusCode === "SCHD" : true;

    const resetModal = () => {
        setSelected(null);
        setTitle("");
        setTypeCode(catalogs?.types[0]?.code ?? "CONS");
        setStartsAt("");
        setEndsAt("");
        setNotes("");
        setPatientId(isPatient ? userId : "");
        setDoctorId(isDoctor ? userId : isPatient ? assignedDoctorId : "");
    };

    const handleDatesSet = (info: DatesSetArg) => {
        const from = info.start.toISOString();
        const to = info.end.toISOString();
        setRange((prev) => (prev?.from === from && prev?.to === to ? prev : { from, to }));
    };

    const handleDateSelect = (selectInfo: DateSelectArg) => {
        resetModal();
        const slot = defaultSlotFromSelect(selectInfo.startStr, selectInfo.endStr);
        setStartsAt(slot.start);
        setEndsAt(slot.end);
        openModal();
    };

    const handleEventClick = (clickInfo: EventClickArg) => {
        const appointment = clickInfo.event.extendedProps.appointment as AppointmentResponse | undefined;
        if (!appointment) return;

        setSelected(appointment);
        setTitle(appointment.title);
        setTypeCode(appointment.typeCode);
        setStartsAt(toDateTimeLocal(appointment.startsAt));
        setEndsAt(toDateTimeLocal(appointment.endsAt));
        setNotes(appointment.notes ?? "");
        setPatientId(appointment.patientId);
        setDoctorId(appointment.doctorId);
        openModal();
    };

    const handleSave = async () => {
        if (!title.trim() || !startsAt || !endsAt) return;

        const resolvedPatientId = isPatient ? userId : patientId;
        const resolvedDoctorId = isDoctor ? userId : isPatient ? assignedDoctorId : doctorId;
        if (!resolvedPatientId || !resolvedDoctorId) return;

        if (selected) {
            const result = await dispatch(
                rescheduleAppointment({
                    id: selected.id,
                    payload: {
                        startsAt: fromDateTimeLocal(startsAt),
                        endsAt: fromDateTimeLocal(endsAt),
                        title: title.trim(),
                        notes: notes.trim() || undefined,
                    },
                })
            );
            if (rescheduleAppointment.fulfilled.match(result)) {
                closeModal();
                resetModal();
            }
            return;
        }

        const result = await dispatch(
            createAppointment({
                patientId: resolvedPatientId,
                doctorId: resolvedDoctorId,
                typeCode,
                startsAt: fromDateTimeLocal(startsAt),
                endsAt: fromDateTimeLocal(endsAt),
                title: title.trim(),
                notes: notes.trim() || undefined,
            })
        );
        if (createAppointment.fulfilled.match(result)) {
            closeModal();
            resetModal();
        }
    };

    const handleCancel = async () => {
        if (!selected) return;
        const result = await dispatch(cancelAppointment(selected.id));
        if (cancelAppointment.fulfilled.match(result)) {
            closeModal();
            resetModal();
        }
    };

    return (
        <>
            <PageMeta
                title={`${t("calendar.pageTitle")} | NeuroVision.AI`}
                description={t("calendar.pageDescription")}
            />
            <div className="rounded-2xl border border-gray-200 bg-white dark:border-gray-800 dark:bg-white/[0.03]">
                {error && (
                    <p className="px-6 pt-4 text-sm text-error-500">{error}</p>
                )}
                {loading && items.length === 0 && (
                    <p className="px-6 pt-4 text-sm text-gray-500">{t("calendar.loading")}</p>
                )}
                <div className="custom-calendar">
                    <FullCalendar
                        ref={calendarRef}
                        plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
                        initialView="dayGridMonth"
                        headerToolbar={{
                            left: "prev,next addEventButton",
                            center: "title",
                            right: "dayGridMonth,timeGridWeek,timeGridDay",
                        }}
                        events={events}
                        selectable={true}
                        select={handleDateSelect}
                        eventClick={handleEventClick}
                        datesSet={handleDatesSet}
                        eventContent={renderEventContent}
                        customButtons={{
                            addEventButton: {
                                text: t("calendar.add"),
                                click: () => {
                                    resetModal();
                                    openModal();
                                },
                            },
                        }}
                    />
                </div>
                <Modal isOpen={isOpen} onClose={closeModal} className="max-w-[700px] p-6 lg:p-10">
                    <div className="flex flex-col px-2 overflow-y-auto custom-scrollbar">
                        <div>
                            <h5 className="mb-2 font-semibold text-gray-800 modal-title text-theme-xl dark:text-white/90 lg:text-2xl">
                                {selected ? t("calendar.edit") : t("calendar.add")}
                            </h5>
                            <p className="text-sm text-gray-500 dark:text-gray-400">
                                {t("calendar.subtitle")}
                            </p>
                        </div>
                        <div className="mt-8 space-y-6">
                            <div>
                                <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                    {t("calendar.eventTitle")}
                                </label>
                                <input
                                    type="text"
                                    value={title}
                                    disabled={!canEdit}
                                    onChange={(e) => setTitle(e.target.value)}
                                    className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                />
                            </div>
                            {!selected && (
                                <div>
                                    <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                        {t("calendar.type")}
                                    </label>
                                    <select
                                        value={typeCode}
                                        onChange={(e) => setTypeCode(e.target.value)}
                                        className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                    >
                                        {(catalogs?.types ?? []).map((item) => (
                                            <option key={item.code} value={item.code}>
                                                {item.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            )}
                            {(isDoctor || isAdmin) && !selected && (
                                <div>
                                    <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                        {t("calendar.patient")}
                                    </label>
                                    <select
                                        value={patientId}
                                        onChange={(e) => setPatientId(e.target.value)}
                                        className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                    >
                                        <option value="">{t("calendar.selectPatient")}</option>
                                        {patients.map((item) => (
                                            <option key={item.id} value={item.id}>
                                                {item.firstName} {item.lastName}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            )}
                            {isAdmin && !selected && (
                                <div>
                                    <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                        {t("calendar.doctor")}
                                    </label>
                                    <select
                                        value={doctorId}
                                        onChange={(e) => setDoctorId(e.target.value)}
                                        className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                    >
                                        <option value="">{t("calendar.selectDoctor")}</option>
                                        {doctors.map((item) => (
                                            <option key={item.id} value={item.id}>
                                                {item.firstName} {item.lastName}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            )}
                            {isPatient && !assignedDoctorId && (
                                <p className="text-sm text-error-500">{t("calendar.noAssignedDoctor")}</p>
                            )}
                            <div>
                                <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                    {t("calendar.start")}
                                </label>
                                <input
                                    type="datetime-local"
                                    value={startsAt}
                                    disabled={!canEdit}
                                    onChange={(e) => setStartsAt(e.target.value)}
                                    className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                />
                            </div>
                            <div>
                                <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                    {t("calendar.end")}
                                </label>
                                <input
                                    type="datetime-local"
                                    value={endsAt}
                                    disabled={!canEdit}
                                    onChange={(e) => setEndsAt(e.target.value)}
                                    className="dark:bg-dark-900 h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                />
                            </div>
                            <div>
                                <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-400">
                                    {t("calendar.notes")}
                                </label>
                                <textarea
                                    value={notes}
                                    disabled={!canEdit}
                                    onChange={(e) => setNotes(e.target.value)}
                                    className="dark:bg-dark-900 min-h-24 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
                                />
                            </div>
                        </div>
                        <div className="flex flex-wrap items-center gap-3 mt-6 modal-footer sm:justify-end">
                            <button
                                onClick={closeModal}
                                type="button"
                                className="flex w-full justify-center rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 sm:w-auto"
                            >
                                {t("calendar.close")}
                            </button>
                            {selected && canEdit && (
                                <button
                                    onClick={() => void handleCancel()}
                                    type="button"
                                    disabled={saving}
                                    className="flex w-full justify-center rounded-lg border border-error-300 bg-white px-4 py-2.5 text-sm font-medium text-error-600 hover:bg-error-50 sm:w-auto"
                                >
                                    {t("calendar.cancelAppointment")}
                                </button>
                            )}
                            {canEdit && (
                                <button
                                    onClick={() => void handleSave()}
                                    type="button"
                                    disabled={saving}
                                    className="btn btn-success btn-update-event flex w-full justify-center rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white hover:bg-brand-600 sm:w-auto"
                                >
                                    {selected ? t("calendar.update") : t("calendar.add")}
                                </button>
                            )}
                        </div>
                    </div>
                </Modal>
            </div>
        </>
    );
};

const renderEventContent = (eventInfo: EventContentArg) => {
    const color = String(eventInfo.event.extendedProps.calendar ?? "Primary").toLowerCase();
    const colorClass = `fc-bg-${color}`;
    return (
        <div className={`event-fc-color flex fc-event-main ${colorClass} p-1 rounded-sm`}>
            <div className="fc-daygrid-event-dot"></div>
            <div className="fc-event-time">{eventInfo.timeText}</div>
            <div className="fc-event-title">{eventInfo.event.title}</div>
        </div>
    );
};

export default Calendar;
