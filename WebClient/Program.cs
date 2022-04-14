using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using SharedKernel.Logger;
using SharedKernel.Services;
using SharedKernel.Services.DownloadService;
using SharedKernel.Services.LoginService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IComputerRepository, ComputerEFRepository>();
builder.Services.AddScoped<IEmployerRepository, EmployerEFRepository>();
builder.Services.AddScoped<IFileRepository, FileEFRepository>();
builder.Services.AddScoped<ISessionRepository, SessionEFRepository>();
builder.Services.AddScoped<IUserRepository, UserEFRepository>();
// а если другому классу надо будет IFileSave DekstopSave?
builder.Services.AddScoped<IFileSave, WebSave>();
builder.Services.AddScoped<IFileDownload, WebDownload>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Logging.AddFile();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//AuthScheduler.Start();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();