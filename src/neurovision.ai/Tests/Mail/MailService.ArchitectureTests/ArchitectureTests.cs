using MailService.Application.Commands;
using MailService.Application.Common.Interfaces;
using MailService.Domain.ValueObjects;
using MailService.Infrastructure.Services;

namespace MailService.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(EmailAddress).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(SendTemplatedEmailCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(SmtpEmailSender).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "MailService.Application",
                "MailService.Infrastructure",
                "MailService.API",
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
            .HaveDependencyOnAny("MailService.Infrastructure", "MailService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("MailService.API")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Application_ShouldDependOn_Domain()
    {
        Types.InAssembly(Application)
            .That()
            .HaveName("SendTemplatedEmailCommandHandler")
            .Should()
            .HaveDependencyOn("MailService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Infrastructure_ShouldDependOn_ApplicationAndDomain()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .HaveName("SmtpEmailSender")
            .Should()
            .HaveDependencyOnAll("MailService.Application", "MailService.Domain")
            .GetResult()
            .ShouldBeSuccessful();
    }
}

public class NamingConventionTests
{
    private static readonly System.Reflection.Assembly Application = typeof(SendTemplatedEmailCommand).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(SmtpEmailSender).Assembly;

    [Fact]
    public void ApplicationInterfaces_ShouldResideIn_CommonInterfaces()
    {
        Types.InAssembly(Application)
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespace("MailService.Application.Common.Interfaces")
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
            .ResideInNamespaceContaining("MailService.Application")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationPorts_ShouldBeImplemented_InInfrastructure()
    {
        Types.InAssembly(Infrastructure)
            .That()
            .ImplementInterface(typeof(IEmailSender))
            .Or()
            .ImplementInterface(typeof(IDocumentGenerator))
            .Should()
            .ResideInNamespace("MailService.Infrastructure.Services")
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
            .ResideInNamespaceContaining("MailService.Application.Commands")
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
