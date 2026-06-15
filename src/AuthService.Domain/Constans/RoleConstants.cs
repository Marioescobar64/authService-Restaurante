namespace AuthService.Domain.Constants;

//Clase que contiene constantes relacionadas con el servicio de autenticación, como los nombres de los roles de usuario y otros valores que se utilizan en la lógica de autenticación y autorización.
public static class RoleConstants
{
    public const string ADMIN_ROLE = "ADMIN_ROLE";
    public const string USER_ROLE = "USER_ROLE";
    public const string MANAGER_ROLE = "MANAGER_ROLE";
    public const string CHEF_ROLE = "CHEF_ROLE";
    public const string WAITER_ROLE = "WAITER_ROLE";

    public static readonly string[] AllowedRoles = { ADMIN_ROLE, USER_ROLE, MANAGER_ROLE, CHEF_ROLE, WAITER_ROLE };

    public static string NormalizeRoleName(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return USER_ROLE;
        }

        var normalized = roleName.Trim().ToUpperInvariant();

        return normalized switch
        {
            "ADMIN" or "ADMIN_ROLE" => ADMIN_ROLE,
            "USER" or "USER_ROLE" => USER_ROLE,
            "GERENTE" or "GERENTE_ROLE" or "MANAGER" or "MANAGER_ROLE" => MANAGER_ROLE,
            "CHEF" or "CHEF_ROLE" => CHEF_ROLE,
            "MESERO" or "MESERO_ROLE" or "WAITER" or "WAITER_ROLE" => WAITER_ROLE,
            _ => normalized
        };
    }
}