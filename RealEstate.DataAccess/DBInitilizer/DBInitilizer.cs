using RealEstate.DataAccess.Data;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Text;
using RealEstate.Utility;
using Microsoft.AspNetCore.Identity;

namespace RealEstate.DataAccess.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<IdentityRole> _rm;

        public DBInitilizer(
            ApplicationDbContext db,
            IUnitOfWork uow,
            RoleManager<IdentityRole> rm
        )
        {
            _db = db;
            _uow = uow;
            _rm = rm;
        }

        public async Task Initilize()
        {
            // 1. Run any unapplied migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Any()) await _db.Database.MigrateAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Error: {ex.Message}");
            }

            // 2. Create Triggers
            var db = "realestateapi";
            var tables = new string[]
            {
                "dbo.Villas",
                "dbo.VillaNumbers",
                "dbo.LocalUsers"
            };

            var triggers = new List<(string Table, string Trigger)>();

            foreach (var table in tables)
            {
                triggers.Add((table, $@"
                    CREATE OR ALTER TRIGGER {table}_update_updated_date  
                    ON {table}
                    AFTER UPDATE   
                    AS   
                    BEGIN
                        SET NOCOUNT ON;
                        UPDATE {table}
                        SET UpdatedDate = GETDATE()
                        FROM {table} 
                        INNER JOIN INSERTED ON {table}.Id = INSERTED.Id;
                    END;         
                "));
            }

            var transaction = _uow.Transaction();
            await _uow.Villas.ExecuteSqlAsync(@$"USE {db};", []);
            foreach (var (Table, Trigger) in triggers)
            {
                // await _uow.Villas.ExecuteSqlAsync(@$"
                //     IF EXISTS (SELECT name FROM sys.objects  
                //         WHERE name = '{Table}_update_created_date' AND type = 'TR')  
                //         DROP TRIGGER {Table}_update_created_date;  
                // ", []);
                await _uow.Villas.ExecuteSqlAsync(Trigger, []);
            }
            transaction.Commit();

            // 2. Create Roles if the do not already exist
            var roles = new List<string> {
                SD.Role_Customer,
                SD.Role_Admin,
                SD.Role_Employee,
            };

            var rolesToCreate = new List<string>();
            foreach (var role in roles)
            {
                if (!await _rm.RoleExistsAsync(role))
                {
                    rolesToCreate.Add(role);
                }
            }

            foreach (var task in rolesToCreate)
            {
                await _rm.CreateAsync(new IdentityRole(task));
            }

            return;
        }
    }
}