import { AppointmentResponse } from "./appointment.types";

const pad = (value: number) => value.toString().padStart(2, "0");

export const toDateTimeLocal = (value: string | Date) => {
    const date = typeof value === "string" ? new Date(value) : value;
    if (Number.isNaN(date.getTime())) return "";

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
};

export const fromDateTimeLocal = (value: string) => {
    const date = new Date(value);
    return date.toISOString();
};

export const defaultSlotFromSelect = (startStr: string, endStr?: string) => {
    const start = new Date(startStr);
    if (Number.isNaN(start.getTime())) {
        return { start: "", end: "" };
    }

    if (!startStr.includes("T")) {
        start.setHours(9, 0, 0, 0);
        const end = new Date(start);
        end.setMinutes(end.getMinutes() + 30);
        return { start: toDateTimeLocal(start), end: toDateTimeLocal(end) };
    }

    let end = endStr ? new Date(endStr) : new Date(start.getTime() + 30 * 60 * 1000);
    if (Number.isNaN(end.getTime()) || end <= start) {
        end = new Date(start.getTime() + 30 * 60 * 1000);
    }

    return { start: toDateTimeLocal(start), end: toDateTimeLocal(end) };
};

export const calendarColor = (statusCode: string) => {
    switch (statusCode) {
        case "CANC":
            return "Danger";
        case "DONE":
            return "Success";
        default:
            return "Primary";
    }
};

export const toCalendarEvent = (item: AppointmentResponse) => ({
    id: item.id,
    title: item.title,
    start: item.startsAt,
    end: item.endsAt,
    extendedProps: {
        calendar: calendarColor(item.statusCode),
        appointment: item,
    },
});
