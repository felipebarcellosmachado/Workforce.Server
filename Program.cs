using Microsoft.EntityFrameworkCore;
using Workforce.Realization.Infrastructure.External.Db;
using Hangfire;
using Hangfire.PostgreSql;
using Workforce.Server.Services;

// Import services namespaces for server-side registration
using Workforce.Services.Admin.Session;
using Workforce.Services.Infra.Environment;
using Workforce.Services.Infra.Party;
using Workforce.Services.Infra.Profile;
using Workforce.Services.Infra.Role.User;
using WorkUnitService = Workforce.Services.Core.FacilityManagement.WorkUnit.WorkUnitService;

// Import localization
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
using Workforce.Services.Core.ProjectManagement.Program;
using Workforce.Services.Core.TourScheduleManagement.BaseTourSchedule;
using Workforce.Services.Core.TourScheduleManagement.TourSchedule;
using Workforce.Services.Core.TourScheduleManagement.TourScheduleOptimization;
using Workforce.Services.Core.StaffingScheduleManagement.StaffingScheduleOptimization;
using Workforce.Services.Core.HumanResourceManagement.PairingManagement.PairingType;
using Workforce.Services.Core.HumanResourceManagement.PairingManagement.Pairing;
using Workforce.Services.Core.LeaveManagement.LeaveRequest;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.RiskFactor;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkingTime;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkingTimeType;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveTake;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.HumanResource.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkAgreement.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.BaseTourSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleOptimization;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleOptimization;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectScheduleManagement.ProjectScheduleOptimization;
using Workforce.Services.Core.ProjectScheduleManagement.ProjectScheduleOptimization;
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

builder.AddServiceDefaults();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Register the services required by the antiforgery middleware.
builder.Services.AddAntiforgery();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Configure Database Context - Aspire will provide connection string
builder.Services.AddDbContext<WorkforceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Hangfire - Use Aspire connection string
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => 
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Adicionar o servidor do Hangfire
builder.Services.AddHangfireServer();

// Registrar o serviço de background
builder.Services.AddScoped<TourScheduleOptimizationBackgroundService>();
builder.Services.AddScoped<StaffingScheduleOptimizationBackgroundService>();

// Register Export Service
builder.Services.AddScoped<Workforce.Server.Services.IExportService, Workforce.Server.Services.ExportService>();

// Configure QuestPDF license (must be set once at startup)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
builder.Services.AddScoped<WorkingTimeTypeRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleType.Repository.StaffingScheduleTypeRepository>();

// Infra - WorkUnit
builder.Services.AddScoped<WorkUnitRepository>();

// Core - PairingManagement
builder.Services.AddScoped<PairingTypeRepository>();
builder.Services.AddScoped<PairingRepository>();

// Core - RiskFactor
builder.Services.AddScoped<RiskFactorRepository>();

// Core - ProjectManagement
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Program.Repository.ProgramRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Project.Repository.ProjectRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Activity.Repository.ActivityRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Activity.Repository.ActivityModeRepository>();

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

// Core - StaffingScheduleOptimization
builder.Services.AddScoped<StaffingScheduleOptimizationRepository>();

// Core - ProjectScheduleOptimization
builder.Services.AddScoped<ProjectScheduleOptimizationRepository>();

// Tour Schedule Services
builder.Services.AddScoped<Workforce.Realization.Application.Core.TourScheduleManagement.Service.TourScheduleResourceDiagnosticService>();

// Core - StaffingScheduleManagement
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingScheduleRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingScheduleDemandRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingSchedulePeriodRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository.BaseStaffingScheduleResourceRepository>();
builder.Services.AddScoped<Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingSchedule.Repository.StaffingScheduleRepository>();

var app = builder.Build();

// Garante aplicação das migrações pendentes antes de atender requisições
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    dbContext.Database.Migrate();
}

// Configure Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Limpa todos os jobs do Hangfire a cada inicializacao do programa.
// Evita que jobs orfaos/pendentes de execucoes anteriores sejam reprocessados
// com dados desatualizados ao subir a aplicacao.
ClearHangfireJobs(app.Services.GetRequiredService<ILogger<Program>>());

// Reseta no banco as otimizacoes que ficaram em estados nao-terminais
// (Pending/InProgress) de execucoes anteriores. Como os jobs do Hangfire foram
// removidos acima, esses registros sao orfaos e nunca mais seriam processados;
// marca-os como Failed para que a UI nao os exiba como "Pending" indefinidamente.
await ResetOrphanedOptimizationsAsync(app.Services, app.Services.GetRequiredService<ILogger<Program>>());

var forwardingOptions = new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardingOptions.KnownNetworks.Clear();
forwardingOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardingOptions);
    

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Frontend");
app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapDefaultEndpoints();

app.Run();

// Remove todos os jobs do Hangfire (enfileirados, agendados, em processamento,
// falhos e aguardando) para evitar reprocessamento de execucoes antigas no startup.
static void ClearHangfireJobs(ILogger logger)
{
    try
    {
        var monitoring = JobStorage.Current.GetMonitoringApi();

        var deleted = 0;

        // Enfileirados em cada fila
        foreach (var queue in monitoring.Queues())
        {
            const int batch = 1000;
            var fetched = monitoring.EnqueuedJobs(queue.Name, 0, batch);
            foreach (var job in fetched)
            {
                if (BackgroundJob.Delete(job.Key))
                {
                    deleted++;
                }
            }
        }

        // Agendados
        foreach (var job in monitoring.ScheduledJobs(0, int.MaxValue))
        {
            if (BackgroundJob.Delete(job.Key))
            {
                deleted++;
            }
        }

        // Em processamento
        foreach (var job in monitoring.ProcessingJobs(0, int.MaxValue))
        {
            if (BackgroundJob.Delete(job.Key))
            {
                deleted++;
            }
        }

        // Falhos
        foreach (var job in monitoring.FailedJobs(0, int.MaxValue))
        {
            if (BackgroundJob.Delete(job.Key))
            {
                deleted++;
            }
        }

        logger.LogInformation("Hangfire: {Count} job(s) removido(s) na inicializacao.", deleted);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao limpar os jobs do Hangfire na inicializacao.");
    }
}

// Marca como Failed as otimizacoes que ficaram em estados nao-terminais
// (Pending/InProgress) de execucoes anteriores. Os jobs correspondentes do Hangfire
// ja foram removidos por ClearHangfireJobs, portanto esses registros sao orfaos e
// nunca mais seriam processados. Sem este reset, a UI continuaria exibindo "Pending".
static async Task ResetOrphanedOptimizationsAsync(IServiceProvider services, ILogger logger)
{
    try
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();

        var tourReset = await db.TourScheduleOptimizations
            .Where(o => o.Status == Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimizationStatus.Pending
                     || o.Status == Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimizationStatus.InProgress)
            .ExecuteUpdateAsync(s => s.SetProperty(
                o => o.Status,
                Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimizationStatus.Failed));

        var staffingReset = await db.StaffingScheduleOptimizations
            .Where(o => o.Status == Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimizationStatus.Pending
                     || o.Status == Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimizationStatus.InProgress)
            .ExecuteUpdateAsync(s => s.SetProperty(
                o => o.Status,
                Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimizationStatus.Failed));

        var projectReset = await db.ProjectScheduleOptimizations
            .Where(o => o.Status == Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimizationStatus.Pending
                     || o.Status == Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimizationStatus.InProgress)
            .ExecuteUpdateAsync(s => s.SetProperty(
                o => o.Status,
                Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimizationStatus.Failed));

        logger.LogInformation(
            "Otimizacoes orfas resetadas para Failed na inicializacao. Tour: {Tour}, Staffing: {Staffing}, Project: {Project}.",
            tourReset, staffingReset, projectReset);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao resetar otimizacoes orfas na inicializacao.");
    }
}
