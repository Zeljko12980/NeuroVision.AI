import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import ComponentCard from "../components/common/ComponentCard";
import PageBreadcrumb from "../components/common/PageBreadCrumb";
import PageMeta from "../components/common/PageMeta";
import Input from "../components/form/input/InputField";
import Label from "../components/form/Label";
import Button from "../components/ui/button/Button";
import { useAppDispatch } from "../store/store";
import { createAdministrator } from "../features/user/userSlice";
import { showAlert } from "../features/ui/uiSlice";

export default function CreateAdministratorPage() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [form, setForm] = useState({ userName: "", email: "" });
    const [loading, setLoading] = useState(false);

    const isFormValid =
        form.userName.trim().length >= 3 && /\S+@\S+\.\S+/.test(form.email.trim());

    const handleSubmit = async () => {
        if (!isFormValid) return;

        try {
            setLoading(true);
            await dispatch(
                createAdministrator({
                    userName: form.userName.trim(),
                    email: form.email.trim(),
                })
            ).unwrap();
            dispatch(showAlert({ message: t("users.createSuccess"), type: "success" }));
            navigate("/admin/users");
        } catch (err: unknown) {
            dispatch(
                showAlert({
                    message: err instanceof Error ? err.message : t("users.createError"),
                    type: "error",
                })
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <PageMeta
                title={t("users.createPageTitle")}
                description={t("users.createPageDescription")}
            />
            <PageBreadcrumb pageTitle={t("users.createPageTitle")} />

            <div className="max-w-2xl mx-auto">
                <ComponentCard title={t("users.createTitle")}>
                    <div className="space-y-5">
                        <div>
                            <Label htmlFor="userName">
                                {t("users.userName")} <span className="text-red-500">*</span>
                            </Label>
                            <Input
                                id="userName"
                                value={form.userName}
                                onChange={(e) =>
                                    setForm((prev) => ({ ...prev, userName: e.target.value }))
                                }
                            />
                        </div>
                        <div>
                            <Label htmlFor="email">
                                {t("users.email")} <span className="text-red-500">*</span>
                            </Label>
                            <Input
                                id="email"
                                type="email"
                                value={form.email}
                                onChange={(e) =>
                                    setForm((prev) => ({ ...prev, email: e.target.value }))
                                }
                            />
                        </div>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {t("users.createHint")}
                        </p>
                        <div className="flex justify-end gap-2 pt-2">
                            <Button
                                variant="outline"
                                type="button"
                                onClick={() => navigate("/admin/users")}
                                disabled={loading}
                            >
                                {t("users.cancel")}
                            </Button>
                            <Button
                                type="button"
                                onClick={handleSubmit}
                                disabled={loading || !isFormValid}
                            >
                                {loading ? t("users.creating") : t("users.create")}
                            </Button>
                        </div>
                    </div>
                </ComponentCard>
            </div>
        </>
    );
}
