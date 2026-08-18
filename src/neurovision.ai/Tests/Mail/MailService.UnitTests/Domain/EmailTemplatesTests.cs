using MailService.Domain.Constants;
using MailService.Domain.Templates;

namespace MailService.UnitTests.Domain;

public class EmailTemplatesTests
{
    [Fact]
    public void Get_ConfirmEmail_ReturnsCatalogEntry()
    {
        var template = EmailTemplates.Get(EmailTemplateCodes.EmailConfirmation);

        template.Should().BeSameAs(EmailTemplates.ConfirmEmail);
        template.DocumentTemplateCode.Should().Be(EmailTemplateCodes.EmailConfirmation);
        template.RequiredPlaceholders.Should().Contain(EmailPlaceholderKeys.ConfirmationUrl);
    }

    [Fact]
    public void ForgotPassword_UsesSetPasswordDocumentTemplate()
    {
        EmailTemplates.ForgotPassword.Id.Should().Be(EmailTemplateCodes.ForgotPassword);
        EmailTemplates.ForgotPassword.DocumentTemplateCode.Should().Be(EmailTemplateCodes.SetPassword);
    }

    [Fact]
    public void Get_UnknownId_Throws()
    {
        var act = () => EmailTemplates.Get("UnknownTemplate");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EnsurePlaceholders_WhenMissing_Throws()
    {
        var act = () => EmailTemplates.ConfirmEmail.EnsurePlaceholders(
            new Dictionary<string, string>
            {
                [EmailPlaceholderKeys.FullName] = "Jane"
            });

        act.Should().Throw<ArgumentException>();
    }
}
