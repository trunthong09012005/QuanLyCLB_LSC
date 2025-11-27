using System;
using System.Threading.Tasks;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC.Services
{
    public class AuditService : IAuditService
    {
        private readonly QlClbLscContext _db;
        public AuditService(QlClbLscContext db) { _db = db; }

        public async Task LogAsync(int? userId, string table, string actionType, string key, string content)
        {
            try
            {
                var entry = new LichSuThaoTac
                {
                    MaTv = userId,
                    TenBang = table,
                    LoaiThaoTac = actionType,
                    KhoaChinh = key,
                    NoiDung = content,
                    NgayThucHien = DateTime.Now
                };
                _db.LichSuThaoTacs.Add(entry);
                await _db.SaveChangesAsync();
            }
            catch
            {
                // swallow exceptions to avoid breaking main flow; consider logging
            }
        }
    }
}