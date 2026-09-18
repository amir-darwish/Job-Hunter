<p align="center">
  <a href="README.md">🇬🇧 English</a> · <a href="README.fr.md">🇫🇷 Français</a>
</p>

# Job Hunter

> A decision-support system for job searching — not a web scraper.

Job Hunter discovers, collects, normalizes, scores, and tracks job opportunities from official ATS (Applicant Tracking System) APIs, helping you focus on the roles that actually match your profile.

Built with **.NET 10 / ASP.NET Core** as a real-world backend project demonstrating production-grade practices.

---

## The Problem

Job hunting is broken:

- **Fragmented sources** — every company uses a different careers portal.
- **Noise** — scrolling through hundreds of irrelevant senior/unrelated postings daily.
- **No memory** — forgetting where you applied, which CV version you used, and what worked.
- **Generic scrapers** — brittle CSS selectors, duplicates everywhere, and zero intelligence.
- **Mass auto-apply bots** — spam applications that tank your response rate.

## The Solution

Job Hunter takes a **quality-over-quantity** approach:

```
Discover → Collect → Normalize → Deduplicate → Filter → Score → Analyze → Review → Tailor → Track → Learn
```

Instead of scraping every website individually, it integrates directly with the **ATS platforms** that power thousands of company career pages — one adapter covers hundreds of companies.

---

## Key Features

| Feature | Description |
|---------|-------------|
| **ATS-First Ingestion** | Direct integrations with Greenhouse, Lever, SmartRecruiters, and SAP SuccessFactors APIs |
| **Company Discovery** | Auto-detects which ATS a company uses from its careers page |
| **Unified Schema** | Every job — regardless of source — is normalized into a consistent domain model |
| **Smart Deduplication** | Prevents the same posting from appearing twice across different sources |
| **Hard Filters** | Eliminates unqualified jobs (wrong seniority, location, language) before any scoring |
| **Deterministic Scoring** | Weighted scoring engine (role match, skills, experience, location, language, contract, freshness) |
| **AI Analysis** | LLM-powered deep analysis — only for top-scoring candidates to minimize cost |
| **CV Tailoring** | Reorder, rephrase, and highlight — never invent. Validation layer enforces honesty |
| **Application Tracker** | Full lifecycle tracking from discovery through offer/rejection |
| **Analytics** | Response rates, interview rates, and conversion metrics broken down by role, source, CV version |
| **Learning Engine** | Adjusts scoring weights based on real application outcomes |

---

## Architecture

Job Hunter follows a **Modular Monolith** architecture with clean separation of concerns:

```
┌─────────────────────────────┐
│        JobHunter.Api        │   REST API / OpenAPI
│        ASP.NET Core         │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│   JobHunter.Application     │   Use Cases / Orchestration
│   Services / Interfaces     │
└──────────────┬──────────────┘
               │
       ┌───────┼───────┐
       │       │       │
       ▼       ▼       ▼
   Discovery Scoring Tracking
       │
       ▼
  ATS Detection
       │
  ┌────┼─────┬──────────┐
  │    │     │          │
  ▼    ▼     ▼          ▼
 GH  Lever  SR    SuccessFactors
  │    │     │          │
  └────┴─────┴──────────┘
       │
       ▼
  Normalization → Deduplication → EF Core → SQLite
```

### Project Structure

```
job-hunter/
│
├── src/
│   ├── JobHunter.Api/                # REST API, OpenAPI, health checks
│   ├── JobHunter.Application/        # Use cases, interfaces, scoring, normalization
│   ├── JobHunter.Domain/             # Entities, value objects, enums, domain rules
│   └── JobHunter.Infrastructure/     # EF Core, ATS clients, external integrations
│
├── tests/
│   ├── JobHunter.UnitTests/
│   └── JobHunter.IntegrationTests/
│
├── docs/
│   ├── ARCHITECTURE.md               # Full architectural blueprint (41 sections)
│   ├── PROJECT_DECISIONS.md           # Architectural Decision Records (D001–D024)
│   └── ROADMAP.md                     # Implementation roadmap (28 phases)
│
└── README.md
```

### Design Principles

- **Domain has zero external dependencies** — no EF Core, no HttpClient, no ATS knowledge
- **Application layer defines contracts** — Infrastructure implements them
- **Source adapters only fetch data** — all business logic stays in .NET
- **Failure isolation** — one ATS going down never blocks the others
- **Provider-agnostic AI** — LLM provider is swappable behind an interface

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# |
| Runtime | .NET 10 LTS |
| Web Framework | ASP.NET Core |
| ORM | Entity Framework Core |
| Database (MVP) | SQLite |
| Database (Production) | PostgreSQL |
| HTTP Clients | `IHttpClientFactory` + Typed Clients |
| Resilience | .NET Resilience Policies (timeout, retry, rate-limit handling) |
| Logging | Microsoft.Extensions.Logging / Serilog |
| Testing | xUnit |
| API Docs | OpenAPI |
| Containerization | Docker |
| CI/CD | GitHub Actions |

---

## Processing Pipeline

```
Company Registry
       ↓
  ATS Detection          ← Greenhouse / Lever / SmartRecruiters / SuccessFactors
       ↓
  Job Acquisition        ← Public ATS APIs → JSON-LD → Direct HTTP
       ↓
  Normalization          ← Title, location, seniority, contract, language
       ↓
  Deduplication          ← Canonical ID fingerprinting
       ↓
  Hard Filters           ← Country, seniority, experience, language, exclusions
       ↓
  Deterministic Scoring  ← Weighted 0–100 composite score
       ↓
  AI Analysis            ← Only for top candidates (score ≥ threshold)
       ↓
  Human Review           ← You decide what to apply for
       ↓
  Tailored CV / LM       ← Reorder + rephrase + highlight (never invent)
       ↓
  Application Tracking   ← Full lifecycle: Applied → Interview → Offer/Rejected
       ↓
  Analytics              ← Response rate, interview rate, conversion by segment
       ↓
  Learning Engine        ← Outcome-driven scoring weight adjustments
```

---

## Scoring Engine

Every job receives a deterministic score before any AI involvement:

| Criterion | Weight | What It Measures |
|-----------|--------|-----------------|
| Role Match | 30 | How closely the title matches target roles |
| Skills | 25 | Overlap between required and possessed skills |
| Experience | 15 | Whether the experience range fits |
| Location | 10 | Geographic match and remote compatibility |
| Language | 10 | Required vs. spoken languages |
| Contract | 5 | Full-time, part-time, freelance alignment |
| Freshness | 5 | How recently the job was posted |
| **Total** | **100** | |

---

## Application Lifecycle

```
New → Shortlisted → ReadyToApply → Applied → FollowUp
                                        ↓
                    Replied → PhoneScreen → Interview → TechnicalInterview → FinalInterview
                                                                                    ↓
                                                                              Offer / Rejected
```

Other terminal states: `Withdrawn`, `Ignored`

---

## API Endpoints

```http
GET    /api/jobs                        # List all jobs (filterable, paginated)
GET    /api/jobs/{id}                   # Job details
GET    /api/jobs/top                    # Top-ranked jobs
POST   /api/jobs/{id}/shortlist         # Shortlist a job

GET    /api/companies                   # List companies
POST   /api/companies                   # Add a company
POST   /api/companies/{id}/discover     # Trigger ATS discovery

GET    /api/applications                # List applications
POST   /api/applications                # Create an application
PATCH  /api/applications/{id}/status    # Update application status

GET    /api/analytics/summary           # Search analytics dashboard
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Build & Run

```bash
# Clone the repository
git clone https://github.com/your-username/job-hunter.git
cd job-hunter

# Build
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/JobHunter.Api
```

The API will be available at `https://localhost:5001` with Swagger UI at `/swagger`.

---

## Roadmap

The project is built incrementally across 28 phases. See [ROADMAP.md](docs/ROADMAP.md) for the full plan.

### Current Focus: MVP (Phases 0–14)

- [x] Architecture & decision documentation
- [ ] Project foundation & solution structure
- [ ] Domain entities & contracts
- [ ] EF Core + SQLite persistence
- [ ] Greenhouse integration
- [ ] Lever integration
- [ ] SmartRecruiters integration
- [ ] Normalization layer
- [ ] Deduplication engine
- [ ] Company discovery & ATS detection
- [ ] SAP SuccessFactors integration
- [ ] Master profile configuration
- [ ] Hard filters
- [ ] Deterministic scoring
- [ ] First real job search output

### Post-MVP

- [ ] REST API with OpenAPI
- [ ] Background job discovery
- [ ] Structured logging & resilience
- [ ] Application tracker
- [ ] AI analysis & CV tailoring
- [ ] Analytics & learning engine
- [ ] PostgreSQL migration
- [ ] Docker & CI/CD

---

## Success Metric

The north star is **not** how many jobs are scraped.

```
North Star  =  Interviews / Qualified Applications
```

Secondary metrics: response rate, interview rate, offer rate — segmented by role, location, source, ATS, and CV version.

---

## Design Decisions

All major architectural decisions are formally recorded in [PROJECT_DECISIONS.md](docs/PROJECT_DECISIONS.md). Key highlights:

| Decision | Choice | Rationale |
|----------|--------|-----------|
| D003 | ATS-first architecture | One adapter covers hundreds of companies |
| D005 | .NET 10 as primary stack | Portfolio value for .NET Backend roles |
| D010 | Hard filters before AI | Reduce cost, latency, and errors |
| D013 | Human-in-the-loop | No mass auto-apply — quality over quantity |
| D014 | No fabrication in CVs | Reorder and rephrase only — never invent |
| D017 | Small MVP first | Ship value before expanding scope |
| D019 | Modular Monolith | Clean architecture without microservice overhead |

---

## Documentation

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Complete architectural blueprint — 41 sections covering every layer |
| [PROJECT_DECISIONS.md](docs/PROJECT_DECISIONS.md) | 24 formal Architectural Decision Records |
| [ROADMAP.md](docs/ROADMAP.md) | 28-phase implementation plan with Definition of Done per phase |

---

## License

This project is for personal use and portfolio demonstration purposes.

---

<p align="center">
  <i>Built to find the right job — not just any job.</i>
</p>
