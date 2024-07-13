start transaction;

-- create schemas
create schema if not exists monitor;

-- create functions
create or replace function monitor.set_updated_at_stamp() returns trigger as $set_updated_at_stamp$
	begin
		new.updated_at = statement_timestamp();
		return new;
	end;
$set_updated_at_stamp$ language plpgsql;

commit;
