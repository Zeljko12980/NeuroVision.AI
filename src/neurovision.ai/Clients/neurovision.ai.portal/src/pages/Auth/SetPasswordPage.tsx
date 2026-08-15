import PageMeta from "../../components/common/PageMeta";
import AuthLayout from "./AuthPageLayout";
import SetPasswordForm from "../../components/auth/SetPasswordForm";

export default function SetPasswordPage() {
    return (
        <>
            <PageMeta
                title="NeuroVision.AI"
                description="Set your password"
            />
            <AuthLayout>
                <SetPasswordForm />
            </AuthLayout>
        </>
    );
}