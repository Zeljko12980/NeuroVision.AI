import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useLocation } from "react-router";
import { useAppSelector } from "../store/store";
import { selectUserClaims } from "../selectors/authSelectors";

import {
    ChevronDownIcon,
    HorizontaLDots,
    TableIcon,
    InfoIcon,
    ArrowUpIcon,
    GroupIcon,
    FolderIcon,
    DocsIcon
} from "../icons";
import { useSidebar } from "../context/SidebarContext";
import { getUserInfoFromClaims } from "../utils/claims";

type NavItem = {
    name: string;
    icon: React.ReactNode;
    path?: string;
    subItems?: { name: string; path: string; pro?: boolean; new?: boolean }[];
};

const patientNavItems: NavItem[] = [
    { name: "Dashboard", icon: <TableIcon />, path: "/" },
    {
        name: "My Scans",
        icon: <FolderIcon />,
        subItems: [{ name: "View Scans", path: "/my-scans" }],
    },
    {
        name: "My Analysis",
        icon: <ArrowUpIcon />,
        subItems: [
            { name: "New Analysis", path: "/my-analysis/new" },
            { name: "Archived Analysis", path: "/my-analysis/archive" },
        ],
    },
    { name: "My Reports", icon: <DocsIcon />, path: "/my-reports" },
];

const doctorNavItems: NavItem[] = [
    { name: "Dashboard", icon: <TableIcon />, path: "/" },
    {
        name: "Patients",
        icon: <GroupIcon />,
        subItems: [
            { name: "Patient List", path: "/patients/list" },
            { name: "Add Patient", path: "/patients/add" },
        ],
    },
    {
        name: "Scans",
        icon: <FolderIcon />,
        subItems: [
            { name: "View Scans", path: "/scans/list" },
            { name: "Add Scan", path: "/scans/add" },
        ],
    },
    {
        name: "Analysis",
        icon: <ArrowUpIcon />,
        subItems: [
            { name: "New Analysis", path: "/analysis/new" },
            { name: "Archived Analysis", path: "/analysis/archive" },
        ],
    },
    { name: "Reports", icon: <DocsIcon />, path: "/reports" },
    { name: "AI Monitoring", icon: <InfoIcon />, path: "/ai-monitoring" },
];

const AppSidebar: React.FC = () => {
    const { isExpanded, isMobileOpen, isHovered, setIsHovered } = useSidebar();
    const location = useLocation();
    const claims = useAppSelector(selectUserClaims);
    const { role } = getUserInfoFromClaims(claims || {});


    const userRole = role.toLowerCase() || "patient";
    const navItems = userRole === "doctor" ? doctorNavItems : patientNavItems;

    const [openSubmenu, setOpenSubmenu] = useState<{ index: number } | null>(null);
    const [subMenuHeight, setSubMenuHeight] = useState<Record<string, number>>({});
    const subMenuRefs = useRef<Record<string, HTMLDivElement | null>>({});

    const isActive = useCallback(
        (path: string) => location.pathname === path,
        [location.pathname]
    );

    useEffect(() => {
        let matched = false;
        navItems.forEach((nav, index) => {
            nav.subItems?.forEach((subItem) => {
                if (isActive(subItem.path)) {
                    setOpenSubmenu({ index });
                    matched = true;
                }
            });
            if (nav.path && isActive(nav.path)) matched = true;
        });
        if (!matched) setOpenSubmenu(null);
    }, [location.pathname, navItems, isActive]);

    useEffect(() => {
        if (openSubmenu !== null) {
            const key = `${openSubmenu.index}`;
            if (subMenuRefs.current[key]) {
                setSubMenuHeight((prev) => ({
                    ...prev,
                    [key]: subMenuRefs.current[key]?.scrollHeight || 0,
                }));
            }
        }
    }, [openSubmenu]);

    const toggleSubmenu = (index: number) => {
        setOpenSubmenu((prev) => (prev?.index === index ? null : { index }));
    };

    const renderMenuItems = (items: NavItem[]) => (
        <ul className="flex flex-col gap-4">
            {items.map((nav, index) => (
                <li key={nav.name}>
                    {nav.subItems ? (
                        <>
                            <button
                                onClick={() => toggleSubmenu(index)}
                                className={`menu-item group ${openSubmenu?.index === index ? "menu-item-active" : "menu-item-inactive"
                                    } cursor-pointer ${!isExpanded && !isHovered ? "lg:justify-center" : "lg:justify-start"}`}
                            >
                                <span
                                    className={`menu-item-icon-size ${openSubmenu?.index === index
                                        ? "menu-item-icon-active"
                                        : "menu-item-icon-inactive"
                                        }`}
                                >
                                    {nav.icon}
                                </span>
                                {(isExpanded || isHovered || isMobileOpen) && (
                                    <span className="menu-item-text">{nav.name}</span>
                                )}
                                {(isExpanded || isHovered || isMobileOpen) && (
                                    <ChevronDownIcon
                                        className={`ml-auto w-5 h-5 transition-transform duration-200 ${openSubmenu?.index === index ? "rotate-180 text-brand-500" : ""
                                            }`}
                                    />
                                )}
                            </button>

                            <div
                                ref={(el) => (subMenuRefs.current[`${index}`] = el)}
                                className="overflow-hidden transition-all duration-300"
                                style={{
                                    height: openSubmenu?.index === index ? `${subMenuHeight[`${index}`]}px` : "0px",
                                }}
                            >
                                <ul className="mt-2 space-y-1 ml-9">
                                    {nav.subItems.map((subItem) => (
                                        <li key={subItem.name}>
                                            <Link
                                                to={subItem.path}
                                                className={`menu-dropdown-item ${isActive(subItem.path)
                                                    ? "menu-dropdown-item-active"
                                                    : "menu-dropdown-item-inactive"
                                                    }`}
                                            >
                                                {subItem.name}
                                                <span className="flex items-center gap-1 ml-auto">
                                                    {subItem.new && (
                                                        <span
                                                            className={`ml-auto ${isActive(subItem.path)
                                                                ? "menu-dropdown-badge-active"
                                                                : "menu-dropdown-badge-inactive"
                                                                } menu-dropdown-badge`}
                                                        >
                                                            new
                                                        </span>
                                                    )}
                                                    {subItem.pro && (
                                                        <span
                                                            className={`ml-auto ${isActive(subItem.path)
                                                                ? "menu-dropdown-badge-active"
                                                                : "menu-dropdown-badge-inactive"
                                                                } menu-dropdown-badge`}
                                                        >
                                                            pro
                                                        </span>
                                                    )}
                                                </span>
                                            </Link>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        </>
                    ) : (
                        nav.path && (
                            <Link
                                to={nav.path}
                                className={`menu-item group ${isActive(nav.path) ? "menu-item-active" : "menu-item-inactive"
                                    }`}
                            >
                                <span
                                    className={`menu-item-icon-size ${isActive(nav.path) ? "menu-item-icon-active" : "menu-item-icon-inactive"
                                        }`}
                                >
                                    {nav.icon}
                                </span>
                                {(isExpanded || isHovered || isMobileOpen) && (
                                    <span className="menu-item-text">{nav.name}</span>
                                )}
                            </Link>
                        )
                    )}
                </li>
            ))}
        </ul>
    );

    return (
        <aside
            className={`fixed mt-16 flex flex-col lg:mt-0 top-0 px-5 left-0 bg-white dark:bg-gray-900 dark:border-gray-800 text-gray-900 h-screen transition-all duration-300 ease-in-out z-50 border-r border-gray-200 ${isExpanded || isMobileOpen
                ? "w-[290px]"
                : isHovered
                    ? "w-[290px]"
                    : "w-[90px]"
                } ${isMobileOpen ? "translate-x-0" : "-translate-x-full"} lg:translate-x-0`}
            onMouseEnter={() => !isExpanded && setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
        >
            <div className={`py-8 flex ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"}`}>
                <Link to="/">
                    {isExpanded || isHovered || isMobileOpen ? (
                        <>
                            <img
                                className="dark:hidden"
                                src="/images/logo/neurovision-light-horizontal.svg"
                                alt="Logo Light"
                                width={300}
                                height={60}
                            />
                            <img
                                className="hidden dark:block"
                                src="/images/logo/neurovision-dark-horizontal.svg"
                                alt="Logo Dark"
                                width={300}
                                height={60}
                            />
                        </>
                    ) : (
                        <img src="/images/logo/neurovision-icon-light.svg" alt="Logo" width={32} height={32} />
                    )}
                </Link>
            </div>
            <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
                <nav className="mb-6">
                    <div className="flex flex-col gap-4">
                        <div>
                            <h2
                                className={`mb-4 text-xs uppercase flex leading-[20px] text-gray-400 ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"
                                    }`}
                            >
                                {isExpanded || isHovered || isMobileOpen ? "Menu" : <HorizontaLDots className="size-6" />}
                            </h2>
                            {renderMenuItems(navItems)}
                        </div>
                    </div>
                </nav>
            </div>
        </aside>
    );
};

export default AppSidebar;