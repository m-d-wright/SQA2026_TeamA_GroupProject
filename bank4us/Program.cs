using Bank4Us.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//const string IN_MEMORY_DB = "TestDb";

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use in-memory DB for transfers and accounts
//builder.Services.AddDbContext<TransferContext>(options =>
//    options.UseInMemoryDatabase(IN_MEMORY_DB));

//builder.Services.AddDbContext<TransferContext>(options =>
//    options.UseInMemoryDatabase(IN_MEMORY_DB));

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

app.Run();
