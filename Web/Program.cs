
using Web.Data;
using Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<WebContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<WebContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
/*builder.Services.AddTransient<ISendGridEmail, SendGridEmail>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));*/
/*builder.Services.AddScoped<IDbConnection>((s) =>
{
    IDbConnection conn = new MySqlConnection(builder.Configuration.GetConnectionString("bestbuy"));
    conn.Open();
    return conn;
});*/
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireLowercase = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
    opt.Lockout.MaxFailedAccessAttempts = 5;
    //opt.SignIn.RequireConfirmedAccount = true;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(s =>
{
    s.Cookie.Name = "NguyenTienTung";// đặt tên cho session
    s.IdleTimeout = new TimeSpan(24, 0, 0);// thiết lập thời gian tồn tại session
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Đây là đường dẫn đến trang đăng nhập
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    // Tạo một tài khoản "Admin" cố định
    CreateAdminAccount(serviceProvider).Wait();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
async Task CreateAdminAccount(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))// kiểm tra xem trong csdl đã có role là admin hay chưa nếu chưa có thì tạo
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("Customer"));
    }
    if (!await roleManager.RoleExistsAsync("Customer"))// kiểm tra xem trong csdl đã có role là customer hay chưa nếu chưa có thì tạo
    {
        await roleManager.CreateAsync(new IdentityRole("Customer"));
    }

    if (await userManager.FindByNameAsync("admin123") == null)
    {
        var adminUser = new AppUser
        {
            UserName = "admin123",
            // Các thông tin khác của người dùng
        };

        var adminUserResult = await userManager.CreateAsync(adminUser, "Admin.123");

        if (adminUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
// làm việc với session: trong session các mục được lưu dưới dạng key,value;Trong Controller, Session được truy cập thông qua giao diện ISession, giao diện này lấy được bằng thuộc tính HttpContext.Session
//Các mục lưu trong session dưới dạng key/value. Bạn có thể đọc và lưu một chuỗi vào session bằng phương thức GetString(key, value) và SetString(key, value). Tuy hai phương thức này là mở rộng của ISession nên cần có using Microsoft.AspNetCore.Http;
// var session = HttpContext.Session;
//Thư viện Newtonsoft.Json giúp làm việc với JSON, ở đây cần nhớ hai chức năng. Chuyển một đối tượng thành chuỗi json và ngược lại phục hồi đối tượng từ chuỗi json
/*
 * string valueString = session.GetString("key");
session.SetString("yourkey", "yourstring");
*/

/*Để chuyển một đối tượng (thuộc tính đối tượng) thành chuỗi json dùng SerializeObject

string jsonstring =  JsonConvert.SerializeObject(ob);
Để chuyển chuỗi json thành đối tượng dùng DeserializeObject<ObjectClas>

Type obj = JsonConvert.DeserializeObject<Type>(jsonstring);*/