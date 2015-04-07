using DbUp.Engine;

namespace DatabaseMigration
{
    internal class EmptyJournal : IJournal
    {
        public string[] GetExecutedScripts()
        {
            return new string[0];
        }

        public void StoreExecutedScript(SqlScript script)
        {
            //NOOP
        }
    }
}