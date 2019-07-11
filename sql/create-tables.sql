create schema cm;

create table cm.conference
(
  id uuid primary key,
  name text not null,
  description text not null,
  location text not null,
  tagline text,
  slug text not null,
  twitter_search text,
  start_date timestamptz not null,
  end_date timestamptz not null,
  access_code text not null,
  owner_name text not null,
  owner_email text not null,
  was_ever_published boolean not null,
  is_published boolean not null
);

create table cm.seat
(
  conference_id uuid not null,
  id uuid primary key,
  name text not null,
  description text not null,
  quantity integer not null,
  price decimal not null
);

create schema r;

create table r.conference
(
  id uuid primary key,
  name text not null,
  description text not null,
  location text not null,
  tagline text,
  slug text not null,
  twitter_search text,
  start_date timestamptz not null,
  is_published boolean not null
);
