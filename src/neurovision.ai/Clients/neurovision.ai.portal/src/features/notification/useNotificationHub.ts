import { useEffect, useRef } from "react";

import { useAppDispatch, useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";
import { getUserInfoFromClaims } from "../../utils/claims";
import { fetchNotifications, notificationReceived } from "./notificationSlice";
import type { NotificationResponse } from "./notification.types";
import {
    ensureNotificationHubStarted,
    getNotificationHubConnection,
    setNotificationHubReconnectedHandler,
    stopNotificationHub,
} from "./notification.hub";

const NOTIFICATION_CREATED = "NotificationCreated";

export function useNotificationHub() {
    const dispatch = useAppDispatch();
    const token = useAppSelector((state) => state.auth.token);
    const claims = useAppSelector(selectUserClaims);
    const { userId } = getUserInfoFromClaims(claims || {});
    const userIdRef = useRef(userId);
    userIdRef.current = userId;

    useEffect(() => {
        if (!token || !userId) {
            void stopNotificationHub();
            return;
        }

        let cancelled = false;

        const onCreated = (payload: NotificationResponse) => {
            if (!cancelled) {
                dispatch(notificationReceived(payload));
            }
        };

        setNotificationHubReconnectedHandler(() => {
            const recipientUserId = userIdRef.current;
            if (recipientUserId) {
                void dispatch(fetchNotifications({ recipientUserId, take: 20 }));
            }
        });

        const subscribe = async () => {
            try {
                const hub = await ensureNotificationHubStarted();
                if (cancelled) return;

                hub.on(NOTIFICATION_CREATED, onCreated);
            } catch {
                // hub unavailable — inbox still loads via REST
            }
        };

        void subscribe();

        return () => {
            cancelled = true;
            setNotificationHubReconnectedHandler(null);
            const hub = getNotificationHubConnection();
            hub.off(NOTIFICATION_CREATED, onCreated);
        };
    }, [dispatch, token, userId]);
}
