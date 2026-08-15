/* eslint-disable @typescript-eslint/no-unused-vars */
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Editor from "@monaco-editor/react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";

import Input from "../../components/form/input/InputField";
import Label from "../../components/form/Label";
import Checkbox from "../../components/form/input/Checkbox";
import Button from "../../components/ui/button/Button";
import { useAppDispatch, useAppSelector } from "../../store/store";


import { createTemplate } from "../../features/pdf/pdfSlice";
import { showAlert } from "../../features/ui/uiSlice";
import { useTheme } from "../../context/ThemeContext";




function CreatePdfTemplatePage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { theme } = useTheme();

    type TabKey = "general" | "html" | "preview";

    const [activeTab, setActiveTab] = useState<TabKey>("general");

    const TABS = [
        {
            key: "general",
            label: t("pdf.editModal.tabs.general"),
        },
        {
            key: "html",
            label: t("pdf.editModal.tabs.html"),
        },
        {
            key: "preview",
            label: t("pdf.editModal.tabs.preview"),
        },
    ] as const;


    const { loading } = useAppSelector((state) => state.pdfTemplate);

    const defaultHtml = '';
    const [form, setForm] = useState({
        name: "",
        code: "",
        version: 1,
        isActive: true,
        requiresSignature: false,
        signaturePage: 1,
        htmlContent: defaultHtml,
    });

    const handleChange = <K extends keyof typeof form>(
        key: K,
        value: (typeof form)[K]
    ) => {
        setForm((prev) => ({
            ...prev,
            [key]: value,
        }));
    };

  

    const handleSubmit = async () => {
        try {
            await dispatch(createTemplate(form)).unwrap();
            dispatch(
                showAlert({
                    type: "success",
                    message: t("pdf.messages.createSuccess"),
                })
            );

            navigate("/admin/pdfs");
        } catch (err: unknown) {
      
            dispatch(
                showAlert({
                    type: "error",
                    message: t("pdf.messages.createError"),
                })
            );
        }
    };
    return (
        <>
            <PageMeta
                title={t("pdf.create.pageTitle")}
                description={t("pdf.create.pageDescription")}
            />

            <PageBreadcrumb pageTitle={t("pdf.create.pageTitle")} />

            <ComponentCard title={t("pdf.create.pageTitle")}>
                <div className="flex h-[780px] flex-col">
                   
                    <div className="flex shrink-0 border-b border-gray-200 dark:border-gray-800">
                        {TABS.map((tab) => (
                            <button
                                key={tab.key}
                                onClick={() => setActiveTab(tab.key)}
                                className={`px-5 py-3 text-sm font-medium transition ${activeTab === tab.key
                                        ? "border-b-2 border-brand-500 text-brand-500"
                                        : "text-gray-500 hover:text-gray-900 dark:hover:text-white"
                                    }`}
                            >
                                {tab.label}
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
                                        onChange={(e) =>
                                            handleChange("name", e.target.value)
                                        }
                                    />
                                </div>

                                <div>
                                    <Label>{t("pdf.editModal.fields.code")}</Label>

                                    <Input
                                        value={form.code}
                                        onChange={(e) =>
                                            handleChange("code", e.target.value)
                                        }
                                    />
                                </div>

                                <div>
                                    <Label>{t("pdf.editModal.fields.version")}</Label>

                                    <Input
                                        type="number"
                                        value={String(form.version)}
                                        onChange={(e) =>
                                            handleChange(
                                                "version",
                                                Number(e.target.value)
                                            )
                                        }
                                    />
                                </div>

                                <div className="flex flex-col">
                                    <Label>
                                        {t("pdf.editModal.fields.status")}
                                    </Label>

                                    <div className="mt-2 flex h-11 items-center">
                                        <Checkbox
                                            label={t(
                                                "pdf.editModal.fields.activeTemplate"
                                            )}
                                            checked={form.isActive}
                                            onChange={(checked) =>
                                                handleChange("isActive", checked)
                                            }
                                        />
                                    </div>
                                </div>

                                <div className="flex flex-col">
                                    <Label>
                                        {t("pdf.editModal.fields.requiresSignature")}
                                    </Label>

                                    <div className="mt-2 flex h-11 items-center">
                                        <Checkbox
                                            label={t(
                                                "pdf.editModal.fields.requiresSignatureLabel"
                                            )}
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
                                    theme={theme === "dark" ? "vs-dark" : "light"}
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
                            <div className="h-full overflow-hidden rounded-xl border border-gray-200 dark:border-gray-800">
                                <iframe
                                    title="preview"
                                    className="h-full w-full bg-white"
                                    srcDoc={form.htmlContent}
                                />
                            </div>
                        )}
                    </div>

                   
                    <div className="flex shrink-0 justify-end gap-3 border-t border-gray-200 p-6 dark:border-gray-800">
                        <Button
                            variant="outline"
                            onClick={() => navigate("/pdf-templates")}
                        >
                            {t("common.cancel")}
                        </Button>

                        <Button
                            onClick={handleSubmit}
                            disabled={loading}
                        >
                            {loading
                                ? t("common.creating")
                                : t("pdf.create.create")}
                        </Button>
                    </div>
                </div>
            </ComponentCard>
        </>
    );
}

export default CreatePdfTemplatePage;

