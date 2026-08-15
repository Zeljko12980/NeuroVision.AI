import { useEffect, useState } from "react";
import Editor from "@monaco-editor/react";

import { Modal } from "../modal";
import Button from "../button/Button";
import Input from "../../form/input/InputField";
import Label from "../../form/Label";
import Checkbox from "../../form/input/Checkbox";

import { PdfTemplateResponse } from "../../../features/pdf/pdfService";
import { useTranslation } from "react-i18next";
import { useTheme } from "../../../context/ThemeContext";

interface EditPdfTemplateModalProps {
    isOpen: boolean;
    template: PdfTemplateResponse | null;
    onClose: () => void;
    onSave: (template: PdfTemplateResponse) => Promise<void> | void;
    loading?: boolean;
}

type TabKey = "general" | "html" | "preview";

const TABS: { key: TabKey; labelKey: string }[] = [
    { key: "general", labelKey: "pdf.editModal.tabs.general" },
    { key: "html", labelKey: "pdf.editModal.tabs.html" },
    { key: "preview", labelKey: "pdf.editModal.tabs.preview" },
];

export default function EditPdfTemplateModal({
    isOpen,
    template,
    onClose,
    onSave,
    loading,
}: EditPdfTemplateModalProps) {
    const [form, setForm] = useState<PdfTemplateResponse | null>(null);
    const [activeTab, setActiveTab] = useState<TabKey>("general");
    const { t } = useTranslation();
    const { theme } = useTheme();

    useEffect(() => {
        setForm(template);
    }, [template]);

    if (!isOpen || !form) return null;

    const handleChange = <K extends keyof PdfTemplateResponse>(
        key: K,
        value: PdfTemplateResponse[K]
    ) => {
        setForm((prev) => (prev ? { ...prev, [key]: value } : prev));
    };

    const handleSubmit = async () => {
        if (!form) return;
        await onSave(form);
        onClose();
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-4xl">
            <div className="flex h-[600px] flex-col overflow-hidden rounded-2xl bg-white dark:bg-gray-900">
               
                <div className="border-b border-gray-200 px-6 py-5 dark:border-gray-800">
                    <div className="flex items-center justify-between">
                        <div>
                            <h2 className="text-xl font-semibold">
                                {t("pdf.editModal.title")}
                            </h2>

                            <p className="mt-1 text-sm text-gray-500">
                                {t("pdf.editModal.description")}
                            </p>
                        </div>

                        <span className="rounded-full bg-blue-100 px-3 py-1 text-xs font-semibold text-blue-600">
                            v{form.version}
                        </span>
                    </div>
                </div>

          
                <div className="flex border-b border-gray-200 dark:border-gray-800">
                    {TABS.map((tab) => (
                        <button
                            key={tab.key}
                            onClick={() => setActiveTab(tab.key)}
                            className={`px-5 py-3 text-sm font-medium transition ${activeTab === tab.key
                                    ? "border-b-2 border-blue-600 text-blue-600"
                                    : "text-gray-500 hover:text-gray-900"
                                }`}
                        >
                            {t(tab.labelKey)}
                        </button>
                    ))}
                </div>

         
                <div className="flex-1 overflow-hidden p-6">
                    {activeTab === "general" && (
                        <div className="grid grid-cols-2 gap-5">
                            <div>
                                <Label>{t("pdf.editModal.fields.name")}</Label>
                                <Input
                                    value={form.name}
                                    onChange={(e) => handleChange("name", e.target.value)}
                                />
                            </div>

                            <div>
                                <Label>{t("pdf.editModal.fields.code")}</Label>
                                <Input value={form.code} disabled onChange={() => { }} />
                            </div>

                            <div>
                                <Label>{t("pdf.editModal.fields.version")}</Label>

                                <Input
                                    type="number"
                                    value={String(form.version)}
                                    onChange={(e) =>
                                        handleChange("version", Number(e.target.value))
                                    }
                                />
                            </div>

                            <div className="flex flex-col">
                                <Label>{t("pdf.editModal.fields.status")}</Label>
                                <div className="mt-2 flex h-15 items-center">
                                    <Checkbox
                                        label={t("pdf.editModal.fields.activeTemplate")}
                                        checked={form.isActive}
                                        onChange={(checked) => handleChange("isActive", checked)}
                                    />
                                </div>
                            </div>

                            <div className="flex flex-col">
                                <Label>{t("pdf.editModal.fields.requiresSignature")}</Label>
                                <div className="mt-2 flex h-15 items-center">
                                    <Checkbox
                                        label={t("pdf.editModal.fields.requiresSignatureLabel")}
                                        checked={form.requiresSignature}
                                        onChange={(checked) =>
                                            handleChange("requiresSignature", checked)
                                        }
                                    />
                                </div>
                            </div>

                            {form.requiresSignature && (
                                <div>
                                    <Label>{t("pdf.editModal.fields.signaturePage")}</Label>

                                    <Input
                                        type="number"
                                        min={1}
                                        value={String(form.signaturePage)}
                                        onChange={(e) =>
                                            handleChange(
                                                "signaturePage",
                                                Number(e.target.value)
                                            )
                                        }
                                    />
                                </div>
                            )}
                        </div>
                    )}

                    {activeTab === "html" && (
                        <div className="h-full overflow-hidden rounded-xl border border-gray-200 dark:border-gray-800">
                            <Editor
                                height="100%"
                                defaultLanguage="html"
                                theme={theme === "dark" ? "vs-dark" : "vs"}
                                value={form.htmlContent}
                                onChange={(value) =>
                                    handleChange("htmlContent", value ?? "")
                                }
                                options={{
                                    automaticLayout: true,
                                    minimap: { enabled: false },
                                    fontSize: 14,
                                    wordWrap: "on",
                                    scrollBeyondLastLine: false,
                                }}
                            />
                        </div>
                    )}

                    {activeTab === "preview" && (
                        <iframe
                            title="preview"
                            className="h-full w-full rounded-xl border border-gray-200 bg-white dark:border-gray-800"
                            srcDoc={form.htmlContent}
                        />
                    )}
                </div>

         
                <div className="flex justify-end gap-3 border-t border-gray-200 px-6 py-4 dark:border-gray-800">
                    <Button variant="outline" onClick={onClose}>
                        {t("common.cancel")}
                    </Button>

                    <Button onClick={handleSubmit} disabled={loading}>
                        {loading
                            ? t("common.saving")
                            : t("common.saveChanges")}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}