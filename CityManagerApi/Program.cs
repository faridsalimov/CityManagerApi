using CityManagerApi.Data;
using CityManagerApi.Data.Abstract;
using CityManagerApi.Data.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://127.0.0.1:5500/").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
             ValidateIssuerSigningKey = true,
             IssuerSigningKey=new SymmetricSecurityKey(key),
             ValidateIssuer = false,
             ValidateAudience = false,
        };
    });

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var connection = builder.Configuration.GetConnectionString("myconn");
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(connection);
});

builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
//for use cors

app.UseCors(x =>
x.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();