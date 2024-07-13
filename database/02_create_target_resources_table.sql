start transaction;

-- create table
create table if not exists monitor.target_resources (
    id uuid default gen_random_uuid() primary key,
    display_name text not null,
    description text, 
    created_at timestamp default statement_timestamp() not null,
    updated_at timestamp
);

-- create triggers
create or replace trigger set_updated_at_stamp before insert or update 
on monitor.target_resources 
for each row execute function monitor.set_updated_at_stamp();
	
commit;
