create table conference
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
  can_delete_seat boolean not null
)