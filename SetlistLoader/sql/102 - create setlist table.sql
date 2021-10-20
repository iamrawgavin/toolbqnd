CREATE TABLE IF NOT EXISTS public.setlist
(
    concertid integer NOT NULL,
    songorder integer NOT NULL,
    songtitle character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT fk_concert FOREIGN KEY (concertid)
        REFERENCES public.concert (concertid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS unq_concert_song
    ON public.setlist USING btree
    (concertid ASC NULLS LAST, songorder ASC NULLS LAST)
    TABLESPACE pg_default;