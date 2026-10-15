namespace DemoMsAgentFw.JsonPlaceholderMcp;

public sealed record TodoItem(int UserId, int Id, string Title, bool Completed)
{
    private const int UnknownUserId = -1;

    public TodoItem() : this(UnknownUserId, 0, string.Empty, false)
    {
    }
}

public sealed record Geo(double Lat, double Lng);

public sealed record Address(string Street, string Suite, string City, string Zipcode, Geo Geo);

public sealed record Company(string Name, string CatchPhrase, string Bs);

public sealed record User(
    int Id,
    string Name,
    string Username,
    string Email,
    string Phone,
    string Website,
    Address Address,
    Company Company)
{
    private const int UnknownUserId = -1;

    private static readonly Geo EmptyGeo = new(0, 0);

    private static readonly Address EmptyAddress = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        EmptyGeo);

    private static readonly Company EmptyCompany = new(
        string.Empty,
        string.Empty,
        string.Empty);

    public User()
        : this(
            UnknownUserId,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            EmptyAddress,
            EmptyCompany)
    {
    }
}