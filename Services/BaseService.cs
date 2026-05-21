using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public abstract class BaseService
    {
        // 'protected' para que solo los hijos lo usen.
        protected readonly DBSevicellContext _db;

        public BaseService()
        {
            _db = new DBSevicellContext();
        }

        protected async Task SaveAuditAsync(AuditAction action, string table, string objectId, string details, DBSevicellContext? db = null)
        {
            string actionName = "";

            switch (action)
            {
                case AuditAction.Create:
                    actionName = "CREACIÓN";
                    break;
                case AuditAction.Update:
                    actionName = "MODIFICACIÓN";
                    break;
                case AuditAction.Delete:
                    actionName = "ELIMINACIÓN";
                    break;
                default:
                    actionName = action.ToString().ToUpper();
                    break;
            }

            AuditTable log = new AuditTable();
            log.DateCreate = DateTime.Now;
            log.UserId = SessionManager.loggedInUser.Id;
            log.Accion = actionName;
            log.AffectedTable = table;
            log.ObjectId = objectId;
            log.Details = details;

            var dbContext = db ?? _db;
            dbContext.AuditTables.Add(log);

            if (db == null)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }

}
