export type NotificationSeverity = "CRIT" | "WARN" | "INFO";

export interface NotificationResponse {
    id: string;
    recipientUserId: string;
    typeCode: string;
    severityCode: NotificationSeverity;
    title: string;
    message: string;
    payload?: string | null;
    relatedEntityType?: string | null;
    relatedEntityId?: string | null;
    healthInstitutionId?: number | null;
    createdAt: string;
    readAt?: string | null;
    isRead: boolean;
}

export interface NotificationInboxResponse {
    items: NotificationResponse[];
    unreadCount: number;
}
