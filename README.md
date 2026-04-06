# Shipay - Backend Test Project 

Projeto voltado ao teste de backend da Shipay em .NET. Este projeto implementa uma API robusta com padrões modernos de desenvolvimento, resiliência e validação de dados.

## 🎯 Objetivo

Demonstrar implementação de boas práticas e padrões de desenvolvimento em .NET, incluindo injeção de dependências, resiliência, validação de dados e testes unitários.

## ✨ Features

- **Extensions para Injeção de Dependências** - Facilita a configuração e gerenciamento de dependências da aplicação
- **Validação de CNPJ Alfanumérico** - Validação segura de CNPJ com restrição até junho/2026
- **Resiliência**: Microsoft.Http.Resilience (Retry Pattern, Circuit Breaker Pattern)
- **AutoMapper** - Mapeamento automático e seguro entre objetos de tipos diferentes
-  **Log** - Registros de execução da aplicação usando Serilog.
- **Generic Response** - Padronização de objeto percorrente em toda a aplicação
- **Notification Pattern** - Padronização de mensagens de retorno informativo a UI
- **Http** - Native HttpClient Core
- **Utils** - Diversas ferramentas desenvolvidas para apoiar no desenvolvimento 
- **Testes Unitários** - Cobertura com testes unitários para garantir qualidade do código
- **Normalização de nomes** - Utilização de FormC e FormD

## 🛠️ Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: .NET 8
- **Injeção de Dependências**: Microsoft.Extensions.DependencyInjection
- **Mapeamento**: AutoMapper
- **Resiliência**: Microsoft.Http.Resilience 
- **Testes**:
  - xUnit
  - Bogus
  - Moq
  - AutoFixture
  - Coverlet Collector
-  **Log** - Serilog.
 
## Estrutura do Projeto
```
tl-shipay-project/
├── src/
│   ├── TL.Shipay.Project.API/             # Mostra o sistema para o mundo
│   ├── TL.Shipay.Project.LogListener/     # Aplicação apartada que escreve o arquivo de log em async
│   ├── TL.Shipay.Project.Application/     # Usa as regras do domínio
│   ├── TL.Shipay.Project.Domain/          # Núcleo da aplicação
│   └── TL.Shipay.Project.Infrastructure/  # Conecta o sistema ao mundo real
├── tests/
│   └── Shipay.Tests/                      # Testes unitários
└── README.md
```
- arquivos de logs em TL.Shipay.Project.LogListener\bin\Debug\net8.0\logs

## 🚀 Como Começar

### Pré-requisitos

- .NET 8.0 ou superior
- Visual Studio 2022+ ou Visual Studio Code

### Instalação

- Clone o repositório:

git clone https://github.com/rayanscs/tl-shipay-project.git
