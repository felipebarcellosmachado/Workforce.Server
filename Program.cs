using Microsoft.EntityFrameworkCore;
using Radzen;
using Workforce.Business.Admin.Culture.Repository;
using Workforce.Business.Admin.Session.Repository;
using Workforce.Business.Infra.Environment.Repository;
using Workforce.Business.Infra.PartyRole.Repository;
using Workforce.Db.Db;
using Workforce.Server.Components;

// Import services namespaces for server-side registration
using Workforce.Services.Admin.Session;
using Workforce.Services.Infra.Environment;
using Workforce.Services.Infra.Party;
using Workforce.Services.Infra.Profile;
using Workforce.Client.State;
using Workforce.Services.Infra.Role.User;
using WorkUnitService = Workforce.Services.Core.FacilityManagement.WorkUnit.WorkUnitService;
using Workforce.Business.Infra.Party.Organization;
using Workforce.Business.Infra.Party.Person;

// Import localization
using Microsoft.Extensions.Localization;
using Workforce.Client.Resources;
using Workforce.Client.Services;
using Workforce.Business.Core.HumanResourceManagement.Behaviour.Repository;
using Workforce.Business.Core.HumanResourceManagement.CompetenceLevel.Repository;
using Workforce.Business.Core.HumanResourceManagement.JobTitle.Repository;
using Workforce.Business.Core.HumanResourceManagement.Qualification.Repository;
using Workforce.Business.Core.HumanResourceManagement.Skill.Repository;
using Workforce.Business.Core.HumanResourceManagement.HumanResource.Repository;
using Workforce.Business.Core.FacilityManagement.Facility.Repository;
using Workforce.Business.Core.HumanResourceManagement.WorkAgreement.Repository;
using Workforce.Business.Core.HumanResourceManagement.WorkingHour;
using Workforce.Business.Core.FacilityManagement.WorkUnit.Repository;
using Workforce.Services.Core.FacilityManagement.WorkUnit;
using Workforce.Business.Core.DemandManagement.DemandEstimative.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
      .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();

// Configure Database Context
builder.Services.AddDbContext<WorkforceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Host=localhost;Database=WorkforceDb;Username=postgres;Password=yourpassword"));

// Configuração de Localização para Server-side rendering
builder.Services.AddLocalization();
builder.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizer<Workforce.Client.Resources.SharedResources>>(provider =>
{
    return new Workforce.Client.Resources.LocalizedStrings();
});
builder.Services.AddScoped<Workforce.Client.Services.ICultureService, Workforce.Client.Services.CultureService>();

// Register HttpClient for server-side services
builder.Services.AddHttpClient();
// Register Server-side dummy services for static rendering (will be replaced by WebAssembly)
// These are only used during the initial server-side render, not for actual functionality
builder.Services.AddScoped<ISessionService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new SessionService(httpClient);
});

builder.Services.AddScoped<IEnvironmentService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new EnvironmentService(httpClient);
});

builder.Services.AddScoped<IPersonService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new PersonService(httpClient);
});

builder.Services.AddScoped<IOrganizationService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new OrganizationService(httpClient);
});

builder.Services.AddScoped<IProfileService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new ProfileService(httpClient);
});

builder.Services.AddScoped<IUserService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new UserService(httpClient);
});

builder.Services.AddScoped<IWorkUnitService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new WorkUnitService(httpClient);
});

// State Management (for server-side compatibility)
builder.Services.AddScoped<IAppState, AppState>();

// Register Repositories from Workforce.Business

// Admin
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<CultureRepository>();

// Infra - Environment
builder.Services.AddScoped<EnvironmentRepository>();

// Infra - Party
builder.Services.AddScoped<PersonRepository>();
builder.Services.AddScoped<OrganizationRepository>();

// Infra - PartyRole
builder.Services.AddScoped<UserRepository>();

// Infra - Role
builder.Services.AddScoped<FacilityRepository>();

// Infra - HumanResource
builder.Services.AddScoped<HumanResourceRepository>();
builder.Services.AddScoped<BehaviourRepository>();
builder.Services.AddScoped<CompetenceLevelRepository>();
builder.Services.AddScoped<JobTitleRepository>();
builder.Services.AddScoped<SkillRepository>();
builder.Services.AddScoped<QualificationRepository>();

// Core - DemandManagement
builder.Services.AddScoped<DemandEstimativeRepository>();

// Core - LeaveManagement
builder.Services.AddScoped<Workforce.Business.Core.LeaveManagement.LeaveType.Repository.LeaveTypeRepository>();
builder.Services.AddScoped<Workforce.Business.Core.LeaveManagement.LeaveRequest.LeaveRequestRepository>();

// Infra - WorkAgreement
builder.Services.AddScoped<WorkAgreementRepository>();

// Infra - WorkingHour
builder.Services.AddScoped<WorkingHourRepository>();
builder.Services.AddScoped<JourRepository>();

// Infra - WorkUnit
builder.Services.AddScoped<WorkUnitRepository>();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "WorkforceTheme";
    options.Duration = TimeSpan.FromDays(365);
});
var app = builder.Build();


var forwardingOptions = new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardingOptions.KnownNetworks.Clear();
forwardingOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardingOptions);
    

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(Workforce.Client._Imports).Assembly);

app.Run();