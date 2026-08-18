import { FormEvent, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import { useModal } from "../../hooks/useModal";
import { Modal } from "../ui/modal";
import Button from "../ui/button/Button";
import Input from "../form/input/InputField";
import Label from "../form/Label";
import { ProfileDto, updateProfileRequest } from "../../features/auth/authService";
import { showAlert } from "../../features/ui/uiSlice";
import { useAppDispatch } from "../../store/store";
import { isValidInternationalPhone, normalizeInternationalPhone, PHONE_EXAMPLE } from "../../utils/phone";

type UserInfoCardProps = {
    profile: ProfileDto;
    onUpdated: (profile: ProfileDto) => void;
};

export default function UserInfoCard({
    profile,
    onUpdated,
}: UserInfoCardProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const { isOpen, openModal, closeModal } = useModal();
    const [userName, setUserName] = useState(profile.userName);
    const [phoneNumber, setPhoneNumber] = useState(profile.phoneNumber ?? "");
    const [phoneError, setPhoneError] = useState("");
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        setUserName(profile.userName);
        setPhoneNumber(profile.phoneNumber ?? "");
        setPhoneError("");
    }, [profile]);

    const handleSave = async (e: FormEvent) => {
        e.preventDefault();

        const trimmedPhone = phoneNumber.trim();
        if (trimmedPhone && !isValidInternationalPhone(trimmedPhone)) {
            setPhoneError(t("profile.phoneInvalid"));
            return;
        }

        setPhoneError("");
        setSaving(true);
        try {
            const updated = await updateProfileRequest({
                userName: userName.trim(),
                phoneNumber: trimmedPhone ? normalizeInternationalPhone(trimmedPhone) : null,
            });
            onUpdated(updated);
            dispatch(showAlert({ message: t("profile.updateSuccess"), type: "success" }));
            closeModal();
        } catch (error: unknown) {
            const message =
                error instanceof Error ? error.message : t("profile.updateError");
            dispatch(showAlert({ message, type: "error" }));
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="p-5 border border-gray-200 rounded-2xl dark:border-gray-800 lg:p-6">
            <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
                <div>
                    <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-6">
                        {t("profile.personalInfo")}
                    </h4>

                    <div className="grid grid-cols-1 gap-4 lg:grid-cols-2 lg:gap-7 2xl:gap-x-32">
                        <div>
                            <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                                {t("profile.userName")}
                            </p>
                            <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                                {profile.userName}
                            </p>
                        </div>

                        <div>
                            <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                                {t("profile.email")}
                            </p>
                            <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                                {profile.email}
                            </p>
                        </div>

                        <div>
                            <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                                {t("profile.phone")}
                            </p>
                            <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                                {profile.phoneNumber || t("profile.notSet")}
                            </p>
                        </div>

                        <div>
                            <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                                {t("profile.emailConfirmed")}
                            </p>
                            <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                                {profile.emailConfirmed
                                    ? t("profile.yes")
                                    : t("profile.no")}
                            </p>
                        </div>
                    </div>
                </div>

                <button
                    type="button"
                    onClick={openModal}
                    className="flex w-full items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200 lg:inline-flex lg:w-auto"
                >
                    <svg
                        className="fill-current"
                        width="18"
                        height="18"
                        viewBox="0 0 18 18"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                    >
                        <path
                            fillRule="evenodd"
                            clipRule="evenodd"
                            d="M15.0911 2.78206C14.2125 1.90338 12.7878 1.90338 11.9092 2.78206L4.57524 10.116C4.26682 10.4244 4.0547 10.8158 3.96468 11.2426L3.31231 14.3352C3.25997 14.5833 3.33653 14.841 3.51583 15.0203C3.69512 15.1996 3.95286 15.2761 4.20096 15.2238L7.29355 14.5714C7.72031 14.4814 8.11172 14.2693 8.42013 13.9609L15.7541 6.62695C16.6327 5.74827 16.6327 4.32365 15.7541 3.44497L15.0911 2.78206ZM12.9698 3.84272C13.2627 3.54982 13.7376 3.54982 14.0305 3.84272L14.6934 4.50563C14.9863 4.79852 14.9863 5.2734 14.6934 5.56629L14.044 6.21573L12.3204 4.49215L12.9698 3.84272ZM11.2597 5.55281L5.6359 11.1766C5.53309 11.2794 5.46238 11.4099 5.43238 11.5522L5.01758 13.5185L6.98394 13.1037C7.1262 13.0737 7.25666 13.003 7.35947 12.9002L12.9833 7.27639L11.2597 5.55281Z"
                            fill=""
                        />
                    </svg>
                    {t("profile.edit")}
                </button>
            </div>

            <Modal isOpen={isOpen} onClose={closeModal} className="max-w-[700px] m-4">
                <div className="no-scrollbar relative w-full max-w-[700px] overflow-y-auto rounded-3xl bg-white p-4 dark:bg-gray-900 lg:p-11">
                    <div className="px-2 pr-14">
                        <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
                            {t("profile.editTitle")}
                        </h4>
                        <p className="mb-6 text-sm text-gray-500 dark:text-gray-400 lg:mb-7">
                            {t("profile.editSubtitle")}
                        </p>
                    </div>
                    <form className="flex flex-col" onSubmit={handleSave}>
                        <div className="grid grid-cols-1 gap-x-6 gap-y-5 px-2 pb-3 lg:grid-cols-2">
                            <div>
                                <Label>{t("profile.userName")}</Label>
                                <Input
                                    type="text"
                                    value={userName}
                                    onChange={(e) => setUserName(e.target.value)}
                                />
                            </div>

                            <div>
                                <Label>{t("profile.phone")}</Label>
                                <Input
                                    type="tel"
                                    value={phoneNumber}
                                    placeholder={PHONE_EXAMPLE}
                                    onChange={(e) => {
                                        setPhoneNumber(e.target.value);
                                        setPhoneError("");
                                    }}
                                    error={!!phoneError}
                                    hint={phoneError || t("profile.phoneHint")}
                                />
                            </div>

                            <div className="lg:col-span-2">
                                <Label>{t("profile.email")}</Label>
                                <Input type="email" value={profile.email} disabled />
                                <p className="mt-1.5 text-xs text-gray-500 dark:text-gray-400">
                                    {t("profile.emailReadOnly")}
                                </p>
                            </div>
                        </div>
                        <div className="flex items-center gap-3 px-2 mt-6 lg:justify-end">
                            <Button size="sm" variant="outline" type="button" onClick={closeModal}>
                                {t("profile.close")}
                            </Button>
                            <Button size="sm" type="submit" disabled={saving || !userName.trim()}>
                                {saving ? t("profile.saving") : t("profile.save")}
                            </Button>
                        </div>
                    </form>
                </div>
            </Modal>
        </div>
    );
}
