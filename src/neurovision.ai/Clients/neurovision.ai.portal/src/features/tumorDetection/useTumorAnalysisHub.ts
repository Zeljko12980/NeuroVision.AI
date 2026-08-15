import { useEffect, useRef } from "react";

import type { AnalysisStatusNotification } from "./tumorDetection.types";
import { ensureTumorHubStarted, getTumorHubConnection } from "./tumorAnalysis.hub";

interface UseTumorAnalysisHubOptions {
    analysisId?: string;
    patientId?: string;
    isDoctor?: boolean;
    enabled?: boolean;
    onStatusChanged: (notification: AnalysisStatusNotification) => void;
}

export function useTumorAnalysisHub({
    analysisId,
    patientId,
    isDoctor = false,
    enabled = true,
    onStatusChanged,
}: UseTumorAnalysisHubOptions) {
    const handlerRef = useRef(onStatusChanged);
    handlerRef.current = onStatusChanged;

    useEffect(() => {
        if (!enabled) return;

        let cancelled = false;

        const handler = (payload: AnalysisStatusNotification) => {
            if (!cancelled) {
                handlerRef.current(payload);
            }
        };

        const subscribe = async () => {
            try {
                const hub = await ensureTumorHubStarted();
                if (cancelled) return;

                hub.on("AnalysisStatusChanged", handler);

                if (analysisId) {
                    await hub.invoke("JoinAnalysis", analysisId);
                }

                if (patientId) {
                    await hub.invoke("JoinPatient", patientId);
                }

                if (isDoctor) {
                    await hub.invoke("JoinAllAnalyses");
                }
            } catch {
                // hub unavailable — table still works via manual refresh
            }
        };

        subscribe();

        return () => {
            cancelled = true;
            const hub = getTumorHubConnection();
            hub.off("AnalysisStatusChanged", handler);

            if (hub.state === "Connected") {
                if (analysisId) {
                    hub.invoke("LeaveAnalysis", analysisId).catch(() => undefined);
                }
                if (patientId) {
                    hub.invoke("LeavePatient", patientId).catch(() => undefined);
                }
                if (isDoctor) {
                    hub.invoke("LeaveAllAnalyses").catch(() => undefined);
                }
            }
        };
    }, [analysisId, patientId, isDoctor, enabled]);
}
