using RepositoryLayer.Context;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using AutoMapper;
using BusinessLayer.Validators;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AddressBookContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Sqlserver")));
//Add Service of AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//Add Service of Fluent Validations

builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddressBookValidators>());

builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

