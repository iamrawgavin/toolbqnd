CREATE TABLE IF NOT EXISTS public.concert
(
    concertid integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    bandid integer NOT NULL,
    concertdate date NOT NULL,
    venueid integer,
    CONSTRAINT concert_pkey PRIMARY KEY (concertid),
    CONSTRAINT concert_bandid_fkey FOREIGN KEY (bandid)
        REFERENCES public.band (bandid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT concert_venueid_fkey FOREIGN KEY (venueid)
        REFERENCES public.venue (venueid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS unq_concert
    ON public.concert USING btree
    (bandid ASC NULLS LAST, concertdate ASC NULLS LAST, venueid ASC NULLS LAST)
    TABLESPACE pg_default;