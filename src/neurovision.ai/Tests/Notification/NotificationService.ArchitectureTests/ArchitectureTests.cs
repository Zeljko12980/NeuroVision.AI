using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Controllers;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Feature.Notification.Command.Create;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure;

namespace NotificationService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(Notification).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(CreateNotificationCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(NotificationController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "NotificationService.Application",
                "NotificationService.Infrastructure",
                "NotificationService.API",
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
                "NotificationService.Infrastructure",
                "NotificationService.API",
                "Microsoft.AspNetCore.SignalR")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("NotificationService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("CreateNotificationCommandHandler")
            .Should()
            .HaveDependencyOn("NotificationService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(CreateNotificationCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(DependencyInjection).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("NotificationService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("NotificationService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void WriteStore_ShouldBeImplemented_InInfrastructurePersistence()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(INotificationWriteStore))
            .Should()
            .ResideInNamespace("NotificationService.Infrastructure.Persistence")
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
    private static readonly System.Reflection.Assembly Api = typeof(NotificationController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("NotificationService.API.Controllers")
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
    public void Hubs_ShouldHave_AuthorizeAttribute()
    {
        Types.InAssembly(Api)
            .That()
            .Inherit(typeof(Hub))
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
            .HaveDependencyOn("NotificationService.Infrastructure.Persistence")
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
