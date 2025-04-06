namespace Rent.WebApi.Jwt
{

    /*
     * JwtClaimNames sınıfı, JWT token oluşturulurken kullanılan claim isimlerini sabit ve merkezi bir şekilde tanımlamanı sağlıyor
     * Yazım hatalarının önüne geçiyoruz.
     * Kodun okunabilirliğini ve bakımını artırıyor.

*/

    public static class JwtClaimNames
    {
        public const string Id = "id";
        public const string Email = "email";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string Role = "role";
        public const string Username = "username";
    }
}
