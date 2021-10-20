CREATE OR REPLACE FUNCTION public.insert_band(
	bname character varying,
	mbid character varying)
    RETURNS integer
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
insert into band (bandname, musicbrainzid)
values (bname, mbid)
on conflict (musicbrainzid) do nothing;

select bandid from band where musicbrainzid = mbid

$BODY$;

