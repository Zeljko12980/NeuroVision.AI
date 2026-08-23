namespace NotificationService.UnitTests.Domain;

public class NotificationTests
{
    [Fact]
    public void Create_WithValidData_SetsInboxFields()
    {
        var notification = NotificationFactory.Create();

        notification.RecipientUserId.Should().Be(NotificationFactory.RecipientId);
        notification.TypeCode.Should().Be(NotificationTypeCodes.System);
        notification.SeverityCode.Should().Be(NotificationSeverityCodes.Info);
        notification.Title.Should().Be("Welcome to NeuroVision");
        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_Throws()
    {
        var act = () => NotificationFactory.Create(id: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("id");
    }

    [Fact]
    public void Create_WithEmptyRecipient_Throws()
    {
        var act = () => NotificationFactory.Create(recipientUserId: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("recipientUserId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_Throws(string? title)
    {
        var act = () => NotificationFactory.Create(title: title!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenRelatedEntityIdWithoutType_Throws()
    {
        var act = () => Notification.Create(
            NotificationFactory.DefaultId,
            NotificationFactory.RecipientId,
            NotificationTypeCodes.System,
            NotificationSeverityCodes.Info,
            "Title",
            "Message",
            NotificationFactory.CreatedAt,
            relatedEntityId: Guid.NewGuid());

        act.Should().Throw<ArgumentException>().WithParameterName("relatedEntityType");
    }

    [Fact]
    public void MarkAsRead_SetsReadAtOnce()
    {
        var notification = NotificationFactory.Create();
        var first = NotificationFactory.CreatedAt.AddMinutes(5);
        var second = first.AddMinutes(5);

        notification.MarkAsRead(first);
        notification.MarkAsRead(second);

        notification.ReadAt.Should().Be(first);
    }
}

public class NotificationPreferenceTests
{
    [Fact]
    public void Create_WithValidData_EnablesChannel()
    {
        var preference = NotificationPreference.Create(
            NotificationFactory.RecipientId,
            NotificationTypeCodes.Tumor,
            NotificationChannelCodes.InApp);

        preference.Enabled.Should().BeTrue();
        preference.TypeCode.Should().Be(NotificationTypeCodes.Tumor);
        preference.ChannelCode.Should().Be(NotificationChannelCodes.InApp);
    }

    [Fact]
    public void Create_WithEmptyUserId_Throws()
    {
        var act = () => NotificationPreference.Create(
            Guid.Empty,
            NotificationTypeCodes.System,
            NotificationChannelCodes.Email);

        act.Should().Throw<ArgumentException>().WithParameterName("userId");
    }
}
