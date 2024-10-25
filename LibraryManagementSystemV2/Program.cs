using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Contexts;
using System.Data.Common;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Repositories;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var databaseName = "LibraryManagmentSystem";
// Add services to the container.  

builder.Services.AddAutoMapper(typeof(MappingProfile));

//// Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

//// Generic Services
builder.Services.AddScoped(typeof(IReadService<,>), typeof(ReadService<,>));
builder.Services.AddScoped(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));


//////////////////////////////////// Services ////////////////////////////////////

// Asset Mappings
builder.Services.AddScoped(typeof(ITestBookMapping), typeof(TestBookService));

builder.Services.AddScoped<IBookAuthorService, BookAuthorsService>(); 
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IRenterService, RenterService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();


builder.Services.AddControllers();
builder.Services.AddDbContext<LibraryManagementContext>();
builder.Services.AddDbContext<SQLiteContext>();
//builder.Services.AddDbContext<BookContext>();
//builder.Services.AddDbContext<RenterContext>();
//builder.Services.AddDbContext<LibraryContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 
//builder.Services.AddDbContext<BookContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
//builder.Services.AddDbContext<EntityContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
//builder.Services.AddDbContext<AuthorContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
//builder.Services.AddDbContext<RenterContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
//builder.Services.AddDbContext<RentalContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
//builder.Services.AddDbContext<AuthorBookContext>(opt =>
//    opt.UseInMemoryDatabase(databaseName));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting application ..."); 
    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception.ToString());
}
finally
{
    await Log.CloseAndFlushAsync();

}
