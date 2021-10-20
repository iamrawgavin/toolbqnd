CREATE TABLE IF NOT EXISTS public.venue
(
    venueid integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    venuename character varying(255) COLLATE pg_catalog."default" NOT NULL,
    city character varying(255) COLLATE pg_catalog."default",
    statename character varying(255) COLLATE pg_catalog."default",
    statecode character varying(255) COLLATE pg_catalog."default",
    country character varying(255) COLLATE pg_catalog."default",
    countrycode character varying(255) COLLATE pg_catalog."default",
    latitude numeric(12,8),
    longitude numeric(12,8),
    CONSTRAINT venue_pkey PRIMARY KEY (venueid)
)

TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS venue_city
    ON public.venue USING btree
    (venuename COLLATE pg_catalog."default" ASC NULLS LAST, city COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;