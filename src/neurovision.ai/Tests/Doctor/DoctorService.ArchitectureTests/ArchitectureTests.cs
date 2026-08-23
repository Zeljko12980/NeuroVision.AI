using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.API.Controllers;
using DoctorService.Application.Common.Interfaces;
using DoctorService.Application.Feature.Doctor.Command.Create;
using DoctorService.Domain.Entities;
using DoctorService.Infrastructure;

namespace DoctorService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(Doctor).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(CreateDoctorCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(DoctorController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "DoctorService.Application",
                "DoctorService.Infrastructure",
                "DoctorService.API",
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
                "DoctorService.Infrastructure",
                "DoctorService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("DoctorService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("CreateDoctorCommandHandler")
            .Should()
            .HaveDependencyOn("DoctorService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(CreateDoctorCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("DoctorService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("DoctorService.Application.Feature")
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
    public void WriteStore_ShouldBeImplemented_InInfrastructurePersistence()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IDoctorWriteStore))
            .Should()
            .ResideInNamespace("DoctorService.Infrastructure.Persistence")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void FileStorage_ShouldBeImplemented_InInfrastructureServices()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IFileStorageService))
            .Should()
            .ResideInNamespace("DoctorService.Infrastructure.Services")
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
    private static readonly System.Reflection.Assembly Api = typeof(DoctorController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("DoctorService.API.Controllers")
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
            .HaveDependencyOn("DoctorService.Infrastructure.Persistence")
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
