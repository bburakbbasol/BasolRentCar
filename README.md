# BasolRentCar - Araç Kiralama Sistemi

ASP.NET Core 8.0 kullanılarak clean architecture prensiplerine uygun olarak geliştirilmiş modern bir araç kiralama sistemi.

## 🚀 Özellikler

- JWT ile kullanıcı kimlik doğrulama ve yetkilendirme
- Araç yönetimi (CRUD işlemleri)
- Rezervasyon sistemi
- Rol tabanlı erişim kontrolü (Admin/User)
- Araç müsaitlik kontrolü
- Rezervasyonlar için ek hizmet seçenekleri
- Yumuşak silme (soft delete) fonksiyonu
- İstek loglama ve hata yönetimi
- Zaman bazlı erişim kontrolü
- Güvenli şifre hashleme

## 🏗 Mimari

Çözüm, clean architecture prensiplerine uygun olarak üç ana projeden oluşmaktadır:

```
BasolRentCar/
├── WebApi/           # API Controller'lar ve yapılandırma
├── Application/      # İş mantığı ve arayüzler
└── Infrastructure/   # Veri erişimi ve implementasyonlar
```

### Ana Bileşenler

- **WebApi**: RESTful API endpoint'leri ve yapılandırma
- **Application**: İş mantığı, DTO'lar ve arayüzler
- **Infrastructure**: Veritabanı bağlamı, varlıklar ve repository'ler

## 🛠 Teknoloji Yığını

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT Kimlik Doğrulama
- Swagger/OpenAPI
- Clean Architecture
- Repository Pattern
- Unit of Work Pattern

## 📝 API Endpoint'leri

### Kimlik Doğrulama
- POST `/api/auth/register` - Yeni kullanıcı kaydı
- POST `/api/auth/register-admin` - Yeni admin kaydı
- POST `/api/auth/login` - Kullanıcı girişi

### Araçlar
- GET `/api/cars` - Tüm araçları listele
- GET `/api/cars/{plateNumber}` - Plakaya göre araç getir
- POST `/api/cars` - Yeni araç ekle (Sadece Admin)
- PUT `/api/cars/{plateNumber}` - Araç güncelle (Sadece Admin)
- PATCH `/api/cars/{plateNumber}` - Araç kısmi güncelle (Sadece Admin)
- DELETE `/api/cars/{plateNumber}` - Araç sil (Sadece Admin)

### Rezervasyonlar
- GET `/api/reservations` - Kullanıcının rezervasyonlarını getir
- POST `/api/reservations` - Yeni rezervasyon oluştur
- GET `/api/reservations/{id}` - Rezervasyon detaylarını getir

## 🚀 Başlangıç

1. Projeyi klonlayın:
```bash
git clone https://github.com/yourusername/BasolRentCar.git
```

2. `WebApi/appsettings.json` dosyasında veritabanı bağlantı dizesini güncelleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Veritabanı_Bağlantı_Dizeniz"
  }
}
```

3. Veritabanı migration'larını uygulayın:
```bash
dotnet ef database update
```

4. Uygulamayı çalıştırın:
```bash
dotnet run --project WebApi/Rent.WebApi.csproj
```

## 🔐 Güvenlik

- JWT token ile kimlik doğrulama
- Şifre hashleme ve tuzlama
- Rol tabanlı yetkilendirme
- İstek doğrulama filtreleri
- Güvenli middleware pipeline

## 🔍 Middleware Bileşenleri

- `ExceptionMiddleware`: Global hata yönetimi
- `JwtMiddleware`: Token doğrulama
- `RequestLoggingMiddleware`: İstek/yanıt loglama
- `TimeControllerFilter`: Zaman bazlı erişim kontrolü

## 📦 Veritabanı Şeması

Ana varlıklar:
- User (Kullanıcı)
- Car (Araç)
- Reservation (Rezervasyon)
- Service (Hizmet)
- ReservationService (Rezervasyon-Hizmet İlişkisi)

## 🛡 Yetkilendirme

İki ana rol:
- **Admin**: Tam sistem erişimi
- **User**: Rezervasyon ve araç görüntüleme ile sınırlı

## 🔄 Hata Yönetimi

Merkezi hata yönetimi:
- Özel istisnalar
- Hata loglama
- Standartlaştırılmış hata yanıtları

## 🧪 Doğrulama

- Model doğrulama
- Özel doğrulama filtreleri
- İş kuralları doğrulama

## 📈 Loglama

- İstek/yanıt loglama
- Hata loglama
- Performans izleme



## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Detaylar için LICENSE dosyasına bakın.

## 👥 Yazarlar

- Burak BAŞOL - İlk geliştirme

