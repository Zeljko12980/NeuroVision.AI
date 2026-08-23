import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import {
    getNotificationInbox,
    markAllNotificationsAsRead,
    markNotificationAsRead,
} from "./notificationService";
import { NotificationInboxResponse, NotificationResponse } from "./notification.types";

interface NotificationState {
    items: NotificationResponse[];
    unreadCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: NotificationState = {
    items: [],
    unreadCount: 0,
    loading: false,
    error: null,
};

const toErrorMessage = (err: unknown, fallback: string) => {
    if (typeof err === "string" && err.trim()) return err;
    if (err instanceof Error && err.message.trim()) return err.message;
    return fallback;
};

export const fetchNotifications = createAsyncThunk<
    NotificationInboxResponse,
    { recipientUserId: string; take?: number },
    { rejectValue: string }
>("notification/fetchInbox", async ({ recipientUserId, take }, { rejectWithValue }) => {
    try {
        return await getNotificationInbox(recipientUserId, take ?? 20);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load notifications"));
    }
});

export const readNotification = createAsyncThunk<
    NotificationResponse,
    { id: string; recipientUserId: string },
    { rejectValue: string }
>("notification/markRead", async ({ id, recipientUserId }, { rejectWithValue }) => {
    try {
        return await markNotificationAsRead(id, recipientUserId);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to mark notification as read"));
    }
});

export const readAllNotifications = createAsyncThunk<
    number,
    string,
    { rejectValue: string }
>("notification/markAllRead", async (recipientUserId, { rejectWithValue }) => {
    try {
        return await markAllNotificationsAsRead(recipientUserId);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to mark notifications as read"));
    }
});

const notificationSlice = createSlice({
    name: "notification",
    initialState,
    reducers: {
        notificationReceived: (state, action: PayloadAction<NotificationResponse>) => {
            const incoming = action.payload;
            if (state.items.some((item) => item.id === incoming.id)) {
                return;
            }

            state.items = [incoming, ...state.items];
            if (!incoming.isRead) {
                state.unreadCount += 1;
            }
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchNotifications.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchNotifications.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload.items;
                state.unreadCount = action.payload.unreadCount;
            })
            .addCase(fetchNotifications.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload ?? "Failed to load notifications";
            })
            .addCase(readNotification.fulfilled, (state, action) => {
                const updated = action.payload;
                state.items = state.items.map((item) =>
                    item.id === updated.id ? updated : item
                );
                state.unreadCount = state.items.filter((item) => !item.isRead).length;
            })
            .addCase(readAllNotifications.fulfilled, (state) => {
                const now = new Date().toISOString();
                state.items = state.items.map((item) => ({
                    ...item,
                    isRead: true,
                    readAt: item.readAt ?? now,
                }));
                state.unreadCount = 0;
            });
    },
});

export const { notificationReceived } = notificationSlice.actions;
export default notificationSlice.reducer;
