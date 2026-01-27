using Hangfire.Dashboard;

namespace Workforce.Server.Services
{
    /// <summary>
    /// Filtro de autorização para o Hangfire Dashboard.
    /// ATENÇÃO: Em produção, implemente uma autorização adequada baseada em roles/claims.
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // DESENVOLVIMENTO: Permite acesso para todos
            // TODO: Em produção, implemente autorização adequada
            // Exemplo: return context.GetHttpContext().User.IsInRole("Admin");
            return true;
        }
    }
}
