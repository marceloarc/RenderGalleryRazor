using Microsoft.AspNetCore.Identity;

namespace RenderGalleyRazor.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
    }
}
