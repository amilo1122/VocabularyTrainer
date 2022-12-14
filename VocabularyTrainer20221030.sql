PGDMP                     	    z            VocabularyTrainer    14.5    14.4 8    .           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            /           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            0           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            1           1262    17196    VocabularyTrainer    DATABASE     p   CREATE DATABASE "VocabularyTrainer" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'Russian_Russia.1251';
 #   DROP DATABASE "VocabularyTrainer";
                postgres    false            ?            1259    17339 
   categories    TABLE     ]   CREATE TABLE public.categories (
    id integer NOT NULL,
    name character varying(255)
);
    DROP TABLE public.categories;
       public         heap    postgres    false            ?            1259    17338    categories_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.categories_id_seq;
       public          postgres    false    216            2           0    0    categories_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.categories_id_seq OWNED BY public.categories.id;
          public          postgres    false    215            ?            1259    17253 	   languages    TABLE     \   CREATE TABLE public.languages (
    id integer NOT NULL,
    name character varying(255)
);
    DROP TABLE public.languages;
       public         heap    postgres    false            ?            1259    17252    languages_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.languages_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.languages_id_seq;
       public          postgres    false    214            3           0    0    languages_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.languages_id_seq OWNED BY public.languages.id;
          public          postgres    false    213            ?            1259    17416 	   personals    TABLE     z   CREATE TABLE public.personals (
    id integer NOT NULL,
    userid bigint,
    wordid integer,
    languageid integer
);
    DROP TABLE public.personals;
       public         heap    postgres    false            ?            1259    17415    personals_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.personals_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.personals_id_seq;
       public          postgres    false    220            4           0    0    personals_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.personals_id_seq OWNED BY public.personals.id;
          public          postgres    false    219            ?            1259    17246    users    TABLE     6   CREATE TABLE public.users (
    id bigint NOT NULL
);
    DROP TABLE public.users;
       public         heap    postgres    false            ?            1259    17245    users_id_seq    SEQUENCE     u   CREATE SEQUENCE public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.users_id_seq;
       public          postgres    false    212            5           0    0    users_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;
          public          postgres    false    211            ?            1259    17383    words    TABLE     ?   CREATE TABLE public.words (
    id integer NOT NULL,
    fromword character varying(255),
    fromlangid integer,
    toword character varying(255),
    tolangid integer,
    wordtypeid integer,
    categoryid integer
);
    DROP TABLE public.words;
       public         heap    postgres    false            ?            1259    17382    words_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.words_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.words_id_seq;
       public          postgres    false    218            6           0    0    words_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.words_id_seq OWNED BY public.words.id;
          public          postgres    false    217            ?            1259    17209 	   wordtypes    TABLE     \   CREATE TABLE public.wordtypes (
    id integer NOT NULL,
    name character varying(255)
);
    DROP TABLE public.wordtypes;
       public         heap    postgres    false            ?            1259    17208    wordtypes_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.wordtypes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.wordtypes_id_seq;
       public          postgres    false    210            7           0    0    wordtypes_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.wordtypes_id_seq OWNED BY public.wordtypes.id;
          public          postgres    false    209            x           2604    17342    categories id    DEFAULT     n   ALTER TABLE ONLY public.categories ALTER COLUMN id SET DEFAULT nextval('public.categories_id_seq'::regclass);
 <   ALTER TABLE public.categories ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    215    216    216            w           2604    17256    languages id    DEFAULT     l   ALTER TABLE ONLY public.languages ALTER COLUMN id SET DEFAULT nextval('public.languages_id_seq'::regclass);
 ;   ALTER TABLE public.languages ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    213    214    214            z           2604    17419    personals id    DEFAULT     l   ALTER TABLE ONLY public.personals ALTER COLUMN id SET DEFAULT nextval('public.personals_id_seq'::regclass);
 ;   ALTER TABLE public.personals ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    220    219    220            v           2604    17249    users id    DEFAULT     d   ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);
 7   ALTER TABLE public.users ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    212    211    212            y           2604    17386    words id    DEFAULT     d   ALTER TABLE ONLY public.words ALTER COLUMN id SET DEFAULT nextval('public.words_id_seq'::regclass);
 7   ALTER TABLE public.words ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    217    218    218            u           2604    17212    wordtypes id    DEFAULT     l   ALTER TABLE ONLY public.wordtypes ALTER COLUMN id SET DEFAULT nextval('public.wordtypes_id_seq'::regclass);
 ;   ALTER TABLE public.wordtypes ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    209    210    210            '          0    17339 
   categories 
   TABLE DATA           .   COPY public.categories (id, name) FROM stdin;
    public          postgres    false    216   t<       %          0    17253 	   languages 
   TABLE DATA           -   COPY public.languages (id, name) FROM stdin;
    public          postgres    false    214   ?<       +          0    17416 	   personals 
   TABLE DATA           C   COPY public.personals (id, userid, wordid, languageid) FROM stdin;
    public          postgres    false    220   ?<       #          0    17246    users 
   TABLE DATA           #   COPY public.users (id) FROM stdin;
    public          postgres    false    212   }=       )          0    17383    words 
   TABLE DATA           c   COPY public.words (id, fromword, fromlangid, toword, tolangid, wordtypeid, categoryid) FROM stdin;
    public          postgres    false    218   ?=       !          0    17209 	   wordtypes 
   TABLE DATA           -   COPY public.wordtypes (id, name) FROM stdin;
    public          postgres    false    210   :?       8           0    0    categories_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.categories_id_seq', 2, true);
          public          postgres    false    215            9           0    0    languages_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.languages_id_seq', 2, true);
          public          postgres    false    213            :           0    0    personals_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.personals_id_seq', 62, true);
          public          postgres    false    219            ;           0    0    users_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.users_id_seq', 1, false);
          public          postgres    false    211            <           0    0    words_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.words_id_seq', 16, true);
          public          postgres    false    217            =           0    0    wordtypes_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.wordtypes_id_seq', 4, true);
          public          postgres    false    209            ?           2606    17344    categories categories_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.categories DROP CONSTRAINT categories_pkey;
       public            postgres    false    216            ?           2606    17260    languages languages_name_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.languages
    ADD CONSTRAINT languages_name_key UNIQUE (name);
 F   ALTER TABLE ONLY public.languages DROP CONSTRAINT languages_name_key;
       public            postgres    false    214            ?           2606    17258    languages languages_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.languages
    ADD CONSTRAINT languages_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.languages DROP CONSTRAINT languages_pkey;
       public            postgres    false    214            ?           2606    17421    personals personals_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.personals
    ADD CONSTRAINT personals_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.personals DROP CONSTRAINT personals_pkey;
       public            postgres    false    220            ?           2606    17251    users users_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    212            ?           2606    17392    words words_fromword_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.words
    ADD CONSTRAINT words_fromword_key UNIQUE (fromword);
 B   ALTER TABLE ONLY public.words DROP CONSTRAINT words_fromword_key;
       public            postgres    false    218            ?           2606    17390    words words_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.words
    ADD CONSTRAINT words_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.words DROP CONSTRAINT words_pkey;
       public            postgres    false    218            ?           2606    17394    words words_toword_key 
   CONSTRAINT     S   ALTER TABLE ONLY public.words
    ADD CONSTRAINT words_toword_key UNIQUE (toword);
 @   ALTER TABLE ONLY public.words DROP CONSTRAINT words_toword_key;
       public            postgres    false    218            |           2606    17216    wordtypes wordtypes_name_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.wordtypes
    ADD CONSTRAINT wordtypes_name_key UNIQUE (name);
 F   ALTER TABLE ONLY public.wordtypes DROP CONSTRAINT wordtypes_name_key;
       public            postgres    false    210            ~           2606    17214    wordtypes wordtypes_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.wordtypes
    ADD CONSTRAINT wordtypes_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.wordtypes DROP CONSTRAINT wordtypes_pkey;
       public            postgres    false    210            ?           2606    17410 !   words fk_categories_id_categoryid    FK CONSTRAINT     ?   ALTER TABLE ONLY public.words
    ADD CONSTRAINT fk_categories_id_categoryid FOREIGN KEY (categoryid) REFERENCES public.categories(id);
 K   ALTER TABLE ONLY public.words DROP CONSTRAINT fk_categories_id_categoryid;
       public          postgres    false    3206    216    218            ?           2606    17395     words fk_languages_id_fromlangid    FK CONSTRAINT     ?   ALTER TABLE ONLY public.words
    ADD CONSTRAINT fk_languages_id_fromlangid FOREIGN KEY (fromlangid) REFERENCES public.languages(id);
 J   ALTER TABLE ONLY public.words DROP CONSTRAINT fk_languages_id_fromlangid;
       public          postgres    false    214    218    3204            ?           2606    17427 $   personals fk_languages_id_languageid    FK CONSTRAINT     ?   ALTER TABLE ONLY public.personals
    ADD CONSTRAINT fk_languages_id_languageid FOREIGN KEY (languageid) REFERENCES public.languages(id);
 N   ALTER TABLE ONLY public.personals DROP CONSTRAINT fk_languages_id_languageid;
       public          postgres    false    214    220    3204            ?           2606    17400    words fk_languages_id_tolangid    FK CONSTRAINT     ?   ALTER TABLE ONLY public.words
    ADD CONSTRAINT fk_languages_id_tolangid FOREIGN KEY (tolangid) REFERENCES public.languages(id);
 H   ALTER TABLE ONLY public.words DROP CONSTRAINT fk_languages_id_tolangid;
       public          postgres    false    214    218    3204            ?           2606    17422    personals fk_users_id_userid    FK CONSTRAINT     z   ALTER TABLE ONLY public.personals
    ADD CONSTRAINT fk_users_id_userid FOREIGN KEY (userid) REFERENCES public.users(id);
 F   ALTER TABLE ONLY public.personals DROP CONSTRAINT fk_users_id_userid;
       public          postgres    false    220    212    3200            ?           2606    17405    words fk_wordtypes_id_orderid    FK CONSTRAINT     ?   ALTER TABLE ONLY public.words
    ADD CONSTRAINT fk_wordtypes_id_orderid FOREIGN KEY (wordtypeid) REFERENCES public.wordtypes(id);
 G   ALTER TABLE ONLY public.words DROP CONSTRAINT fk_wordtypes_id_orderid;
       public          postgres    false    3198    210    218            '   %   x?3??H,??2?t?????S(K-J*?????? ??(      %   !   x?3?t?K??,??2?*-.?L??????? `??      +   ?   x?UйDAQ{f?????
Or???3??~?ψ6'A6A5,?հ	v?!8??6<?(?o	?N+i???ؘ-\?[??[?\G.?Օ ???????0e?d`???-??[ W?@?nIۮ[ ?[ ?[Ҿ???2zX?      #      x??0074?000?????? ??      )   ?  x?]?Mn?0???)r Z5???K7Qb?jj$??-$?]P???J??^a|?>;A??????xL?g?_???N??}????T5/??s?LH?'?XJ(???v?SIGCS?W??D'???u??Q?+?T?ڂ(Q??T???겁??fp=?V????å?'7??X?Z/?eI???T2??6l2=??Ȧ??9?l???GBZh?3?\?llv|??'&y|Ɍ?зMq??????=?c!??ۀ8?$F?9??zuV?X(РT=?q??1?+Y?c??ī?L??0?Kڂ??͂(Q؎??Й?/??A?ƴ??H???pd/???????f?.Jr???b???.?&?ilz3???????&???ܮ???7?K?g?Du?9?????/????YA      !   +   x?3?LL?JM.?,K?2???/??2?,K-J?2ʀ1z\\\ ??Q     