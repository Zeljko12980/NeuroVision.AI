export const formatNotificationTime = (createdAt: string) => {
    const created = new Date(createdAt);
    if (Number.isNaN(created.getTime())) return "";

    const diffMs = Date.now() - created.getTime();
    const minutes = Math.max(0, Math.floor(diffMs / 60000));

    if (minutes < 1) return "just now";
    if (minutes < 60) return `${minutes} min ago`;

    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours} hr ago`;

    const days = Math.floor(hours / 24);
    return `${days}d ago`;
};

export const mapSeverity = (code: string): "critical" | "warning" | "info" => {
    switch (code) {
        case "CRIT":
            return "critical";
        case "WARN":
            return "warning";
        default:
            return "info";
    }
};
