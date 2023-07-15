using System.Net;
using App.Security.Requirements;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using App.Models;  

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton<IEmailSender,SendMailService>();




builder.Services.AddDbContext<AppDbContext>(options=>{
    var connectionString = builder.Configuration.GetConnectionString("MyBlogContext");
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));
    options.UseMySql(connectionString, serverVersion);//.LogTo(Console.WriteLine, LogLevel.Information);
                // .EnableSensitiveDataLogging()
                // .EnableDetailedErrors();
});
builder.Services.AddIdentity<AppUser,IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();


// builder.Services.AddDefaultIdentity<AppUser>()
//         .AddEntityFrameworkStores<MyBlogContext>()
//         .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});


builder.Services.ConfigureApplicationCookie(options=>{
    options.LoginPath="/login";
    options.LogoutPath="/logout";
    options.AccessDeniedPath ="/khongthetruycap.html";
});


builder.Services.AddAuthentication()
                .AddGoogle(options=>{
                    var ggConfig = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = ggConfig["ClientId"];
                    options.ClientSecret =  ggConfig["ClientSecret"];
                    //Default :SignIn goole
                    options.CallbackPath ="/dang-nhap-tu-google";
                })
                .AddFacebook(options =>{
                    var ggConfig = builder.Configuration.GetSection("Authentication:Facebook");
                    options.ClientId = ggConfig["AppId"];
                    options.ClientSecret =  ggConfig["AppSecret"];
                    //Default :SignIn goole
                    options.CallbackPath ="/dang-nhap-tu-facebook";
                });

builder.Services.AddSingleton<IdentityErrorDescriber,AppIdentityErrorDescriber>();

builder.Services.AddAuthorization(options =>{

    options.AddPolicy("AllowEditRole",policyBuilder =>{
        //dieu kien Policy
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("duocphepxoa","user");
        //policyBuilder.RequireRole("Admin");
       // policyBuilder.RequireRole("Editor");
        // policyBuilder.RequireClaim("Ten claim", new string[]{

        // });
    });
    options.AddPolicy("InGenZ",policyBuilder =>{
        //dieu kien Policy
        policyBuilder.RequireAuthenticatedUser();
        //  policyBuilder.RequireClaim("duocphepxoa","user");
        policyBuilder.Requirements.Add(new GenZRequirement());
    });
    options.AddPolicy("ShowAdminMenu",pb =>{
        pb.RequireRole("Admin");
    });
     options.AddPolicy("CanUpdateArticle",pb =>{
         pb.Requirements.Add(new ArticleUpdateRequirement());
    });
});
builder.Services.AddTransient<IAuthorizationHandler,AppAuthorizationHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


/*

Identity: 
    -Athentication:  xac dinh danh tinh -> login .out
    -Authorzation : xac dinh quyen tru cap
    -Quan ly User:  User,Role

*/