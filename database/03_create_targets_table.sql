start transaction;

-- create table
create table if not exists monitor.targets (
    id uuid primary key,
    resource_id uuid references monitor.target_resources(id),
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
);

-- create triggers
create or replace trigger set_updated_at_stamp before insert or update 
on monitor.targets 
for each row execute function monitor.set_updated_at_stamp();
	
commit;
