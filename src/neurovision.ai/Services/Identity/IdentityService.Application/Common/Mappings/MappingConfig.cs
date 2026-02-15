namespace IdentityService.Application.Common.Mappings
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {

            TypeAdapterConfig<(bool IsSuccess, string Token, string UserId, string UserName), AuthResponse>
                .NewConfig()
                .Map(dest => dest.IsSuccess, src => src.IsSuccess)
                .Map(dest => dest.Token, src => src.Token)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.UserName, src => src.UserName);


            TypeAdapterConfig<bool, SignInResponse>
                .NewConfig()
                .Map(dest => dest.IsSignedIn, src => src);


            TypeAdapterConfig<bool, ConfirmEmailResponse>
                .NewConfig()
                .Map(dest => dest.IsConfirmed, src => src);

            TypeAdapterConfig<(string Id, string Name), RoleResponse>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name);

            TypeAdapterConfig<List<(string Id, string Name)>, List<RoleResponse>>
                .NewConfig()
                .MapWith(src => src.Select(r => r.Adapt<RoleResponse>()).ToList());

            TypeAdapterConfig<User, UserResponse>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName);

            TypeAdapterConfig<List<User>, List<UserResponse>>
                .NewConfig()
                .MapWith(src => src.Select(u => u.Adapt<UserResponse>()).ToList());
        }

    }
}
