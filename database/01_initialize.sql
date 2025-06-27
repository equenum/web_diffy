start transaction;

-- create schemas
create schema if not exists monitor;

-- create functions
create or replace function monitor.set_updated_at_stamp() 
    returns trigger 
    language plpgsql 
as $$
    begin
        new.updated_at = statement_timestamp();
        return new;
    end;
$$;

commit;
