using IdentityService.API.Controllers;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(User).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(LoginCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(FrontendLinkService).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(AuthenticationController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "IdentityService.Application",
                "IdentityService.Infrastructure",
                "IdentityService.API",
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
            .HaveDependencyOnAny("IdentityService.Infrastructure", "IdentityService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("IdentityService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("IdentityMappings")
            .Should()
            .HaveDependencyOn("IdentityService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldDependOn_ApplicationAndDomain()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .HaveName("UserService")
            .Should()
            .HaveDependencyOnAll("IdentityService.Application", "IdentityService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(LoginCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(FrontendLinkService).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("IdentityService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("IdentityService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationPorts_ShouldBeImplemented_InInfrastructure()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IUserService))
            .Or()
            .ImplementInterface(typeof(IRoleService))
            .Or()
            .ImplementInterface(typeof(IIdentityService))
            .Or()
            .ImplementInterface(typeof(IJwtTokenGenerator))
            .Or()
            .ImplementInterface(typeof(IFrontendLinkService))
            .Or()
            .ImplementInterface(typeof(ICurrentUser))
            .Should()
            .ResideInNamespace("IdentityService.Infrastructure.Services")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Commands_ShouldResideIn_ApplicationCommands()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ResideInNamespaceContaining("IdentityService.Application.Commands")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Queries_ShouldResideIn_ApplicationQueries()
    {
        Types.InAssembly(Application)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .ResideInNamespaceContaining("IdentityService.Application.Queries")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class ControllerConventionTests
{
    private static readonly System.Reflection.Assembly Api = typeof(AuthenticationController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("IdentityService.API.Controllers")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Controllers_ShouldHave_AuthorizeAttribute_ExceptAuthentication()
    {
        Types.InAssembly(Api)
            .That()
            .Inherit(typeof(ControllerBase))
            .And()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("AuthenticationController")
            .Should()
            .HaveCustomAttribute(typeof(AuthorizeAttribute))
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Controllers_ShouldDependOn_MediatR_NotInfrastructureServices()
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
            .HaveDependencyOn("IdentityService.Infrastructure.Services")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Controllers_ShouldNotReference_InfrastructurePersistence()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .ShouldNot()
            .HaveDependencyOn("IdentityService.Infrastructure.Persistence")
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
