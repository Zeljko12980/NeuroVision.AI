import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import Input from '../../../components/form/input/InputField';
import Label from '../../../components/form/Label';
import Button from '../../../components/ui/button/Button';
import { Modal } from '../../../components/ui/modal';

export interface GovernmentTypeForm {
    code: string;
    name: string;
}

interface Props {
    isOpen: boolean;
    governmentType: GovernmentTypeForm | null;
    loading: boolean;
    onClose: () => void;
    onSave: (form: GovernmentTypeForm) => Promise<void>;
}

const EMPTY_FORM: GovernmentTypeForm = { code: '', name: '' };

export default function EditGovernmentTypeModal({
    isOpen,
    governmentType,
    loading,
    onClose,
    onSave,
}: Props) {
    const { t } = useTranslation();

    const [form, setForm] = useState<GovernmentTypeForm>(EMPTY_FORM);

    useEffect(() => {
        setForm(governmentType ? { ...governmentType } : EMPTY_FORM);
    }, [governmentType]);

    if (!isOpen) return null;

    const isValid = form.code.trim().length > 0 && form.name.trim().length > 0;

    const handleSubmit = async () => {
        if (!isValid) return;

        // No try/catch here on purpose: let the error bubble up to the
        // parent, which does dispatch(...).unwrap() in its own try/catch
        // and shows the success/error toast. If we swallow it here, the
        // parent never finds out the save failed and no toast fires.
        await onSave(form);
        onClose();
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} className="max-w-lg">
            <div className="bg-white dark:bg-gray-900 rounded-2xl p-6">
                <h2 className="text-xl font-semibold">
                    {governmentType
                        ? t('location.governmentTypes.editTitle')
                        : t('location.governmentTypes.createTitle')}
                </h2>

                <div className="mt-4 space-y-4">
                    <div>
                        <Label>{t('location.governmentTypes.fields.code')}</Label>
                        <Input
                            value={form.code}
                            onChange={(e) => setForm((p) => ({ ...p, code: e.target.value }))}
                            disabled={!!governmentType}
                        />
                    </div>
                    <div>
                        <Label>{t('location.governmentTypes.fields.name')}</Label>
                        <Input
                            value={form.name}
                            onChange={(e) => setForm((p) => ({ ...p, name: e.target.value }))}
                        />
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3">
                    <Button variant="ghost" onClick={onClose} disabled={loading}>
                        {t('common.cancel')}
                    </Button>
                    <Button onClick={handleSubmit} disabled={loading || !isValid}>
                        {loading ? t('common.saving') : t('common.save')}
                    </Button>
                </div>
            </div>
        </Modal>
    );
}