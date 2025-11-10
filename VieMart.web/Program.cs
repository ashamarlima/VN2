using Microsoft.EntityFrameworkCore;
using VieMart.web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services
    .AddFluentValidationAutoValidation()          // populates ModelState from validators
    .AddFluentValidationClientsideAdapters();     // enable client-side messages

builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
builder.Services.AddControllersWithViews();

// NEW: also add plain controllers (for [ApiController] ones)
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
 
app.MapControllers();
 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
