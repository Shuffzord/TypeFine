using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Domain.Entities;

namespace Domain
{
    public class TypeFineDatabaseInitializer
        : IDatabaseInitializer<TypeFineContext>
    {
        public void InitializeDatabase(TypeFineContext context)
        {
            var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
            context.Database.ExecuteSqlCommand(dbCreationScript);

            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_Phrases_Value ON Phrases ( Value )");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_Keywords_Value ON Keywords ( Value )");
        }
    }
}