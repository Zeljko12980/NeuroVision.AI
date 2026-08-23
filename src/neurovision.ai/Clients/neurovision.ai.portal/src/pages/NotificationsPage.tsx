import { useEffect } from "react";
import PageBreadcrumb from "../components/common/PageBreadCrumb";
import PageMeta from "../components/common/PageMeta";
import { useAppDispatch, useAppSelector } from "../store/store";
import { selectUserClaims } from "../selectors/authSelectors";
import { getUserInfoFromClaims } from "../utils/claims";
import {
    fetchNotifications,
    readAllNotifications,
    readNotification,
} from "../features/notification/notificationSlice";
import { formatNotificationTime, mapSeverity } from "../features/notification/notification.utils";

const statusColor = (type: string) => {
    switch (type) {
        case "critical":
            return "bg-error-500";
        case "warning":
            return "bg-orange-400";
        default:
            return "bg-success-500";
    }
};

export default function NotificationsPage() {
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});
    const { items, unreadCount, loading } = useAppSelector((state) => state.notification);

    useEffect(() => {
        if (!userId) return;
        void dispatch(fetchNotifications({ recipientUserId: userId, take: 50 }));
    }, [dispatch, userId]);

    return (
        <div>
            <PageMeta title="Notifications | NeuroVision.AI" description="Inbox" />
            <PageBreadcrumb pageTitle="Notifications" />
            <div className="rounded-2xl border border-gray-200 bg-white px-5 py-7 dark:border-gray-800 dark:bg-white/[0.03] xl:px-10 xl:py-12">
                <div className="mb-6 flex items-center justify-between">
                    <h3 className="font-semibold text-gray-800 text-theme-xl dark:text-white/90">
                        Inbox {unreadCount > 0 ? `(${unreadCount} unread)` : ""}
                    </h3>
                    {userId && unreadCount > 0 && (
                        <button
                            className="rounded-lg border px-3 py-1.5 text-sm hover:bg-gray-100 dark:hover:bg-white/5"
                            onClick={() => void dispatch(readAllNotifications(userId))}
                        >
                            Mark all as read
                        </button>
                    )}
                </div>

                {loading && items.length === 0 && (
                    <p className="text-sm text-gray-500">Loading notifications...</p>
                )}
                {!loading && items.length === 0 && (
                    <p className="text-sm text-gray-500">No notifications yet.</p>
                )}

                <ul className="flex flex-col divide-y divide-gray-100 dark:divide-gray-800">
                    {items.map((item) => (
                        <li key={item.id}>
                            <button
                                className={`flex w-full gap-3 px-1 py-4 text-left ${
                                    item.isRead ? "opacity-70" : ""
                                }`}
                                onClick={() => {
                                    if (userId && !item.isRead) {
                                        void dispatch(
                                            readNotification({
                                                id: item.id,
                                                recipientUserId: userId,
                                            })
                                        );
                                    }
                                }}
                            >
                                <span
                                    className={`mt-2 h-3 w-3 shrink-0 rounded-full ${statusColor(
                                        mapSeverity(item.severityCode)
                                    )}`}
                                />
                                <span>
                                    <span className="block text-sm text-gray-700 dark:text-gray-300">
                                        <span className="font-medium text-gray-900 dark:text-white">
                                            {item.title}
                                        </span>{" "}
                                        {item.message}
                                    </span>
                                    <span className="text-xs text-gray-400">
                                        {formatNotificationTime(item.createdAt)}
                                    </span>
                                </span>
                            </button>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
}
