CREATE OR REPLACE FUNCTION public.insert_concert(
	bid integer,
	cdate date,
	vid integer)
    RETURNS integer
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

insert into concert (bandid, concertdate, venueid)
values (bid, cdate, vid)
on conflict (bandid, concertdate, venueid) do nothing;
select concertid from concert where bandid = bid and concertdate = cdate and venueid = vid;

$BODY$;
