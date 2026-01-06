using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Website_Designer_Ghana.Data;

namespace Website_Designer_Ghana.Components.Account.Pages;

public partial class Login
{
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [Inject] private ILogger<Login> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IdentityRedirectManager RedirectManager { get; set; } = default!;

    private string? errorMessage;
    private EditContext editContext = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Input ??= new();

        editContext = new EditContext(Input);

        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task LoginUser()
    {
        if (!string.IsNullOrEmpty(Input.Passkey?.Error))
        {
            errorMessage = $"Error: {Input.Passkey.Error}";
            return;
        }

        SignInResult result;
        if (!string.IsNullOrEmpty(Input.Passkey?.CredentialJson))
        {
            // When performing passkey sign-in, don't perform form validation.
            result = await SignInManager.PasskeySignInAsync(Input.Passkey.CredentialJson);
        }
        else
        {
            // If doing a password sign-in, validate the form.
            if (!editContext.Validate())
            {
                return;
            }

            // First find the user by email to get their actual UserName
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user != null)
            {
                result = await SignInManager.PasswordSignInAsync(user.UserName!, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            }
            else
            {
                // Fallback to using email as username if user not found by email (though usually they are the same now)
                result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            }
        }

        if (result.Succeeded)
        {
            Logger.LogInformation("User logged in.");
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.RequiresTwoFactor)
        {
            RedirectManager.RedirectTo(
                "Account/LoginWith2fa",
                new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
        }
        else if (result.IsLockedOut)
        {
            Logger.LogWarning("User account locked out.");
            RedirectManager.RedirectTo("Account/Lockout");
        }
        else
        {
            errorMessage = "Error: Invalid login attempt.";
        }
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public PasskeyInputModel? Passkey { get; set; }
    }
}
