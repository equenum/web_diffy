using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace WebPageChangeMonitor.Data;

public static class DbInitializer
{
    public static async Task ExecuteAsync(string connectionString)
    {
        var adminConnectionString = string.Join(string.Empty, connectionString.Split(' ').Take(3));

        using (var dataSource = NpgsqlDataSource.Create(adminConnectionString))
        {
            var dbExistsCommand = dataSource.CreateCommand("select 1 from pg_database where datname = 'web_diffy';");
            using (var reader = await dbExistsCommand.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                {
                    var createDbCommand = dataSource.CreateCommand("create database web_diffy;");
                    await createDbCommand.ExecuteNonQueryAsync();
                }
            }
        }

        using (var dataSource = NpgsqlDataSource.Create(connectionString))
        {
            using (var connection = await dataSource.OpenConnectionAsync())
            {
                // schema + last updated timestamp trigger
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    using (var command = new NpgsqlCommand("create schema if not exists monitor;", connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    using (var command = new NpgsqlCommand(@"
                        create or replace function monitor.set_updated_at_stamp() 
                            returns trigger 
                            language plpgsql 
                        as $$
                            begin
                                new.updated_at = statement_timestamp();
                                return new;
                            end;
                        $$;",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }

                // target resources
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    using (var command = new NpgsqlCommand(@"
                        create table if not exists monitor.target_resources (
                            id uuid primary key,
                            display_name text not null,
                            description text, 
                            created_at timestamp default statement_timestamp() not null,
                            updated_at timestamp
                        );",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    using (var command = new NpgsqlCommand(@"
                        create or replace trigger set_updated_at_stamp before insert or update 
                        on monitor.target_resources 
                        for each row execute function monitor.set_updated_at_stamp();",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }

                // targets
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    using (var command = new NpgsqlCommand(@"
                        create table if not exists monitor.targets (
                            id uuid primary key,
                            resource_id uuid references monitor.target_resources(id) on delete cascade,
                            display_name text not null,
                            description text,
                            url text not null,
                            cron_schedule text not null,
                            change_type text not null,
                            html_tag text not null,
                            selector_type text not null,
                            selector_value text not null,
                            expected_value text,
                            created_at timestamp default statement_timestamp() not null,
                            updated_at timestamp
                        );",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    using (var command = new NpgsqlCommand(@"
                        create or replace trigger set_updated_at_stamp before insert or update 
                        on monitor.targets 
                        for each row execute function monitor.set_updated_at_stamp();",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                } 
                
                // target snapshots
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    using (var command = new NpgsqlCommand(@"
                        create table if not exists monitor.target_snapshots (
                            id uuid primary key,
                            target_id uuid references monitor.targets(id) on delete cascade,
                            value text not null,
                            is_expected_value boolean default false not null,
                            is_change_detected boolean default false not null,
                            outcome text not null,
                            message text null,
                            created_at timestamp default statement_timestamp() not null,
                            updated_at timestamp
                        );",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    using (var command = new NpgsqlCommand(@"
                        create or replace trigger set_updated_at_stamp before insert or update 
                        on monitor.target_snapshots
                        for each row execute function monitor.set_updated_at_stamp();",
                        connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                } 
            }
        }
    }
}
