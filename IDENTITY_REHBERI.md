# ASP.NET Core Identity Yapılandırma ve Kullanım Kılavuzu

Bu doküman, projede ASP.NET Core Identity framework'ünün nasıl yapılandırıldığını ve kullanıldığını adım adım açıklamaktadır.

## 0. Gerekli NuGet Paketleri

Identity ve Entity Framework Core kullanımı için aşağıdaki paketlerin projeye yüklenmesi gerekmektedir:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## 1. Kullanıcı Modelinin Özelleştirilmesi (`AppUser.cs`)

Varsayılan `IdentityUser` sınıfına ek özellikler (Ad, Soyad, Adres, Telefon vb.) eklemek için `AppUser` sınıfı oluşturulmuştur.

```csharp
public class AppUser : IdentityUser
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Adres { get; set; }
    public string Telefon { get; set; }
}
```

## 2. Veritabanı Bağlamının Yapılandırılması (`AppdbContext.cs`)

Identity tablolarının veritabanında oluşturulabilmesi için `DbContext` sınıfının `IdentityDbContext<AppUser>` sınıfından türetilmesi gerekir.

```csharp
public class AppdbContext : IdentityDbContext<AppUser>
{
    public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
    {
    }
}
```

## 3. Servis Kaydı ve Middleware Ayarları (`Program.cs`)

Identity servislerinin uygulamaya dahil edilmesi ve kimlik doğrulama süreçlerinin aktif edilmesi için `Program.cs` dosyasında şu ayarlar yapılmıştır:

### Servis Kaydı:
```csharp
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    // Şifre politikaları
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppdbContext>()
.AddDefaultTokenProviders();
```

### Middleware (Ara Yazılım) Ayarları:
Kimlik doğrulama (`UseAuthentication`) ve yetkilendirme (`UseAuthorization`) middleware'leri mutlaka `UseRouting` ile `UseMapControllerRoute` arasında eklenmelidir.

```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

## 4. Identity Kullanımı (`AccountController.cs`)

Identity işlemlerini (Kayıt, Giriş, Çıkış) gerçekleştirmek için `UserManager` ve `SignInManager` sınıfları Dependency Injection ile kullanılır.

### Kayıt İşlemi (`Register`):
`UserManager<AppUser>.CreateAsync` metodu ile yeni kullanıcı oluşturulur.

```csharp
var user = new AppUser { UserName = model.Email, Email = model.Email, ... };
var sonuc = await _userManager.CreateAsync(user, model.Password);
```

### Giriş İşlemi (`Login`):
`SignInManager<AppUser>.PasswordSignInAsync` metodu ile kullanıcı girişi kontrol edilir.

```csharp
var sonuc = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
```

### Çıkış İşlemi (`Logout`):
`SignInManager<AppUser>.SignOutAsync` metodu ile oturum sonlandırılır.

```csharp
await _signInManager.SignOutAsync();
```

---
*Not: Veritabanı değişikliklerinin yansıması için Migration oluşturulması ve güncellenmesi (`dotnet ef migrations add ...` ve `dotnet ef database update`) unutulmamalıdır.*
