# Consumer Worker

Aplicação .NET que simula o consumo de materiais de um "broker" fictício e persiste os dados em um banco PostgreSQL utilizando Entity Framework Core.

## Visão geral

O projeto está organizado em camadas seguindo princípios de arquitetura limpa:

- **Domain** – Entidades e contratos base (por exemplo, `Material`, `IUseCase`, `IMaterialRepository`).
- **Application** – Casos de uso orquestrando as operações de negócio (`CreateMaterialUseCase`).
- **Infra/Database** – Implementação de repositórios, `DbContext`, configurações de entidades e registro de dependências.
- **Worker** – Aplicação ASP.NET Core que hospeda um `BackgroundService` para gerar materiais com [Bogus](https://github.com/bchavez/Bogus), expõe health check e injeta as dependências das camadas anteriores.

O serviço `MaterialsConsumer` executa em loop, gera dados falsos de materiais e os envia para o caso de uso de criação. O caso de uso valida o identificador recebido, registra logs e delega a persistência para o repositório.

### Validações e Resiliência

- O `CreateMaterialUseCase` garante que os dados do material sejam válidos (nome obrigatório, preço positivo e estoque não negativo) e evita persistir registros duplicados.
- A infraestrutura de dados utiliza `EnableRetryOnFailure` do provider Npgsql para lidar automaticamente com falhas transitórias do banco.
- O worker reutiliza a instância de `Faker<Material>` para reduzir alocações e pressão de GC durante a geração contínua de materiais.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 15+ (ou compatível) com a extensão `pgcrypto` habilitada (necessária para o `gen_random_uuid()` configurado na entidade `Material`).

## Configuração

1. Ajuste a connection string no arquivo `src/Consumer/Worker/appsettings.Development.json` ou defina a variável de ambiente `ConnectionStrings__DefaultConnection` com os dados do seu banco.
2. Opcionalmente, altere o intervalo de geração dos materiais através de `ConsumersConfigurations:ConsumerDelayMs` (valor padrão: 5000 ms).

Como ainda não existem migrations, certifique-se de criar a tabela `Materials` manualmente ou adicione migrations antes de executar o worker.

## Como executar

```bash
cd src/Consumer
dotnet restore
dotnet build
dotnet run --project Worker
```

A aplicação disponibiliza um health check em `https://localhost:5001/health` por padrão.

## Como parar

O worker respeita sinais de cancelamento (CTRL+C). Ao ocorrer uma exceção não tratada o host é finalizado via `IHostApplicationLifetime.StopApplication()`.

## Estrutura do repositório

```
src/Consumer/
├── Domain/            # Contratos e entidades
├── Application/       # Casos de uso
├── Infra/Database/    # DbContext, repositórios e DI
└── Worker/            # ASP.NET Core Worker e background services
```

## Próximos passos sugeridos

- Adicionar migrations e versionamento de banco via EF Core.
- Incluir testes automatizados (unitários para casos de uso e integração para o repositório).
- Expor observabilidade (métricas, tracing) e logs estruturados.
- Configurar containerização/Docker Compose com PostgreSQL para facilitar a execução local.
- Parametrizar totalmente as configurações via variáveis de ambiente ou Secret Manager.
