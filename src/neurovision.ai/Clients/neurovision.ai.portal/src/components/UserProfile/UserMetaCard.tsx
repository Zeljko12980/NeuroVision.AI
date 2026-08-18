import { useTranslation } from "react-i18next";
import ProfileAvatar from "./ProfileAvatar";

type UserMetaCardProps = {
    userName: string;
    email: string;
    role: string;
    photoUrl?: string | null;
};

export default function UserMetaCard({
    userName,
    email,
    role,
    photoUrl,
}: UserMetaCardProps) {
    const { t } = useTranslation();

    return (
        <div className="p-5 border border-gray-200 rounded-2xl dark:border-gray-800 lg:p-6">
            <div className="flex flex-col items-center w-full gap-6 xl:flex-row">
                <div className="w-20 h-20 overflow-hidden border border-gray-200 rounded-full dark:border-gray-800">
                    <ProfileAvatar
                        src={photoUrl}
                        alt={userName}
                        className="h-full w-full object-cover"
                    />
                </div>
                <div>
                    <h4 className="mb-2 text-lg font-semibold text-center text-gray-800 dark:text-white/90 xl:text-left">
                        {userName || t("profile.unnamed")}
                    </h4>
                    <div className="flex flex-col items-center gap-1 text-center xl:flex-row xl:gap-3 xl:text-left">
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {role}
                        </p>
                        <div className="hidden h-3.5 w-px bg-gray-300 dark:bg-gray-700 xl:block"></div>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            {email}
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
