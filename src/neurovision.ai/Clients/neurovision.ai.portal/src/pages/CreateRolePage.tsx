import { useState } from "react";
import { useNavigate } from "react-router-dom";

import ComponentCard from "../components/common/ComponentCard";
import PageBreadcrumb from "../components/common/PageBreadCrumb";
import PageMeta from "../components/common/PageMeta";

import Input from "../components/form/input/InputField";
import Label from "../components/form/Label";
import Button from "../components/ui/button/Button";

import { useAppDispatch } from "../store/store";
import { createRole } from "../features/role/roleSlice";
import { showAlert } from "../features/ui/uiSlice";

export default function CreateRolePage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [form, setForm] = useState({
        roleName: "",
        description: "",
    });

    const [loading, setLoading] = useState(false);

    const handleChange = (key: string, value: string) => {
        setForm((prev) => ({
            ...prev,
            [key]: value,
        }));
    };

    const isFormValid = form.roleName.trim().length > 0;

    const handleSubmit = async () => {
        if (!isFormValid) {
            dispatch(
                showAlert({
                    message: "Role name is required",
                    type: "error",
                })
            );
            return;
        }

        try {
            setLoading(true);

            await dispatch(
                createRole({
                    roleName: form.roleName,
                    description: form.description,
                })
            ).unwrap();

            dispatch(
                showAlert({
                    message: "Role created successfully",
                    type: "success",
                })
            );

            navigate("/admin/roles");
        } catch (err: any) {
            dispatch(
                showAlert({
                    message: err?.message || "Failed to create role",
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
                title="Create Role | NeuroVision.AI"
                description="Create new system role"
            />

            <PageBreadcrumb pageTitle="Create Role" />

            <div className="max-w-2xl mx-auto">
                <ComponentCard title="New Role">

                    <div className="space-y-5">

                        {/* ROLE NAME */}
                        <div>
                            <Label htmlFor="roleName">
                                Role Name <span className="text-red-500">*</span>
                            </Label>

                            <Input
                                id="roleName"
                                value={form.roleName}
                                onChange={(e) =>
                                    handleChange("roleName", e.target.value)
                                }
                                placeholder="Enter role name"
                            />
                        </div>

                        <div>
                            <Label htmlFor="description">
                                Description
                            </Label>

                            <Input
                                id="description"
                                value={form.description}
                                onChange={(e) =>
                                    handleChange("description", e.target.value)
                                }
                                placeholder="Enter description"
                            />
                        </div>
                        <div className="flex justify-end gap-2 pt-2">

                            <Button
                                variant="outline"
                                onClick={() => navigate("/admin/roles")}
                                disabled={loading}
                            >
                                Cancel
                            </Button>

                            <Button
                                onClick={handleSubmit}
                                disabled={loading || !isFormValid}
                                className={`text-white ${isFormValid
                                        ? "bg-blue-600 hover:bg-blue-700"
                                        : "bg-gray-400 cursor-not-allowed"
                                    }`}
                            >
                                {loading ? "Creating..." : "Create Role"}
                            </Button>

                        </div>

                    </div>

                </ComponentCard>
            </div>
        </>
    );
}