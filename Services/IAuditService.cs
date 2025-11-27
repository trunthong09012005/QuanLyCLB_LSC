using System.Threading.Tasks;

namespace QuanLyCLB_LSC.Services
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string table, string actionType, string key, string content);
    }
}