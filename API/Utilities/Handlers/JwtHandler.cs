using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Utilities.Handlers;

public interface IJwtHandler
{
    string Generate(IEnumerable<Claim> claims);
}

public class JwtHandler : IJwtHandler
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiration;

    public JwtHandler(string key, string issuer, string audience, int expiration)
    {
        _key = key;
        _issuer = issuer;
        _audience = audience;
        _expiration = expiration;
    }

    public string Generate(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(issuer: _issuer,
                                                audience: _audience,
                                                claims: claims,
                                                expires: DateTime.Now.AddMinutes(_expiration),
                                                signingCredentials: signinCredentials);
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return tokenString;
    }
}
