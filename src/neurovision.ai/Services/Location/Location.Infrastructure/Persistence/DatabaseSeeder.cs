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

    public static async Task SeedGovernmentTypesAsync(this DbContext context)
    {
        var items = new List<GovernmentType>
        {
            GovernmentType.Create("REP", "Republika", "Opšti oblik republike"),
            GovernmentType.Create("PARL", "Parlamentarna republika", "Vlast proizilazi iz parlamenta"),
            GovernmentType.Create("SEMI", "Polupredsjednička republika", "Podijeljena izvršna vlast"),
            GovernmentType.Create("PRES", "Predsjednička republika", "Predsjednik je šef izvršne vlasti"),
            GovernmentType.Create("FED", "Federalna republika", "Federalno uređena država"),
            GovernmentType.Create("KONF", "Konfederacija", "Labava zajednica država"),
            GovernmentType.Create("MON", "Ustavna monarhija", "Monarhija ograničena ustavom"),
            GovernmentType.Create("KRALJ", "Kraljevina", "Apsolutna ili ustavna kraljevina"),
            GovernmentType.Create("SOC", "Socijalistička republika", "Bivši socijalistički sistem"),
            GovernmentType.Create("TRANZ", "Prelazna vlast", "Privremeni/tranzicioni oblik vlasti"),
        };

        await context.Set<GovernmentType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionTypesAsync(this DbContext context)
    {
        var items = new List<RegionType>
        {
            RegionType.Create("ENTITET", "Entitet", "Državni entitet unutar zemlje"),
            RegionType.Create("KANTON", "Kanton", "Kantonalna administrativna jedinica"),
            RegionType.Create("OBLAST", "Oblast", "Šira geopolitička oblast"),
            RegionType.Create("GRUPA", "Grupacija država", "Grupa država sa zajedničkim ciljem"),
            RegionType.Create("SAVEZ", "Savez", "Vojno-politički savez"),
            RegionType.Create("UNIJA", "Unija", "Ekonomsko-politička unija"),
            RegionType.Create("REGIJA", "Geografska regija", "Prirodno-geografska cjelina"),
            RegionType.Create("CARINA", "Carinska zona", "Zona slobodne trgovine"),
            RegionType.Create("KULTURA", "Kulturna regija", "Regija povezana kulturom/istorijom"),
            RegionType.Create("ADMIN", "Administrativna zona", "Opšta administrativna podjela"),
        };

        await context.Set<RegionType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedHealthInstitutionTypesAsync(this DbContext context)
    {
        var items = new List<HealthInstitutionType>
        {
            HealthInstitutionType.Create("BOLN", "Bolnica", "Opšta bolnica"),
            HealthInstitutionType.Create("KLIN", "Klinika", "Specijalizovana klinika"),
            HealthInstitutionType.Create("DZ", "Dom zdravlja", "Primarna zdravstvena zaštita"),
            HealthInstitutionType.Create("POLI", "Poliklinika", "Ambulantno liječenje"),
            HealthInstitutionType.Create("HITNA", "Centar za hitnu pomoć", "Hitna medicinska pomoć"),
            HealthInstitutionType.Create("SPEC", "Specijalna bolnica", "Specijalizovano liječenje"),
            HealthInstitutionType.Create("STOM", "Stomatološka klinika", "Stomatološke usluge"),
            HealthInstitutionType.Create("REHAB", "Rehabilitacioni centar", "Fizikalna rehabilitacija"),
            HealthInstitutionType.Create("DIJAG", "Dijagnostički centar", "Dijagnostičke usluge"),
            HealthInstitutionType.Create("PORO", "Porodilište", "Ustanova za porođaje"),
        };

        await context.Set<HealthInstitutionType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedCountriesAsync(this DbContext context)
    {
        var items = new List<Country>
        {
            Country.Create("BA", "Bosna i Hercegovina", new DateTime(1995, 12, 14), callingCode: 387, governmentTypeCode: "REP"),
            Country.Create("RS", "Srbija", new DateTime(2006, 6, 5), callingCode: 381, governmentTypeCode: "REP"),
            Country.Create("HR", "Hrvatska", new DateTime(1991, 6, 25), callingCode: 385, governmentTypeCode: "PARL"),
            Country.Create("ME", "Crna Gora", new DateTime(2006, 6, 3), callingCode: 382, governmentTypeCode: "REP"),
            Country.Create("SI", "Slovenija", new DateTime(1991, 6, 25), callingCode: 386, governmentTypeCode: "PARL"),
            Country.Create("MK", "Sjeverna Makedonija", new DateTime(1991, 9, 8), callingCode: 389, governmentTypeCode: "PARL"),
            Country.Create("AL", "Albanija", new DateTime(1912, 11, 28), callingCode: 355, governmentTypeCode: "PARL"),
            Country.Create("GR", "Grčka", new DateTime(1830, 2, 3), callingCode: 30, governmentTypeCode: "PARL"),
            Country.Create("HU", "Mađarska", new DateTime(1000, 1, 1), callingCode: 36, governmentTypeCode: "PARL"),
            Country.Create("RO", "Rumunija", new DateTime(1859, 1, 24), callingCode: 40, governmentTypeCode: "SEMI"),
        };

        await context.Set<Country>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedSettlementsAsync(this DbContext context)
    {
        var items = new List<Settlement>
        {
            Settlement.Create("BA", 1, "Sarajevo", "71000"),
            Settlement.Create("RS", 1, "Beograd", "11000"),
            Settlement.Create("HR", 1, "Zagreb", "10000"),
            Settlement.Create("ME", 1, "Podgorica", "81000"),
            Settlement.Create("SI", 1, "Ljubljana", "1000"),
            Settlement.Create("MK", 1, "Skoplje", "1000"),
            Settlement.Create("AL", 1, "Tirana", "1001"),
            Settlement.Create("GR", 1, "Atina", "10431"),
            Settlement.Create("HU", 1, "Budimpešta", "1011"),
            Settlement.Create("RO", 1, "Bukurešt", "010011"),
        };

        await context.Set<Settlement>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedCapitalsAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };
        var founding = new[]
        {
            new DateTime(1995, 12, 14), new DateTime(2006, 6, 5), new DateTime(1991, 6, 25),
            new DateTime(2006, 6, 3), new DateTime(1991, 6, 25), new DateTime(1991, 9, 8),
            new DateTime(1912, 11, 28), new DateTime(1830, 2, 3), new DateTime(1000, 1, 1),
            new DateTime(1859, 1, 24),
        };

        var items = countries
            .Select((code, i) => Capital.Create(code, 1, 1, founding[i]))
            .ToList();

        await context.Set<Capital>().AddRangeAsync(items);
        await context.SaveChangesAsync();

        foreach (var country in await context.Set<Country>().ToListAsync())
            country.SetCapitalSettlement(1);

        await context.SaveChangesAsync();
    }

    public static async Task SeedMunicipalitiesAsync(this DbContext context)
    {
        var items = new List<Municipality>
        {
            Municipality.Create("BA", 1, "Opština Centar", 1),
            Municipality.Create("RS", 1, "Gradska opština Stari grad", 1),
            Municipality.Create("HR", 1, "Grad Zagreb", 1),
            Municipality.Create("ME", 1, "Opština Podgorica", 1),
            Municipality.Create("SI", 1, "Mestna občina Ljubljana", 1),
            Municipality.Create("MK", 1, "Opština Centar Skoplje", 1),
            Municipality.Create("AL", 1, "Bashkia Tiranë", 1),
            Municipality.Create("GR", 1, "Dimos Athinaion", 1),
            Municipality.Create("HU", 1, "Belváros-Lipótváros", 1),
            Municipality.Create("RO", 1, "Sectorul 1 București", 1),
        };

        await context.Set<Municipality>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMunicipalitySettlementCoveragesAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };
        var items = countries
            .Select(code => MunicipalitySettlementCoverage.Create(code, 1, 1))
            .ToList();

        await context.Set<MunicipalitySettlementCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLocalCommunitiesAsync(this DbContext context)
    {
        var items = new List<LocalCommunity>
        {
            LocalCommunity.Create("BA", 1, 1, "MZ Centar", 1),
            LocalCommunity.Create("RS", 1, 1, "MZ Stari Grad", 1),
            LocalCommunity.Create("HR", 1, 1, "MZ Gornji Grad", 1),
            LocalCommunity.Create("ME", 1, 1, "MZ Zabjelo", 1),
            LocalCommunity.Create("SI", 1, 1, "MZ Center", 1),
            LocalCommunity.Create("MK", 1, 1, "MZ Centar", 1),
            LocalCommunity.Create("AL", 1, 1, "Njesia Njesi 1", 1),
            LocalCommunity.Create("GR", 1, 1, "Geitonia Plaka", 1),
            LocalCommunity.Create("HU", 1, 1, "Belváros Negyed", 1),
            LocalCommunity.Create("RO", 1, 1, "Cartierul Centru", 1),
        };

        await context.Set<LocalCommunity>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLocalCommunityCoveragesAsync(this DbContext context)
    {
        var countries = new[] { "BA", "RS", "HR", "ME", "SI", "MK", "AL", "GR", "HU", "RO" };
        var items = countries
            .Select(code => LocalCommunityCoverage.Create(code, 1, 1, 1))
            .ToList();

        await context.Set<LocalCommunityCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionsAsync(this DbContext context)
    {
        var items = new List<Region>
        {
            Region.Create("ENTITET", 1, "Federacija Bosne i Hercegovine", "BA", administrativeSeatSettlementCode: 1),
            Region.Create("ENTITET", 2, "Republika Srpska", "BA"),
            Region.Create("OBLAST", 1, "Zapadni Balkan"),
            Region.Create("OBLAST", 2, "Panonska regija"),
            Region.Create("REGIJA", 1, "Jadranska regija"),
            Region.Create("REGIJA", 2, "Dinarske Alpe"),
            Region.Create("UNIJA", 1, "Evropska unija"),
            Region.Create("SAVEZ", 1, "NATO"),
            Region.Create("CARINA", 1, "CEFTA"),
            Region.Create("KULTURA", 1, "Balkansko poluostrvo"),
        };

        await context.Set<Region>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionCompositionsAsync(this DbContext context)
    {
        var items = new List<RegionComposition>
        {
            RegionComposition.Create("OBLAST", 1, "ENTITET", 1),
            RegionComposition.Create("OBLAST", 1, "ENTITET", 2),
            RegionComposition.Create("OBLAST", 1, "REGIJA", 1),
            RegionComposition.Create("OBLAST", 1, "REGIJA", 2),
            RegionComposition.Create("KULTURA", 1, "OBLAST", 1),
            RegionComposition.Create("KULTURA", 1, "OBLAST", 2),
            RegionComposition.Create("CARINA", 1, "OBLAST", 1),
            RegionComposition.Create("SAVEZ", 1, "REGIJA", 1),
            RegionComposition.Create("UNIJA", 1, "REGIJA", 2),
            RegionComposition.Create("UNIJA", 1, "KULTURA", 1),
        };

        await context.Set<RegionComposition>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRegionSettlementCoveragesAsync(this DbContext context)
    {
        var items = new List<RegionSettlementCoverage>
        {
            RegionSettlementCoverage.Create("ENTITET", 1, "BA", 1),
            RegionSettlementCoverage.Create("OBLAST", 1, "BA", 1),
            RegionSettlementCoverage.Create("OBLAST", 1, "RS", 1),
            RegionSettlementCoverage.Create("OBLAST", 1, "HR", 1),
            RegionSettlementCoverage.Create("OBLAST", 1, "ME", 1),
            RegionSettlementCoverage.Create("OBLAST", 2, "HU", 1),
            RegionSettlementCoverage.Create("OBLAST", 2, "RO", 1),
            RegionSettlementCoverage.Create("REGIJA", 1, "HR", 1),
            RegionSettlementCoverage.Create("REGIJA", 1, "AL", 1),
            RegionSettlementCoverage.Create("KULTURA", 1, "GR", 1),
        };

        await context.Set<RegionSettlementCoverage>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedCountryCompositionsAsync(this DbContext context)
    {
        var items = new List<CountryComposition>
        {
            CountryComposition.Create("RS", "ME", 1, new DateTime(2003, 2, 4), new DateTime(2006, 6, 3)),
            CountryComposition.Create("SI", "HR", 1, new DateTime(1991, 1, 1), new DateTime(1991, 6, 25)),
            CountryComposition.Create("HR", "BA", 1, new DateTime(2000, 1, 1)),
            CountryComposition.Create("GR", "AL", 1, new DateTime(2010, 1, 1)),
            CountryComposition.Create("HU", "RO", 1, new DateTime(2007, 1, 1)),
            CountryComposition.Create("MK", "AL", 1, new DateTime(2005, 1, 1)),
            CountryComposition.Create("BA", "RS", 1, new DateTime(1918, 1, 1), new DateTime(1992, 3, 1)),
            CountryComposition.Create("BA", "HR", 2, new DateTime(1918, 1, 1), new DateTime(1992, 3, 1)),
            CountryComposition.Create("BA", "SI", 3, new DateTime(1918, 1, 1), new DateTime(1991, 6, 25)),
            CountryComposition.Create("BA", "MK", 4, new DateTime(1918, 1, 1), new DateTime(1991, 9, 8)),
        };

        await context.Set<CountryComposition>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLegalSuccessorsAsync(this DbContext context)
    {
        var items = new List<LegalSuccessor>
        {
            LegalSuccessor.Create("RS", "ME"),
            LegalSuccessor.Create("BA", "RS"),
            LegalSuccessor.Create("HR", "RS"),
            LegalSuccessor.Create("SI", "RS"),
            LegalSuccessor.Create("MK", "RS"),
            LegalSuccessor.Create("ME", "RS"),
            LegalSuccessor.Create("AL", "GR"),
            LegalSuccessor.Create("RO", "HU"),
            LegalSuccessor.Create("HU", "AL"),
            LegalSuccessor.Create("GR", "RO"),
        };

        await context.Set<LegalSuccessor>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedGovernmentHistoriesAsync(this DbContext context)
    {
        var items = new List<GovernmentHistory>
        {
            GovernmentHistory.Create("BA", 1, "SOC", new DateTime(1945, 1, 1), new DateTime(1992, 3, 1)),
            GovernmentHistory.Create("BA", 2, "REP", new DateTime(1992, 3, 1)),
            GovernmentHistory.Create("RS", 1, "SOC", new DateTime(1945, 1, 1), new DateTime(2006, 6, 5)),
            GovernmentHistory.Create("RS", 2, "REP", new DateTime(2006, 6, 5)),
            GovernmentHistory.Create("HR", 1, "PARL", new DateTime(1991, 6, 25)),
            GovernmentHistory.Create("ME", 1, "REP", new DateTime(2006, 6, 3)),
            GovernmentHistory.Create("SI", 1, "PARL", new DateTime(1991, 6, 25)),
            GovernmentHistory.Create("MK", 1, "PARL", new DateTime(1991, 9, 8)),
            GovernmentHistory.Create("AL", 1, "SOC", new DateTime(1946, 1, 1), new DateTime(1991, 1, 1)),
            GovernmentHistory.Create("AL", 2, "PARL", new DateTime(1991, 1, 1)),
        };

        await context.Set<GovernmentHistory>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedHealthInstitutionsAsync(this DbContext context)
    {
        var items = new List<HealthInstitution>
        {
            HealthInstitution.Create("Klinički centar Sarajevo", "BOLN", "BA", 1, "Bolnička 25", 1200, new DateTime(1944, 1, 1), "+387 33 297 000"),
            HealthInstitution.Create("Klinički centar Srbije", "BOLN", "RS", 1, "Pasterova 2", 3000, new DateTime(1874, 1, 1), "+381 11 366 3699"),
            HealthInstitution.Create("Klinički bolnički centar Zagreb", "BOLN", "HR", 1, "Kišpatićeva 12", 1700, new DateTime(1920, 1, 1), "+385 1 2367 111"),
            HealthInstitution.Create("Klinički centar Crne Gore", "BOLN", "ME", 1, "Ljubljanska bb", 1000, new DateTime(1975, 1, 1), "+382 20 412 412"),
            HealthInstitution.Create("Univerzitetski klinički centar Ljubljana", "KLIN", "SI", 1, "Zaloška 2", 2000, new DateTime(1920, 1, 1), "+386 1 522 5050"),
            HealthInstitution.Create("Klinički centar Skoplje", "KLIN", "MK", 1, "Vodnjanska 17", 1500, new DateTime(1943, 1, 1), "+389 2 3147 147"),
            HealthInstitution.Create("Univerzitetski bolnički centar Majka Tereza", "BOLN", "AL", 1, "Rruga Dibres", 1300, new DateTime(1958, 1, 1), "+355 4 236 3374"),
            HealthInstitution.Create("Opšta bolnica Evangelismos", "BOLN", "GR", 1, "Ipsilantou 45", 1400, new DateTime(1884, 1, 1), "+30 21 3204 1000"),
            HealthInstitution.Create("Semmelweis klinika", "KLIN", "HU", 1, "Üllői út 26", 900, new DateTime(1769, 1, 1), "+36 1 459 1500"),
            HealthInstitution.Create("Spitalul Universitar București", "BOLN", "RO", 1, "Splaiul Independentei 169", 1100, new DateTime(1900, 1, 1), "+40 21 318 0522"),
        };

        await context.Set<HealthInstitution>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
}
