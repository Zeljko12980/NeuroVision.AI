using PdfService.API.Controllers;
using PdfService.Application.Commands.Templates;
using PdfService.Application.Common.Interfaces;
using PdfService.Domain.Entities;
using PdfService.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PdfService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(PdfTemplate).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(GeneratePdfCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(HtmlPdfGenerator).Assembly;
    private static readonly System.Reflection.Assembly Api = typeof(PdfController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "PdfService.Application",
                "PdfService.Infrastructure",
                "PdfService.API",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore",
                "MassTransit",
                "MediatR",
                "iText",
                "Org.BouncyCastle")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrApi()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "PdfService.Infrastructure",
                "PdfService.API",
                "iText",
                "Org.BouncyCastle")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("PdfService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("GeneratePdfCommandHandler")
            .Should()
            .HaveDependencyOn("PdfService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldDependOn_ApplicationAndDomain()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .HaveName("PdfSigningService")
            .Should()
            .HaveDependencyOnAll("PdfService.Application", "PdfService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(GeneratePdfCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(HtmlPdfGenerator).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("PdfService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("PdfService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationPorts_ShouldBeImplemented_InInfrastructure()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IPdfGenerator))
            .Or()
            .ImplementInterface(typeof(IPdfSigningService))
            .Or()
            .ImplementInterface(typeof(ICertificateStorage))
            .Or()
            .ImplementInterface(typeof(ICertificatePasswordProtector))
            .Or()
            .ImplementInterface(typeof(ICertificateFileParser))
            .Or()
            .ImplementInterface(typeof(IPdfTemplateReadStore))
            .Or()
            .ImplementInterface(typeof(ICertificateReadStore))
            .Should()
            .ResideInNamespace("PdfService.Infrastructure.Services")
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
            .ResideInNamespaceContaining("PdfService.Application.Commands")
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
            .ResideInNamespaceContaining("PdfService.Application.Queries")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class ControllerConventionTests
{
    private static readonly System.Reflection.Assembly Api = typeof(PdfController).Assembly;

    [Fact]
    public void Controllers_ShouldResideIn_ApiControllersNamespace()
    {
        Types.InAssembly(Api)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveName("ControllerBase")
            .Should()
            .ResideInNamespace("PdfService.API.Controllers")
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
            .HaveDependencyOn("PdfService.Infrastructure.Services")
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
