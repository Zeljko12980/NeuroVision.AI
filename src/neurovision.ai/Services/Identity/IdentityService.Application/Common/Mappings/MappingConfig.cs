namespace IdentityService.Application.Common.Mappings
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<RoleDto, RoleResponse>
                .NewConfig();
        }
    }
}
