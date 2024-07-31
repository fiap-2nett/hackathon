[![hackathon](https://github.com/fiap-2nett/hackathon/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/fiap-2nett/hackathon/actions/workflows/dotnet.yml)

# Hackthon - Health&Med API

A Health&Med API é uma plataforma que conecta médicos e pacientes possibilitando o agendamento de consultas
de forma rápida e assertiva. A plataforma ainda possibilita aos médicos a customização de horários disponíveis
para consultas.

## Colaboradores

- [Ailton Alves de Araujo](https://www.linkedin.com/in/ailton-araujo-b4ba0520/) - RM350781
- [Bruno Fecchio Salgado](https://www.linkedin.com/in/bfecchio/) - RM350780
- [Cecília Gonçalves Wlinger](https://www.linkedin.com/in/cec%C3%ADlia-wlinger-6a5459100/) - RM351312
- [Cesar Julio Spaziante](https://www.linkedin.com/in/cesar-spaziante/) - RM351311
- [Paulo Felipe do Nascimento de Sousa](https://www.linkedin.com/in/paulo-felipe06/) - RM351707

## Tecnologias utilizadas

- .NET 7.0
- Entity Framework Core 7.0
- Swashbuckle 6.5
- FluentValidation 11.7
- FluentAssertions 6.12
- NetArchTest 1.3
- SqlServer 2019
- Docker 24.0.5
- Docker Compose 2.20

## Arquitetura, Padrões Arquiteturais e Convenções

- REST Api
- Domain-Driven Design
- EF Code-first
- Service Pattern
- Repository Pattern & Unit Of Work
- Architecture Tests
- Unit Tests

## Definições técnicas

A solução da Health&Med API é composta pelos seguintes projetos:

| Projeto                           | Descrição                                                                               |
|-----------------------------------|-----------------------------------------------------------------------------------------|
| _HealthMed.Api_                   | Contém a implementação dos endpoints de comunicação da REST Api.                        |
| _HealthMed.Application_           | Contém a implementação dos contratos de comunicação e classes de serviços.              |
| _HealthMed.Domain_                | Contém a implementação das entidades e interfaces do domínio da aplicação.              |
| _HealthMed.Infrastructure_        | Contém a implementação dos componentes relacionados a infraestrutura da aplicação.      |
| _HealthMed.Persistence_           | Contém a implementação dos componentes relacionados a consulta e persistencia de dados. |
| _HealthMed.Application.UnitTests_ | Contém a implementação dos testes unitários focados nas classes de serviço.             |
| _HealthMed.ArchitectureTests_     | Contém a implementação dos testes de arquitetura.                                       |

## Modelagem de dados

A Health&Med API utiliza o paradigma de CodeFirst através dos recursos disponibilizados pelo Entity Framework, no entanto para melhor
entendimento da modelagem de dados apresentamos a seguir o MER e suas respectivas definições:

![Modelagem de Dados](doc/assets/img/der.png)

Com base na imagem acima iremos detalhar as tabelas e os dados contidos em cada uma delas:

| Schema | Tabela            | Descrição                                                                                      |
|--------|-------------------|------------------------------------------------------------------------------------------------|
| dbo    | users             | Tabela que contém os dados referentes aos usuários (médicos e pacientes da plataforma).         |
| dbo    | roles             | Tabela que contém os dados referentes aos tipos de perfis de usuário da plataforma.            |
| dbo    | schedules         | Tabela que contém os dados referentes aos horários de atendimento dos médicos.                 |
| dbo    | appointments      | Tabela que contém os dados referentes aos horários de agendamento de consultas.                |
| dbo    | appointmentstatus | Tabela que contém os dados referentes aos status de agendamentos.                              |


## Como executar

Toda a infraestrutura necessária para execução da Health&Med API
deve ser provisionada automaticamente configurando o **"docker-compose"**
como projeto de inicialização no Visual Studio.:

![Startup Project](doc/assets/img/startup.png)

Também é possível executar a solução diretamente sem a necessidade do Visual Studio,
para tal, apenas necessitamos do Docker previamente instalado.
Para executar a solução diretamente através do Docker, abra um terminal no diretório
raíz do projeto e execute o seguinte comando:

```sh
$ docker compose up -d
```


## CI/CD Pipeline

A Health&Med API disponibiliza uma CI/CD Pipeline com o objetivo de melhorar e automatizar
processos envolvidos no desenvolvimento da plataforma com foco na integração contínua do código escrito
pelos membros da equipe (CI) e a disponibilização e implantação do software em ambiente de produção (CD),
incluso neste processo a execução de Testes Unitários e Arquiteturais garantindo sempre
a integridade da solução.:


![CI/CD Pipeline](doc/assets/img/pipeline.png)

Além disso, a CI/CD Pipeline é responsável ainda por
realizar a publicação da imagem do Container da solução no Docker Hub.:

[Link para Imagem no Docker Hub](https://hub.docker.com/repository/docker/techchallengephase2/healthmed-api/general)
