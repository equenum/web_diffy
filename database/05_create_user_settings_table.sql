start transaction;

-- create table
create table if not exists monitor.user_settings (
    id uuid primary key,
    value text not null,
    created_at timestamp default statement_timestamp() not null,
    updated_at timestamp
);

-- create triggers
create or replace trigger set_updated_at_stamp before insert or update 
on monitor.user_settings
for each row execute function monitor.set_updated_at_stamp();

commit;