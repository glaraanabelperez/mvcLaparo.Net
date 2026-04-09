using CmmandService;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mvc.Laparoscopy.Persistence;
using QueryService;
using Repositorys;
using Repositorys.Interfaces;

using Utils.Exception;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<ApplicationDbContext>(
//    options => 
//    options.UseSqlServer(
//        builder.Configuration["ConnectionStrings:DefaultConnection"])
//         .EnableSensitiveDataLogging()
//           .LogTo(
//               message => System.Diagnostics.Debug.WriteLine(message),
//               LogLevel.Information
//           ),
//            ServiceLifetime.Scoped

//    );



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

builder.Services.Configure<PathsOptions>(
    builder.Configuration.GetSection("Paths")
);

var tempPath = builder.Configuration["Paths:tempPath"];
 var imagesPath = builder.Configuration["Paths:imagesPath"];


// Add services to the container.
builder.Services.AddTransient<IProductServiceQuery, ProductServiceQuery>();
builder.Services.AddTransient<IDiscountServiceQuery, DiscountServiceQuery>();

builder.Services.AddTransient<IProductCommandService, ProductCommandService>();
builder.Services.AddTransient<IGenericRepository, GenericRepository>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(
    cfg => { }, // configuración
    AppDomain.CurrentDomain.GetAssemblies()
);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMvc(option =>
{
    option.Filters.Add<ExceptionHandlerFilter>();
});


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


var app = builder.Build();

var imagesPath_ = Path.IsPathRooted(imagesPath)
    ? imagesPath
    : Path.Combine(app.Environment.ContentRootPath, imagesPath);

var tempPath_ = Path.IsPathRooted(tempPath)
    ? tempPath
    : Path.Combine(app.Environment.ContentRootPath, tempPath);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization(); 

//app.UseCors(MyAllowSpecificOrigins);
app.UseCors("MyAllowSpecificOrigins");

app.MapControllers();

app.Run();
