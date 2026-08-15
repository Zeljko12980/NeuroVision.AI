import { useState } from "react";
import { Dropdown } from "../ui/dropdown/Dropdown";
import { DropdownItem } from "../ui/dropdown/DropdownItem";
import { Link } from "react-router";

type Notification = {
    id: number;
    title: string;
    message: string;
    time: string;
    type: "critical" | "warning" | "info";
};

const notifications: Notification[] = [
    {
        id: 1,
        title: "ICU Monitor",
        message: "Critical vitals detected for patient ID #48291",
        time: "2 min ago",
        type: "critical",
    },
    {
        id: 2,
        title: "Lab System",
        message: "Abnormal potassium level (6.8 mmol/L)",
        time: "10 min ago",
        type: "warning",
    },
    {
        id: 3,
        title: "Medication Service",
        message: "Dosage conflict detected for patient ID #19302",
        time: "15 min ago",
        type: "critical",
    },
    {
        id: 4,
        title: "Security Alert",
        message: "Multiple failed login attempts on admin account",
        time: "25 min ago",
        type: "critical",
    },
    {
        id: 5,
        title: "Radiology API",
        message: "Connection lost to PACS server",
        time: "40 min ago",
        type: "warning",
    },
    {
        id: 6,
        title: "Database Service",
        message: "Nightly backup completed successfully",
        time: "1 hr ago",
        type: "info",
    },
];

export default function NotificationDropdown() {
    const [isOpen, setIsOpen] = useState(false);
    const [notifying, setNotifying] = useState(true);

    const toggleDropdown = () => setIsOpen(!isOpen);
    const closeDropdown = () => setIsOpen(false);

    const handleClick = () => {
        toggleDropdown();
        setNotifying(false);
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
                    className={`absolute right-0 top-0.5 h-2 w-2 rounded-full bg-orange-400 ${!notifying ? "hidden" : "flex"
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
                    {notifications.map((n) => (
                        <li key={n.id}>
                            <DropdownItem
                                onItemClick={closeDropdown}
                                className="flex gap-3 p-3 border-b hover:bg-gray-100 dark:hover:bg-white/5"
                            >
                                <span
                                    className={`h-3 w-3 mt-2 rounded-full ${getStatusColor(
                                        n.type
                                    )}`}
                                ></span>

                                {/* CONTENT */}
                                <span className="block">
                                    <span className="block text-sm text-gray-500">
                                        <span className="font-medium text-gray-800 dark:text-white">
                                            {n.title}
                                        </span>{" "}
                                        {n.message}
                                    </span>

                                    <span className="text-xs text-gray-400">{n.time}</span>
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