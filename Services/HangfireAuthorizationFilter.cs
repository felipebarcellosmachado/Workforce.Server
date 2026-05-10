using Hangfire.Dashboard;

namespace Workforce.Server.Services
{
    /// <summary>
    /// Filtro de autorizaÃ§Ã£o para o Hangfire Dashboard.
    /// ATENÃ‡ÃƒO: Em produÃ§Ã£o, implemente uma autorizaÃ§Ã£o adequada baseada em roles/claims.
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // DESENVOLVIMENTO: Permite acesso para todos
            // TODO: Em produÃ§Ã£o, implemente autorizaÃ§Ã£o adequada
            // Exemplo: return context.GetHttpContext().User.IsInRole("Admin");
            return true;
        }
    }
}
