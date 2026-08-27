 

import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import ComponentCard from "../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../components/ui/table";

import Badge from "../../components/ui/badge/Badge";
import Pagination from "../../components/ui/pagination/Pagination";

import { RefreshIcon } from "../../icons";

import { useAppDispatch, useAppSelector } from "../../store/store";
import { fetchSystemHealth } from "../../features/health/healthSlice";

import { ServiceHealth } from "../../features/health/healthService";


const AUTO_REFRESH_INTERVAL = 5000;


const statusBadgeColor: Record<
    string,
    "success" | "error" | "warning" | "light"
> = {
    Healthy: "success",
    Unhealthy: "error",
    Degraded: "warning",
};



export default function HealthPage() {


    const { t } = useTranslation();

    const dispatch = useAppDispatch();


    const {
        data,
        loading,
        error
    } = useAppSelector(
        state => state.health
    );



    const [autoRefresh, setAutoRefresh] = useState(true);


    const [page, setPage] = useState(0);

    const [pageSize, setPageSize] = useState(10);



    const intervalRef =
        useRef<ReturnType<typeof setInterval> | null>(null);



    const loadHealth = () => {

        dispatch(
            fetchSystemHealth({
                pageIndex: page,
                pageSize
            })
        );

    };



    const services =
        data?.services.data ?? [];



    const totalCount =
        data?.services.count ?? 0;



    const totalPages =
        Math.max(
            1,
            Math.ceil(
                totalCount / pageSize
            )
        );



    const getHealthyCount = (
        services: ServiceHealth[]
    ) =>
        services.filter(
            x => x.status === "Healthy"
        ).length;



    const getUnhealthyCount = (
        services: ServiceHealth[]
    ) =>
        services.filter(
            x => x.status !== "Healthy"
        ).length;





    useEffect(() => {


        loadHealth();



        if (autoRefresh) {

            intervalRef.current =
                setInterval(
                    () => {

                        loadHealth();

                    },
                    AUTO_REFRESH_INTERVAL
                );

        }



        return () => {


            if (intervalRef.current) {

                clearInterval(
                    intervalRef.current
                );

                intervalRef.current = null;

            }


        };


    }, [
        autoRefresh,
        page,
        pageSize
    ]);





    const handleRefresh = () => {

        loadHealth();

    };



    return (

        <>

            <PageMeta
                title={t("health.pageTitle")}
                description={t("health.pageDescription")}
            />


            <PageBreadcrumb
                pageTitle={t("health.pageTitle")}
            />



            <div className="space-y-6">


                <ComponentCard
                    title={t("health.title")}
                >


                    <div className="flex justify-between items-center mb-6">


                        <div className="flex items-center gap-3">


                            <span className="text-sm text-gray-500">

                                {t("health.systemStatus")}:

                            </span>



                            <Badge
                                size="sm"
                                color={
                                    statusBadgeColor[
                                    data?.status ?? "Unhealthy"
                                    ]
                                }
                            >

                                {
                                    data?.status ??
                                    "Unknown"
                                }

                            </Badge>


                        </div>





                        <div className="flex items-center gap-4">


                            <label
                                className="
                                flex
                                items-center
                                gap-2
                                text-sm
                                text-gray-500
                                cursor-pointer
                                "
                            >

                                <input

                                    type="checkbox"

                                    checked={autoRefresh}

                                    onChange={(e) =>
                                        setAutoRefresh(
                                            e.target.checked
                                        )
                                    }

                                    className="
                                    w-4
                                    h-4
                                    rounded
                                    border-gray-300
                                    "
                                />


                                {t("health.autoRefresh")}


                            </label>





                            <button

                                onClick={handleRefresh}

                                disabled={loading}

                                className="
                                w-9
                                h-9
                                flex
                                items-center
                                justify-center
                                rounded-lg
                                text-gray-500
                                hover:text-black
                                disabled:opacity-50
                                "

                            >

                                <RefreshIcon

                                    className={`
                                    w-5
                                    h-5
                                    ${loading
                                            ? "animate-spin"
                                            : ""
                                        }
                                    `}

                                />


                            </button>



                        </div>


                    </div>
                                        {
                        error && (

                            <div className="text-error-500 mb-5">

                                {error}

                            </div>

                        )
                    }






                    <div
                        className="
                        grid
                        grid-cols-1
                        sm:grid-cols-2
                        xl:grid-cols-4
                        gap-5
                        mb-6
                        "
                    >


                        <ComponentCard
                            title={t("health.cards.total")}
                        >

                            <h3
                                className="
                                text-3xl
                                font-bold
                                text-gray-800
                                dark:text-white
                                "
                            >

                                {totalCount}

                            </h3>


                        </ComponentCard>




                        <ComponentCard
                            title={t("health.cards.healthy")}
                        >

                            <h3
                                className="
                                text-3xl
                                font-bold
                                text-success-500
                                "
                            >

                                {
                                    data?.healthyCount ??
                                    getHealthyCount(services)
                                }

                            </h3>


                        </ComponentCard>




                        <ComponentCard
                            title={t("health.cards.unhealthy")}
                        >

                            <h3
                                className="
                                text-3xl
                                font-bold
                                text-error-500
                                "
                            >

                                {
                                    data?.unhealthyCount ??
                                    getUnhealthyCount(services)
                                }

                            </h3>


                        </ComponentCard>




                        <ComponentCard
                            title={t("health.cards.status")}
                        >

                            <Badge
                                size="sm"
                                color={
                                    statusBadgeColor[
                                        data?.status ?? "Unhealthy"
                                    ]
                                }
                            >

                                {
                                    data?.status ??
                                    "Unknown"
                                }


                            </Badge>


                        </ComponentCard>


                    </div>

                    {services.length === 0 ? (
                        <p className="mb-6 text-center text-sm text-gray-500 dark:text-gray-400">
                            {t("health.empty")}
                        </p>
                    ) : (
                    <div
                        className="
                        grid
                        grid-cols-1
                        md:grid-cols-2
                        xl:grid-cols-4
                        gap-5
                        mb-6
                        "
                    >


                        {
                            services.map(service => (


                                <ComponentCard

                                    key={service.name}

                                    title={service.name}

                                >


                                    <div
                                        className="
                                        flex
                                        items-center
                                        justify-between
                                        "
                                    >


                                        <div
                                            className="
                                            flex
                                            items-center
                                            gap-2
                                            "
                                        >

                                            <span

                                                className={`
                                                w-3
                                                h-3
                                                rounded-full

                                                ${
                                                    service.status === "Healthy"
                                                    ? "bg-success-500"
                                                    : service.status === "Degraded"
                                                    ? "bg-warning-500"
                                                    : "bg-error-500 animate-pulse"
                                                }
                                                `}

                                            />


                                            <span
                                                className="
                                                font-medium
                                                text-gray-700
                                                dark:text-gray-300
                                                "
                                            >

                                                {service.status}

                                            </span>


                                        </div>





                                        <Badge

                                            size="sm"

                                            color={
                                                statusBadgeColor[
                                                    service.status
                                                ]
                                            }

                                        >

                                            {service.status}


                                        </Badge>


                                    </div>






                                    <div
                                        className="
                                        mt-4
                                        text-sm
                                        text-gray-500
                                        "
                                    >

                                        {t("health.columns.duration")}:

                                        <span
                                            className="
                                            ml-2
                                            font-medium
                                            text-gray-700
                                            dark:text-gray-300
                                            "
                                        >

                                            {service.duration}


                                        </span>


                                    </div>






                                    {
                                        service.error && (

                                            <div
                                                className="
                                                mt-3
                                                text-sm
                                                text-error-500
                                                "
                                            >

                                                {service.error}

                                            </div>

                                        )
                                    }


                                </ComponentCard>


                            ))
                        }


                    </div>
                    )}

                    {totalCount > 0 && (
                        <Pagination
                            currentPage={page + 1}
                            totalPages={totalPages}
                            pageSize={pageSize}
                            onPageChange={(nextPage) => setPage(nextPage - 1)}
                            onPageSizeChange={(size) => {
                                setPageSize(size);
                                setPage(0);
                            }}
                        />
                    )}

                </ComponentCard>


            </div>


        </>

    );

}