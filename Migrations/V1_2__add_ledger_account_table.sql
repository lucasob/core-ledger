create table if not exists public.ledger_accounts
(
    id      uuid primary key default public.uuid_generate_v4(),
    balance numeric          default 0.00
);
