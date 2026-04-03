import PageMeta from "../../components/common/PageMeta";
import AuthLayout from "./AuthPageLayout";
import TwoFAForm from "../../components/auth/TwoFAForm";

export default function TwoFA() {
    return (
        <>
            <PageMeta
                title="NeuroVision.AI"
                description="NeuroVision.AI"
            />
            <AuthLayout>
                <TwoFAForm />
            </AuthLayout>
        </>
    );
}
