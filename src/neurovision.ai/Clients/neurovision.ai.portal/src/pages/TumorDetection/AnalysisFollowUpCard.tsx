import { useCallback, useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

import ComponentCard from "../../components/common/ComponentCard";
import Button from "../../components/ui/button/Button";
import Label from "../../components/form/Label";
import CatalogMultiSelect from "../../components/form/CatalogMultiSelect";
import { useAppDispatch } from "../../store/store";
import { showAlert } from "../../features/ui/uiSlice";
import {
    fetchClinicalCatalogs,
    fetchClinicalFollowUp,
    saveClinicalFollowUp,
} from "../../features/tumorDetection/tumorDetection.service";
import type {
    AnalysisResponse,
    ClinicalCatalogItemResponse,
    ClinicalCatalogsResponse,
    ClinicalFollowUpResponse,
} from "../../features/tumorDetection/tumorDetection.types";
import { formatTumorClass, isNoTumorClass, primaryFindingClass, tumorSelectClass, tumorTextareaClass } from "./tumorUtils";

interface AnalysisFollowUpCardProps {
    analysis: AnalysisResponse;
    audience: "doctor" | "patient";
    canEdit: boolean;
}

const toOptions = (items: ClinicalCatalogItemResponse[]) =>
    items.map((item) => ({ value: item.code, label: item.name }));

const emptyCatalogs: ClinicalCatalogsResponse = {
    grades: [],
    operabilityStatuses: [],
    spreadStatuses: [],
    treatmentOptions: [],
};

function ReadOnlyRow({ label, value }: { label: string; value?: string | null }) {
    if (!value) return null;

    return (
        <div>
            <p className="text-xs font-medium uppercase tracking-wide text-gray-500 dark:text-gray-400">
                {label}
            </p>
            <p className="mt-1 text-sm text-gray-800 dark:text-white/90">{value}</p>
        </div>
    );
}

export default function AnalysisFollowUpCard({ analysis, audience, canEdit }: AnalysisFollowUpCardProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const finding = primaryFindingClass(analysis);
    const noTumor = finding ? isNoTumorClass(finding) : false;
    const classLabel = finding ? formatTumorClass(finding, t) : "";

    const [catalogs, setCatalogs] = useState<ClinicalCatalogsResponse>(emptyCatalogs);
    const [followUp, setFollowUp] = useState<ClinicalFollowUpResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    const [gradeCode, setGradeCode] = useState("");
    const [operabilityCode, setOperabilityCode] = useState("");
    const [spreadCode, setSpreadCode] = useState("");
    const [treatmentCodes, setTreatmentCodes] = useState<string[]>([]);
    const [sizeLocationNotes, setSizeLocationNotes] = useState("");
    const [clinicalNotes, setClinicalNotes] = useState("");

    const gradeOptions = useMemo(() => toOptions(catalogs.grades), [catalogs.grades]);
    const operabilityOptions = useMemo(
        () => toOptions(catalogs.operabilityStatuses),
        [catalogs.operabilityStatuses]
    );
    const spreadOptions = useMemo(() => toOptions(catalogs.spreadStatuses), [catalogs.spreadStatuses]);
    const treatmentOptions = useMemo(
        () => toOptions(catalogs.treatmentOptions),
        [catalogs.treatmentOptions]
    );

    const applyFollowUpToForm = useCallback((data: ClinicalFollowUpResponse | null) => {
        setFollowUp(data);
        setGradeCode(data?.gradeCode ?? "");
        setOperabilityCode(data?.operabilityCode ?? "");
        setSpreadCode(data?.spreadCode ?? "");
        setTreatmentCodes(data?.treatmentOptionCodes ?? []);
        setSizeLocationNotes(data?.sizeLocationNotes ?? "");
        setClinicalNotes(data?.clinicalNotes ?? "");
    }, []);

    useEffect(() => {
        let active = true;

        const load = async () => {
            setLoading(true);
            try {
                const [catalogData, followUpData] = await Promise.all([
                    fetchClinicalCatalogs(),
                    fetchClinicalFollowUp(analysis.id),
                ]);
                if (!active) return;
                setCatalogs(catalogData);
                applyFollowUpToForm(followUpData);
            } catch (err: unknown) {
                if (!active) return;
                dispatch(
                    showAlert({
                        type: "error",
                        message:
                            err instanceof Error
                                ? err.message
                                : t("tumor.followUp.messages.loadError"),
                    })
                );
            } finally {
                if (active) setLoading(false);
            }
        };

        void load();
        return () => {
            active = false;
        };
    }, [analysis.id, applyFollowUpToForm, dispatch, t]);

    const handleSave = async () => {
        setSaving(true);
        try {
            const saved = await saveClinicalFollowUp(analysis.id, {
                gradeCode: gradeCode || undefined,
                operabilityCode: operabilityCode || undefined,
                spreadCode: spreadCode || undefined,
                treatmentOptionCodes: treatmentCodes,
                sizeLocationNotes: sizeLocationNotes || undefined,
                clinicalNotes: clinicalNotes || undefined,
            });
            applyFollowUpToForm(saved);
            dispatch(
                showAlert({
                    type: "success",
                    message: t("tumor.followUp.messages.saveSuccess"),
                })
            );
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    type: "error",
                    message:
                        err instanceof Error
                            ? err.message
                            : t("tumor.followUp.messages.saveError"),
                })
            );
        } finally {
            setSaving(false);
        }
    };

    if (!finding) return null;

    const hasSavedData =
        followUp &&
        (followUp.gradeName ||
            followUp.operabilityName ||
            followUp.spreadName ||
            followUp.treatmentOptionNames.length > 0 ||
            followUp.sizeLocationNotes ||
            followUp.clinicalNotes);

    return (
        <ComponentCard
            title={t("tumor.followUp.title")}
            desc={t(`tumor.followUp.audience.${audience}`)}
        >
            <div className="space-y-4 text-sm text-gray-600 dark:text-gray-300">
                <p className="rounded-xl border border-warning-500 bg-warning-50 px-4 py-3 text-warning-600 dark:border-warning-500/30 dark:bg-warning-500/10 dark:text-orange-400">
                    {t("tumor.followUp.disclaimer")}
                </p>

                {noTumor ? (
                    <p>{t("tumor.followUp.noTumor")}</p>
                ) : (
                    <p>{t("tumor.followUp.intro", { class: classLabel })}</p>
                )}

                {loading ? (
                    <p className="text-gray-500">{t("tumor.followUp.loading")}</p>
                ) : canEdit ? (
                    <div className="space-y-4 rounded-xl border border-gray-200 p-4 dark:border-gray-800">
                        <p className="font-medium text-gray-800 dark:text-white/90">
                            {t("tumor.followUp.formTitle")}
                        </p>

                        <div className="grid gap-4 md:grid-cols-2">
                            <div>
                                <Label>{t("tumor.followUp.fields.grade")}</Label>
                                <select
                                    className={tumorSelectClass}
                                    value={gradeCode}
                                    onChange={(e) => setGradeCode(e.target.value)}
                                >
                                    <option value="">{t("tumor.followUp.placeholders.select")}</option>
                                    {gradeOptions.map((option) => (
                                        <option key={option.value} value={option.value}>
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <Label>{t("tumor.followUp.fields.operability")}</Label>
                                <select
                                    className={tumorSelectClass}
                                    value={operabilityCode}
                                    onChange={(e) => setOperabilityCode(e.target.value)}
                                >
                                    <option value="">{t("tumor.followUp.placeholders.select")}</option>
                                    {operabilityOptions.map((option) => (
                                        <option key={option.value} value={option.value}>
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <Label>{t("tumor.followUp.fields.spread")}</Label>
                                <select
                                    className={tumorSelectClass}
                                    value={spreadCode}
                                    onChange={(e) => setSpreadCode(e.target.value)}
                                >
                                    <option value="">{t("tumor.followUp.placeholders.select")}</option>
                                    {spreadOptions.map((option) => (
                                        <option key={option.value} value={option.value}>
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <Label>{t("tumor.followUp.fields.treatments")}</Label>
                                <CatalogMultiSelect
                                    options={treatmentOptions}
                                    values={treatmentCodes}
                                    placeholder={t("tumor.followUp.placeholders.treatments")}
                                    onChange={setTreatmentCodes}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>{t("tumor.followUp.fields.sizeLocation")}</Label>
                            <textarea
                                className={tumorTextareaClass}
                                rows={3}
                                value={sizeLocationNotes}
                                onChange={(e) => setSizeLocationNotes(e.target.value)}
                                placeholder={t("tumor.followUp.placeholders.sizeLocation")}
                            />
                        </div>

                        <div>
                            <Label>{t("tumor.followUp.fields.notes")}</Label>
                            <textarea
                                className={tumorTextareaClass}
                                rows={4}
                                value={clinicalNotes}
                                onChange={(e) => setClinicalNotes(e.target.value)}
                                placeholder={t("tumor.followUp.placeholders.notes")}
                            />
                        </div>

                        <div className="flex justify-end">
                            <Button onClick={handleSave} disabled={saving}>
                                {saving ? t("tumor.followUp.saving") : t("tumor.followUp.save")}
                            </Button>
                        </div>
                    </div>
                ) : hasSavedData ? (
                    <div className="space-y-4 rounded-xl border border-gray-200 p-4 dark:border-gray-800">
                        <ReadOnlyRow label={t("tumor.followUp.fields.grade")} value={followUp?.gradeName} />
                        <ReadOnlyRow
                            label={t("tumor.followUp.fields.operability")}
                            value={followUp?.operabilityName}
                        />
                        <ReadOnlyRow label={t("tumor.followUp.fields.spread")} value={followUp?.spreadName} />
                        {followUp && followUp.treatmentOptionNames.length > 0 && (
                            <div>
                                <p className="text-xs font-medium uppercase tracking-wide text-gray-500 dark:text-gray-400">
                                    {t("tumor.followUp.fields.treatments")}
                                </p>
                                <ul className="mt-2 list-disc space-y-1 pl-5 text-sm text-gray-800 dark:text-white/90">
                                    {followUp.treatmentOptionNames.map((name) => (
                                        <li key={name}>{name}</li>
                                    ))}
                                </ul>
                            </div>
                        )}
                        <ReadOnlyRow
                            label={t("tumor.followUp.fields.sizeLocation")}
                            value={followUp?.sizeLocationNotes}
                        />
                        <ReadOnlyRow label={t("tumor.followUp.fields.notes")} value={followUp?.clinicalNotes} />
                    </div>
                ) : (
                    <p className="rounded-xl border border-dashed border-gray-300 px-4 py-3 text-gray-500 dark:border-gray-700">
                        {t("tumor.followUp.emptyPatient")}
                    </p>
                )}
            </div>
        </ComponentCard>
    );
}
