start transaction;

-- create table
create table if not exists monitor.target_snapshots (
    id uuid primary key,
    target_id uuid references monitor.targets(id),
    value text not null,
    is_expected_value boolean default false not null,
    is_change_detected boolean default false not null,
    created_at timestamp default statement_timestamp() not null,
    updated_at timestamp
);

-- create triggers
create or replace trigger set_updated_at_stamp before insert or update 
on monitor.target_snapshots
for each row execute function monitor.set_updated_at_stamp();
	
commit;
