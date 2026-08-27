using TumorDetectionService.API.Controllers;
using TumorDetectionService.Application.Analyses.Commands.Start;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TumorDetectionService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(BrainScan).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(StartAnalysisCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjectionExtension).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(ScansController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "TumorDetectionService.Application",
                "TumorDetectionService.Infrastructure",
                "TumorDetectionService.API",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore",
                "MassTransit",
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
                "TumorDetectionService.Infrastructure",
                "TumorDetectionService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("TumorDetectionService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("StartAnalysisCommandHandler")
            .Should()
            .HaveDependencyOn("TumorDetectionService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(StartAnalysisCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjectionExtension).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("TumorDetectionService.Application.Common.Interfaces")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Handlers_ShouldResideIn_Application()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .ResideInNamespaceContaining("TumorDetectionService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ScanStorage_ShouldBeImplemented_InInfrastructureServices()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IScanStorageService))
            .Or()
            .ImplementInterface(typeof(IModelStorageService))
            .Should()
            .ResideInNamespace("TumorDetectionService.Infrastructure.Services")
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
            .ResideInNamespaceContaining("Queries")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class ControllerConventionTests
{
    private static readonly System.Reflection.Assembly Api = typeof(ScansController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("TumorDetectionService.API.Controllers")
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
            .HaveDependencyOn("TumorDetectionService.Infrastructure.Persistence")
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
