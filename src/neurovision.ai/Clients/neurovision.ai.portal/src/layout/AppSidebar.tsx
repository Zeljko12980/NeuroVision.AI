import {
    useCallback,
    useEffect,
    useState
} from "react";

import {
    Link,
    useLocation
} from "react-router";


import {
    useTranslation
} from "react-i18next";


import {
    useAppSelector
} from "../store/store";


import {
    selectUserClaims
} from "../selectors/authSelectors";


import {
    ChevronDownIcon,
    HorizontaLDots,
    TableIcon,
    InfoIcon,
    ArrowUpIcon,
    GroupIcon,
    FolderIcon,
    DocsIcon,
    GlobeIcon
} from "../icons";


import {
    useSidebar
} from "../context/SidebarContext";


import {
    getUserInfoFromClaims
} from "../utils/claims";

import BrandLogo from "../components/common/BrandLogo";



type NavItem = {

    nameKey: string;

    icon?: React.ReactNode;

    path?: string;

    subItems?: NavItem[];

};





/*
===========================
PATIENT MENU
===========================
*/


const patientNavItems: NavItem[] = [


    {
        nameKey: "sidebar.dashboard",
        icon: <TableIcon />,
        path: "/"
    },


    {
        nameKey: "sidebar.myScans",
        icon: <FolderIcon />,

        subItems: [

            {
                nameKey: "sidebar.viewScans",
                icon: <TableIcon />,
                path: "/my-scans"
            }

        ]
    },



    {
        nameKey: "sidebar.myAnalysis",
        icon: <ArrowUpIcon />,

        subItems: [

            {
                nameKey: "sidebar.newAnalysis",
                icon: <DocsIcon />,
                path: "/my-analysis/new"
            },

            {
                nameKey: "sidebar.archivedAnalysis",
                icon: <DocsIcon />,
                path: "/my-analysis/archive"
            }

        ]
    },



    {
        nameKey: "sidebar.myReports",
        icon: <DocsIcon />,
        path: "/my-reports"
    }


];







/*
===========================
DOCTOR MENU
===========================
*/


const doctorNavItems: NavItem[] = [



    {
        nameKey: "sidebar.dashboard",
        icon: <TableIcon />,
        path: "/"
    },




    {
        nameKey: "sidebar.patients",
        icon: <GroupIcon />,

        subItems: [


            {
                nameKey: "sidebar.patientList",
                icon: <TableIcon />,
                path: "/patients/list"
            },


            {
                nameKey: "sidebar.addPatient",
                icon: <DocsIcon />,
                path: "/patients/add"
            }


        ]
    },





    {
        nameKey: "sidebar.scans",
        icon: <FolderIcon />,

        subItems: [


            {
                nameKey: "sidebar.viewScans",
                icon: <TableIcon />,
                path: "/scans/list"
            },


            {
                nameKey: "sidebar.addScan",
                icon: <DocsIcon />,
                path: "/scans/add"
            }


        ]
    },





    {
        nameKey: "sidebar.analysis",
        icon: <ArrowUpIcon />,

        subItems: [


            {
                nameKey: "sidebar.newAnalysis",
                icon: <DocsIcon />,
                path: "/analysis/new"
            },


            {
                nameKey: "sidebar.archivedAnalysis",
                icon: <DocsIcon />,
                path: "/analysis/archive"
            }


        ]
    },





    {
        nameKey: "sidebar.reports",
        icon: <DocsIcon />,
        path: "/reports"
    },





    {
        nameKey: "sidebar.aiMonitoring",
        icon: <InfoIcon />,
        path: "/ai-monitoring"
    }



];

/*
===========================
SUPER ADMIN MENU
===========================
*/


const superAdminNavItems: NavItem[] = [


    {
        nameKey: "sidebar.dashboard",
        icon: <TableIcon />,
        path: "/"
    },





    {
        nameKey: "sidebar.roleManagement",
        icon: <GroupIcon />,
        subItems: [
            {
                nameKey: "sidebar.roles",
                icon: <TableIcon />,
                path: "/admin/roles"
            },
            {
                nameKey: "sidebar.createRole",
                icon: <DocsIcon />,
                path: "/admin/roles/create"
            }
        ]
    },

    {
        nameKey: "sidebar.userManagement",
        icon: <GroupIcon />,
        subItems: [
            {
                nameKey: "sidebar.users",
                icon: <TableIcon />,
                path: "/admin/users"
            },
            {
                nameKey: "sidebar.createAdministrator",
                icon: <DocsIcon />,
                path: "/admin/users/create"
            }
        ]
    },






    {
        nameKey: "sidebar.doctors",
        icon: <GroupIcon />,

        subItems: [


            {
                nameKey: "sidebar.doctorList",
                icon: <TableIcon />,
                path: "/admin/doctors"
            },


            {
                nameKey: "sidebar.addDoctor",
                icon: <DocsIcon />,
                path: "/admin/doctors/add"
            }


        ]
    },






    {
        nameKey: "sidebar.patients",
        icon: <GroupIcon />,

        subItems: [


            {
                nameKey: "sidebar.patientList",
                icon: <TableIcon />,
                path: "/admin/patients"
            }


        ]
    },







    {
        nameKey: "sidebar.pdfManagement",
        icon: <DocsIcon />,

        subItems: [


            {
                nameKey: "sidebar.pdfTemplates",
                icon: <TableIcon />,
                path: "/admin/pdfs"
            },


            {
                nameKey: "sidebar.createPdfTemplate",
                icon: <DocsIcon />,
                path: "/admin/pdfs/create"
            }


        ]
    },

    {
        nameKey: "sidebar.certificateManagement",
        icon: <DocsIcon />,

        subItems: [


            {
                nameKey: "sidebar.certificates",
                icon: <TableIcon />,
                path: "/admin/certificates"
            },


            {
                nameKey: "sidebar.createCertificate",
                icon: <DocsIcon />,
                path: "/admin/certificates/create"
            }


        ]
    },







    /*
    ===========================
    LOCATION MANAGEMENT
    ===========================
    */


    {
        nameKey: "sidebar.territorialOrganization",
        icon: <GlobeIcon />,


        subItems: [



            {
                nameKey: "sidebar.countries",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.countriesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/countries"
                    },


                    {
                        nameKey: "sidebar.createCountry",
                        icon: <DocsIcon />,
                        path: "/admin/location/countries/create"
                    }


                ]

            },
            {
                nameKey: "sidebar.governmentTypes",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.governmentTypesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/government-types"
                    },


                    {
                        nameKey: "sidebar.createGovernmentType",
                        icon: <DocsIcon />,
                        path: "/admin/location/gov/create"
                    }

                ]

            },
            {
                nameKey: "sidebar.healthInstitutions",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.healthInstitutionsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/health-institutions"
                    },


                    {
                        nameKey: "sidebar.createHealthInstitution",
                        icon: <DocsIcon />,
                        path: "/admin/location/health-institutions/create"
                    }

                ]

            },

            {
                nameKey: "sidebar.healthInstitutionTypes",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.healthInstitutionTypesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/health-institutions-types"
                    },


                    {
                        nameKey: "sidebar.createHealthInstitutionType",
                        icon: <DocsIcon />,
                        path: "/admin/location/health-institutions-types/create"
                    }

                ]

            },






            {
                nameKey: "sidebar.settlements",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.settlementsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/settlements"
                    },


                    {
                        nameKey: "sidebar.createSettlement",
                        icon: <DocsIcon />,
                        path: "/admin/location/settlements/create"
                    }


                ]

            },








            {
                nameKey: "sidebar.municipalities",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.municipalitiesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/municipalities"
                    },


                    {
                        nameKey: "sidebar.createMunicipality",
                        icon: <DocsIcon />,
                        path: "/admin/location/municipalities/create"
                    }


                ]

            },








            {
                nameKey: "sidebar.regions",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.regionsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/regions"
                    },


                    {
                        nameKey: "sidebar.createRegion",
                        icon: <DocsIcon />,
                        path: "/admin/location/regions/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.capitals",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.capitalsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/capitals"
                    },


                    {
                        nameKey: "sidebar.createCapital",
                        icon: <DocsIcon />,
                        path: "/admin/location/capitals/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.regionTypes",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.regionTypesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/region-types"
                    },


                    {
                        nameKey: "sidebar.createRegionType",
                        icon: <DocsIcon />,
                        path: "/admin/location/region-types/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.localCommunities",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.localCommunitiesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/local-communities"
                    },


                    {
                        nameKey: "sidebar.createLocalCommunity",
                        icon: <DocsIcon />,
                        path: "/admin/location/local-communities/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.countryCompositions",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.countryCompositionsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/country-compositions"
                    },


                    {
                        nameKey: "sidebar.createCountryComposition",
                        icon: <DocsIcon />,
                        path: "/admin/location/country-compositions/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.regionCompositions",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.regionCompositionsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/region-compositions"
                    },


                    {
                        nameKey: "sidebar.createRegionComposition",
                        icon: <DocsIcon />,
                        path: "/admin/location/region-compositions/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.regionSettlementCoverages",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.regionSettlementCoveragesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/region-settlement-coverages"
                    },


                    {
                        nameKey: "sidebar.createRegionSettlementCoverage",
                        icon: <DocsIcon />,
                        path: "/admin/location/region-settlement-coverages/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.municipalitySettlementCoverages",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.municipalitySettlementCoveragesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/municipality-settlement-coverages"
                    },


                    {
                        nameKey: "sidebar.createMunicipalitySettlementCoverage",
                        icon: <DocsIcon />,
                        path: "/admin/location/municipality-settlement-coverages/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.localCommunityCoverages",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.localCommunityCoveragesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/local-community-coverages"
                    },


                    {
                        nameKey: "sidebar.createLocalCommunityCoverage",
                        icon: <DocsIcon />,
                        path: "/admin/location/local-community-coverages/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.legalSuccessors",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.legalSuccessorsTable",
                        icon: <TableIcon />,
                        path: "/admin/location/legal-successors"
                    },


                    {
                        nameKey: "sidebar.createLegalSuccessor",
                        icon: <DocsIcon />,
                        path: "/admin/location/legal-successors/create"
                    }


                ]

            },

            {
                nameKey: "sidebar.governmentHistories",
                icon: <GlobeIcon />,


                subItems: [


                    {
                        nameKey: "sidebar.governmentHistoriesTable",
                        icon: <TableIcon />,
                        path: "/admin/location/government-histories"
                    },


                    {
                        nameKey: "sidebar.createGovernmentHistory",
                        icon: <DocsIcon />,
                        path: "/admin/location/government-histories/create"
                    }


                ]

            }



        ]

    },


    {
        nameKey: "sidebar.system",
        icon: <InfoIcon />,


        subItems: [


            {
                nameKey: "sidebar.healthMonitoring",
                icon: <InfoIcon />,
                path: "/admin/health"
            },

            {
                nameKey: "sidebar.aiMonitoring",
                icon: <InfoIcon />,
                path: "/admin/ai-monitoring"
            },


            {
                nameKey: "sidebar.logs",
                icon: <DocsIcon />,
                path: "/admin/logs"
            },


            {
                nameKey: "sidebar.settings",
                icon: <DocsIcon />,
                path: "/admin/settings"
            }


        ]

    },







    {
        nameKey: "sidebar.reports",
        icon: <DocsIcon />,
        path: "/admin/reports"
    }



];

const AppSidebar = () => {


    const { t } = useTranslation();


    const {
        isExpanded,
        isMobileOpen,
        isHovered,
        setIsHovered
    } = useSidebar();



    const location = useLocation();



    const claims = useAppSelector(selectUserClaims);



    const { role } = getUserInfoFromClaims(claims || {});



    const userRole = role?.toLowerCase() ?? "patient";



    const navItems =
        userRole === "superadministrator"
            ? superAdminNavItems
            : userRole === "doctor"
                ? doctorNavItems
                : patientNavItems;



    const [openMenus, setOpenMenus] = useState<string[]>([]);



    const isActive = useCallback(
        (path: string) =>
            location.pathname === path,
        [location.pathname]
    );




    const hasActiveChild = useCallback(
        (item: NavItem): boolean => {


            if (item.path) {
                return isActive(item.path);
            }



            return item.subItems?.some(
                child => hasActiveChild(child)
            ) ?? false;


        },
        [isActive]
    );



    useEffect(() => {



        const openParents = (
            items: NavItem[],
            parentKey = ""
        ) => {



            items.forEach((item, index) => {



                const key = `${parentKey}-${index}`;



                if (hasActiveChild(item)) {


                    setOpenMenus(prev => {


                        if (prev.includes(key))
                            return prev;



                        return [
                            ...prev,
                            key
                        ];


                    });


                }



                if (item.subItems) {
                    openParents(
                        item.subItems,
                        key
                    );
                }



            });



        };




        openParents(navItems);



    }, [
        location.pathname,
        navItems,
        hasActiveChild
    ]);



    const toggleMenu = (key: string) => {



        setOpenMenus(prev =>


            prev.includes(key)

                ?

                prev.filter(
                    item => item !== key
                )


                :

                [
                    ...prev,
                    key
                ]


        );


    };

    const renderMenuItems = (
        items: NavItem[],
        parentKey = ""
    ) => {


        return (

            <ul className="flex flex-col gap-2">


                {
                    items.map((item, index) => {


                        const key = `${parentKey}-${index}`;


                        const opened =
                            openMenus.includes(key);



                        const active =
                            hasActiveChild(item);




                        return (

                            <li key={key}>


                                {
                                    item.subItems

                                        ?

                                        (

                                            <>


                                                <button

                                                    onClick={() =>
                                                        toggleMenu(key)
                                                    }


                                                    className={`
                                                        menu-item 
                                                        group 
                                                        cursor-pointer

                                                        ${active
                                                            ?
                                                            "menu-item-active"
                                                            :
                                                            "menu-item-inactive"
                                                        }

                                                        ${!isExpanded &&
                                                            !isHovered
                                                            ?
                                                            "lg:justify-center"
                                                            :
                                                            "lg:justify-start"
                                                        }

                                                    `}

                                                >


                                                    {
                                                        item.icon &&

                                                        <span

                                                            className={`
                                                                menu-item-icon-size

                                                                ${active
                                                                    ?
                                                                    "menu-item-icon-active"
                                                                    :
                                                                    "menu-item-icon-inactive"
                                                                }
                                                            `}

                                                        >

                                                            {item.icon}

                                                        </span>
                                                    }





                                                    {
                                                        (
                                                            isExpanded ||
                                                            isHovered ||
                                                            isMobileOpen
                                                        )

                                                        &&

                                                        <span className="menu-item-text">

                                                            {t(item.nameKey)}

                                                        </span>
                                                    }






                                                    {
                                                        (
                                                            isExpanded ||
                                                            isHovered ||
                                                            isMobileOpen
                                                        )

                                                        &&


                                                        <ChevronDownIcon

                                                            className={`
                                                                ml-auto
                                                                w-5
                                                                h-5
                                                                transition-transform
                                                                duration-200

                                                                ${opened
                                                                    ?
                                                                    "rotate-180 text-brand-500"
                                                                    :
                                                                    ""
                                                                }

                                                            `}

                                                        />

                                                    }



                                                </button>





                                                <div

                                                    className={`
                                                        overflow-hidden
                                                        transition-all
                                                        duration-300

                                                        ${opened
                                                            ?
                                                            "max-h-[2000px]"
                                                            :
                                                            "max-h-0"
                                                        }

                                                    `}

                                                >



                                                    <div className="ml-6 mt-2">


                                                        {
                                                            renderMenuItems(
                                                                item.subItems,
                                                                key
                                                            )
                                                        }


                                                    </div>


                                                </div>



                                            </>

                                        )



                                        :



                                        (

                                            <Link

                                                to={item.path!}


                                                className={`

                                                    menu-dropdown-item


                                                    ${isActive(item.path!)
                                                        ?
                                                        "menu-dropdown-item-active"
                                                        :
                                                        "menu-dropdown-item-inactive"
                                                    }

                                                `}


                                            >


                                                {
                                                    item.icon &&

                                                    <span className="mr-2">

                                                        {item.icon}

                                                    </span>
                                                }



                                                {t(item.nameKey)}



                                            </Link>

                                        )

                                }



                            </li>

                        );


                    })
                }


            </ul>

        );


    };









    return (
        <aside
            className={`
            fixed
            top-0
            left-0

            h-screen

            flex
            flex-col

            bg-white
            dark:bg-gray-900

            border-r
            border-gray-200
            dark:border-gray-800

            px-5

            z-50

            transition-all
            duration-300
            ease-in-out


            ${isExpanded ||
                    isHovered ||
                    isMobileOpen

                    ?

                    "w-[290px]"

                    :

                    "w-[90px]"
                }


            ${isMobileOpen

                    ?

                    "translate-x-0"

                    :

                    "-translate-x-full lg:translate-x-0"
                }

        `}

            onMouseEnter={() =>
                !isExpanded &&
                setIsHovered(true)
            }

            onMouseLeave={() =>
                setIsHovered(false)
            }

        >


            <div
                className={`
                flex-shrink-0
                py-8

                ${!isExpanded &&
                        !isHovered &&
                        !isMobileOpen

                        ?

                        "lg:justify-center"

                        :

                        "justify-start"
                    }

                flex
            `}
            >

                <Link to="/" className="flex items-center">
                    {isExpanded || isHovered || isMobileOpen ? (
                        <BrandLogo className="h-9 w-auto max-w-[200px]" />
                    ) : (
                        <BrandLogo variant="icon" className="h-8 w-8" />
                    )}
                </Link>


            </div>

            <div
                className="
                flex-1

                overflow-y-auto

                pb-10

                no-scrollbar
            "
            >


                <nav>


                    <h2

                        className={`

                        mb-4

                        text-xs
                        uppercase

                        text-gray-400


                        ${!isExpanded &&
                                !isHovered &&
                                !isMobileOpen

                                ?

                                "lg:text-center"

                                :

                                ""
                            }

                    `}

                    >


                        {
                            isExpanded ||
                                isHovered ||
                                isMobileOpen

                                ?

                                t("sidebar.menu")

                                :

                                <HorizontaLDots
                                    className="size-6"
                                />

                        }


                    </h2>



                    {
                        renderMenuItems(navItems)
                    }



                </nav>


            </div>


        </aside>
    );


};


export default AppSidebar;