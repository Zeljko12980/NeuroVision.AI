import { useEffect, useState } from "react";
import { Dropdown } from "../ui/dropdown/Dropdown";
import { DropdownItem } from "../ui/dropdown/DropdownItem";
import { Link } from "react-router";
import { useAppDispatch, useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import { fetchNotifications, readNotification } from "../../features/notification/notificationSlice";
import { formatNotificationTime, mapSeverity } from "../../features/notification/notification.utils";

export default function NotificationDropdown() {
    const dispatch = useAppDispatch();
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});
    const { items, unreadCount, loading } = useAppSelector((state) => state.notification);

    const [isOpen, setIsOpen] = useState(false);

    const toggleDropdown = () => setIsOpen(!isOpen);
    const closeDropdown = () => setIsOpen(false);

    useEffect(() => {
        if (!userId) return;
        void dispatch(fetchNotifications({ recipientUserId: userId, take: 20 }));
    }, [dispatch, userId]);

    const handleClick = () => {
        toggleDropdown();
        if (userId) {
            void dispatch(fetchNotifications({ recipientUserId: userId, take: 20 }));
        }
    };

    const handleItemClick = (id: string) => {
        if (userId) {
            void dispatch(readNotification({ id, recipientUserId: userId }));
        }
        closeDropdown();
    };

    const getStatusColor = (type: string) => {
        switch (type) {
            case "critical":
                return "bg-error-500";
            case "warning":
                return "bg-orange-400";
            default:
                return "bg-success-500";
        }
    };

    return (
        <div className="relative">
            <button
                className="relative flex items-center justify-center text-gray-500 bg-white border rounded-full h-11 w-11 hover:bg-gray-100 dark:bg-gray-900 dark:text-gray-400"
                onClick={handleClick}
            >
                <span
                    className={`absolute right-0 top-0.5 h-2 w-2 rounded-full bg-orange-400 ${
                        unreadCount <= 0 ? "hidden" : "flex"
                    }`}
                >
                    <span className="absolute w-full h-full bg-orange-400 rounded-full opacity-75 animate-ping"></span>
                </span>

                🔔
            </button>

            <Dropdown
                isOpen={isOpen}
                onClose={closeDropdown}
                className="absolute right-0 mt-4 w-[350px] h-[480px] flex flex-col rounded-2xl border bg-white p-3 shadow-lg dark:bg-gray-dark"
            >
                <div className="flex justify-between pb-3 mb-3 border-b">
                    <h5 className="text-lg font-semibold">System Notifications</h5>
                    <button onClick={toggleDropdown}>✕</button>
                </div>

                <ul className="flex flex-col overflow-y-auto">
                    {loading && items.length === 0 && (
                        <li className="p-3 text-sm text-gray-400">Loading notifications...</li>
                    )}
                    {!loading && items.length === 0 && (
                        <li className="p-3 text-sm text-gray-400">No notifications yet.</li>
                    )}
                    {items.map((n) => (
                        <li key={n.id}>
                            <DropdownItem
                                onItemClick={() => handleItemClick(n.id)}
                                className={`flex gap-3 p-3 border-b hover:bg-gray-100 dark:hover:bg-white/5 ${
                                    n.isRead ? "opacity-70" : ""
                                }`}
                            >
                                <span
                                    className={`h-3 w-3 mt-2 rounded-full ${getStatusColor(
                                        mapSeverity(n.severityCode)
                                    )}`}
                                ></span>

                                <span className="block">
                                    <span className="block text-sm text-gray-500">
                                        <span className="font-medium text-gray-800 dark:text-white">
                                            {n.title}
                                        </span>{" "}
                                        {n.message}
                                    </span>

                                    <span className="text-xs text-gray-400">
                                        {formatNotificationTime(n.createdAt)}
                                    </span>
                                </span>
                            </DropdownItem>
                        </li>
                    ))}
                </ul>

                <Link
                    to="/notifications"
                    className="block px-4 py-2 mt-3 text-sm text-center border rounded-lg hover:bg-gray-100"
                >
                    View All Notifications
                </Link>
            </Dropdown>
        </div>
    );
}
