# Welcome

## For EF Migration

```
cd Domain
dotnet ef migrations add <Name>
dotnet ef migrations remove
dotnet ef database update
```

## Generate scripts

```
dotnet ef migrations script <From> <To> --idempotent -o <OutputFile>
```

## Local Debug

```
cd Api
dotnet run
```

## Add nuget package

```
dotnet add package <package name>
```

## Postgres Db Grant
GRANT CONNECT ON DATABASE database_name TO username;
GRANT USAGE ON SCHEMA database_name TO username;

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO backend_api;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO backend_api;

ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT ALL PRIVILEGES ON TABLES TO backend_api;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT ALL PRIVILEGES ON SEQUENCES TO backend_api;