using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Services;
using AccuStock.Hubs;
using Microsoft.AspNetCore.WebSockets;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddHangfire(config => 
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseDefaultTypeSerializer()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("AccountConnection"));
});
builder.Services.AddHangfireServer();

// Register services
builder.Services.AddSignalR();
builder.Services.AddAuthentication();
builder.Services.AddScoped<BaseService>();
builder.Services.AddScoped<HashedPassword>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBusinessYear, BusinessYearService>();
builder.Services.AddScoped<IBankAccountService, BankAccountsService>();
builder.Services.AddScoped<IChartOfAccount, ChartOfAccountService>();
builder.Services.AddScoped<IOpeningBalanceService, OpeningBalanceService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IGLedger, GLedgerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITrialBalanceService, TrialBalanceService>();
builder.Services.AddScoped<IProfitAndLossService, ProfitAndLossService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IVendor, VendorService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IVendorPaymentService, VendorPaymentService>();
builder.Services.AddScoped<ICustomerPaymentService, CustomerPaymentService>();
builder.Services.AddScoped<IpurchaseReturnService, PurchaseReturnService>();
builder.Services.AddScoped<ISaleReturnService, SaleReturnService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configure DbContext with resilience
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountConnection")));

// Configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
    {
        config.LoginPath = "/Auth/Login";
        config.LogoutPath = "/Auth/Logout";
        config.AccessDeniedPath = "/Auth/AccessDenied";
        config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        config.SlidingExpiration = true;
    });
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");
    
app.MapHub<ChatHub>("/chathub");
app.Run();