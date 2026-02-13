using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Workforce.Realization.Infrastructure.External.Db;
using Radzen;
using Workforce.Server.Components;
using Hangfire;
using Hangfire.PostgreSql;
using Workforce.Server.Services;

// Import services namespaces for server-side registration
using Workforce.Services.Admin.Session;
using Workforce.Services.Infra.Environment;
using Workforce.Services.Infra.Party;
using Workforce.Services.Infra.Profile;
using Workforce.Client.State;
using Workforce.Services.Infra.Role.User;
using WorkUnitService = Workforce.Services.Core.FacilityManagement.WorkUnit.WorkUnitService;

// Import localization
using Microsoft.Extensions.Localization;
using Workforce.Client.Resources;
using Workforce.Client.Services;
using Workforce.Services.Core.FacilityManagement.WorkUnit;
// using Workforce.Realization.Core.DemandManagement.DemandEstimative.Repository; // Comentado: namespace não existe
// using Workforce.Realization.Core.DemandManagement.BaseDemandEstimative.Repository; // Comentado: namespace não existe
// using Workforce.Realization.Core.WorkScheduleManagement.BaseWorkSchedule.Repository; // Comentado: namespace não existe
using Workforce.Services.Core.HumanResourceManagement.WorkAgreement;
using Workforce.Services.Core.HumanResourceManagement.JobTitle;
using Workforce.Services.Core.HumanResourceManagement.WorkingTime;
using Workforce.Services.Core.HumanResourceManagement.HumanResource;
using Workforce.Services.Core.HumanResourceManagement.Behaviour;
using Workforce.Services.Core.HumanResourceManagement.Qualification;
using Workforce.Services.Infra.HumanResource.Skill;
using Workforce.Services.Infra.HumanResource.CompetenceLevel;
using Workforce.Services.Core.HumanResourceManagement.Tag;
using Workforce.Services.Core.HumanResourceManagement.RiskFactor;
using Workforce.Services.Core.TourScheduleManagement.BaseTourSchedule;
using Workforce.Services.Core.TourScheduleManagement.TourSchedule;
using Workforce.Services.Core.TourScheduleManagement.TourScheduleOptimization;
using Workforce.Services.Core.HumanResourceManagement.PairingManagement.PairingType;
using Workforce.Services.Core.HumanResourceManagement.PairingManagement.Pairing;
using Workforce.Services.Core.LeaveManagement.LeaveRequest;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.RiskFactor;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkingTime;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveTake;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.HumanResource.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkAgreement.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.BaseTourSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleOptimization;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.PairingManagement.PairingType.Repository;
using Workforce.Realization.Infrastructure.Persistence.Infra.Party.Organization;
using Workforce.Realization.Infrastructure.Persistence.Infra.Party.Person;
using Workforce.Realization.Infrastructure.Persistence.Infra.PartyRole.Repository;
using Workforce.Realization.Infrastructure.Persistence.Admin.Culture;
using Workforce.Realization.Infrastructure.Persistence.Admin.Session;
using Workforce.Realization.Infrastructure.Persistence.Core.FacilityManagement.Facility;
using Workforce.Realization.Infrastructure.Persistence.Core.FacilityManagement.WorkUnit;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Availability;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Behaviour;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.CompetenceLevel;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Holiday;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.JobTitle;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.PairingManagement.Pairing;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Qualification;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Skill;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveType;
using Workforce.Realization.Infrastructure.Persistence.Infra.Environment;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveRequest;

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

// Configure Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => 
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                                   "Host=localhost;Database=WorkforceDb;Username=postgres;Password=yourpassword")));

// Adicionar o servidor do Hangfire
builder.Services.AddHangfireServer();

// Registrar o serviço de background
builder.Services.AddScoped<TourScheduleOptimizationBackgroundService>();

// Register Export Service
builder.Services.AddScoped<Workforce.Server.Services.IExportService, Workforce.Server.Services.ExportService>();

// Configure QuestPDF license (must be set once at startup)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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

builder.Services.AddScoped<ITagService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new TagService(httpClient);
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

builder.Services.AddScoped<IBaseTourScheduleService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new BaseTourScheduleService(httpClient);
});

builder.Services.AddScoped<ITourScheduleService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new TourScheduleService(httpClient);
});

builder.Services.AddScoped<Workforce.Services.Core.TourScheduleManagement.TourScheduleOptimization.ITourScheduleOptimizationService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new Workforce.Services.Core.TourScheduleManagement.TourScheduleOptimization.TourScheduleOptimizationService(httpClient);
});

builder.Services.AddScoped<IPairingTypeService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new PairingTypeService(httpClient);
});

builder.Services.AddScoped<IPairingService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new PairingService(httpClient);
});

builder.Services.AddScoped<Workforce.Services.Core.LeaveManagement.LeaveRequest.ILeaveRequestService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new Workforce.Services.Core.LeaveManagement.LeaveRequest.LeaveRequestService(httpClient);
});

// State Management (for server-side compatibility)
builder.Services.AddScoped<IAppState, AppState>();

// Register NavigationService for server-side rendering compatibility
builder.Services.AddScoped<NavigationService>(sp =>
{
    var localizer = sp.GetRequiredService<IStringLocalizer<SharedResources>>();
    return new NavigationService(localizer);
});

// Register Repositories from Workforce.Realization

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
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Tag.TagRepository>();

// Core - DemandManagement - Comentado: namespaces não existem
// builder.Services.AddScoped<DemandEstimativeRepository>();
// builder.Services.AddScoped<BaseDemandEstimativeRepository>();
// builder.Services.AddScoped<Workforce.Realização.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandRepository>();
// builder.Services.AddScoped<Workforce.Realização.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandDayRepository>();
// builder.Services.AddScoped<Workforce.Realização.Core.DemandManagement.BaseDemandEstimative.Repository.BaseDemandPeriodRepository>();

// Core - WorkScheduleManagement - Comentado: namespace não existe
// builder.Services.AddScoped<BaseWorkScheduleRepository>();

// Core - LeaveManagement
builder.Services.AddScoped<LeaveTypeRepository>();
builder.Services.AddScoped<LeaveRequestRepository>();
builder.Services.AddScoped<LeaveTakeRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveBalance.LeaveBalanceRepository>();

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

// Core - Class
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Class.ClassRepository>();

// Core - FunctionalUnit
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.FunctionalUnit.FunctionalUnitRepository>();

// Core - Holiday
builder.Services.AddScoped<HolidayRepository>();

// Core - TourScheduleManagement
builder.Services.AddScoped<BaseTourScheduleRepository>();
builder.Services.AddScoped<BaseTourScheduleDemandRepository>();
builder.Services.AddScoped<BaseTourSchedulePeriodRepository>();
builder.Services.AddScoped<TourScheduleRepository>();
builder.Services.AddScoped<TourScheduleOptimizationRepository>();

// Tour Schedule Services
builder.Services.AddScoped<Workforce.Realization.Application.Core.TourScheduleManagement.Service.TourScheduleResourceDiagnosticService>();

// Core - StaffingScheduleManagement
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingScheduleRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingScheduleDemandRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingSchedulePeriodRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingSchedule.Repository.StaffingScheduleRepository>();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "WorkforceTheme";
    options.Duration = TimeSpan.FromDays(365);
});

var app = builder.Build();

// Configure Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

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

app.MapStaticAssets();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(Workforce.Client._Imports).Assembly);

app.Run();