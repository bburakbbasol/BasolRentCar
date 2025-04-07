namespace Rent.WebApi.Jwt
{
    /*
     * appsettings.json veya başka bir konfigürasyondan gelen JWT ayarlarını nesne olarak alıp kullanmak.
     *Token üretimi için ihtiyaç duyulan kullanıcı bilgilerini ve ayarları tek bir DTO üzerinden geçirm.

     *JwtHelper gibi servislerde parametre karmaşasını azaltmak.
     */
    public class JwtDto
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresMinutes { get; set; }
        public String UserName { get; set; }   
    }

}
