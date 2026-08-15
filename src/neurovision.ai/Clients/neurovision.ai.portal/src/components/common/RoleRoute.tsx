import { Navigate, Outlet } from "react-router-dom";
import { useAppSelector } from "../../store/store";
import { selectUserClaims } from "../../selectors/authSelectors";

interface Props {
    allowedRoles: string[];
}

const RoleRoute: React.FC<Props> = ({ allowedRoles }) => {
    const claims = useAppSelector(selectUserClaims);

    if (!claims) return <Navigate to="/signin" replace />;

    const role =
        claims.role ||
        claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    if (!role || !allowedRoles.includes(role.toLowerCase())) {
        return <Navigate to="/" replace />;
    }

    return <Outlet />;
};

export default RoleRoute;