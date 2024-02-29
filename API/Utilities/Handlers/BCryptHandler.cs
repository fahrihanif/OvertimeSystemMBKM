namespace API.Utilities.Handlers;

public class BCryptHandler
{
    private static string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt(12); // 10 is default
    }

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
