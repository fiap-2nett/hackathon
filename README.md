[![hackathon](https://github.com/fiap-2nett/hackathon/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/fiap-2nett/hackathon/actions/workflows/dotnet.yml)

# Hackthon - Health&Med API

A HelpDesk API é uma plataforma de gerenciamento de tickets que visa aprimorar a gestão operacional,
oferecendo maior controle e centralização das informações relacionadas aos tickets. Este sistema permite que
os usuários criem, gerenciem e monitorem o status de tickets, enquanto os administradores têm a capacidade de
coordenar e controlar todo o processo. Os analistas desempenham um papel crucial na resolução dos tickets e na
atualização do seu andamento.

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

A solução do HelpDesk API é composta pelos seguintes projetos:

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

A HelpDesk API utiliza o paradigma de CodeFirst através dos recursos disponibilizados pelo Entity Framework, no entanto para melhor
entendimento da modelagem de dados apresentamos a seguir o MER e suas respectivas definições:

![Modelagem de Dados](doc/assets/img/der.png)

Com base na imagem acima iremos detalhar as tabelas e os dados contidos em cada uma delas:

| Schema | Tabela            | Descrição                                                                                      |
|--------|-------------------|------------------------------------------------------------------------------------------------|
| dbo    | users             | Tabela que contém os dados referentes aos usuários (médicos e pacientes da plataforma.         |
| dbo    | roles             | Tabela que contém os dados referentes aos tipos de perfis de usuário da plataforma.            |
| dbo    | schedules         | Tabela que contém os dados referentes aos horários de atendimento dos médicos.                 |
| dbo    | appointments      | Tabela que contém os dados referentes aos horários de agendamento de consultas.                |
| dbo    | appointmentstatus | Tabela que contém os dados referentes aos status de agendamentos.                              |


