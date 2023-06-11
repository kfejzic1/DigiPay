
using MySqlConnector;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.BLL.Services;
using ProcessingServer.DAL;
using ProcessingServer.DAL.Interfaces;
using ProcessingServer.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var MyAllowAllOrigins = "_myAllowAllOrigins";


builder.Services.AddEntityFrameworkMySql().AddDbContext<AppDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
            new MySqlServerVersion(new Version(8, 0, 32)));
});
// Add services to the container.

builder.Services.AddScoped<IAdministrationService, AdminstrationService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserBankAccountService, UserBankAccountService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IVendorBankAccountService, VendorBankAccountService>();
builder.Services.AddScoped<IEInvoicePaymentHistoryService, EInvoicePaymentHistoryService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowAllOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                      });
});




var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors(MyAllowAllOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
