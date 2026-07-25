// File: Authorization/PermissionPolicyProvider.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.Api.Authorization;

/// <summary>
/// Lets you write [Authorize(Policy = "Products.Edit")] anywhere using a raw permission code
/// straight from DataSeeder.PermissionCodes, without registering a policy per permission in
/// Program.cs. Falls back to the default provider first, so normal named policies still work.
/// </summary>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var existing = await _fallbackProvider.GetPolicyAsync(policyName);
        if (existing is not null)
        {
            return existing;
        }

        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(policyName));
        return policy.Build();
    }
}