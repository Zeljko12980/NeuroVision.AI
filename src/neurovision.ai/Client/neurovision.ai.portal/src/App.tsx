import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "./store/store";
import { isTokenExpired } from "./utils/jwt";
import { logout } from "./features/auth/authSlice";

import SignIn from "./pages/AuthPages/SignIn";
import TwoFA from "./pages/AuthPages/TwoFA";
import NotFound from "./pages/OtherPage/NotFound";
import Home from "./pages/Dashboard/Home";
import UserProfiles from "./pages/UserProfiles";
import Calendar from "./pages/Calendar";
import Blank from "./pages/Blank";
import FormElements from "./pages/Forms/FormElements";
import BasicTables from "./pages/Tables/BasicTables";
import Videos from "./pages/UiElements/Videos";
import Images from "./pages/UiElements/Images";
import Alerts from "./pages/UiElements/Alerts";
import Badges from "./pages/UiElements/Badges";
import Avatars from "./pages/UiElements/Avatars";
import Buttons from "./pages/UiElements/Buttons";
import LineChart from "./pages/Charts/LineChart";
import BarChart from "./pages/Charts/BarChart";

import AppLayout from "./layout/AppLayout";
import PrivateRoute from "./components/common/PrivateRoute";
import RoleRoute from "./components/common/RoleRoute";
import { ScrollToTop } from "./components/common/ScrollToTop";

export default function App() {
    const token = useAppSelector((state) => state.auth.token);
    const dispatch = useAppDispatch();


    useEffect(() => {
        if (token && isTokenExpired(token)) {
            dispatch(logout());
        }
    }, [token, dispatch]);

    return (
        <Router>
            <ScrollToTop />
            <Routes>
                <Route path="/signin" element={<SignIn />} />
                <Route path="/confirm-2fa" element={<TwoFA />} />
                <Route path="*" element={<NotFound />} />

                <Route element={<PrivateRoute><AppLayout /></PrivateRoute>}>
                    <Route index path="/" element={<Home />} />

 
                    <Route element={<RoleRoute allowedRoles={["doctor"]} />}>
                        <Route path="/basic-tables" element={<BasicTables />} />
                    </Route>

                    <Route path="/profile" element={<UserProfiles />} />
                    <Route path="/calendar" element={<Calendar />} />
                    <Route path="/blank" element={<Blank />} />
                    <Route path="/form-elements" element={<FormElements />} />

                    <Route path="/alerts" element={<Alerts />} />
                    <Route path="/avatars" element={<Avatars />} />
                    <Route path="/badge" element={<Badges />} />
                    <Route path="/buttons" element={<Buttons />} />
                    <Route path="/images" element={<Images />} />
                    <Route path="/videos" element={<Videos />} />

                    <Route path="/line-chart" element={<LineChart />} />
                    <Route path="/bar-chart" element={<BarChart />} />
                </Route>
            </Routes>
        </Router>
    );
}