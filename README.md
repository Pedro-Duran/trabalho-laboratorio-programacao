# Agentic Context Engine

Plataforma Web de Memória e Gestão de Agentes

**Disciplina:** Laboratório de Programação — 2ª Unidade
**Curso:** Sistemas de Informação
**Instituição:** Universidade Tiradentes (UNIT)

## Sobre o Projeto

O Agentic Context Engine é uma aplicação web desenvolvida em C# ASP.NET Core MVC que funciona como o "cérebro estrutural" para a memória de agentes digitais.

O sistema armazena e orquestra o contexto de conversas entre usuários e agentes digitais. Se um usuário conversar com o "Agente de Vendas" no Site A, o sistema salva o histórico. Quando outra pessoa acessar o mesmo agente em outro site, o histórico anterior é carregado automaticamente.

## Tecnologias Utilizadas

- **C# / ASP.NET Core MVC** — Framework web principal
- **Entity Framework Core** — ORM para acesso ao banco de dados
- **MySQL 8.0** — Banco de dados relacional, executado via Docker
- **Docker / Docker Compose** — Containerização do banco de dados, garantindo ambiente idêntico em qualquer máquina
- **Chart.js** — Gráficos dinâmicos do Dashboard
- **HTML / CSS / JavaScript** — Interface do usuário

## Conceitos de OO Aplicados

| Conceito | Onde está no código |
|---|---|
| Interfaces | `IAgente`, `IContextoMemoria`, `IEstatistica` em `Models/Models_OO.cs` |
| Herança | `AgenteBase` (classe abstrata) → `AgenteVendas`, `AgenteSuporte`, `AgenteFinanceiro`, `AgenteAgendamento` |
| Polimorfismo | Método `ProcessarMensagem()` sobrescrito de forma diferente em cada subclasse |
| Encapsulamento | Propriedades com getters/setters em todas as entidades |
| Abstração | Classe abstrata `AgenteBase` com método abstrato `ProcessarMensagem()` |
| Factory Pattern | `AgenteFactory.CriarAgente()` decide dinamicamente qual subclasse instanciar, baseado na categoria do agente vindo do banco |

## Estrutura do Projeto

```
AgenticContextEngine/
├── Controllers/
│   ├── AuthController.cs          # Login, logout e acesso como Convidado
│   ├── CategoriasController.cs    # CRUD de Categorias de Agente
│   ├── AgentesController.cs       # CRUD de Agentes
│   ├── UsuariosController.cs      # CRUD de Usuários
│   ├── CanaisController.cs        # CRUD de Canais de Origem
│   ├── SimulacaoController.cs     # Chat/simulação com histórico persistido
│   └── DashboardController.cs     # Dashboard estatístico
├── Data/
│   └── AppDbContext.cs            # Contexto do Entity Framework (10 tabelas)
├── Models/
│   ├── Models_OO.cs               # Entidades + Interfaces + Herança + Polimorfismo
│   ├── ChatViewModel.cs           # ViewModels do chat
│   └── DashboardViewModel.cs      # ViewModel do dashboard
├── Services/
│   ├── IDashboardService.cs       # Interface do serviço de dashboard
│   └── DashboardService.cs        # Lógica de cálculo das estatísticas
├── Views/
│   ├── Auth/                      # Tela de Login
│   ├── Categorias/                # CRUD de Categorias
│   ├── Agentes/                   # CRUD de Agentes
│   ├── Usuarios/                  # CRUD de Usuários
│   ├── Canais/                    # CRUD de Canais
│   ├── Simulacao/                 # Chat/simulação
│   └── Dashboard/                 # Dashboard com gráficos
├── docker-compose.yml             # Configuração do container MySQL
├── .env.example                   # Modelo de variáveis de ambiente (copiar para .env)
├── script_banco_mysql.sql         # Script de criação das tabelas + dados de teste
└── Program.cs                     # Configuração da aplicação e seed inicial
```

## Banco de Dados — 10 Tabelas

| Tabela | Descrição |
|---|---|
| PerfilAcesso | Perfis de acesso ao sistema (Administrador, Convidado) |
| Usuario | Usuários do sistema |
| CategoriaAgente | Categorias dos agentes (Vendas, Suporte, Financeiro, Agendamento) |
| Agente | Agentes digitais |
| CanalOrigem | Sites/canais de origem |
| SessaoAtendimento | Sessões de atendimento |
| Mensagem | Mensagens trocadas, vinculadas a uma sessão |
| ContextoMemoria | Memória contextual associada a agente e sessão |
| LogAuditoria | Log de ações do sistema |
| EstatisticaAcesso | Estatísticas agregadas de uso, usadas no Dashboard |

## Como Instalar e Rodar

### Pré-requisitos

- .NET SDK 10+
- Docker Desktop (para rodar o banco de dados MySQL via container)

### Passo a Passo

**1.** Clone o repositório:
```
git clone https://github.com/Pedro-Duran/trabalho-laboratorio-programacao.git
cd trabalho-laboratorio-programacao
```

**2.** Copie o arquivo de variáveis de ambiente e ajuste se necessário:
```
copy .env.example .env
```

**3.** Suba o banco de dados MySQL via Docker:
```
docker compose up -d
```
Aguarde ficar saudável: `docker ps` deve mostrar o container `agente_mysql` como `(healthy)`.

**4.** Restaure os pacotes e rode a aplicação:
```
dotnet restore
dotnet run
```

**5.** Acesse no navegador:
```
http://localhost:5147/Auth/Login
```

### Credenciais de Teste

| Email | Senha |
|---|---|
| admin@sistema.com | 123456 |

Também é possível acessar como **Convidado**, com permissões somente de visualização (sem criar, editar ou excluir).

### Roteiro de Testes Sugerido

1. Faça login → deve redirecionar para o Dashboard.
2. Navegue pelos CRUDs (Categorias, Agentes, Canais, Usuários) e cadastre algo — confirme que persiste reabrindo a tela.
3. Acesse o Simulador de Chat → selecione um agente e canal, envie mensagens e observe a resposta polimórfica de cada agente (ex: pergunte sobre "preço" para o SellerBot e depois para o AgendaBot — as respostas são diferentes, mesma pergunta).
4. Acesse o Dashboard → confirme que os números e gráficos refletem os dados reais cadastrados e conversados.
5. Saia e entre de novo no mesmo agente/canal → confirme que o histórico da conversa anterior é carregado automaticamente.

Para parar tudo depois:
```
docker compose down
```
(remove o container, mas mantém os dados salvos no volume `mysql_data`)

## Módulos Implementados

- [x] Login e Autenticação (com modo Convidado)
- [x] CRUD de Categorias
- [x] CRUD de Agentes
- [x] CRUD de Usuários
- [x] CRUD de Canais de Origem
- [x] Interface de Chat/Simulação com histórico persistido
- [x] Dashboard Estatístico com gráficos reais

## Agentes Digitais Implementados

| Agente | Categoria | Exemplo de comportamento |
|---|---|---|
| SellerBot | Vendas | Responde sobre preços, estoque, prazos de entrega, formas de pagamento |
| SupportBot | Suporte Técnico | Responde sobre erros, senha, lentidão, app |
| FinanceBot | Financeiro | Responde sobre fatura, pagamento, parcelamento, reembolso |
| AgendaBot | Agendamento | Responde sobre agendar, remarcar, cancelar, visita técnica |

## Grupo

| Nome | Responsabilidade |
|---|---|
| Allan Gustavo | Banco de dados (10 tabelas, FKs), estrutura de Orientação a Objetos (Herança, Polimorfismo, Interfaces), Login e autenticação |
| Pedro Duran | Infraestrutura com Docker e Docker Compose, configuração do banco MySQL, Ajustes na autenticação, na exibição de algumas telas e nos DTOs |
| Guilherme | CRUD de Categorias de Agente |
| Gabriel Costa | CRUD de Agentes |
| Lucas | CRUD de Usuários |
| Reinan | CRUD de Canais de Origem |
| Leonardo José | Interface de Chat/Simulação e histórico persistido |
| Nicolas | Dashboard Estatístico e gráficos |

**Data de Apresentação:** 19/06/2026