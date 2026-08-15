import Button from "../../components/ui/button/Button";
import { RefreshIcon } from "../../icons";

interface TumorRefreshButtonProps {
    label: string;
    spinning?: boolean;
    onClick: () => void;
}

export default function TumorRefreshButton({ label, spinning, onClick }: TumorRefreshButtonProps) {
    return (
        <Button
            variant="outline"
            size="sm"
            onClick={onClick}
            disabled={spinning}
            startIcon={<RefreshIcon className={spinning ? "animate-spin" : ""} />}
        >
            {label}
        </Button>
    );
}
