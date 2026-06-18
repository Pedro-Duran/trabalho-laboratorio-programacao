# Agentic Context Engine
### Plataforma Web de Memória e Gestão de Agentes
**Disciplina:** Laboratório de Programação — 2ª Unidade  
**Curso:** Sistemas de Informação  
**Instituição:** Universidade Tiradentes (UNIT)

---

## Sobre o Projeto

O **Agentic Context Engine** é uma aplicação web desenvolvida em **C# ASP.NET Core MVC** que funciona como o "cérebro estrutural" para a memória de agentes digitais.

O sistema armazena e orquestra o contexto de conversas entre usuários e agentes digitais. Se um usuário conversar com o "Agente de Vendas" no Site A, o sistema salva o histórico. Quando outra pessoa acessar o mesmo agente em outro site, o histórico anterior é carregado automaticamente.

---

## Tecnologias Utilizadas

- **C# / ASP.NET Core MVC** — Framework web principal
- **Entity Framework Core** — ORM para acesso ao banco de dados
- **SQL Server** — Banco de dados relacional
- **HTML / CSS** — Interface do usuário

---

## Conceitos de OO Aplicados

| Conceito | Onde está no código |
|---|---|
| **Interfaces** | `IAgente`, `IContextoMemoria`, `IEstatistica` em `Models/Models_OO.cs` |
| **Herança** | `AgenteBase` → `AgenteVendas`, `AgenteSuporte`, `AgenteFinanceiro` |
| **Polimorfismo** | Método `ProcessarMensagem()` sobrescrito em cada subclasse |
| **Encapsulamento** | Propriedades com getters/setters em todas as entidades |
| **Abstração** | Classe abstrata `AgenteBase` com método abstrato |

---

## Estrutura do Projeto

```
AgenticContextEngine/
├── Controllers/          # Controladores MVC
│   ├── AuthController.cs         # Login e autenticação
│   ├── CategoriasController.cs   # CRUD de Categorias
│   ├── AgentesController.cs      # CRUD de Agentes
│   ├── UsuariosController.cs     # CRUD de Usuários
│   └── CanaisController.cs       # CRUD de Canais
├── Data/
│   └── AppDbContext.cs           # Contexto do Entity Framework
├── Models/
│   └── Models_OO.cs              # Entidades + Classes OO (herança, interfaces)
├── Views/
│   ├── Auth/                     # Tela de Login
│   ├── Categorias/               # CRUD de Categorias
│   ├── Agentes/                  # CRUD de Agentes
│   ├── Usuarios/                 # CRUD de Usuários
│   └── Canais/                   # CRUD de Canais
├── banco_agentic.sql             # Script do banco de dados
└── Program.cs                    # Configuração da aplicação
```

---

## Banco de Dados — 10 Tabelas

| Tabela | Descrição |
|---|---|
| `PerfilAcesso` | Perfis de acesso ao sistema |
| `Usuario` | Usuários do sistema |
| `CategoriaAgente` | Categorias dos agentes |
| `Agente` | Agentes digitais |
| `CanalOrigem` | Sites/canais de origem |
| `SessaoAtendimento` | Sessões de atendimento |
| `Mensagem` | Mensagens trocadas |
| `ContextoMemoria` | Memória contextual dos agentes |
| `LogAuditoria` | Log de ações do sistema |
| `EstatisticaAcesso` | Estatísticas de uso |

---

## Como Instalar e Rodar

### Pré-requisitos
- [.NET SDK 10+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)

### Passo a Passo

**1. Clone o repositório:**
```bash
git clone https://github.com/Pedro-Duran/trabalho-laboratorio-programacao.git
cd trabalho-laboratorio-programacao
```
 Suba o banco MySQL :
 
  `docker compose up -d`
  
  Aguarde ficar saudável: `docker ps` deve mostrar agente_mysql como (healthy).

  2. Restaure os pacotes e rode a aplicação:
  dotnet restore
  dotnet run

  3. Acesse no navegador:
  http://localhost:5147/Auth/Login

  4. Credenciais de teste:

  ┌───────────────────┬────────┐
  │       Email       │ Senha  │
  ├───────────────────┼────────┤
  │ admin@sistema.com │ 123456 │
  └───────────────────┴────────┘

  5. Para testar:
  - Faça login → deve redirecionar para /Categorias/Index.
  - Navegue pelos CRUDs (Categorias, Agentes, Canais, Usuários) e cadastre algo — confirme que persiste reabrindo a tela.
  - Acesse Simulação → selecione o agente/canal seed (SellerBot / Site Principal), envie mensagens e veja a resposta polimórfica do agente e o histórico persistido.

  Para parar tudo depois:
  docker compose down      # remove o container (mantém o volume mysql_data)

## Módulos Implementados

- [x] Login e Autenticação
- [x] CRUD de Categorias
- [x] CRUD de Agentes
- [x] CRUD de Usuários
- [x] CRUD de Canais
- [x] Interface de Chat/Simulação
- [ ] Dashboard Estatístico

---

## Grupo

| Nome | Responsabilidade |
|---|---|
| Allan Gustavo | Banco de dados, estrutura OO, CRUDs, Login |
| Pedro Duran | Infraestrutura, Docker |

---

**Data de Apresentação:** 19/06/2026
