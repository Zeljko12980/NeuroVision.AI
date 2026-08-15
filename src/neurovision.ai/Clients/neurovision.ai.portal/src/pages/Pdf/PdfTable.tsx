import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";

import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";
import { Dropdown } from "../../components/ui/dropdown/Dropdown";
import ConfirmDialog from "../../components/ui/dialog/ConfirmDialog";

import { RefreshIcon } from "../../icons";

import { useAppDispatch, useAppSelector } from "../../store/store";

import {
    fetchPdfTemplates,
    deleteTemplate,
    updateTemplate,
} from "../../features/pdf/pdfSlice";

import { showAlert } from "../../features/ui/uiSlice";
import EditPdfTemplateModal from "../../components/ui/dialog/EditPdfTemplateModal";
import { PdfTemplateResponse } from "../../features/pdf/pdfService";
import PdfTemplatePreviewDialog from "../../components/ui/dialog/PdfTemplatePreviewDialog";
import PdfTemplateTableSkeleton from "./PdfTableSkeleton";

export default function PdfTable() {
    const { t } = useTranslation();

    const dispatch = useAppDispatch();

    const templates = useAppSelector(
        (state) => state.pdfTemplate.templates
    );

    const total = useAppSelector(
        (state) => state.pdfTemplate.totalCount
    );

    const loading = useAppSelector(
        (state) => state.pdfTemplate.loading
    );

    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);

    const [spinning, setSpinning] = useState(false);
    const [fetching, setFetching] = useState(false);

    const [previewOpen, setPreviewOpen] = useState(false);
    const [previewHtml, setPreviewHtml] = useState("");
    const [previewTitle, setPreviewTitle] = useState("");

    const [openDropdownId, setOpenDropdownId] =
        useState<string | null>(null);

    const [confirmOpen, setConfirmOpen] = useState(false);
    const [selectedId, setSelectedId] =
        useState<string | null>(null);

    const [editOpen, setEditOpen] = useState(false);

    const [selectedTemplate, setSelectedTemplate] =
        useState<PdfTemplateResponse | null>(null);

    const [saving, setSaving] = useState(false);

    const [deleting, setDeleting] = useState(false);

    const totalPages = Math.max(
        1,
        Math.ceil(total / pageSize)
    );

    const delay = (ms: number) =>
        new Promise((resolve) => setTimeout(resolve, ms));

    const loadTemplates = async () => {
        setSpinning(true);
        setFetching(true);

        try {
            await Promise.all([
                dispatch(
                    fetchPdfTemplates({
                        pageIndex: page,
                        pageSize,
                    })
                ).unwrap(),
                delay(600),
            ]);
        } finally {
            setSpinning(false);
            setFetching(false);
        }
    };

    useEffect(() => {
        loadTemplates();
    }, [page, pageSize]);

    const toggleDropdown = (id: string) => {
        setOpenDropdownId((prev) =>
            prev === id ? null : id
        );
    };

    const closeDropdown = () => {
        setOpenDropdownId(null);
    };

    const handleDeleteClick = (id: string) => {
        setSelectedId(id);
        setConfirmOpen(true);
        closeDropdown();
    };

    const handleConfirmDelete = async () => {


        if (!selectedId) {
            return;
        }

        if (!selectedId) return;

        try {
            setDeleting(true);

            await dispatch(deleteTemplate(selectedId)).unwrap();


            dispatch(
                showAlert({
                    type: "success",
                    message: t("pdf.messages.deleteSuccess")
                })
            );

            setConfirmOpen(false);
            setSelectedId(null);

            await loadTemplates();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("pdf.messages.deleteError")
                })
            );
        } finally {
            setDeleting(false);
        }
    };

    const handleSaveTemplate = async (
        template: PdfTemplateResponse
    ) => {
        try {
            setSaving(true);

            const payload = {
                id: template.id,
                request: {
                    name: template.name,
                    code: template.code,
                    version: template.version,
                    htmlContent: template.htmlContent,
                    isActive: template.isActive,

                }
            };

            await dispatch(updateTemplate(payload)).unwrap();

            dispatch(
                showAlert({
                    type: "success",
                    message: t("pdf.messages.updateSuccess")
                })
            );

            setEditOpen(false);
            setSelectedTemplate(null);

            await loadTemplates();
        } catch (err: any) {
            dispatch(
                showAlert({
                    type: "error",
                    message: t("pdf.messages.updateError")
                })
            );
        } finally {
            setSaving(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("pdf.pageTitle")}
                description={t("pdf.pageDescription")}
            />

            <PageBreadcrumb
                pageTitle={t("pdf.pageTitle")}
            />

            <div className="space-y-6">
                <ComponentCard title={t("pdf.title")}>

                    <div className="flex justify-end mb-3">
                        <button
                            onClick={loadTemplates}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon className={`w-5 h-5 bg-color-gray ${spinning ? "animate-spin" : "rotate-90"}`} />
                        </button>
                    </div>
                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">

                        <div className="flex-1 overflow-y-auto">

                            <Table>

                                <TableHeader className="sticky top-0 bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-white/[0.05] z-10">

                                    <TableRow>
                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.name")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.code")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.version")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.status")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.created")}
                                        </TableCell>

                                        <TableCell
                                            isHeader
                                            className="px-5 py-3 text-xs font-semibold uppercase"
                                        >
                                            {t("pdf.columns.actions")}
                                        </TableCell>
                                    </TableRow>

                                </TableHeader>

                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">

                                    {fetching ? (
                                        < PdfTemplateTableSkeleton rows={5} />

                                    ) : templates.length === 0 ? (

                                        <TableRow>

                                            <TableCell
                                                colSpan={6}
                                                className="text-center py-10 text-gray-500"
                                            >
                                                {t("pdf.messages.empty")}
                                            </TableCell>

                                        </TableRow>

                                    ) : (

                                        templates.map((template) => (

                                            <TableRow
                                                key={template.id}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >

                                                <TableCell className="px-5 py-4 font-semibold">
                                                    {template.name}
                                                </TableCell>

                                                <TableCell className="px-5 py-4">
                                                    {template.code}
                                                </TableCell>

                                                <TableCell className="px-5 py-4">
                                                    v{template.version}
                                                </TableCell>

                                                <TableCell className="px-5 py-4">

                                                    <Badge
                                                        size="sm"
                                                        color={template.isActive ? "success" : "error"}
                                                    >
                                                        {template.isActive
                                                            ? t("pdf.status.active")
                                                            : t("pdf.status.inactive")}
                                                    </Badge>

                                                </TableCell>

                                                <TableCell className="px-5 py-4">

                                                    {new Date(
                                                        template.createdAt
                                                    ).toLocaleDateString()}

                                                </TableCell>

                                                <TableCell className="px-5 py-4 relative">

                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() =>
                                                            toggleDropdown(template.id)
                                                        }
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={
                                                            openDropdownId === template.id
                                                        }
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >

                                                        <div className="py-2 flex flex-col">

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedTemplate(template);
                                                                    setEditOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("pdf.actions.edit")}
                                                            </button>

                                                            <button
                                                                onClick={() => {
                                                                    setSelectedTemplate(template);
                                                                    setPreviewHtml(template.htmlContent);
                                                                    setPreviewTitle(template.name);
                                                                    setPreviewOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("pdf.actions.preview")}
                                                            </button>

                                                            <button
                                                                onClick={() => handleDeleteClick(template.id)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("pdf.actions.delete")}
                                                            </button>

                                                        </div>

                                                    </Dropdown>

                                                </TableCell>

                                            </TableRow>

                                        ))

                                    )}

                                </TableBody>

                            </Table>

                        </div>

                        <div className="border-t border-gray-100 dark:border-white/[0.05] shrink-0">

                            <Pagination
                                currentPage={page + 1}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={(p) => setPage(p - 1)}
                                onPageSizeChange={(size) => {
                                    setPageSize(size);
                                    setPage(0);
                                }}
                            />

                        </div>

                    </div>
                </ComponentCard>
            </div>

            <ConfirmDialog
                isOpen={confirmOpen}
                title="Delete PDF Template"
                description="Are you sure you want to delete this PDF template?"
                onConfirm={handleConfirmDelete}
                onCancel={() => {
                    setConfirmOpen(false);

                    setSelectedId(null);
                }}
                loading={deleting}
            />

            <PdfTemplatePreviewDialog
                isOpen={previewOpen}
                title={previewTitle}
                html={previewHtml}
                onClose={() => setPreviewOpen(false)}
            />

            <EditPdfTemplateModal
                isOpen={editOpen}
                template={selectedTemplate}
                onClose={() => {
                    setEditOpen(false);
                    setSelectedTemplate(null);
                }}
                onSave={handleSaveTemplate}
                loading={saving}
            />
        </>
    );
}

