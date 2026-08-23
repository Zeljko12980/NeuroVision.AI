using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentService.API.Controllers;
using AppointmentService.Application.Common.Interfaces;
using AppointmentService.Application.Feature.Appointment.Command.Create;
using AppointmentService.Domain.Entities;
using AppointmentService.Infrastructure;

namespace AppointmentService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(Appointment).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(CreateAppointmentCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(AppointmentController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "AppointmentService.Application",
                "AppointmentService.Infrastructure",
                "AppointmentService.API",
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
                "AppointmentService.Infrastructure",
                "AppointmentService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("AppointmentService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("CreateAppointmentCommandHandler")
            .Should()
            .HaveDependencyOn("AppointmentService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(CreateAppointmentCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("AppointmentService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("AppointmentService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void WriteStore_ShouldBeImplemented_InInfrastructurePersistence()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IAppointmentWriteStore))
            .Should()
            .ResideInNamespace("AppointmentService.Infrastructure.Persistence")
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
    private static readonly System.Reflection.Assembly Api = typeof(AppointmentController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("AppointmentService.API.Controllers")
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
            .HaveDependencyOn("AppointmentService.Infrastructure.Persistence")
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
