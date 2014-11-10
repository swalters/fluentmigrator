using FluentMigrator.Runner.Generators.DB2;

namespace FluentMigrator.Runner.Processors.DB2
{
    public class DB2ProcessorFactory : MigrationProcessorFactory
    {
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new DB2DbFactory();
            var connection = factory.CreateConnection(connectionString);
            return new DB2Processor(connection, new DB2Generator(), announcer, options, factory);
        }
    }
}