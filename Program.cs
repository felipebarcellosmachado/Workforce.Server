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
using Workforce.Business.Core.HumanResourceManagement.WorkingTime;
using Workforce.Business.Core.FacilityManagement.WorkUnit.Repository;
using Workforce.Services.Core.FacilityManagement.WorkUnit;
// using Workforce.Business.Core.DemandManagement.DemandEstimative.Repository; // Comentado: namespace não existe
// using Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository; // Comentado: namespace não existe
// using Workforce.Business.Core.WorkScheduleManagement.BaseWorkSchedule.Repository; // Comentado: namespace não existe
using Workforce.Business.Core.HumanResourceManagement.PairingManagement.PairingType.Repository;
using Workforce.Business.Core.HumanResourceManagement.PairingManagement.Pairing.Repository;
using Workforce.Business.Core.HumanResourceManagement.RiskFactor;
using Workforce.Business.Core.HumanResourceManagement.Availability.Repository;
using Workforce.Business.Core.TourScheduleManagement.BaseTourSchedule.Repository;
using Workforce.Business.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Services.Core.HumanResourceManagement.WorkAgreement;
using Workforce.Services.Core.HumanResourceManagement.JobTitle;
using Workforce.Services.Core.HumanResourceManagement.WorkingTime;
using Workforce.Services.Core.HumanResourceManagement.HumanResource;
using Workforce.Services.Core.HumanResourceManagement.Behaviour;
using Workforce.Services.Core.HumanResourceManagement.Qualification;
using Workforce.Services.Infra.HumanResource.Skill;
using Workforce.Services.Infra.HumanResource.CompetenceLevel;
using Workforce.Services.Core.HumanResourceManagement.RiskFactor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
      .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
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

// Register HttpClient for server-side services with proper base address for prerendering
// Use configuration to get base URL, fallback to localhost for development
var baseUrl = builder.Configuration["BaseUrl"] ?? "https://localhost:6001/";
builder.Services.AddHttpClient("ServerHttpClient", client =>
{
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("ServerHttpClient");
});
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

builder.Services.AddScoped<IWorkAgreementService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new WorkAgreementService(httpClient);
});

builder.Services.AddScoped<IJobTitleService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new JobTitleService(httpClient);
});

builder.Services.AddScoped<IWorkingTimeService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new WorkingTimeService(httpClient);
});

builder.Services.AddScoped<IHumanResourceService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new HumanResourceService(httpClient);
});

builder.Services.AddScoped<IBehaviourService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new BehaviourService(httpClient);
});

builder.Services.AddScoped<IQualificationService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new QualificationService(httpClient);
});

builder.Services.AddScoped<ISkillService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new SkillService(httpClient);
});

builder.Services.AddScoped<ICompetenceLevelService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new CompetenceLevelService(httpClient);
});

builder.Services.AddScoped<IRiskFactorService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new RiskFactorService(httpClient);
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

// Core - DemandManagement - Comentado: namespaces não existem
// builder.Services.AddScoped<DemandEstimativeRepository>();
// builder.Services.AddScoped<BaseDemandEstimativeRepository>();
// builder.Services.AddScoped<Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandRepository>();
// builder.Services.AddScoped<Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandDayRepository>();
// builder.Services.AddScoped<Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandPeriodRepository>();

// Core - WorkScheduleManagement - Comentado: namespace não existe
// builder.Services.AddScoped<BaseWorkScheduleRepository>();

// Core - LeaveManagement
builder.Services.AddScoped<Workforce.Business.Core.LeaveManagement.LeaveType.Repository.LeaveTypeRepository>();
builder.Services.AddScoped<Workforce.Business.Core.LeaveManagement.LeaveRequest.Repository.LeaveRequestRepository>();
builder.Services.AddScoped<Workforce.Business.Core.LeaveManagement.LeaveTake.LeaveTakeRepository>();

// Infra - WorkAgreement
builder.Services.AddScoped<WorkAgreementRepository>();

// Infra - WorkingTime
builder.Services.AddScoped<WorkingTimeRepository>();
builder.Services.AddScoped<JourRepository>();

// Infra - WorkUnit
builder.Services.AddScoped<WorkUnitRepository>();

// Core - PairingManagement
builder.Services.AddScoped<PairingTypeRepository>();
builder.Services.AddScoped<PairingRepository>();

// Core - RiskFactor
builder.Services.AddScoped<RiskFactorRepository>();

// Core - Availability
builder.Services.AddScoped<AvailabilityRepository>();

// Core - TourScheduleManagement
builder.Services.AddScoped<BaseTourScheduleRepository>();
builder.Services.AddScoped<BaseTourScheduleDemandRepository>();
builder.Services.AddScoped<TourScheduleRepository>();

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