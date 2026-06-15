# harc-api

Harc API, kurumsal İK portalı için geliştirilen .NET 10 tabanlı bir Web API uygulamasıdır. Proje, modüler monolit yaklaşımıyla organize edilmiş ve her özellik dikey dilim mantığıyla kendi klasöründe izole edilmiştir. Mevcut kod tabanında kimlik ve oturum akışı ön plandadır.

## Mimari Özeti

Uygulama tek bir ASP.NET Core host üzerinde çalışır; ancak iş alanı kodu `Modules/` altında modüllere ayrılır. Şu anki ana modül `Identity`'dir ve aşağıdaki katmanları içerir:

- `Data`: Entity Framework Core `DbContext`, entity ve migration dosyaları.
- `Features`: Endpoint, request ve response tipleri.
- `Infrastructure`: Kimlik doğrulama sonrası claim zenginleştirme işlemleri.

Çalışma akışı kısaca şöyledir:

1. Google tabanlı JWT kimlik doğrulaması `Program.cs` içinde yapılandırılır.
2. İstek doğrulandıktan sonra `ClaimsTransformation` devreye girer.
3. Kullanıcının e-postası veritabanında yoksa `identity.Users` tablosuna yeni kayıt açılır.
4. Kullanıcıya rol ve iç sistem kullanıcı kimliği claim olarak eklenir.
5. FastEndpoints rotaları host edilir ve Scalar/OpenAPI dokümantasyonu geliştirme ortamında yayınlanır.

## Teknoloji Yığını

- .NET 10 / ASP.NET Core Web API
- FastEndpoints
- Entity Framework Core 10
- Npgsql PostgreSQL provider
- PostgreSQL 18
- JWT Bearer Authentication
- Google Identity doğrulama altyapısı
- Scalar API Reference
- OpenAPI / Swagger uyumlu uçlar

## Proje Yapısı

```text
harc-api/
├─ Program.cs
├─ appsettings.json
├─ docker-compose.yml
├─ harc-api.csproj
├─ harc-api.http
├─ Modules/
│  └─ Identity/
│     ├─ Data/
│     │  ├─ IdentityDbContext.cs
│     │  ├─ Role.cs
│     │  ├─ User.cs
│     │  └─ Migrations/
│     ├─ Features/
│     │  ├─ GetMe/
│     │  └─ LoginWithGoogle/
│     └─ Infrastructure/
│        └─ ClaimsTransformation.cs
└─ Properties/
	 └─ launchSettings.json
```

## Kimlik Modülü Detayları

`IdentityDbContext` varsayılan olarak `identity` şemasını kullanır. Kullanıcı kayıtları `identity.Users` tablosunda tutulur.

Roller ayrı bir tabloda tutulur: `identity.Roles`. Bu tablo `int` auto-increment birincil anahtar kullanır ve başlangıçta seed edilmez; roller şimdilik veritabanına manuel insert edilerek oluşturulur.

`User` entity alanları:

- `Id`: `Guid`
- `Email`: zorunlu, benzersiz
- `FullName`: zorunlu
- `RoleId`: zorunlu, `identity.Roles` tablosuna foreign key
- `AvatarUrl`: opsiyonel
- `CreatedAt`: UTC zaman damgası

`ClaimsTransformation` davranışı:

- Token içinde zaten `role` claim varsa hiçbir ek işlem yapmaz.
- `email` claim'ini alır.
- E-posta veritabanında yoksa yeni kullanıcı oluşturur.
- `IdentitySettings:SystemAdminEmail` ile eşleşirse rolü `Admin`, aksi halde `Employee` yapar.
- Kullanıcıya `role` ve `harc_user_id` claim'lerini ekler.

## Endpoint'ler

### `POST /api/identity/login-google`

Google ile giriş için tasarlanmış giriş noktasıdır.

- Request: `LoginWithGoogleRequest`
- Alan: `googleToken`
- Response: `LoginWithGoogleResponse`

Not: Mevcut implementasyon demo/mock amaçlıdır. Gelen token doğrulanmıyor; örnek bir JWT ve kullanıcı nesnesi döndürülüyor.

### `GET /api/identity/me`

Kimliği doğrulanmış kullanıcıdan claim bilgilerini döndürür.

- `email`
- `role`
- `harc_user_id`

## Yapılandırma

`appsettings.json` içinde şu ayarlar kullanılıyor:

- `ConnectionStrings:DefaultConnection`
- `Authentication:Google:ClientId`

Rol zenginleştirme davranışı için ayrıca şu ayar bekleniyor:

- `IdentitySettings:SystemAdminEmail`

Örnek yapılandırma:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Host=localhost;Port=5433;Database=harc_db;Username=harc_user;Password=harc_password"
	},
	"Authentication": {
		"Google": {
			"ClientId": "your-google-client-id.apps.googleusercontent.com"
		}
	},
	"IdentitySettings": {
		"SystemAdminEmail": "admin@your-company.com"
	}
}
```

## Yerel Çalıştırma

### Gereksinimler

- .NET 10 SDK
- Docker Desktop
- İsteğe bağlı: `dotnet-ef`

### 1. Veritabanını başlatın

```bash
docker compose up -d
```

Bu komut PostgreSQL 18 kapsayıcısını çalıştırır ve varsayılan olarak `localhost:5433` üzerinden erişim açar.

### 2. Migration'ları uygulayın

```bash
dotnet ef database update --context IdentityDbContext
```

İlk kurulumda global EF aracına ihtiyaç duyarsanız:

```bash
dotnet tool install --global dotnet-ef
```

### 3. API'yi çalıştırın

```bash
dotnet run
```

`launchSettings.json` profilinde uygulama şu adreslerde açılır:

- `http://localhost:5100`
- `https://localhost:5101`

Geliştirme ortamında Scalar arayüzü şu adrestedir:

- `http://localhost:5100/scalar/v1`

## API Kullanımı

Repo içinde bulunan `harc-api.http` dosyası hızlı denemeler için kullanılabilir. Uçları doğrudan test etmek isterseniz aşağıdaki örnek isteği kullanabilirsiniz:

```http
POST http://localhost:5100/api/identity/login-google
Content-Type: application/json

{
	"googleToken": "example-token"
}
```

`GET /api/identity/me` isteği için geçerli bir bearer token gerekir.

## Veritabanı Notları

Mevcut migration seti `InitialIdentity` adını taşır ve şu nesneleri oluşturur:

- `identity.Roles` tablosu
- `identity.Users` tablosu
- `RoleId` için foreign key ilişkisi
- `Email` alanı üzerinde unique index

## Önemli Notlar

- Proje şu anda kimlik modülü etrafında şekillenmiştir; ileride yeni modüller aynı klasörleme yaklaşımıyla eklenebilir.
- `login-google` endpoint'i gerçek Google token doğrulaması yapmayan demo implementasyonudur.
- `ClaimsTransformation` gerçek kullanıcı kaydını ilk doğrulanmış istekte oluşturur.
- Uygulama geliştirme ortamında otomatik API dokümantasyonunu Scalar ile sunar.

## Çalışma Prensipleri

- Her modül kendi alanında yaşar; modüller arasında gevşek bağımlılık tercih edilir.
- Yeni özellikler kendi endpoint, request, response ve iş mantığı tipleriyle birlikte ilgili feature klasöründe tutulur.
- Kimlik doğrulama ve claim yönetimi host seviyesinde yapılır; iş mantığı modül tarafında kalır.

## Veritabanı Seed Örnekleri
```sql
Insert into identity."Titles" ("Id", "Name", "DisplayName", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt") 
values 
(1, 'Software_Engineer', '{"en": "Software Engineer", "tr": "Yazılım Mühendisi"}', NOW(), 'sys', NOW(), 'sys', null),
(2, 'Business_Analyst', '{"en": "Business Analyst", "tr": "İş Analisti"}', NOW(), 'sys', NOW(), 'sys', null);

Insert Into identity."Roles" ("Name", "DisplayName", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
Values ('Admin', '{"en": "System Admin", "tr": "Sistem Yöneticisi"}', NOW(), NOW(), 'sys', 'sys'),
       ('Owner', '{"en":"Company Owner", "tr":"Şirket Sahibi"}', NOW(), NOW(), 'sys', 'sys'),
       ('HR', '{"en":"HR Specialist", "tr":"İK Uzmanı"}', NOW(), NOW(), 'sys', 'sys'),
       ('TeamLead', '{"en":"Team Lead", "tr":"Takım Lideri"}', NOW(), NOW(), 'sys', 'sys'),
       ('Employee', '{"en":"Employee", "tr":"Çalışan"}', NOW(), NOW(), 'sys', 'sys');

INSERT INTO identity."Users" ("Id", "Email", "FullName", "RoleId", "TitleId", "TeamId", "ManagerId", "Status", "AvatarUrl", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy")
values 
('54a4ac29-0a50-44e8-bdd0-5bf200cea6cb', 'evrmarda@gmail.com', 'Evrim Kalafat', 1, 1, NULL, NULL, 1, 'https://example.com/avatar.jpg', NOW(), 'sys', NOW(), 'sys');
```