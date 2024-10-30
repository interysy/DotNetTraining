using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Contexts;
using System.Data.Common;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Repositories;
using Serilog;
using System;
using LibraryManagementSystemV2.Configuration;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using LibraryManagementSystemV2.Services.GenericServices;
using LibraryManagementSystemV2.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);
var databaseName = "LibraryManagmentSystem";

builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));


builder.Services.AddScoped(typeof(IReadService<,>), typeof(ReadService<,>));
builder.Services.AddScoped(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));


builder.Services.AddScoped(typeof(ITestBookMapping), typeof(TestBookService));
builder.Services.AddScoped(typeof(ITestRentalMapping), typeof(TestRentalService));

builder.Services.AddScoped<IBookAuthorService, BookAuthorsService>(); 
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IRenterService, RenterService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
//builder.Services.AddDbContext<LibraryManagementContext>();
builder.Services.AddDbContext<LibraryManagementContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var configuration = builder.Configuration;


var logger = LoggerSetup.SetUpLogger(configuration);
builder.Host.UseSerilog(logger);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting application ...");
    await app.RunAsync();
    Log.Information("App started ...");
}
catch (Exception exception)
{
    Log.Fatal(exception.ToString());
}
finally
{
    await Log.CloseAndFlushAsync();

}


public partial class Program { }; 