export type EntityService = "doctor" | "patient" | "tumor";

export type EntityFieldKind =
    | "text"
    | "guid"
    | "int"
    | "decimal"
    | "bool"
    | "datetime"
    | "time";

export interface EntityTableField {
    key: string;
    kind: EntityFieldKind;
    required?: boolean;
}

export interface EntityTableDef {
    key: string;
    nameKey: string;
    apiPath: string;
    fields: EntityTableField[];
}

const catalogFields: EntityTableField[] = [
    { key: "code", kind: "text", required: true },
    { key: "name", kind: "text", required: true },
    { key: "description", kind: "text" },
];

export const doctorTables: EntityTableDef[] = [
    { key: "statuses", apiPath: "/doctorstatus", nameKey: "sidebar.doctorStatuses", fields: catalogFields },
    { key: "specializations", apiPath: "/specialization", nameKey: "sidebar.doctorSpecializations", fields: catalogFields },
    { key: "languages", apiPath: "/doctorlanguage", nameKey: "sidebar.doctorLanguages", fields: catalogFields },
    { key: "degree-types", apiPath: "/degreetype", nameKey: "sidebar.doctorDegreeTypes", fields: catalogFields },
    { key: "license-authorities", apiPath: "/licenseauthority", nameKey: "sidebar.doctorLicenseAuthorities", fields: catalogFields },
    {
        key: "status-histories",
        apiPath: "/doctorstatushistory",
        nameKey: "sidebar.doctorStatusHistories",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "statusCode", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "license-histories",
        apiPath: "/doctorlicensehistory",
        nameKey: "sidebar.doctorLicenseHistories",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "licenseNumber", kind: "text", required: true },
            { key: "licenseAuthorityCode", kind: "text" },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "affiliation-histories",
        apiPath: "/doctoraffiliationhistory",
        nameKey: "sidebar.doctorAffiliationHistories",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "healthInstitutionId", kind: "int" },
            { key: "institutionName", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "language-coverages",
        apiPath: "/doctorlanguagecoverage",
        nameKey: "sidebar.doctorLanguageCoverages",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "languageCode", kind: "text", required: true },
        ],
    },
    {
        key: "degree-coverages",
        apiPath: "/doctordegreecoverage",
        nameKey: "sidebar.doctorDegreeCoverages",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "degreeTypeCode", kind: "text", required: true },
            { key: "institutionName", kind: "text" },
            { key: "year", kind: "int" },
        ],
    },
    {
        key: "specialization-coverages",
        apiPath: "/doctorspecializationcoverage",
        nameKey: "sidebar.doctorSpecializationCoverages",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "specializationCode", kind: "text", required: true },
            { key: "isPrimary", kind: "bool", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "working-slots",
        apiPath: "/workingslot",
        nameKey: "sidebar.doctorWorkingSlots",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "dayOfWeek", kind: "int", required: true },
            { key: "start", kind: "time", required: true },
            { key: "end", kind: "time", required: true },
            { key: "validFrom", kind: "datetime", required: true },
            { key: "validTo", kind: "datetime" },
        ],
    },
    {
        key: "reviews",
        apiPath: "/doctorreview",
        nameKey: "sidebar.doctorReviews",
        fields: [
            { key: "doctorId", kind: "guid", required: true },
            { key: "rating", kind: "decimal", required: true },
            { key: "comment", kind: "text" },
            { key: "reviewerUserId", kind: "guid" },
        ],
    },
];

export const patientTables: EntityTableDef[] = [
    { key: "statuses", apiPath: "/status", nameKey: "sidebar.patientStatuses", fields: catalogFields },
    { key: "genders", apiPath: "/gender", nameKey: "sidebar.patientGenders", fields: catalogFields },
    { key: "blood-types", apiPath: "/bloodtype", nameKey: "sidebar.patientBloodTypes", fields: catalogFields },
    { key: "languages", apiPath: "/language", nameKey: "sidebar.patientLanguages", fields: catalogFields },
    { key: "allergies", apiPath: "/allergy", nameKey: "sidebar.patientAllergies", fields: catalogFields },
    { key: "conditions", apiPath: "/condition", nameKey: "sidebar.patientConditions", fields: catalogFields },
    { key: "insurance-payers", apiPath: "/insurancepayer", nameKey: "sidebar.patientInsurancePayers", fields: catalogFields },
    { key: "relationship-types", apiPath: "/relationshiptype", nameKey: "sidebar.patientRelationshipTypes", fields: catalogFields },
    { key: "consent-types", apiPath: "/consenttype", nameKey: "sidebar.patientConsentTypes", fields: catalogFields },
    {
        key: "status-histories",
        apiPath: "/patientstatushistory",
        nameKey: "sidebar.patientStatusHistories",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "statusCode", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "affiliation-histories",
        apiPath: "/patientaffiliationhistory",
        nameKey: "sidebar.patientAffiliationHistories",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "healthInstitutionId", kind: "int" },
            { key: "institutionName", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "insurance-histories",
        apiPath: "/patientinsurancehistory",
        nameKey: "sidebar.patientInsuranceHistories",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "payerCode", kind: "text", required: true },
            { key: "policyNumber", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "doctor-assignments",
        apiPath: "/patientdoctorassignmenthistory",
        nameKey: "sidebar.patientDoctorAssignments",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "doctorId", kind: "guid", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "language-coverages",
        apiPath: "/patientlanguagecoverage",
        nameKey: "sidebar.patientLanguageCoverages",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "languageCode", kind: "text", required: true },
        ],
    },
    {
        key: "allergy-coverages",
        apiPath: "/patientallergycoverage",
        nameKey: "sidebar.patientAllergyCoverages",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "allergyCode", kind: "text", required: true },
            { key: "note", kind: "text" },
        ],
    },
    {
        key: "condition-coverages",
        apiPath: "/patientconditioncoverage",
        nameKey: "sidebar.patientConditionCoverages",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "conditionCode", kind: "text", required: true },
            { key: "diagnosedYear", kind: "int" },
            { key: "note", kind: "text" },
        ],
    },
    {
        key: "consent-coverages",
        apiPath: "/patientconsentcoverage",
        nameKey: "sidebar.patientConsentCoverages",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "consentTypeCode", kind: "text", required: true },
            { key: "from", kind: "datetime", required: true },
            { key: "to", kind: "datetime" },
        ],
    },
    {
        key: "emergency-contacts",
        apiPath: "/patientemergencycontact",
        nameKey: "sidebar.patientEmergencyContacts",
        fields: [
            { key: "patientId", kind: "guid", required: true },
            { key: "fullName", kind: "text", required: true },
            { key: "phone", kind: "text", required: true },
            { key: "relationshipCode", kind: "text", required: true },
        ],
    },
];

export const tumorTables: EntityTableDef[] = [
    { key: "model-types", apiPath: "/tumor/model-types", nameKey: "sidebar.modelTypes", fields: catalogFields },
    { key: "tumor-grades", apiPath: "/tumor/tumor-grades", nameKey: "sidebar.tumorGrades", fields: catalogFields },
    { key: "treatment-options", apiPath: "/tumor/treatment-options", nameKey: "sidebar.treatmentOptions", fields: catalogFields },
    { key: "operability-statuses", apiPath: "/tumor/operability-statuses", nameKey: "sidebar.operabilityStatuses", fields: catalogFields },
    { key: "spread-statuses", apiPath: "/tumor/spread-statuses", nameKey: "sidebar.spreadStatuses", fields: catalogFields },
];

export const getTablesForService = (service: EntityService): EntityTableDef[] => {
    switch (service) {
        case "doctor":
            return doctorTables;
        case "patient":
            return patientTables;
        case "tumor":
            return tumorTables;
    }
};

export const getEntityTableBasePath = (service: EntityService) => {
    switch (service) {
        case "doctor":
            return "/admin/doctors/tables";
        case "patient":
            return "/admin/patients/tables";
        case "tumor":
            return "/admin/tumor/tables";
    }
};
