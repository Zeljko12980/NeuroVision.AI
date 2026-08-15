import PageMeta from "../../components/common/PageMeta";
import ConfirmEmail from "../../components/auth/ConfirmEmail";
import AuthLayout from "./AuthPageLayout";

export default function ConfirmEmailPage() {
    return (
        <>
            <PageMeta
                title="NeuroVision.AI"
                description="NeuroVision.AI"
            />
            <AuthLayout>
                <ConfirmEmail />
            </AuthLayout>
        </>
    );
}
