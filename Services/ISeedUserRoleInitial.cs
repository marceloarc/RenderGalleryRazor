namespace RenderGalleyRazor.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedRolesAsync();

        Task SeedUsersAsync();
    }
}
