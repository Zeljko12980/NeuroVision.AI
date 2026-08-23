import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
} from "@microsoft/signalr";

let connection: HubConnection | null = null;
let startPromise: Promise<void> | null = null;
let onReconnectedHandler: (() => void) | null = null;

export const getNotificationHubUrl = () =>
    `${import.meta.env.VITE_API_URL ?? "http://localhost:5000/api"}/notification/hubs/inbox`;

export const setNotificationHubReconnectedHandler = (handler: (() => void) | null) => {
    onReconnectedHandler = handler;
};

export const getNotificationHubConnection = () => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl(getNotificationHubUrl(), {
                accessTokenFactory: () => localStorage.getItem("token") ?? "",
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();

        connection.onreconnected(() => {
            onReconnectedHandler?.();
        });
    }

    return connection;
};

export const ensureNotificationHubStarted = async () => {
    const hub = getNotificationHubConnection();

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

export const stopNotificationHub = async () => {
    if (!connection) return;

    if (connection.state !== "Disconnected") {
        await connection.stop();
    }

    connection = null;
    startPromise = null;
};
