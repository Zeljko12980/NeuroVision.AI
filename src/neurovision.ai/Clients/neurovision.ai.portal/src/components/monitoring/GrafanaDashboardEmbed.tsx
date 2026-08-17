import { useTheme } from "../../context/ThemeContext";

const GRAFANA_URL = (import.meta.env.VITE_GRAFANA_URL ?? "http://localhost:3000").replace(
    /\/$/,
    ""
);

const DASHBOARD_UID = "neurovision-overview";

function buildEmbedUrl(theme: "light" | "dark") {
    const params = new URLSearchParams({
        orgId: "1",
        from: "now-15m",
        to: "now",
        refresh: "5s",
        theme,
        kiosk: "1",
    });

    return `${GRAFANA_URL}/d/${DASHBOARD_UID}/${DASHBOARD_UID}?${params.toString()}`;
}

type GrafanaDashboardEmbedProps = {
    openInGrafanaLabel: string;
};

export default function GrafanaDashboardEmbed({
    openInGrafanaLabel,
}: GrafanaDashboardEmbedProps) {
    const { theme } = useTheme();
    const src = buildEmbedUrl(theme);

    return (
        <div className="overflow-hidden rounded-2xl border border-gray-200 dark:border-gray-800">
            <div className="flex items-center justify-end border-b border-gray-100 px-4 py-2 dark:border-gray-800">
                <a
                    href={`${GRAFANA_URL}/d/${DASHBOARD_UID}`}
                    target="_blank"
                    rel="noreferrer"
                    className="text-sm font-medium text-brand-600 hover:underline dark:text-brand-400"
                >
                    {openInGrafanaLabel}
                </a>
            </div>
            <iframe
                title="NeuroVision Grafana"
                src={src}
                className="h-[calc(100vh-220px)] min-h-[640px] w-full border-0 bg-white dark:bg-gray-900"
                allow="fullscreen"
            />
        </div>
    );
}
