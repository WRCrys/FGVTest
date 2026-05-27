# FGVTest

Projeto de vendas com API em ASP.NET Core, acesso a dados com Dapper/SQL Server e front-end em Next.js.

## Estrutura

```text
src/
  FGVTest.Api/        API ASP.NET Core
  FGVTest.Business/   Regras de negocio e DTOs
  FGVTest.Data/       Repositorios e script SQL
  FGVTest.Test/       Testes unitarios e E2E
  fgvtest-web/        Front-end Next.js
```

## Pre-requisitos

- .NET SDK 10
- Node.js e npm
- Docker, caso queira subir o SQL Server em container
- SQL Server local ou remoto, caso nao use Docker

## Banco de dados

A API espera um SQL Server com o banco `FGVTest`. O script de criacao esta em:

```text
src/FGVTest.Data/Scripts/init.sql
```

### Opcao 1: SQL Server via Docker

Suba um container SQL Server:

```bash
docker run -d \
  --name fgvtest-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=1q2w3e4r@#$" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

Aguarde alguns segundos para o banco inicializar e execute o script:

```bash
docker cp src/FGVTest.Data/Scripts/init.sql fgvtest-sqlserver:/tmp/init.sql
docker exec -it fgvtest-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P '1q2w3e4r@#$' \
  -C \
  -i /tmp/init.sql
```

Se a imagem usada nao tiver `sqlcmd` em `/opt/mssql-tools18/bin/sqlcmd`, tente:

```bash
docker exec -it fgvtest-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P '1q2w3e4r@#$' \
  -i /tmp/init.sql
```

### Opcao 2: SQL Server ja instalado

Execute `src/FGVTest.Data/Scripts/init.sql` no seu SQL Server usando SQL Server Management Studio, Azure Data Studio ou `sqlcmd`.

Exemplo:

```bash
sqlcmd -S localhost -U sa -P '1q2w3e4r@#$' -C -i src/FGVTest.Data/Scripts/init.sql
```

## Configurar a conexao da API

A connection string fica em `src/FGVTest.Api/appsettings.json`, na chave `ConnectionStrings:DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FGVTest;User Id=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True;"
  }
}
```

Para sobrescrever sem editar o arquivo, use variavel de ambiente:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost;Database=FGVTest;User Id=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True;"
```

## Executar a API

Na raiz do repositorio:

```bash
dotnet restore src/FGVTest.sln
dotnet run --project src/FGVTest.Api/FGVTest.Api.csproj
```

Por padrao, a API sobe em:

```text
http://localhost:5243
```

Swagger:

```text
http://localhost:5243/swagger
```

## Executar o front-end

Entre na pasta do front:

```bash
cd src/fgvtest-web
npm install
```

Crie ou ajuste o arquivo `.env.local`:

```bash
NEXT_PUBLIC_API_URL=http://localhost:5243
```

Suba o servidor de desenvolvimento:

```bash
npm run dev
```

O Next.js fica disponivel em:

```text
http://localhost:3000
```

## Executar tudo localmente

Em um terminal, suba o banco e aplique o script SQL.

Em outro terminal, suba a API:

```bash
dotnet run --project src/FGVTest.Api/FGVTest.Api.csproj
```

Em outro terminal, suba o front:

```bash
cd src/fgvtest-web
npm run dev
```

## Docker da API

Existe um `compose.yaml` em `src/` para buildar a API:

```bash
cd src
docker compose up --build
```

Esse compose nao sobe o SQL Server. Se usar a API em container, configure a connection string apontando para um SQL Server acessivel pelo container, por exemplo usando `host.docker.internal` para acessar um SQL Server publicado na maquina host:

```bash
ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=FGVTest;User Id=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True;"
```

## Testes

Na raiz do repositorio:

```bash
dotnet test src/FGVTest.sln
```

Para rodar apenas testes unitarios:

```bash
dotnet test src/FGVTest.sln --filter "FullyQualifiedName~Unit"
```

Para rodar apenas E2E:

```bash
dotnet test src/FGVTest.sln --filter "FullyQualifiedName~E2E"
```

Os testes E2E usam Testcontainers e precisam do Docker em execucao.

## Scripts do front-end

Dentro de `src/fgvtest-web`:

```bash
npm run dev      # servidor de desenvolvimento
npm run build    # build de producao
npm run start    # executa build de producao
npm run lint     # lint
```
