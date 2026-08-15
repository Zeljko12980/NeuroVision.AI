import { useEffect, useState } from "react";
import { Modal } from "../modal/index";
import Button from "../button/Button";
import Input from "../../form/input/InputField";
import Label from "../../form/Label";
import { RoleDto } from "../../../features/role/roleService";


interface EditRoleModalProps {
    isOpen: boolean;
    role: RoleDto | null;
    onClose: () => void;
    onSave: (role: RoleDto) => Promise<void> | void;
    loading?: boolean;
}

const EditRoleModal: React.FC<EditRoleModalProps> = ({
    isOpen,
    role,
    onClose,
    onSave,
    loading,
}) => {
    const [form, setForm] = useState<RoleDto | null>(null);

    useEffect(() => {
        setForm(role);
    }, [role]);

    if (!isOpen || !form) return null;

    const handleChange = (key: keyof RoleDto, value: string) => {
        setForm((prev) =>
            prev ? { ...prev, [key]: value } : prev
        );
    };

    const handleSubmit = async () => {
        if (!form) return;
        await onSave(form);
        onClose();
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-lg">
            <div className="p-6 space-y-5">
                <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                    Edit Role
                </h2>

                <div>
                    <Label htmlFor="roleName">Role Name</Label>
                    <Input
                        id="roleName"
                        value={form.name}
                        onChange={(e) =>
                            handleChange("name", e.target.value)
                        }
                        placeholder="Enter role name"
                    />
                </div>

                <div>
                    <Label htmlFor="description">Description</Label>
                    <Input
                        id="description"
                        value={form.description ?? ""}
                        onChange={(e) =>
                            handleChange("description", e.target.value)
                        }
                        placeholder="Enter description"
                    />
                </div>

                <div className="flex justify-end gap-2 pt-2">
                    <Button variant="outline" onClick={onClose} disabled={loading}>
                        Cancel
                    </Button>

                    <Button
                        onClick={handleSubmit}
                        disabled={loading}
                        className="bg-blue-600 hover:bg-blue-700 text-white"
                    >
                        {loading ? "Saving..." : "Save Changes"}
                    </Button>
                </div>
            </div>
        </Modal>
    );
};

export default EditRoleModal;