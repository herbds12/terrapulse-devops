# TerraPulse AI - API RESTful em ASP.NET Core 8

API desenvolvida para o projeto TerraPulse AI, uma solucao ESG voltada ao monitoramento ambiental, analise de indicadores, geracao de alertas e recomendacoes sustentaveis.

## Tecnologias utilizadas

- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- xUnit
- JWT Bearer Authentication
- AutoMapper
- Swagger / OpenAPI
- Docker
- Docker Compose

## Funcionalidades principais

A API possui endpoints RESTful para:

- Gerenciar areas monitoradas
- Registrar indicadores ambientais
- Gerar alertas ambientais com base nos indicadores
- Gerar recomendacoes sustentaveis a partir de alertas
- Autenticar usuarios com JWT
- Proteger endpoints criticos com autorizacao
- Aplicar paginacao nos endpoints de listagem
- Tratar erros de validacao e excecoes de forma padronizada

## Controllers principais

- `AreasMonitoradasController`
- `IndicadoresAmbientaisController`
- `AlertasAmbientaisController`
- `RecomendacoesSustentaveisController`
- `AuthController`

## Banco de dados

A aplicacao utiliza SQL Server com Entity Framework Core e migrations.

As migrations estao incluidas na pasta:

```text
Migrations
```

No ambiente Docker, as migrations sao aplicadas automaticamente ao iniciar a API.

## Usuario padrao para teste

```text
E-mail: admin@terrapulse.ai
Senha: Terra@123
Perfil: Administrador
```

## Como executar com Docker

Na pasta raiz do projeto, onde esta o arquivo `docker-compose.yml`, execute:

```powershell
docker compose up -d --build
```

Depois acesse o Swagger:

```text
http://localhost:8080/swagger
```

Para parar os containers:

```powershell
docker compose down
```

## Containers criados

O `docker-compose.yml` sobe dois servicos:

```text
terrapulse-api
terrapulse-sqlserver
```

A API fica disponivel em:

```text
http://localhost:8080
```

O SQL Server fica disponivel localmente na porta:

```text
14333
```

## Variaveis de ambiente no Docker

O `docker-compose.yml` configura as variaveis necessarias para execucao da aplicacao:

```text
ASPNETCORE_ENVIRONMENT=Docker
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

Essas variaveis permitem que a API rode em container, conecte ao SQL Server do Docker e gere tokens JWT.

## Como testar autenticacao

1. Acesse o Swagger.
2. Execute o endpoint:

```http
POST /api/auth/login
```

Body:

```json
{
  "email": "admin@terrapulse.ai",
  "senha": "Terra@123"
}
```

3. Copie o valor de `accessToken`.
4. Clique no botao `Authorize`.
5. Cole somente o token no campo Bearer.
6. Execute os endpoints protegidos.

## Endpoints publicos

Os endpoints de consulta sao publicos:

```http
GET /api/areas-monitoradas
GET /api/areas-monitoradas/{id}

GET /api/indicadores-ambientais
GET /api/indicadores-ambientais/{id}

GET /api/alertas-ambientais
GET /api/alertas-ambientais/{id}

GET /api/recomendacoes-sustentaveis
GET /api/recomendacoes-sustentaveis/{id}
```

## Endpoints protegidos

Os endpoints de criacao, atualizacao, geracao e exclusao exigem autenticacao JWT.

Exemplos:

```http
POST /api/areas-monitoradas
PUT /api/areas-monitoradas/{id}
DELETE /api/areas-monitoradas/{id}

POST /api/indicadores-ambientais
DELETE /api/indicadores-ambientais/{id}

POST /api/alertas-ambientais/gerar
PATCH /api/alertas-ambientais/{id}/status

POST /api/recomendacoes-sustentaveis/gerar
PATCH /api/recomendacoes-sustentaveis/{id}/status
```

## Exemplo de fluxo de uso

### 1. Login

```http
POST /api/auth/login
```

Body:

```json
{
  "email": "admin@terrapulse.ai",
  "senha": "Terra@123"
}
```

### 2. Criar area monitorada

```http
POST /api/areas-monitoradas
```

Body:

```json
{
  "nome": "Fazenda Docker TerraPulse",
  "pais": "Brasil",
  "regiao": "Mato Grosso",
  "cultura": "Soja",
  "tipoArea": 1,
  "tamanhoHectares": 1250.5,
  "latitude": -12.345678,
  "longitude": -55.987654
}
```

### 3. Registrar indicador ambiental

```http
POST /api/indicadores-ambientais
```

Body:

```json
{
  "areaMonitoradaId": 1,
  "tipoIndicador": 2,
  "valor": 18.5,
  "unidadeMedida": "%",
  "fonte": "Sensor IoT simulado",
  "observacao": "Umidade do solo abaixo do ideal para a area monitorada."
}
```

### 4. Gerar alertas ambientais

```http
POST /api/alertas-ambientais/gerar
```

Body:

```json
{
  "areaMonitoradaId": 1
}
```

### 5. Gerar recomendacao sustentavel

```http
POST /api/recomendacoes-sustentaveis/gerar
```

Body:

```json
{
  "alertaAmbientalId": 1
}
```

## Paginacao

Os endpoints de listagem possuem paginacao.

Exemplo:

```http
GET /api/areas-monitoradas?pageNumber=1&pageSize=10
```

Resposta esperada:

```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 0,
  "totalPages": 0
}
```

## Tratamento de validacao

A API retorna erro `400 Bad Request` de forma padronizada quando existem campos invalidos.

Exemplo de resposta:

```json
{
  "message": "Existem campos invalidos na requisicao.",
  "statusCode": 400,
  "timestamp": "2026-06-19T00:00:00Z",
  "errors": {
    "Nome": [
      "O nome da area monitorada e obrigatorio."
    ]
  }
}
```

## Tratamento global de excecoes

A aplicacao possui middleware global para tratamento de excecoes, evitando retorno de stack trace diretamente pelos controllers.

Em caso de erro interno, a API retorna uma resposta padronizada com:

```text
message
detail
statusCode
timestamp
```

## Testes automatizados

O projeto possui testes xUnit para validar status code 200 dos controllers principais.

Para executar os testes no Visual Studio:

```text
Test Explorer > Run All Tests
```

Resultado esperado:

```text
5 testes passando
0 falhas
```

Controllers testados:

```text
AuthController
AreasMonitoradasController
IndicadoresAmbientaisController
AlertasAmbientaisController
RecomendacoesSustentaveisController
```

## Collection Postman

A collection do Postman e o environment estao no arquivo separado:

```text
terrapulse-api-postman.zip
```

O environment possui as variaveis:

```text
baseUrl = http://localhost:8080
accessToken = preenchido automaticamente apos login
```

## Arquivos importantes da entrega

O ZIP do codigo-fonte contem:

```text
Dockerfile
docker-compose.yml
.dockerignore
README.md
appsettings.json
Program.cs
Migrations
Controllers
Models
ViewModels
Repositories
Services
Exceptions
TerraPulse.Tests
```

## Observacoes

- O projeto utiliza SQL Server local em container Docker.
- As migrations sao aplicadas automaticamente no ambiente Docker.
- Os endpoints criticos exigem autenticacao JWT.
- A collection Postman foi preparada para usar `baseUrl` e salvar o token automaticamente apos login.