import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import PageMeta from "../../../components/common/PageMeta";
import PageBreadcrumb from "../../../components/common/PageBreadCrumb";
import ComponentCard from "../../../components/common/ComponentCard";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../../components/ui/table";

import { RefreshIcon } from "../../../icons";

import Pagination from "../../../components/ui/pagination/Pagination";
import { Dropdown } from "../../../components/ui/dropdown/Dropdown";

import ConfirmDialog from "../../../components/ui/dialog/ConfirmDialog";
import EditHealthInstitutionTypeModal from "./EditHealthInstitutionTypeModal";



import {
    useAppDispatch,
    useAppSelector
} from "../../../store/store";


import {
    fetchHealthInstitutionTypes,
    deleteExistingHealthInstitutionType,
    createNewHealthInstitutionType,
    updateExistingHealthInstitutionType
} from "../../../features/location/healthInstitutionsType/healthInstitutionType.slice";

import {
    showAlert
} from "../../../features/ui/uiSlice";
import { HealthInstitutionTypeRequest } from "../../../features/location/healthInstitutionsType/healthInstitutionType.type";
import HealthInstitutionTypeTableSkeleton from "./HealthInstitutionTypeTableSkeleton";

export interface HealthInstitutionTypeItem {
    code: string;
    name: string;
}
export default function HealthInstitutionTypesTable() {


    const { t } = useTranslation();

    const dispatch = useAppDispatch();



    const items = useAppSelector(
        s => s.healthInstitutionTypes.items
    );


    const total = useAppSelector(
        s => s.healthInstitutionTypes.totalCount
    );


    const loading = useAppSelector(
        s => s.healthInstitutionTypes.loading
    );



    const [page, setPage] = useState(0);

    const [pageSize, setPageSize] = useState(5);



    const [fetching, setFetching] = useState(false);

    const [spinning, setSpinning] = useState(false);



    const [openDropdownId, setOpenDropdownId] =
        useState<string | null>(null);



    const [confirmOpen, setConfirmOpen] =
        useState(false);



    const [editOpen, setEditOpen] =
        useState(false);



    const [selectedItem, setSelectedItem] =
        useState<HealthInstitutionTypeItem | null>(null);

    const handleSave = async (data: HealthInstitutionTypeRequest) => {

        try {

            if (selectedItem) {

                await dispatch(
                    updateExistingHealthInstitutionType({
                        code: selectedItem.code,
                        request: data
                    })
                ).unwrap();

            }
            else {

                await dispatch(
                    createNewHealthInstitutionType(data)
                ).unwrap();

            }


            dispatch(
                showAlert({
                    type: "success",
                    message: t("location.healthInstitutionTypes.messages.saved")
                })
            );


            setEditOpen(false);
            setSelectedItem(null);

            await load();


        }
        catch (e) {

            dispatch(
                showAlert({
                    type: "error",
                    message: t("location.healthInstitutionTypes.messages.saveFailed")
                })
            );

            throw e;
        }
    };


    const [deleting, setDeleting] =
        useState(false);



    const totalPages =
        Math.ceil(total / pageSize);



    const delay = (ms: number) =>
        new Promise(
            resolve => setTimeout(resolve, ms)
        );



    const load = async () => {


        setSpinning(true);

        setFetching(true);


        try {


            await Promise.all([

                dispatch(
                    fetchHealthInstitutionTypes({

                        pageIndex: page,

                        pageSize

                    })
                ).unwrap(),


                delay(700)

            ]);


        }
        finally {


            setSpinning(false);

            setFetching(false);


        }


    };




    useEffect(() => {


        load();


    }, [page, pageSize]);





    const toggleDropdown = (code: string) => {


        setOpenDropdownId(
            prev =>
                prev === code
                    ? null
                    : code
        );


    };



    const closeDropdown = () => {

        setOpenDropdownId(null);

    };






    const handleDeleteClick = (code: string) => {


        setSelectedItem(items.find(item => item.code === code) || null);

        setConfirmOpen(true);

        closeDropdown();


    };






    const handleConfirmDelete = async () => {

        if (!selectedItem)
            return;


        try {

            setDeleting(true);


            await dispatch(
                deleteExistingHealthInstitutionType(
                    selectedItem.code
                )
            ).unwrap();



            const response = await dispatch(
                fetchHealthInstitutionTypes({
                    pageIndex: page,
                    pageSize
                })
            ).unwrap();



            const totalPagesAfterDelete =
                Math.ceil(response.count / pageSize);



            if (
                page >= totalPagesAfterDelete &&
                page > 0
            ) {

                setPage(page - 1);

            }
            else {

                await load();

            }



            dispatch(
                showAlert({
                    type: "success",
                    message: t(
                        "location.healthInstitutionTypes.messages.deleteSuccess"
                    )
                })
            );


            setConfirmOpen(false);
            setSelectedItem(null);


        }
        catch (error: any) {

            dispatch(
                showAlert({
                    type: "error",
                    message:
                        error?.message ??
                        t(
                            "location.healthInstitutionTypes.messages.deleteError"
                        )
                })
            );

        }
        finally {

            setDeleting(false);

        }

    };





    return (

        <>


            <PageMeta

                title={
                    t(
                        "location.healthInstitutionTypes.pageTitle"
                    )
                }

                description={
                    t(
                        "location.healthInstitutionTypes.pageDescription"
                    )
                }

            />



            <PageBreadcrumb

                pageTitle={
                    t(
                        "location.healthInstitutionTypes.pageTitle"
                    )
                }

            />



            <div className="space-y-6">
                <ComponentCard title={t("location.healthInstitutionTypes.title")}>
                    <div className="flex justify-end mb-3 gap-2">

                        <button
                            onClick={load}
                            disabled={loading || spinning}
                            className="w-9 h-9 flex items-center justify-center rounded-lg text-gray-500 hover:text-black"
                        >
                            <RefreshIcon
                                className={`w-5 h-5 bg-color-gray ${spinning ? "animate-spin" : "rotate-90"
                                    }`}
                            />
                        </button>
                    </div>

                    <div className="rounded-xl border border-gray-200 dark:border-white/[0.05] flex flex-col h-[520px]">
                        <div className="flex-1 overflow-y-auto">
                            <Table>
                                <TableHeader
                                    className="
                                    sticky
                                    top-0
                                    bg-white
                                    dark:bg-gray-900
                                    "
                                >


                                    <TableRow>


                                        <TableCell isHeader>

                                            {t("location.healthInstitutionTypes.table.code")}

                                        </TableCell>



                                        <TableCell isHeader>

                                            {t("location.healthInstitutionTypes.table.name")}

                                        </TableCell>



                                        <TableCell isHeader>

                                            {t("location.healthInstitutionTypes.table.actions")}

                                        </TableCell>


                                    </TableRow>


                                </TableHeader>


                                <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                    {fetching ? (
                                        <HealthInstitutionTypeTableSkeleton rows={5} />
                                    ) : (
                                        items.map((item) => (
                                            <TableRow
                                                key={item.code}
                                                className="hover:bg-gray-50 dark:hover:bg-white/[0.03] transition"
                                            >
                                                <TableCell className="px-5 py-4 text-sm font-semibold">
                                                    {item.code}
                                                </TableCell>

                                                <TableCell className="px-5 py-4 text-sm">
                                                    {item.name}
                                                </TableCell>

                                            
                                             

                                                <TableCell className="px-5 py-4 relative">
                                                    <button
                                                        className="w-8 h-8 flex items-center justify-center"
                                                        onClick={() => toggleDropdown(item.code)}
                                                    >
                                                        ⋮
                                                    </button>

                                                    <Dropdown
                                                        isOpen={openDropdownId === item.code}
                                                        onClose={closeDropdown}
                                                        className="w-44"
                                                    >
                                                        <div className="py-2 flex flex-col">
                                                            <button
                                                                onClick={() => {
                                                                    setSelectedItem(item);
                                                                    setEditOpen(true);
                                                                    closeDropdown();
                                                                }}
                                                                className="px-4 py-2 text-left hover:bg-gray-100"
                                                            >
                                                                {t("location.healthInstitutionTypes.actions.edit")}
                                                            </button>

                                                            <button
                                                                onClick={() => handleDeleteClick(item.code)}
                                                                className="px-4 py-2 text-left text-red-500 hover:bg-gray-100"
                                                            >
                                                                {t("location.healthInstitutionTypes.actions.delete")}
                                                            </button>
                                                        </div>
                                                    </Dropdown>
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                        </div>

                        <div className="border-t border-gray-100 dark:border-white/[0.05] shrink-0">
                            <Pagination
                                currentPage={page + 1}
                                totalPages={totalPages}
                                pageSize={pageSize}
                                onPageChange={(p) => setPage(p - 1)}
                                onPageSizeChange={(size) => {
                                    setPageSize(size);
                                    setPage(0);
                                }}
                            />
                        </div>
                    </div>
                </ComponentCard>
            </div>



            <ConfirmDialog

                isOpen={confirmOpen}

                title={
                    t(
                        "location.healthInstitutionTypes.messages.deleteTitle"
                    )
                }

                description={
                    t(
                        "location.healthInstitutionTypes.messages.deleteDescription"
                    )
                }

                onConfirm={
                    handleConfirmDelete
                }

                onCancel={() =>
                    setConfirmOpen(false)
                }

                loading={
                    deleting
                }

            />





            <EditHealthInstitutionTypeModal

                isOpen={editOpen}

                item={selectedItem}

                loading={loading}

                onClose={() => {
                    setEditOpen(false);
                }}

                onSave={handleSave}

            />



        </>

    );

}