using LocationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(this DbContext context)
    {

        if (await context.Set<Country>().AnyAsync())
            return;

        await context.SeedGovernmentTypesAsync();
        await context.SeedRegionTypesAsync();
        await context.SeedHealthInstitutionTypesAsync();

        await context.SeedCountriesAsync();
        await context.SeedSettlementsAsync();
        await context.SeedCapitalsAsync();

        await context.SeedMunicipalitiesAsync();
        await context.SeedMunicipalitySettlementCoveragesAsync();

        await context.SeedLocalCommunitiesAsync();
        await context.SeedLocalCommunityCoveragesAsync();

        await context.SeedRegionsAsync();
        await context.SeedRegionCompositionsAsync();
        await context.SeedRegionSettlementCoveragesAsync();

        await context.SeedCountryCompositionsAsync();
        await context.SeedLegalSuccessorsAsync();
        await context.SeedGovernmentHistoriesAsync();

        await context.SeedHealthInstitutionsAsync();
    }

    // ---------------------------------------------------------------
    // GovernmentType
    // ---------------------------------------------------------------
    public static async Task SeedGovernmentTypesAsync(this DbContext context)
    {
        var items = new List<GovernmentType>
        {
            new() { Code = "REP",  Name = "Republika",                    Description = "Opšti oblik republike" },
            new() { Code = "PARL", Name = "Parlamentarna republika",      Description = "Vlast proizilazi iz parlamenta" },
            new() { Code = "SEMI", Name = "Polupredsjednička republika",  Description = "Podijeljena izvršna vlast" },
            new() { Code = "PRES", Name = "Predsjednička republika",      Description = "Predsjednik je šef izvršne vlasti" },
            new() { Code = "FED",  Name = "Federalna republika",          Description = "Federalno uređena država" },
            new() { Code = "KONF", Name = "Konfederacija",                Description = "Labava zajednica država" },
            new() { Code = "MON",  Name = "Ustavna monarhija",            Description = "Monarhija ograničena ustavom" },
            new() { Code = "KRALJ",Name = "Kraljevina",                   Description = "Apsolutna ili ustavna kraljevina" },
            new() { Code = "SOC",  Name = "Socijalistička republika",     Description = "Bivši socijalistički sistem" },
            new() { Code = "TRANZ",Name = "Prelazna vlast",               Description = "Privremeni/tranzicioni oblik vlasti" },
        };

        await context.Set<GovernmentType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    // ---------------------------------------------------------------
    // RegionType
    // ---------------------------------------------------------------
    public static async Task SeedRegionTypesAsync(this DbContext context)
    {
        var items = new List<RegionType>
        {
            new() { Code = "ENTITET", Name = "Entitet",              Description = "Državni entitet unutar zemlje" },
            new() { Code = "KANTON",  Name = "Kanton",                Description = "Kantonalna administrativna jedinica" },
            new() { Code = "OBLAST",  Name = "Oblast",                Description = "Šira geopolitička oblast" },
            new() { Code = "GRUPA",   Name = "Grupacija država",      Description = "Grupa država sa zajedničkim ciljem" },
            new() { Code = "SAVEZ",   Name = "Savez",                 Description = "Vojno-politički savez" },
            new() { Code = "UNIJA",   Name = "Unija",                 Description = "Ekonomsko-politička unija" },
            new() { Code = "REGIJA",  Name = "Geografska regija",     Description = "Prirodno-geografska cjelina" },
            new() { Code = "CARINA",  Name = "Carinska zona",         Description = "Zona slobodne trgovine" },
            new() { Code = "KULTURA", Name = "Kulturna regija",       Description = "Regija povezana kulturom/istorijom" },
            new() { Code = "ADMIN",   Name = "Administrativna zona",  Description = "Opšta administrativna podjela" },
        };

        await context.Set<RegionType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedHealthInstitutionTypesAsync(this DbContext context)
    {
        var items = new List<HealthInstitutionType>
        {
            new() { Code = "BOLN",  Name = "Bolnica",                   Description = "Opšta bolnica" },
            new() { Code = "KLIN",  Name = "Klinika",                   Description = "Specijalizovana klinika" },
            new() { Code = "DZ",    Name = "Dom zdravlja",              Description = "Primarna zdravstvena zaštita" },
            new() { Code = "POLI",  Name = "Poliklinika",               Description = "Ambulantno liječenje" },
            new() { Code = "HITNA", Name = "Centar za hitnu pomoć",     Description = "Hitna medicinska pomoć" },
            new() { Code = "SPEC",  Name = "Specijalna bolnica",        Description = "Specijalizovano liječenje" },
            new() { Code = "STOM",  Name = "Stomatološka klinika",      Description = "Stomatološke usluge" },
            new() { Code = "REHAB", Name = "Rehabilitacioni centar",    Description = "Fizikalna rehabilitacija" },
            new() { Code = "DIJAG", Name = "Dijagnostički centar",      Description = "Dijagnostičke usluge" },
            new() { Code = "PORO",  Name = "Porodilište",               Description = "Ustanova za porođaje" },
        };

        await context.Set<HealthInstitutionType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedCountriesAsync(this DbContext context)
    {
        var items = new List<Country>
        {
            new() { Code = "BA", Name = "Bosna i Hercegovina",   FoundingDate = new DateTime(1995, 12, 14), CallingCode = 387, GovernmentTypeCode = "REP" },
            new() { Code = "RS", Name = "Srbija",                FoundingDate = new DateTime(2006, 6, 5),   CallingCode = 381, GovernmentTypeCode = "REP" },
            new() { Code = "HR", Name = "Hrvatska",               FoundingDate = new DateTime(1991, 6, 25),  CallingCode = 385, GovernmentTypeCode = "PARL" },
            new() { Code = "ME", Name = "Crna Gora",              FoundingDate = new DateTime(2006, 6, 3),   CallingCode = 382, GovernmentTypeCode = "REP" },
            new() { Code = "SI", Name = "Slovenija",              FoundingDate = new DateTime(1991, 6, 25),  CallingCode = 386, GovernmentTypeCode = "PARL" },
            new() { Code = "MK", Name = "Sjeverna Makedonija",    FoundingDate = new DateTime(1991, 9, 8),   CallingCode = 389, GovernmentTypeCode = "PARL" },
            new() { Code = "AL", Name = "Albanija",               FoundingDate = new DateTime(1912, 11, 28), CallingCode = 355, GovernmentTypeCode = "PARL" },
            new() { Code = "GR", Name = "Grčka",                  FoundingDate = new DateTime(1830, 2, 3),   CallingCode = 30,  GovernmentTypeCode = "PARL" },
            new() { Code = "HU", Name = "Mađarska",                FoundingDate = new DateTime(1000, 1, 1),   CallingCode = 36,  GovernmentTypeCode = "PARL" },
            new() { Code = "RO", Name = "Rumunija",                FoundingDate = new DateTime(1859, 1, 24),  CallingCode = 40,  GovernmentTypeCode = "SEMI" },
        };

        await context.Set<Country>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }


    public static async Task SeedSettlementsAsync(this DbContext context)
    {
        var items = new List<Settlement>
        {
            new() { CountryCode = "BA", Code = 1, Name = "Sarajevo",     PostalCode = "71000" },
            new() { CountryCode = "RS", Code = 1, Name = "Beograd",      PostalCode = "11000" },
            new() { CountryCode = "HR", Code = 1, Name = "Zagreb",       PostalCode = "10000" },
            new() { CountryCode = "ME", Code = 1, Name = "Podgorica",    PostalCode = "81000" },
            new() { CountryCode = "SI", Code = 1, Name = "Ljubljana",    PostalCode = "1000" },
            new() { CountryCode = "MK", Code = 1, Name = "Skoplje",      PostalCode = "1000" },
            new() { CountryCode = "AL", Code = 1, Name = "Tirana",       PostalCode = "1001" },
            new() { CountryCode = "GR", Code = 1, Name = "Atina",        PostalCode = "10431" },
            new() { CountryCode = "HU", Code = 1, Name = "Budimpešta",   PostalCode = "1011" },
            new() { CountryCode = "RO", Code = 1, Name = "Bukurešt",     PostalCode = "010011" },
        };

        await context.Set<Settlement>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }


    public static async Task SeedCapitalsAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };
        var founding = new[]
        {
            new DateTime(1995,12,14), new DateTime(2006,6,5), new DateTime(1991,6,25),
            new DateTime(2006,6,3),   new DateTime(1991,6,25), new DateTime(1991,9,8),
            new DateTime(1912,11,28), new DateTime(1830,2,3),  new DateTime(1000,1,1),
            new DateTime(1859,1,24),
        };

        var items = countries.Select((code, i) => new Capital
        {
            CountryCode = code,
            SettlementCode = 1,
            SequenceNumber = 1,
            From = founding[i],
            To = null,
        }).ToList();

        await context.Set<Capital>().AddRangeAsync(items);
        await context.SaveChangesAsync();


        foreach (var country in await context.Set<Country>().ToListAsync())
        {
            country.CapitalSettlementCode = 1;
        }
        await context.SaveChangesAsync();
    }

    public static async Task SeedMunicipalitiesAsync(this DbContext context)
    {
        var items = new List<Municipality>
        {
            new() { CountryCode = "BA", Code = 1, Name = "Opština Centar",          SeatSettlementCode = 1 },
            new() { CountryCode = "RS", Code = 1, Name = "Gradska opština Stari grad", SeatSettlementCode = 1 },
            new() { CountryCode = "HR", Code = 1, Name = "Grad Zagreb",             SeatSettlementCode = 1 },
            new() { CountryCode = "ME", Code = 1, Name = "Opština Podgorica",       SeatSettlementCode = 1 },
            new() { CountryCode = "SI", Code = 1, Name = "Mestna občina Ljubljana", SeatSettlementCode = 1 },
            new() { CountryCode = "MK", Code = 1, Name = "Opština Centar Skoplje",  SeatSettlementCode = 1 },
            new() { CountryCode = "AL", Code = 1, Name = "Bashkia Tiranë",          SeatSettlementCode = 1 },
            new() { CountryCode = "GR", Code = 1, Name = "Dimos Athinaion",         SeatSettlementCode = 1 },
            new() { CountryCode = "HU", Code = 1, Name = "Belváros-Lipótváros",     SeatSettlementCode = 1 },
            new() { CountryCode = "RO", Code = 1, Name = "Sectorul 1 București",    SeatSettlementCode = 1 },
        };

        await context.Set<Municipality>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMunicipalitySettlementCoveragesAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };

        var items = countries.Select(code => new MunicipalitySettlementCoverage
        {
            CountryCode = code,
            MunicipalityCode = 1,
            SettlementCode = 1,
        }).ToList();

        await context.Set<MunicipalitySettlementCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLocalCommunitiesAsync(this DbContext context)
    {
        var items = new List<LocalCommunity>
        {
            new() { CountryCode = "BA", MunicipalityCode = 1, Identifier = 1, Name = "MZ Centar",       OfficeSettlementCode = 1 },
            new() { CountryCode = "RS", MunicipalityCode = 1, Identifier = 1, Name = "MZ Stari Grad",    OfficeSettlementCode = 1 },
            new() { CountryCode = "HR", MunicipalityCode = 1, Identifier = 1, Name = "MZ Gornji Grad",   OfficeSettlementCode = 1 },
            new() { CountryCode = "ME", MunicipalityCode = 1, Identifier = 1, Name = "MZ Zabjelo",       OfficeSettlementCode = 1 },
            new() { CountryCode = "SI", MunicipalityCode = 1, Identifier = 1, Name = "MZ Center",        OfficeSettlementCode = 1 },
            new() { CountryCode = "MK", MunicipalityCode = 1, Identifier = 1, Name = "MZ Centar",        OfficeSettlementCode = 1 },
            new() { CountryCode = "AL", MunicipalityCode = 1, Identifier = 1, Name = "Njesia Njesi 1",   OfficeSettlementCode = 1 },
            new() { CountryCode = "GR", MunicipalityCode = 1, Identifier = 1, Name = "Geitonia Plaka",   OfficeSettlementCode = 1 },
            new() { CountryCode = "HU", MunicipalityCode = 1, Identifier = 1, Name = "Belváros Negyed",  OfficeSettlementCode = 1 },
            new() { CountryCode = "RO", MunicipalityCode = 1, Identifier = 1, Name = "Cartierul Centru", OfficeSettlementCode = 1 },
        };

        await context.Set<LocalCommunity>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLocalCommunityCoveragesAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };

        var items = countries.Select(code => new LocalCommunityCoverage
        {
            CountryCode = code,
            MunicipalityCode = 1,
            LocalCommunityIdentifier = 1,
            SettlementCode = 1,
        }).ToList();

        await context.Set<LocalCommunityCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
    public static async Task SeedRegionsAsync(this DbContext context)
    {
        var items = new List<Region>
        {
            new() { TypeCode = "ENTITET", Code = 1, Name = "Federacija Bosne i Hercegovine", BelongsToCountryCode = "BA", AdministrativeSeatSettlementCode = 1 },
            new() { TypeCode = "ENTITET", Code = 2, Name = "Republika Srpska",               BelongsToCountryCode = "BA", AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "OBLAST",  Code = 1, Name = "Zapadni Balkan",                 BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "OBLAST",  Code = 2, Name = "Panonska regija",                BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "REGIJA",  Code = 1, Name = "Jadranska regija",                BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "REGIJA",  Code = 2, Name = "Dinarske Alpe",                   BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "UNIJA",   Code = 1, Name = "Evropska unija",                  BelongsToCountryCode = null, HeadquartersCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "SAVEZ",   Code = 1, Name = "NATO",                             BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "CARINA",  Code = 1, Name = "CEFTA",                            BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
            new() { TypeCode = "KULTURA", Code = 1, Name = "Balkansko poluostrvo",             BelongsToCountryCode = null, AdministrativeSeatSettlementCode = null },
        };

        await context.Set<Region>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionCompositionsAsync(this DbContext context)
    {
        var items = new List<RegionComposition>
        {
            new() { ParentRegionTypeCode = "OBLAST",  ParentRegionCode = 1, MemberRegionTypeCode = "ENTITET", MemberRegionCode = 1 },
            new() { ParentRegionTypeCode = "OBLAST",  ParentRegionCode = 1, MemberRegionTypeCode = "ENTITET", MemberRegionCode = 2 },
            new() { ParentRegionTypeCode = "OBLAST",  ParentRegionCode = 1, MemberRegionTypeCode = "REGIJA",  MemberRegionCode = 1 },
            new() { ParentRegionTypeCode = "OBLAST",  ParentRegionCode = 1, MemberRegionTypeCode = "REGIJA",  MemberRegionCode = 2 },
            new() { ParentRegionTypeCode = "KULTURA", ParentRegionCode = 1, MemberRegionTypeCode = "OBLAST",  MemberRegionCode = 1 },
            new() { ParentRegionTypeCode = "KULTURA", ParentRegionCode = 1, MemberRegionTypeCode = "OBLAST",  MemberRegionCode = 2 },
            new() { ParentRegionTypeCode = "CARINA",  ParentRegionCode = 1, MemberRegionTypeCode = "OBLAST",  MemberRegionCode = 1 },
            new() { ParentRegionTypeCode = "SAVEZ",   ParentRegionCode = 1, MemberRegionTypeCode = "REGIJA",  MemberRegionCode = 1 },
            new() { ParentRegionTypeCode = "UNIJA",   ParentRegionCode = 1, MemberRegionTypeCode = "REGIJA",  MemberRegionCode = 2 },
            new() { ParentRegionTypeCode = "UNIJA",   ParentRegionCode = 1, MemberRegionTypeCode = "KULTURA", MemberRegionCode = 1 },
        };

        await context.Set<RegionComposition>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionSettlementCoveragesAsync(this DbContext context)
    {
        var items = new List<RegionSettlementCoverage>
        {
            new() { RegionTypeCode = "ENTITET", RegionCode = 1, CountryCode = "BA", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 1, CountryCode = "BA", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 1, CountryCode = "RS", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 1, CountryCode = "HR", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 1, CountryCode = "ME", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 2, CountryCode = "HU", SettlementCode = 1 },
            new() { RegionTypeCode = "OBLAST",  RegionCode = 2, CountryCode = "RO", SettlementCode = 1 },
            new() { RegionTypeCode = "REGIJA",  RegionCode = 1, CountryCode = "HR", SettlementCode = 1 },
            new() { RegionTypeCode = "REGIJA",  RegionCode = 1, CountryCode = "AL", SettlementCode = 1 },
            new() { RegionTypeCode = "KULTURA", RegionCode = 1, CountryCode = "GR", SettlementCode = 1 },
        };

        await context.Set<RegionSettlementCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedCountryCompositionsAsync(this DbContext context)
    {
        var items = new List<CountryComposition>
        {
            new() { UnionCountryCode = "RS", MemberCountryCode = "ME", SequenceNumber = 1, From = new DateTime(2003,2,4), To = new DateTime(2006,6,3) },
            new() { UnionCountryCode = "SI", MemberCountryCode = "HR", SequenceNumber = 1, From = new DateTime(1991,1,1), To = new DateTime(1991,6,25) },
            new() { UnionCountryCode = "HR", MemberCountryCode = "BA", SequenceNumber = 1, From = new DateTime(2000,1,1), To = null },
            new() { UnionCountryCode = "GR", MemberCountryCode = "AL", SequenceNumber = 1, From = new DateTime(2010,1,1), To = null },
            new() { UnionCountryCode = "HU", MemberCountryCode = "RO", SequenceNumber = 1, From = new DateTime(2007,1,1), To = null },
            new() { UnionCountryCode = "MK", MemberCountryCode = "AL", SequenceNumber = 1, From = new DateTime(2005,1,1), To = null },
            new() { UnionCountryCode = "BA", MemberCountryCode = "RS", SequenceNumber = 1, From = new DateTime(1918,1,1), To = new DateTime(1992,3,1) },
            new() { UnionCountryCode = "BA", MemberCountryCode = "HR", SequenceNumber = 2, From = new DateTime(1918,1,1), To = new DateTime(1992,3,1) },
            new() { UnionCountryCode = "BA", MemberCountryCode = "SI", SequenceNumber = 3, From = new DateTime(1918,1,1), To = new DateTime(1991,6,25) },
            new() { UnionCountryCode = "BA", MemberCountryCode = "MK", SequenceNumber = 4, From = new DateTime(1918,1,1), To = new DateTime(1991,9,8) },
        };

        await context.Set<CountryComposition>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }


    public static async Task SeedLegalSuccessorsAsync(this DbContext context)
    {
        var items = new List<LegalSuccessor>
        {
            new() { SuccessorCountryCode = "RS", PredecessorCountryCode = "ME" },
            new() { SuccessorCountryCode = "BA", PredecessorCountryCode = "RS" },
            new() { SuccessorCountryCode = "HR", PredecessorCountryCode = "RS" },
            new() { SuccessorCountryCode = "SI", PredecessorCountryCode = "RS" },
            new() { SuccessorCountryCode = "MK", PredecessorCountryCode = "RS" },
            new() { SuccessorCountryCode = "ME", PredecessorCountryCode = "RS" },
            new() { SuccessorCountryCode = "AL", PredecessorCountryCode = "GR" },
            new() { SuccessorCountryCode = "RO", PredecessorCountryCode = "HU" },
            new() { SuccessorCountryCode = "HU", PredecessorCountryCode = "AL" },
            new() { SuccessorCountryCode = "GR", PredecessorCountryCode = "RO" },
        };

        await context.Set<LegalSuccessor>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedGovernmentHistoriesAsync(this DbContext context)
    {
        var items = new List<GovernmentHistory>
        {
            new() { CountryCode = "BA", SequenceNumber = 1, GovernmentTypeCode = "SOC",  From = new DateTime(1945,1,1), To = new DateTime(1992,3,1) },
            new() { CountryCode = "BA", SequenceNumber = 2, GovernmentTypeCode = "REP",  From = new DateTime(1992,3,1), To = null },
            new() { CountryCode = "RS", SequenceNumber = 1, GovernmentTypeCode = "SOC",  From = new DateTime(1945,1,1), To = new DateTime(2006,6,5) },
            new() { CountryCode = "RS", SequenceNumber = 2, GovernmentTypeCode = "REP",  From = new DateTime(2006,6,5), To = null },
            new() { CountryCode = "HR", SequenceNumber = 1, GovernmentTypeCode = "PARL", From = new DateTime(1991,6,25), To = null },
            new() { CountryCode = "ME", SequenceNumber = 1, GovernmentTypeCode = "REP",  From = new DateTime(2006,6,3), To = null },
            new() { CountryCode = "SI", SequenceNumber = 1, GovernmentTypeCode = "PARL", From = new DateTime(1991,6,25), To = null },
            new() { CountryCode = "MK", SequenceNumber = 1, GovernmentTypeCode = "PARL", From = new DateTime(1991,9,8), To = null },
            new() { CountryCode = "AL", SequenceNumber = 1, GovernmentTypeCode = "SOC",  From = new DateTime(1946,1,1), To = new DateTime(1991,1,1) },
            new() { CountryCode = "AL", SequenceNumber = 2, GovernmentTypeCode = "PARL", From = new DateTime(1991,1,1), To = null },
        };

        await context.Set<GovernmentHistory>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    // ---------------------------------------------------------------
    // HealthInstitution
    // ---------------------------------------------------------------
    public static async Task SeedHealthInstitutionsAsync(this DbContext context)
    {
        var items = new List<HealthInstitution>
        {
            new() { Name = "Klinički centar Sarajevo",        TypeCode = "BOLN",  CountryCode = "BA", SettlementCode = 1, Address = "Bolnička 25",     BedCount = 1200, FoundingDate = new DateTime(1944,1,1), Phone = "+387 33 297 000" },
            new() { Name = "Klinički centar Srbije",          TypeCode = "BOLN",  CountryCode = "RS", SettlementCode = 1, Address = "Pasterova 2",     BedCount = 3000, FoundingDate = new DateTime(1874,1,1), Phone = "+381 11 366 3699" },
            new() { Name = "Klinički bolnički centar Zagreb",  TypeCode = "BOLN",  CountryCode = "HR", SettlementCode = 1, Address = "Kišpatićeva 12",  BedCount = 1700, FoundingDate = new DateTime(1920,1,1), Phone = "+385 1 2367 111" },
            new() { Name = "Klinički centar Crne Gore",        TypeCode = "BOLN",  CountryCode = "ME", SettlementCode = 1, Address = "Ljubljanska bb",  BedCount = 1000, FoundingDate = new DateTime(1975,1,1), Phone = "+382 20 412 412" },
            new() { Name = "Univerzitetski klinički centar Ljubljana", TypeCode = "KLIN", CountryCode = "SI", SettlementCode = 1, Address = "Zaloška 2", BedCount = 2000, FoundingDate = new DateTime(1920,1,1), Phone = "+386 1 522 5050" },
            new() { Name = "Klinički centar Skoplje",          TypeCode = "KLIN",  CountryCode = "MK", SettlementCode = 1, Address = "Vodnjanska 17",   BedCount = 1500, FoundingDate = new DateTime(1943,1,1), Phone = "+389 2 3147 147" },
            new() { Name = "Univerzitetski bolnički centar Majka Tereza", TypeCode = "BOLN", CountryCode = "AL", SettlementCode = 1, Address = "Rruga Dibres", BedCount = 1300, FoundingDate = new DateTime(1958,1,1), Phone = "+355 4 236 3374" },
            new() { Name = "Opšta bolnica Evangelismos",       TypeCode = "BOLN",  CountryCode = "GR", SettlementCode = 1, Address = "Ipsilantou 45",   BedCount = 1400, FoundingDate = new DateTime(1884,1,1), Phone = "+30 21 3204 1000" },
            new() { Name = "Semmelweis klinika",                TypeCode = "KLIN",  CountryCode = "HU", SettlementCode = 1, Address = "Üllői út 26",     BedCount = 900,  FoundingDate = new DateTime(1769,1,1), Phone = "+36 1 459 1500" },
            new() { Name = "Spitalul Universitar București",   TypeCode = "BOLN",  CountryCode = "RO", SettlementCode = 1, Address = "Splaiul Independentei 169", BedCount = 1100, FoundingDate = new DateTime(1900,1,1), Phone = "+40 21 318 0522" },
        };

        await context.Set<HealthInstitution>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
}
