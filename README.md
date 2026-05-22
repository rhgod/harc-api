# harc-api - Kurumsal İK Portalı Çekirdek API

Bu repository, İK uygulamasının iş mantığını, veritabanı süreçlerini ve dikey dilimlerini (Vertical Slices) barındıran ana .NET 10 Web API uygulamasıdır. **Modüler Monolit** yapıda tasarlanmıştır.

## 🚀 Teknolojiler
- **Framework:** .NET 10 (Minimal APIs)
- **Mimari:** Modular Monolith & Vertical Slice Architecture (CQRS)
- **Veritabanı:** PostgreSQL 18 (Docker üzerinde)
- **ORM:** Entity Framework Core 10 (Şema bazlı modüler Context yapısı)
- **Kütüphaneler:** MediatR (İç iletişim), Carter (Otomatik Endpoint keşfi)
- **Dokümantasyon:** Scalar UI (Modern Swagger alternatifi)

---

## 🛠️ Yerel Kurulum ve Çalıştırma (Geliştiriciler İçin)

Projeyi kendi bilgisayarınızda ayağa kaldırmak için aşağıdaki adımları sırasıyla uygulayın:

### 1. Docker Konteynerini Başlatın
Veritabanının (PostgreSQL 18) arka planda otomatik kurulması ve çalışması için proje kök dizininde terminalden şu komutu çalıştırın:
```bash
docker compose up -d
```

### 2. EF Core Araçlarını Yükleyin (İlk Sefer İçin Opsiyonel)
Eğer bilgisayarınızda dotnet ef komutları çalışmıyorsa global aracı yükleyin/güncelleyin:

```bash
dotnet tool install --global dotnet-ef
# Zaten yüklüyse güncellemek için: dotnet tool update --global dotnet-ef
```

### 3. Veritabanı Tablolarını Oluşturun (Migration)
Mevcut veri şemalarının PostgreSQL içine fiziksel olarak basılması için migration komutunu tetikleyin:

```bash
dotnet ef database update --context IdentityDbContext
```

### 4. Projeyi Çalıştırın
Her şey hazır! API uygulamasını ayağa kaldırmak için:

```bash
dotnet run
---

## 📄 API Dokümantasyonu (Scalar UI)
Uygulama çalıştıktan sonra tarayıcınızdan aşağıdaki adresi ziyaret ederek tüm API uçlarını modern Scalar arayüzü ile test edebilirsiniz:

🔗 **URL:** http://localhost:5100/scalar/v1

---

## 👥 Çalışma Prensipleri

**Modül İzolasyonu:** Her modül kendi klasörü (Modules/ModulAdi) altında yaşar. Modüller birbirlerinin veritabanı tablolarına doğrudan JOIN atamaz, sıkı bağımlılık oluşturamaz.

**Dikey Dilimler:** Her yeni özellik (Feature) kendi Command/Query, Handler ve Endpoint tanımlarını tek bir dosya/klasör içinde barındırır.