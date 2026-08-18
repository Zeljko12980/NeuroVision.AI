import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageBreadcrumb from "../components/common/PageBreadCrumb";
import UserMetaCard from "../components/UserProfile/UserMetaCard";
import UserInfoCard from "../components/UserProfile/UserInfoCard";
import ChangePasswordCard from "../components/UserProfile/ChangePasswordCard";
import PageMeta from "../components/common/PageMeta";
import { getMeRequest, ProfileDto } from "../features/auth/authService";
import { useAppSelector } from "../store/store";
import { selectUserClaims } from "../selectors/authSelectors";
import { getUserInfoFromClaims } from "../utils/claims";

export default function UserProfiles() {
    const { t } = useTranslation();
    const claims = useAppSelector(selectUserClaims);
    const { name, email, role } = getUserInfoFromClaims(claims || {});
    const [profile, setProfile] = useState<ProfileDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;

        const load = async () => {
            try {
                const me = await getMeRequest();
                if (!cancelled) setProfile(me);
            } catch {
                if (!cancelled) {
                    setProfile({
                        id: "",
                        userName: name,
                        email,
                        phoneNumber: null,
                        emailConfirmed: false,
                        roles: role ? [role] : [],
                    });
                }
            } finally {
                if (!cancelled) setLoading(false);
            }
        };

        load();
        return () => {
            cancelled = true;
        };
    }, [name, email, role]);

    const displayName = profile?.userName || name;
    const displayEmail = profile?.email || email;
    const displayRole = profile?.roles?.[0] || role;

    return (
        <>
            <PageMeta
                title="NeuroVision.AI"
                description="This is NeuroVision.AI."
            />
            <PageBreadcrumb pageTitle={t("profile.title")} />
            <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] lg:p-6">
                <h3 className="mb-5 text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-7">
                    {t("profile.title")}
                </h3>
                {loading ? (
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                        {t("profile.loading")}
                    </p>
                ) : (
                    <div className="space-y-6">
                        <UserMetaCard
                            userName={displayName}
                            email={displayEmail}
                            role={displayRole}
                        />
                        {profile && (
                            <UserInfoCard
                                profile={profile}
                                onUpdated={setProfile}
                            />
                        )}
                        <ChangePasswordCard />
                    </div>
                )}
            </div>
        </>
    );
}
