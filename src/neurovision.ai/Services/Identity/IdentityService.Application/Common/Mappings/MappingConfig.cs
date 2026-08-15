using IdentityService.Application.Common.DTOs;

namespace IdentityService.Application.Common.Mappings
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {

            TypeAdapterConfig<bool, SignInResponse>
                .NewConfig()
                .Map(dest => dest.IsSignedIn, src => src);


            TypeAdapterConfig<bool, ConfirmEmailResponse>
                .NewConfig()
                .Map(dest => dest.IsConfirmed, src => src);

            TypeAdapterConfig<RoleDto, RoleResponse>
                .NewConfig();
        }

    }
}
