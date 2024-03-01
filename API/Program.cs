using System.Text;
using System.Text.Json.Serialization;
using API.Data;
using API.Repositories.Data;
using API.Repositories.Interfaces;
using API.Services;
using API.Services.Interfaces;
using API.Utilities.Handlers;
using API.Utilities.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
       .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add repositories to the container
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IOvertimeRepository, OvertimeRepository>();
builder.Services.AddScoped<IOvertimeRequestRepository, OvertimeRequestRepositoy>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Add services to the container
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOvertimeService, OvertimeService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Add custom middleware to the container
builder.Services.AddTransient<ErrorHandlingMiddleware>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add email handler
builder.Services.AddTransient<IEmailHandler, EmailHandler>(_ =>
    new EmailHandler(builder.Configuration["EmailSettings:SmtpServer"],
                     int.Parse(builder.Configuration["EmailSettings:SmtpPort"]),
                     builder.Configuration["EmailSettings:Username"],
                     builder.Configuration["EmailSettings:Password"],
                     builder.Configuration["EmailSettings:MailFrom"]));

// Add database context
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<OvertimeSystemDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.UseLazyLoadingProxies();
    
});

// Add authentication schema using JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters() {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add token handler to the container
builder.Services.AddScoped<IJwtHandler, JwtHandler>(_ =>
    new JwtHandler(builder.Configuration["Jwt:Key"],
                     builder.Configuration["Jwt:Issuer"],
                     builder.Configuration["Jwt:Audience"],
                     int.Parse(builder.Configuration["Jwt:DurationInMinutes"])
));

builder.Services.AddCors(x =>
{
    x.AddDefaultPolicy(option =>
    {
        //option.WithOrigins("https://brm.metrodataacademy.id", "https://portal.metrodataacademy.id");
        //option.WithHeaders("Content-Type", "Authorization", "Accept");
        //option.WithMethods("GET", "POST", "PUT", "DELETE");
        option.AllowAnyOrigin();
        option.AllowAnyHeader();
        option.AllowAnyMethod();
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
