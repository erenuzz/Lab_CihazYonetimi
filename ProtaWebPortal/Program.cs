using BusinessLayer.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using DataAccess.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProtaWebPortal.Models;
using Serilog;
using Serilog.Events;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProtaDbContext>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<ProtaDbContext>().AddErrorDescriber<KayitValidasyon>();

builder.Services.AddTransient<IRezervasyonDal, EfRezervasyonDal>();
builder.Services.AddTransient<RezervasyonManager>();
builder.Services.AddTransient<ICihazDal, EfCihazDal>();
builder.Services.AddTransient<CihazManager>();
builder.Services.AddTransient<IFiyatlandirmaDal, EfFiyatlandirmaDal>();
builder.Services.AddTransient<FiyatlandirmaManager>();
builder.Services.AddTransient<IEgitimDal , EfEgitimDal>();
builder.Services.AddTransient<EgitimManager>();
builder.Services.AddTransient<IMentorDal, EfMentorDal>();
builder.Services.AddTransient<MentorManager>();
builder.Services.AddTransient<IMentorEgitimDal, EfMentorEgitimleriDal>();
builder.Services.AddTransient<MentorEgitimManager>();
builder.Services.AddTransient<IKullaniciveMentorEgitimleriDal, EfKullaniciveMentorEgitimleriDal>();
builder.Services.AddTransient<KullaniciveMentorEgitimleriManager>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSession();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

    // User settings.
    //options.User.AllowedUserNameCharacters =
    //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    //options.User.RequireUniqueEmail = false;
});


//------------------------------------------------//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Giris}/{action=Index}/{id?}");

app.Run();
