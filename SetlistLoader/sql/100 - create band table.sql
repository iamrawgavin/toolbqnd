CREATE TABLE IF NOT EXISTS public.band
(
    bandid integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    bandname character varying(255) COLLATE pg_catalog."default" NOT NULL,
    musicbrainzid character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT band_pkey PRIMARY KEY (bandid)
)

TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS unq_musicbrainzid
    ON public.band USING btree
    (musicbrainzid COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;