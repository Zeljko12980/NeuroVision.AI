import { SidebarProvider, useSidebar } from "../context/SidebarContext";
import { Outlet } from "react-router";
import AppHeader from "./AppHeader";
import Backdrop from "./Backdrop";
import AppSidebar from "./AppSidebar";
import {  useAppSelector } from "../store/store";
import Alert from "../components/ui/alert/Alert";


const LayoutContent: React.FC = () => {
    const { isExpanded, isHovered, isMobileOpen } = useSidebar();
    const { message, type, visible } = useAppSelector((state) => state.ui);

    return (
        <div className="min-h-screen xl:flex">
            <div>
                <AppSidebar />
                <Backdrop />
            </div>

            <div
                className={`flex-1 transition-all duration-300 ease-in-out ${isExpanded || isHovered ? "lg:ml-[290px]" : "lg:ml-[90px]"
                    } ${isMobileOpen ? "ml-0" : ""}`}
            >
                <AppHeader />


                <div className="p-4 mx-auto max-w-(--breakpoint-2xl) md:p-6 space-y-4">
                    {visible && type && (
                        <div className="flex justify-end">
                            <Alert
                                variant={type}
                                title={type === "success" ? "Success" : "Error"}
                                message={message}
                            />
                        </div>
                    )}

                    <Outlet />
                </div>
            </div>
        </div>
    );
};

const AppLayout: React.FC = () => {
    return (
        <SidebarProvider>
            <LayoutContent />
        </SidebarProvider>
    );
};

export default AppLayout;