# Job Hunter — Architecture

> هذا الملف يصف البنية المعمارية الرسمية لمشروع `job-hunter`.
> التطبيق الرئيسي مبني بـ `.NET 10 / ASP.NET Core`، مع إمكانية إضافة Python Worker لاحقًا فقط للحالات التي تستدعي Scrapling.

---

# 1. Technology Baseline

```text
Language:        C#
Runtime:         .NET 10 LTS
Backend:         ASP.NET Core
ORM:             Entity Framework Core
MVP Database:    SQLite
Later Database:  PostgreSQL
HTTP:            HttpClientFactory
Logging:         Microsoft.Extensions.Logging / Serilog
Testing:         xUnit
API Docs:        OpenAPI
Container:       Docker
CI/CD:           GitHub Actions
```

---

# 2. Architectural Goal

`job-hunter` نظام لدعم قرار البحث عن وظيفة، وليس Web Scraper.

```text
Discover
→ Collect
→ Normalize
→ Deduplicate
→ Filter
→ Score
→ Analyze
→ Review
→ Tailor
→ Track
→ Learn
```

---

# 3. Architecture Style

نبدأ بـ:

```text
Modular Monolith
+
Clean separation of concerns
```

ولا نبدأ بـMicroservices.

السبب:

- أقل تعقيدًا.
- أسرع في التطوير.
- أسهل في الاختبار.
- مناسب لمشروع Portfolio.
- يسمح بالفصل لاحقًا إذا ظهرت حاجة حقيقية.

---

# 4. Solution Structure

```text
job-hunter/
│
├── JobHunter.sln
│
├── src/
│   ├── JobHunter.Api/
│   ├── JobHunter.Application/
│   ├── JobHunter.Domain/
│   └── JobHunter.Infrastructure/
│
├── tests/
│   ├── JobHunter.UnitTests/
│   └── JobHunter.IntegrationTests/
│
├── docs/
│   ├── PROJECT_DECISIONS.md
│   ├── ARCHITECTURE.md
│   └── ROADMAP.md
│
├── docker/
│
├── .github/
│   └── workflows/
│
└── README.md
```

---

# 5. Responsibilities by Project

## JobHunter.Domain

لا يعرف أي شيء عن:

- HTTP
- EF Core
- Greenhouse
- Lever
- OpenAI
- SQLite

يحتوي على:

```text
Entities
Value Objects
Enums
Domain rules
```

أمثلة:

```text
Job
Company
Application
ApplicationEvent
CvVersion
JobScore
```

---

## JobHunter.Application

يحتوي على Use Cases وعقود النظام.

أمثلة:

```text
DiscoverJobs
NormalizeJob
ScoreJob
ShortlistJob
TrackApplication
```

ويحتوي على Interfaces مثل:

```csharp
public interface IJobSource
{
    Task<IReadOnlyCollection<ExternalJob>> GetJobsAsync(
        CompanySource company,
        CancellationToken cancellationToken);
}
```

---

## JobHunter.Infrastructure

يحتوي على التفاصيل الخارجية:

```text
EF Core
SQLite/PostgreSQL
Greenhouse API
Lever API
SmartRecruiters API
SuccessFactors extraction
Logging infrastructure
External AI providers
```

---

## JobHunter.Api

واجهة النظام:

```text
REST API
OpenAPI
Health Checks
Authentication later if needed
```

---

# 6. High-Level Architecture

```text
                 ┌────────────────────┐
                 │   Master Profile   │
                 │   Master CV JSON   │
                 └─────────┬──────────┘
                           │
                           │
              ┌────────────▼────────────┐
              │     JobHunter.Api       │
              │     ASP.NET Core        │
              └────────────┬────────────┘
                           │
                           ▼
              ┌─────────────────────────┐
              │  Application Layer      │
              │  Use Cases / Services   │
              └────────────┬────────────┘
                           │
             ┌─────────────┼─────────────┐
             │             │             │
             ▼             ▼             ▼
       Job Discovery    Scoring       Tracking
             │
             ▼
        ATS Detection
             │
     ┌───────┼────────┬───────────┐
     │       │        │           │
     ▼       ▼        ▼           ▼
Greenhouse Lever SmartRecruiters SuccessFactors
     │       │        │           │
     └───────┴────────┴───────────┘
             │
             ▼
        Normalization
             │
             ▼
        Deduplication
             │
             ▼
         EF Core
             │
             ▼
          SQLite
```

---

# 7. Acquisition Strategy

ترتيب الوصول للبيانات:

```text
1. Public ATS API
2. JSON-LD / Structured Data
3. Direct HTTP
4. Scrapling Python Worker
5. Browser Automation
```

## Important

Scrapling ليس جزءًا من MVP.

إذا احتجناه لاحقًا:

```text
ASP.NET Core
      ↓
Unsupported Source
      ↓
Python Scrapling Worker
      ↓
Normalized Response
      ↓
.NET pipeline
```

---

# 8. Job Sources

الموقع:

```text
src/JobHunter.Infrastructure/Jobs/Sources/
```

البنية:

```text
Sources/
├── Greenhouse/
│   ├── GreenhouseJobSource.cs
│   ├── GreenhouseClient.cs
│   └── Models/
│
├── Lever/
│   ├── LeverJobSource.cs
│   ├── LeverClient.cs
│   └── Models/
│
├── SmartRecruiters/
│   ├── SmartRecruitersJobSource.cs
│   ├── SmartRecruitersClient.cs
│   └── Models/
│
└── SuccessFactors/
    ├── SuccessFactorsJobSource.cs
    ├── SuccessFactorsClient.cs
    └── Models/
```

---

# 9. IJobSource Contract

```csharp
public interface IJobSource
{
    string Name { get; }

    Task<IReadOnlyCollection<ExternalJob>> GetJobsAsync(
        CompanySource company,
        CancellationToken cancellationToken);
}
```

الهدف:

```text
Application Layer
```

لا يعرف هل الوظيفة جاءت من Greenhouse أو Lever أو SuccessFactors.

---

# 10. HttpClientFactory

كل Integration خارجي يستخدم:

```text
IHttpClientFactory
```

ويفضل Typed Clients:

```csharp
public sealed class GreenhouseClient
{
    private readonly HttpClient _httpClient;

    public GreenhouseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}
```

الفائدة:

- Central configuration
- Base address
- Timeout
- Headers
- Testability
- Resilience

---

# 11. Company Discovery

المسؤولية:

```text
Company Website
      ↓
Find Careers URL
      ↓
Detect ATS
      ↓
Create CompanySource
```

مكونات مقترحة:

```text
JobHunter.Application/
└── Discovery/
    ├── DetectAts/
    └── DiscoverCareers/

JobHunter.Infrastructure/
└── Discovery/
    ├── CareersPageFinder.cs
    └── AtsDetector.cs
```

---

# 12. ATS Detection

أمثلة Detection Signals:

```text
boards.greenhouse.io
jobs.lever.co
careers.smartrecruiters.com
jobs.smartrecruiters.com
SuccessFactors URL patterns
```

الناتج:

```csharp
public sealed record CompanySource(
    string CompanyName,
    Uri CareersUrl,
    AtsType Ats,
    string? Identifier);
```

---

# 13. External Job Model

قبل Normalization:

```csharp
public sealed record ExternalJob(
    string SourceJobId,
    string Title,
    string? Location,
    string? Description,
    Uri JobUrl,
    Uri? ApplyUrl,
    DateTimeOffset? DatePosted,
    string Source,
    string? Ats);
```

---

# 14. Domain Job Model

بعد Normalization:

```text
Job
├── Id
├── CanonicalJobId
├── Title
├── NormalizedTitle
├── CompanyId
├── Location
├── City
├── Region
├── Country
├── RemoteType
├── Description
├── EmploymentType
├── Seniority
├── ExperienceMin
├── ExperienceMax
├── DatePosted
├── FirstSeen
├── LastSeen
├── JobUrl
├── ApplyUrl
└── Status
```

---

# 15. Normalization

المكان المقترح:

```text
JobHunter.Application/
└── Jobs/
    └── Normalization/
```

تشمل:

```text
Title normalization
Company normalization
Location normalization
Date normalization
Contract detection
Seniority detection
Experience extraction
Language extraction
```

---

# 16. Deduplication

المرحلة الأولى:

```text
NormalizedCompany
+
NormalizedTitle
+
NormalizedLocation
```

ثم لاحقًا:

```text
ATS identifiers
Description hash
Fuzzy matching
Posting date
```

---

# 17. Persistence

في MVP:

```text
EF Core
+
SQLite
```

الجداول الأولية:

```text
Companies
CompanySources
Jobs
JobSourceReferences
Applications
ApplicationEvents
CvVersions
ScoringResults
```

Infrastructure:

```text
JobHunter.Infrastructure/
└── Persistence/
    ├── JobHunterDbContext.cs
    ├── Configurations/
    ├── Migrations/
    └── Repositories/
```

---

# 18. Hard Filters

قبل AI:

```text
Location
Seniority
Experience
Language
Contract Type
Hard exclusions
```

الهدف:

استبعاد الوظائف غير المناسبة بأقل تكلفة ممكنة.

---

# 19. Deterministic Scoring

وزن مبدئي:

```text
Role Match        30
Skills            25
Experience        15
Location          10
Language          10
Contract           5
Freshness          5
--------------------
Total             100
```

Application Layer:

```text
Scoring/
├── JobScorer.cs
├── RoleScorer.cs
├── SkillScorer.cs
├── ExperienceScorer.cs
├── LocationScorer.cs
└── FreshnessScorer.cs
```

---

# 20. AI Analysis

تطبق فقط على الوظائف الأعلى.

```text
Deterministic Score
        ↓
Threshold passed
        ↓
AI Analysis
```

المهام:

```text
Requirements extraction
Skill gap analysis
Application recommendation
Risk flags
CV tailoring guidance
```

يجب أن يكون Provider قابلًا للاستبدال.

---

# 21. Master Profile

ملف مبدئي:

```text
profiles/master_profile.json
```

يشمل:

```json
{
  "target_roles": [],
  "secondary_roles": [],
  "skills": [],
  "languages": [],
  "education": [],
  "locations": [],
  "remote_preferences": [],
  "contract_types": [],
  "experience_level": [],
  "hard_exclusions": []
}
```

---

# 22. Tailoring

المبدأ:

```text
Reorder
+
Rephrase
+
Highlight
```

وليس:

```text
Invent
```

يجب وجود Validation Layer تمنع إضافة معلومات غير موجودة في Master CV/Profile.

---

# 23. Application Tracking

الحالات:

```text
New
Shortlisted
ReadyToApply
Applied
FollowUp
Replied
PhoneScreen
Interview
TechnicalInterview
FinalInterview
Offer
Rejected
Withdrawn
Ignored
```

---

# 24. Analytics

المقاييس:

```text
Qualified Applications
Responses
Interviews
Offers
Response Rate
Interview Rate
Offer Rate
```

حسب:

```text
Role
Location
Company
Company size
Source
ATS
CV Version
Score range
```

---

# 25. Learning Engine

لاحقًا:

```text
Application history
       ↓
Outcome features
       ↓
Weight adjustments
       ↓
Future Ranking
```

في البداية لا نستخدم ML معقدًا.

قد نبدأ بـ:

```text
rule-based adaptive weights
```

قبل أي Model Training.

---

# 26. Background Jobs

MVP:

```text
BackgroundService
```

مثل:

```csharp
public sealed class JobDiscoveryWorker : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // scheduled discovery
    }
}
```

لاحقًا فقط إذا دعت الحاجة:

```text
Hangfire
Quartz.NET
```

---

# 27. REST API

Endpoints مبدئية:

```text
GET    /api/jobs
GET    /api/jobs/{id}
GET    /api/jobs/top
POST   /api/jobs/{id}/shortlist

GET    /api/companies
POST   /api/companies
POST   /api/companies/{id}/discover

GET    /api/applications
POST   /api/applications
PATCH  /api/applications/{id}/status

GET    /api/analytics/summary
```

---

# 28. Logging

مثال:

```text
INFO  Greenhouse fetched 42 jobs
INFO  Normalization completed: 42
INFO  Deduplication removed: 7
INFO  Hard filters rejected: 21
INFO  Scoring completed: 14
INFO  Top jobs: 5
```

لاحقًا يمكن استخدام Serilog مع Structured Logging.

---

# 29. Error Handling

فشل Source لا يجب أن يوقف الجميع.

```text
Greenhouse        OK
Lever             FAILED
SmartRecruiters   OK
SuccessFactors    OK
```

النظام:

```text
Log Lever error
Continue processing
```

---

# 30. Resilience

يتم التعامل مع:

```text
Timeout
Transient HTTP errors
Rate limiting
Invalid responses
Unavailable ATS
```

مبدئيًا باستخدام capabilities .NET الحديثة وسياسات Resilience.

---

# 31. Testing

## Unit Tests

```text
Normalization
Filters
Scoring
Canonical IDs
ATS detection
```

## Integration Tests

```text
EF Core persistence
HTTP clients
API endpoints
Source adapters
```

Framework:

```text
xUnit
```

---

# 32. Configuration

ملفات مثل:

```text
appsettings.json
appsettings.Development.json
```

تشمل:

```text
Database
Source settings
Scoring weights
Feature flags
External APIs
```

مع عدم وضع Secrets داخل Git.

---

# 33. API Documentation

نستخدم:

```text
OpenAPI
```

حتى يكون المشروع قابلًا للاستعراض بسهولة أمام Recruiter أو Interviewer.

---

# 34. Docker

بعد استقرار MVP:

```text
Dockerfile
docker-compose.yml
```

وعند الانتقال إلى PostgreSQL:

```text
JobHunter.Api
+
PostgreSQL
```

---

# 35. CI/CD

GitHub Actions لاحقًا:

```text
Restore
Build
Test
Publish
```

والهدف أن يظهر المشروع ممارسات إنتاجية حقيقية.

---

# 36. Optional Python Scrapling Worker

ليس ضمن MVP.

مستقبليًا:

```text
workers/
└── scrapling-worker/
    ├── app/
    ├── tests/
    └── requirements.txt
```

يجب ألا يحتوي على Business Logic.

دوره الوحيد:

```text
Fetch unsupported source
→ extract job data
→ return normalized transport payload
```

الـBusiness Rules تبقى في .NET.

---

# 37. MVP Architecture

```text
                   ASP.NET Core
                        │
                        ▼
                 Application Layer
                        │
          ┌─────────────┼─────────────┐
          │             │             │
          ▼             ▼             ▼
    Greenhouse        Lever     SmartRecruiters
          │             │             │
          └─────────────┼─────────────┘
                        │
                        ▼
                 Normalization
                        │
                        ▼
                 Deduplication
                        │
                        ▼
                 EF Core / SQLite
                        │
                        ▼
                  Hard Filters
                        │
                        ▼
                     Score
                        │
                        ▼
                   REST API
```

SuccessFactors يدخل مباشرة بعد إثبات أول 3 Adapters أو معهم إذا كان ضروريًا لسوقنا المستهدف.

---

# 38. Out of Scope for MVP

```text
Mass Auto Apply
Captcha Bypass
Full Browser Agent
Microservices
Kafka
RabbitMQ
Kubernetes
Complex ML
Large Dashboard
Scrapling Worker
```

---

# 39. Definition of Done لأي ATS Adapter

```text
[ ] يجلب الوظائف بنجاح
[ ] يعالج pagination
[ ] يعالج empty results
[ ] يعالج network failures
[ ] يعيد ExternalJob صحيحة
[ ] يستخدم CancellationToken
[ ] لديه Unit/Integration tests
[ ] لا يوقف باقي المصادر عند الفشل
[ ] يسجل Logs واضحة
```

---

# 40. Portfolio Requirements

قبل اعتبار المشروع Portfolio-ready يجب أن يحتوي على:

```text
Clean README
Architecture docs
OpenAPI
Unit tests
Integration tests
Docker
CI pipeline
Structured logging
Meaningful Git history
Sample screenshots/API examples
Real metrics from personal usage
```

---

# 41. Architecture Rule

> Source Adapter يعرف كيف يجلب البيانات فقط.
>
> Application Layer يعرف كيف يعالجها.
>
> Domain يعرف قواعد العمل.
>
> Infrastructure يعرف التفاصيل الخارجية.
>
> API يعرض الوظائف للمستخدم.
>
> Tracker يسجل النتائج.
>
> Learning Engine يحسن القرارات المستقبلية.

---

**Last updated:** 2026-09-18
