CREATE OR REPLACE FUNCTION public.insert_song(
	cid integer,
	so integer,
	sname character varying)
    RETURNS integer
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

insert into setlist (concertid, songorder, songtitle)
values (cid, so, sname)
on conflict (concertid, songorder) do nothing;
select count(1) from setlist where concertId = cid and songorder = so;
$BODY$;
