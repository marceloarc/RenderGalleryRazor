using Microsoft.AspNetCore.Identity;

namespace RenderGalleyRazor.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SeedUserRoleInitial(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            if(! await _roleManager.RoleExistsAsync("Artista"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Artista";
                role.NormalizedName = "ARTISTA";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsersAsync()
        {
            if(await _userManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "admin@gmail.com";
                user.Email = "admin@gmail.com";
                user.NormalizedEmail = "ADMIN@GMAIL.COM";
                user.NormalizedUserName = "ADMIN@GMAIL.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;

                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = await _userManager.CreateAsync(user, "admin#2023");

                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
