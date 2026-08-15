import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const localesDir = path.resolve(__dirname, "../public/locales");

const ENTITY_I18N = {
  regions: {
    sr: { title: "Regije", pageTitle: "Regije", pageDescription: "Upravljanje regijama", createTitle: "Kreiraj regiju", editTitle: "Izmijeni regiju" },
    en: { title: "Regions", pageTitle: "Regions", pageDescription: "Manage regions", createTitle: "Create Region", editTitle: "Edit Region" },
    de: { title: "Regionen", pageTitle: "Regionen", pageDescription: "Regionen verwalten", createTitle: "Region erstellen", editTitle: "Region bearbeiten" },
    columns: ["typeCode", "code", "name", "belongsToCountryCode", "headquartersCountryCode", "administrativeSeatSettlementCode", "actions"],
    fieldLabels: {
      sr: { typeCode: "Tip regije", code: "Kod", name: "Naziv", belongsToCountryCode: "Država", headquartersCountryCode: "Sjedište države", administrativeSeatSettlementCode: "Sjedište naselja" },
      en: { typeCode: "Region Type", code: "Code", name: "Name", belongsToCountryCode: "Country", headquartersCountryCode: "HQ Country", administrativeSeatSettlementCode: "Admin Seat Settlement" },
      de: { typeCode: "Regionstyp", code: "Code", name: "Name", belongsToCountryCode: "Land", headquartersCountryCode: "Hauptsitz Land", administrativeSeatSettlementCode: "Verwaltungssitz" },
    },
  },
  municipalities: {
    sr: { title: "Opštine", pageTitle: "Opštine", pageDescription: "Upravljanje opštinama", createTitle: "Kreiraj opštinu", editTitle: "Izmijeni opštinu" },
    en: { title: "Municipalities", pageTitle: "Municipalities", pageDescription: "Manage municipalities", createTitle: "Create Municipality", editTitle: "Edit Municipality" },
    de: { title: "Gemeinden", pageTitle: "Gemeinden", pageDescription: "Gemeinden verwalten", createTitle: "Gemeinde erstellen", editTitle: "Gemeinde bearbeiten" },
    columns: ["countryCode", "code", "name", "seatSettlementCode", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", code: "Kod", name: "Naziv", seatSettlementCode: "Sjedište naselja" },
      en: { countryCode: "Country", code: "Code", name: "Name", seatSettlementCode: "Seat Settlement" },
      de: { countryCode: "Land", code: "Code", name: "Name", seatSettlementCode: "Sitz Siedlung" },
    },
  },
  capitals: {
    sr: { title: "Glavni gradovi", pageTitle: "Glavni gradovi", pageDescription: "Upravljanje glavnim gradovima", createTitle: "Kreiraj glavni grad", editTitle: "Izmijeni glavni grad" },
    en: { title: "Capitals", pageTitle: "Capitals", pageDescription: "Manage capitals", createTitle: "Create Capital", editTitle: "Edit Capital" },
    de: { title: "Hauptstädte", pageTitle: "Hauptstädte", pageDescription: "Hauptstädte verwalten", createTitle: "Hauptstadt erstellen", editTitle: "Hauptstadt bearbeiten" },
    columns: ["countryCode", "settlementCode", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", settlementCode: "Kod naselja" },
      en: { countryCode: "Country", settlementCode: "Settlement Code" },
      de: { countryCode: "Land", settlementCode: "Siedlungscode" },
    },
  },
  regionTypes: {
    sr: { title: "Tipovi regija", pageTitle: "Tipovi regija", pageDescription: "Upravljanje tipovima regija", createTitle: "Kreiraj tip regije", editTitle: "Izmijeni tip regije" },
    en: { title: "Region Types", pageTitle: "Region Types", pageDescription: "Manage region types", createTitle: "Create Region Type", editTitle: "Edit Region Type" },
    de: { title: "Regionstypen", pageTitle: "Regionstypen", pageDescription: "Regionstypen verwalten", createTitle: "Regionstyp erstellen", editTitle: "Regionstyp bearbeiten" },
    columns: ["code", "name", "actions"],
    fieldLabels: {
      sr: { code: "Kod", name: "Naziv" },
      en: { code: "Code", name: "Name" },
      de: { code: "Code", name: "Name" },
    },
  },
  localCommunities: {
    sr: { title: "Lokalne zajednice", pageTitle: "Lokalne zajednice", pageDescription: "Upravljanje lokalnim zajednicama", createTitle: "Kreiraj lokalnu zajednicu", editTitle: "Izmijeni lokalnu zajednicu" },
    en: { title: "Local Communities", pageTitle: "Local Communities", pageDescription: "Manage local communities", createTitle: "Create Local Community", editTitle: "Edit Local Community" },
    de: { title: "Lokale Gemeinschaften", pageTitle: "Lokale Gemeinschaften", pageDescription: "Lokale Gemeinschaften verwalten", createTitle: "Lokale Gemeinschaft erstellen", editTitle: "Lokale Gemeinschaft bearbeiten" },
    columns: ["countryCode", "municipalityCode", "identifier", "name", "officeSettlementCode", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", municipalityCode: "Opština", identifier: "Identifikator", name: "Naziv", officeSettlementCode: "Sjedište" },
      en: { countryCode: "Country", municipalityCode: "Municipality", identifier: "Identifier", name: "Name", officeSettlementCode: "Office Settlement" },
      de: { countryCode: "Land", municipalityCode: "Gemeinde", identifier: "Kennung", name: "Name", officeSettlementCode: "Büro Siedlung" },
    },
  },
  countryCompositions: {
    sr: { title: "Sastav država", pageTitle: "Sastav država", pageDescription: "Upravljanje sastavom država", createTitle: "Kreiraj sastav", editTitle: "Izmijeni sastav" },
    en: { title: "Country Compositions", pageTitle: "Country Compositions", pageDescription: "Manage country compositions", createTitle: "Create Composition", editTitle: "Edit Composition" },
    de: { title: "Länderzusammensetzungen", pageTitle: "Länderzusammensetzungen", pageDescription: "Länderzusammensetzungen verwalten", createTitle: "Zusammensetzung erstellen", editTitle: "Zusammensetzung bearbeiten" },
    columns: ["unionCountryCode", "memberCountryCode", "sequenceNumber", "from", "to", "actions"],
    fieldLabels: {
      sr: { unionCountryCode: "Unija", memberCountryCode: "Članica", sequenceNumber: "Redni broj", from: "Od", to: "Do" },
      en: { unionCountryCode: "Union", memberCountryCode: "Member", sequenceNumber: "Sequence", from: "From", to: "To" },
      de: { unionCountryCode: "Union", memberCountryCode: "Mitglied", sequenceNumber: "Sequenz", from: "Von", to: "Bis" },
    },
  },
  regionCompositions: {
    sr: { title: "Sastav regija", pageTitle: "Sastav regija", pageDescription: "Upravljanje sastavom regija", createTitle: "Kreiraj sastav regije", editTitle: "Izmijeni sastav regije" },
    en: { title: "Region Compositions", pageTitle: "Region Compositions", pageDescription: "Manage region compositions", createTitle: "Create Region Composition", editTitle: "Edit Region Composition" },
    de: { title: "Regionszusammensetzungen", pageTitle: "Regionszusammensetzungen", pageDescription: "Regionszusammensetzungen verwalten", createTitle: "Regionszusammensetzung erstellen", editTitle: "Regionszusammensetzung bearbeiten" },
    columns: ["parentRegionTypeCode", "parentRegionCode", "memberRegionTypeCode", "memberRegionCode", "actions"],
    fieldLabels: {
      sr: { parentRegionTypeCode: "Roditeljski tip", parentRegionCode: "Roditeljski kod", memberRegionTypeCode: "Članski tip", memberRegionCode: "Članski kod" },
      en: { parentRegionTypeCode: "Parent Type", parentRegionCode: "Parent Code", memberRegionTypeCode: "Member Type", memberRegionCode: "Member Code" },
      de: { parentRegionTypeCode: "Elterntyp", parentRegionCode: "Elterncode", memberRegionTypeCode: "Mitgliedstyp", memberRegionCode: "Mitgliedscode" },
    },
  },
  regionSettlementCoverages: {
    sr: { title: "Pokrivenost regija", pageTitle: "Pokrivenost regija", pageDescription: "Upravljanje pokrivenošću regija", createTitle: "Kreiraj pokrivenost", editTitle: "Izmijeni pokrivenost" },
    en: { title: "Region Settlement Coverages", pageTitle: "Region Settlement Coverages", pageDescription: "Manage region settlement coverages", createTitle: "Create Coverage", editTitle: "Edit Coverage" },
    de: { title: "Regionsabdeckungen", pageTitle: "Regionsabdeckungen", pageDescription: "Regionsabdeckungen verwalten", createTitle: "Abdeckung erstellen", editTitle: "Abdeckung bearbeiten" },
    columns: ["regionTypeCode", "regionCode", "countryCode", "settlementCode", "actions"],
    fieldLabels: {
      sr: { regionTypeCode: "Tip regije", regionCode: "Kod regije", countryCode: "Država", settlementCode: "Naselje" },
      en: { regionTypeCode: "Region Type", regionCode: "Region Code", countryCode: "Country", settlementCode: "Settlement" },
      de: { regionTypeCode: "Regionstyp", regionCode: "Regionscode", countryCode: "Land", settlementCode: "Siedlung" },
    },
  },
  municipalitySettlementCoverages: {
    sr: { title: "Pokrivenost opština", pageTitle: "Pokrivenost opština", pageDescription: "Upravljanje pokrivenošću opština", createTitle: "Kreiraj pokrivenost", editTitle: "Izmijeni pokrivenost" },
    en: { title: "Municipality Settlement Coverages", pageTitle: "Municipality Settlement Coverages", pageDescription: "Manage municipality settlement coverages", createTitle: "Create Coverage", editTitle: "Edit Coverage" },
    de: { title: "Gemeindeabdeckungen", pageTitle: "Gemeindeabdeckungen", pageDescription: "Gemeindeabdeckungen verwalten", createTitle: "Abdeckung erstellen", editTitle: "Abdeckung bearbeiten" },
    columns: ["countryCode", "municipalityCode", "settlementCode", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", municipalityCode: "Opština", settlementCode: "Naselje" },
      en: { countryCode: "Country", municipalityCode: "Municipality", settlementCode: "Settlement" },
      de: { countryCode: "Land", municipalityCode: "Gemeinde", settlementCode: "Siedlung" },
    },
  },
  localCommunityCoverages: {
    sr: { title: "Pokrivenost lokalnih zajednica", pageTitle: "Pokrivenost lokalnih zajednica", pageDescription: "Upravljanje pokrivenošću lokalnih zajednica", createTitle: "Kreiraj pokrivenost", editTitle: "Izmijeni pokrivenost" },
    en: { title: "Local Community Coverages", pageTitle: "Local Community Coverages", pageDescription: "Manage local community coverages", createTitle: "Create Coverage", editTitle: "Edit Coverage" },
    de: { title: "Lokalgemeinschaftsabdeckungen", pageTitle: "Lokalgemeinschaftsabdeckungen", pageDescription: "Lokalgemeinschaftsabdeckungen verwalten", createTitle: "Abdeckung erstellen", editTitle: "Abdeckung bearbeiten" },
    columns: ["countryCode", "municipalityCode", "localCommunityIdentifier", "settlementCode", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", municipalityCode: "Opština", localCommunityIdentifier: "Lokalna zajednica", settlementCode: "Naselje" },
      en: { countryCode: "Country", municipalityCode: "Municipality", localCommunityIdentifier: "Local Community", settlementCode: "Settlement" },
      de: { countryCode: "Land", municipalityCode: "Gemeinde", localCommunityIdentifier: "Lokalgemeinschaft", settlementCode: "Siedlung" },
    },
  },
  legalSuccessors: {
    sr: { title: "Pravni sljedbenici", pageTitle: "Pravni sljedbenici", pageDescription: "Upravljanje pravnim sljedbenicima", createTitle: "Kreiraj sljedbenika", editTitle: "Izmijeni sljedbenika" },
    en: { title: "Legal Successors", pageTitle: "Legal Successors", pageDescription: "Manage legal successors", createTitle: "Create Legal Successor", editTitle: "Edit Legal Successor" },
    de: { title: "Rechtsnachfolger", pageTitle: "Rechtsnachfolger", pageDescription: "Rechtsnachfolger verwalten", createTitle: "Rechtsnachfolger erstellen", editTitle: "Rechtsnachfolger bearbeiten" },
    columns: ["successorCountryCode", "predecessorCountryCode", "actions"],
    fieldLabels: {
      sr: { successorCountryCode: "Sljedbenik", predecessorCountryCode: "Prethodnik" },
      en: { successorCountryCode: "Successor", predecessorCountryCode: "Predecessor" },
      de: { successorCountryCode: "Nachfolger", predecessorCountryCode: "Vorgänger" },
    },
  },
  governmentHistories: {
    sr: { title: "Historija vlasti", pageTitle: "Historija vlasti", pageDescription: "Upravljanje historijom vlasti", createTitle: "Kreiraj zapis", editTitle: "Izmijeni zapis" },
    en: { title: "Government Histories", pageTitle: "Government Histories", pageDescription: "Manage government histories", createTitle: "Create History", editTitle: "Edit History" },
    de: { title: "Regierungshistorie", pageTitle: "Regierungshistorie", pageDescription: "Regierungshistorie verwalten", createTitle: "Historie erstellen", editTitle: "Historie bearbeiten" },
    columns: ["countryCode", "sequenceNumber", "governmentTypeCode", "from", "to", "actions"],
    fieldLabels: {
      sr: { countryCode: "Država", sequenceNumber: "Redni broj", governmentTypeCode: "Tip vlasti", from: "Od", to: "Do" },
      en: { countryCode: "Country", sequenceNumber: "Sequence", governmentTypeCode: "Government Type", from: "From", to: "To" },
      de: { countryCode: "Land", sequenceNumber: "Sequenz", governmentTypeCode: "Regierungstyp", from: "Von", to: "Bis" },
    },
  },
};

function buildEntityBlock(storeKey, locale) {
  const cfg = ENTITY_I18N[storeKey];
  const meta = cfg[locale];
  const labels = cfg.fieldLabels[locale];
  const actionLabel = locale === "sr" ? "Akcije" : locale === "de" ? "Aktionen" : "Actions";
  const editLabel = locale === "sr" ? "Izmijeni" : locale === "de" ? "Bearbeiten" : "Edit";
  const deleteLabel = locale === "sr" ? "Obriši" : locale === "de" ? "Löschen" : "Delete";

  const columns = {};
  for (const col of cfg.columns) {
    columns[col] = col === "actions" ? actionLabel : (labels[col] ?? col);
  }

  const fields = { ...labels };
  if (!fields.actions) fields.actions = actionLabel;

  const msg = (sr, en, de) => ({ sr, en, de }[locale]);

  return {
    pageTitle: meta.pageTitle,
    pageDescription: meta.pageDescription,
    title: meta.title,
    createTitle: meta.createTitle,
    editTitle: meta.editTitle,
    columns,
    fields,
    actions: { edit: editLabel, delete: deleteLabel },
    messages: {
      deleteTitle: msg(`Obriši ${meta.title.toLowerCase()}`, `Delete ${meta.title}`, `${meta.title} löschen`),
      deleteDescription: msg("Da li ste sigurni?", "Are you sure?", "Sind Sie sicher?"),
      deleteSuccess: msg("Uspješno obrisano.", "Deleted successfully.", "Erfolgreich gelöscht."),
      deleteError: msg("Brisanje nije uspjelo.", "Delete failed.", "Löschen fehlgeschlagen."),
      updateSuccess: msg("Uspješno ažurirano.", "Updated successfully.", "Erfolgreich aktualisiert."),
      updateError: msg("Ažuriranje nije uspjelo.", "Update failed.", "Aktualisierung fehlgeschlagen."),
      createSuccess: msg("Uspješno kreirano.", "Created successfully.", "Erfolgreich erstellt."),
      createError: msg("Kreiranje nije uspjelo.", "Create failed.", "Erstellen fehlgeschlagen."),
    },
  };
}

for (const locale of ["sr", "en", "de"]) {
  const filePath = path.join(localesDir, locale, "translation.json");
  const data = JSON.parse(fs.readFileSync(filePath, "utf8"));
  if (!data.location) data.location = {};

  for (const storeKey of Object.keys(ENTITY_I18N)) {
    data.location[storeKey] = buildEntityBlock(storeKey, locale);
  }

  if (!data.common.save) {
    data.common = data.common ?? {};
    data.common.save = locale === "sr" ? "Sačuvaj" : locale === "de" ? "Speichern" : "Save";
    data.common.saveChanges = locale === "sr" ? "Sačuvaj izmjene" : locale === "de" ? "Änderungen speichern" : "Save Changes";
    data.common.saving = locale === "sr" ? "Čuvanje..." : locale === "de" ? "Speichern..." : "Saving...";
  }

  fs.writeFileSync(filePath, JSON.stringify(data, null, 2) + "\n");
  console.log(`Patched ${locale}/translation.json`);
}
