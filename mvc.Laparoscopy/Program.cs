using CmmandService;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using mvc.Laparoscopy.Persistence;
using QueryService;
using QueryService.Interfaces;
using Repositorys;
using Repositorys.Interfaces;
using Serilog;
using Utils.Exception;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Utils.Exception.ExceptionHandlerFilter>();


builder.Services.AddMvc(options =>
{
    options.Filters.AddService<Utils.Exception.ExceptionHandlerFilter>();
});

builder.Services.AddControllersWithViews();

// Authentication (cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "mvcLaparoAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });


string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});


#region Logs y conection String

var efLogPath = builder.Configuration["Paths:EfLogPath"]
                ?? "./logs/eflog.txt";

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseMySql(
            builder.Configuration["ConnectionStrings:DefaultConnection"],
            ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:DefaultConnection"])
        )
        .EnableSensitiveDataLogging()
        .LogTo(
    message =>
    {
        var lines = message.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            File.AppendAllText(
                efLogPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {line}{Environment.NewLine}"
            );
        }
    },
    LogLevel.Information
),
    ServiceLifetime.Scoped
);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion


builder.Services.AddTransient<IProductServiceQuery, ProductServiceQuery>();
builder.Services.AddTransient<IDiscountServiceQuery, DiscountServiceQuery>();

builder.Services.AddScoped<ICategoryServiceQuery, CategoryServiceQuery>();

builder.Services.AddTransient<IProductCommandService, ProductCommandService>();
builder.Services.AddTransient<IGenericRepository, GenericRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();


//builder.Services.AddMvc(option =>
//{
//    option.Filters.Add<ExceptionHandlerFilter>();
//});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(
    cfg => { },
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.Configure<PathsOptions>(
    builder.Configuration.GetSection("Paths")
);

var imagesPath = builder.Configuration["Paths:imagesPath"];
var logs = builder.Configuration["Paths:logs"];


builder.Services.Configure<EmailSettingsOptions>(
    builder.Configuration.GetSection("Smtp")
);


var app = builder.Build();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#region Configuracion de Rutas para imagenes

var imagesPathConfig = Path.IsPathRooted(imagesPath)
    ? imagesPath
    : Path.Combine(app.Environment.ContentRootPath, imagesPath);


var logs_ = Path.IsPathRooted(logs)
    ? logs
    : Path.Combine(app.Environment.ContentRootPath, logs);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


if (!Directory.Exists(imagesPathConfig))
{
    Directory.CreateDirectory(imagesPathConfig);
}

if (!Directory.Exists(logs_))
{
    Directory.CreateDirectory(logs_);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPathConfig),
    RequestPath = "/product-images"
});

if (!Directory.Exists(logs))
{
    Directory.CreateDirectory(logs);
}

#endregion

//Migraciones Pomelo
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Error migracion");
}



app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/home");
    return Task.CompletedTask;
});

app.Run();
