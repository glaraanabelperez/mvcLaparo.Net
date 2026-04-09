using CmmandService;
using CmmandService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using mvc.Laparoscopy.Persistence;
using QueryService;
using Repositorys;
using Repositorys.Interfaces;
using Utils.Exception;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient para la API de productos
//builder.Services.AddHttpClient("ProductsApi", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7036/");
//});

//Cors

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

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseMySql(
            builder.Configuration["ConnectionStrings:DefaultConnection"],
            ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:DefaultConnection"])
        )
        .EnableSensitiveDataLogging()
        .LogTo(
            message => System.Diagnostics.Debug.WriteLine(message),
            LogLevel.Information
        ),
    ServiceLifetime.Scoped
);



// Add services to the container.
builder.Services.AddTransient<IProductServiceQuery, ProductServiceQuery>();
builder.Services.AddTransient<IDiscountServiceQuery, DiscountServiceQuery>();

builder.Services.AddTransient<IProductCommandService, ProductCommandService>();
builder.Services.AddTransient<IGenericRepository, GenericRepository>();


builder.Services.AddMvc(option =>
{
    option.Filters.Add<ExceptionHandlerFilter>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(
    cfg => { }, // configuración
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.Configure<PathsOptions>(
    builder.Configuration.GetSection("Paths")
);

var tempPath = builder.Configuration["Paths:tempPath"];
var imagesPath = builder.Configuration["Paths:imagesPath"];

var app = builder.Build();


var imagesPathConfig = Path.IsPathRooted(imagesPath)
    ? imagesPath
    : Path.Combine(app.Environment.ContentRootPath, imagesPath);

var tempPathConfig = Path.IsPathRooted(tempPath)
    ? tempPath
    : Path.Combine(app.Environment.ContentRootPath, tempPath);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles(); // wwwroot


if (!Directory.Exists(imagesPathConfig))
{
    Directory.CreateDirectory(imagesPathConfig);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPathConfig),
    RequestPath = "/product-images"
});




app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
