import { configureStore } from "@reduxjs/toolkit";
import { useDispatch, TypedUseSelectorHook, useSelector } from "react-redux";
import authReducer from "../features/auth/authSlice";
import uiReducer from "../features/ui/uiSlice";
import languageReducer from "../features/language/languageSlice";
import roleReducer from "../features/role/roleSlice";
import doctorReducer from "../features/doctor/doctorSlice";
import patientReducer from "../features/patient/patientSlice";
import pdfReducer from "../features/pdf/pdfSlice";
import certificateReducer from "../features/certificate/certificateSlice";
import countryReducer from "../features/location/country/country.slice";
import settlementReducer from "../features/location/settlement/settlement.slice";
import capitalReducer from "../features/location/capital/capital.slice";
import municipalityReducer from "../features/location/municipality/municipality.slice";
import regionReducer from "../features/location/region/region.slice";
import governmentTypeReducer from "../features/location/governmentTypeSlice";
import regionTypeReducer from "../features/location/regionType/regionType.slice";
import healthInstitutionTypeReducer from "../features/location/healthInstitutionsType/healthInstitutionType.slice";
import localCommunityReducer from "../features/location/localCommunity/localCommunity.slice";
import healthInstitutionReducer from "../features/location/healthInstitutions/healthInstitution.slice";
import healthReducer from "../features/health/healthSlice";
import countryCompositionReducer from "../features/location/countryComposition/countryComposition.slice";
import regionCompositionReducer from "../features/location/regionComposition/regionComposition.slice";
import regionSettlementCoverageReducer from "../features/location/regionSettlementCoverage/regionSettlementCoverage.slice";
import municipalitySettlementCoverageReducer from "../features/location/municipalitySettlementCoverage/municipalitySettlementCoverage.slice";
import localCommunityCoverageReducer from "../features/location/localCommunityCoverage/localCommunityCoverage.slice";
import legalSuccessorReducer from "../features/location/legalSuccessor/legalSuccessor.slice";
import governmentHistoryReducer from "../features/location/governmentHistory/governmentHistory.slice";
import tumorDetectionReducer from "../features/tumorDetection/tumorDetection.slice";
import usersReducer from "../features/user/userSlice";
import notificationReducer from "../features/notification/notificationSlice";
import appointmentReducer from "../features/appointment/appointmentSlice";

export const store = configureStore({
    reducer: {
        auth: authReducer,
        ui: uiReducer,
        language: languageReducer,
        roles: roleReducer,
        doctor: doctorReducer,
        patient: patientReducer,
        pdfTemplate: pdfReducer,
        certificate: certificateReducer,
        countries: countryReducer,
        settlements: settlementReducer,
        capitals: capitalReducer,
        municipalities: municipalityReducer,
        regions: regionReducer,
        governmentTypes: governmentTypeReducer,
        regionTypes: regionTypeReducer,
        healthInstitutionTypes: healthInstitutionTypeReducer,
        localCommunities: localCommunityReducer,
        healthInstitutions: healthInstitutionReducer,
        health: healthReducer,
        countryCompositions: countryCompositionReducer,
        regionCompositions: regionCompositionReducer,
        regionSettlementCoverages: regionSettlementCoverageReducer,
        municipalitySettlementCoverages: municipalitySettlementCoverageReducer,
        localCommunityCoverages: localCommunityCoverageReducer,
        legalSuccessors: legalSuccessorReducer,
        governmentHistories: governmentHistoryReducer,
        tumorDetection: tumorDetectionReducer,
        users: usersReducer,
        notification: notificationReducer,
        appointment: appointmentReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
