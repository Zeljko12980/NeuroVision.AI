import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
} from "@microsoft/signalr";

let connection: HubConnection | null = null;
let startPromise: Promise<void> | null = null;

export const getTumorHubUrl = () =>
    `${import.meta.env.VITE_API_URL}/tumor/hubs/analysis`;

export const getTumorHubConnection = () => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl(getTumorHubUrl(), {
                accessTokenFactory: () => localStorage.getItem("token") ?? "",
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();
    }

    return connection;
};

export const ensureTumorHubStarted = async () => {
    const hub = getTumorHubConnection();

    if (hub.state === "Connected") {
        return hub;
    }

    if (!startPromise) {
        startPromise = hub.start().finally(() => {
            startPromise = null;
        });
    }

    await startPromise;
    return hub;
};

export const stopTumorHub = async () => {
    if (!connection) return;

    if (connection.state !== "Disconnected") {
        await connection.stop();
    }

    connection = null;
    startPromise = null;
};
