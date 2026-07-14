namespace Severina.Domain.Entities;

public class UserPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    public User? User { get; private set; }

    private UserPreference() { }

    public UserPreference(Guid userId, string key, string value)
    {
        UserId = userId;
        Key = key;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        Value = value;
        UpdateTimestamp();
    }
}
