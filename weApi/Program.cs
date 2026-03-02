using CmmandService;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using mvc.Laparoscopy.Persistence;
using QueryService;
using Repositorys;
using Repositorys.Interfaces;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Utils.Exception;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration["ConnectionStrings:DefaultConnection"])
         .EnableSensitiveDataLogging()
           .LogTo(
               message => System.Diagnostics.Debug.WriteLine(message),
               LogLevel.Information
           ),
            ServiceLifetime.Scoped

    );

// Add services to the container.
builder.Services.AddScoped<IProductServiceQuery, ProductServiceQuery>();
builder.Services.AddScoped<IDiscountServiceQuery, DiscountServiceQuery>();

builder.Services.AddTransient<IGenericRepository, GenericRepository>();
//builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IProductCommandService, ProductCommandService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Add Filters, tu handler Exceeptions with microsoft'libreries
builder.Services.AddMvc(option =>
{
    option.Filters.Add<ExceptionHandlerFilter>();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/auth/login/";  // Ruta de inicio de sesión
        options.AccessDeniedPath = "/auth/login/accessdenied";  // Ruta de acceso denegado
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Tiempo de expiración de la cookie
    });


//Config Logger
var logDB = builder.Configuration["ConnectionStrings:DefaultConnection"];
var sinkOpts = new MSSqlServerSinkOptions();
sinkOpts.TableName = "Log";
var columnOpts = new ColumnOptions();
columnOpts.Store.Remove(StandardColumn.Properties);
columnOpts.Store.Add(StandardColumn.LogEvent);
columnOpts.LogEvent.DataLength = 2048;
columnOpts.TimeStamp.NonClusteredIndex = true;

Log.Logger = new LoggerConfiguration()
    //.WriteTo.File(new CompactJsonFormatter(), "Log.json", rollingInterval: RollingInterval.Day)
    //.WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .WriteTo.MSSqlServer(
            connectionString: logDB,
            sinkOptions: sinkOpts,
            columnOptions: columnOpts
     )
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();




builder.Host.UseSerilog();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularApp", builder =>
//    {
//        builder.WithOrigins("http://localhost:4200")  
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials();
//    });
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


var app = builder.Build();

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
app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();
