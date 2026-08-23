import { get, post } from "../../api/api";
import { NotificationInboxResponse, NotificationResponse } from "./notification.types";

export const getNotificationInbox = async (
    recipientUserId: string,
    take = 20
): Promise<NotificationInboxResponse> => {
    const query = new URLSearchParams({
        recipientUserId,
        take: take.toString(),
    });

    return await get(`/notification?${query.toString()}`);
};

export const markNotificationAsRead = async (
    id: string,
    recipientUserId: string
): Promise<NotificationResponse> => {
    const query = new URLSearchParams({ recipientUserId });
    return await post(`/notification/${id}/read?${query.toString()}`, {});
};

export const markAllNotificationsAsRead = async (
    recipientUserId: string
): Promise<number> => {
    const query = new URLSearchParams({ recipientUserId });
    return await post(`/notification/read-all?${query.toString()}`, {});
};
