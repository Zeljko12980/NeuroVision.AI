using LocationService.API.Controllers;
using LocationService.Application;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Feature.Country.Command.Create;
using LocationService.Domain.Entities;
using LocationService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(Country).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(CreateCountryCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(LocationService.Infrastructure.DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(CountryController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LocationService.Application",
                "LocationService.Infrastructure",
                "LocationService.API",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore",
                "MediatR")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrApi()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LocationService.Infrastructure",
                "LocationService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldNotReference_AspNetCoreHttp()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore.Http")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("LocationService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("CreateCountryCommandHandler")
            .Should()
            .HaveDependencyOn("LocationService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(CreateCountryCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(LocationService.Infrastructure.DependencyInjection).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("LocationService.Application.Common.Interfaces")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Handlers_ShouldResideIn_ApplicationFeature()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .ResideInNamespaceContaining("LocationService.Application.Feature")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationShouldNotContain_PassthroughServices()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Service")
            .And()
            .AreClasses()
            .GetTypes()
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void ApplicationPorts_ShouldBeImplemented_InInfrastructure()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(ILocationWriteStore))
            .Should()
            .ResideInNamespace("LocationService.Infrastructure.Persistence")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Commands_ShouldResideIn_FeatureCommand()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ResideInNamespaceContaining("Command")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Queries_ShouldResideIn_FeatureQuery()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .ResideInNamespaceContaining("Query")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class ControllerConventionTests
{
    private static readonly System.Reflection.Assembly Api = typeof(CountryController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("LocationService.API.Controllers")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Controllers_ShouldHave_AuthorizeAttribute()
    {
        Types.InAssembly(Api)
            .That()
            .Inherit(typeof(ControllerBase))
            .And()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveCustomAttribute(typeof(AuthorizeAttribute))
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Controllers_ShouldDependOn_MediatR_NotInfrastructurePersistence()
    {
        Types.InAssembly(Api)
            .That()
            .Inherit(typeof(ControllerBase))
            .And()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn(typeof(ISender).Namespace!)
            .GetResult()
            .ShouldBeSuccessful();

        Types.InAssembly(Api)
            .That()
            .Inherit(typeof(ControllerBase))
            .And()
            .HaveNameEndingWith("Controller")
            .ShouldNot()
            .HaveDependencyOn("LocationService.Infrastructure.Persistence")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

internal static class ArchitectureAssertionExtensions
{
    public static void ShouldBeSuccessful(this TestResult result)
    {
        var failures = result.FailingTypeNames is null
            ? "unknown types"
            : string.Join(", ", result.FailingTypeNames);

        result.IsSuccessful.Should().BeTrue($"violating types: {failures}");
    }
}
