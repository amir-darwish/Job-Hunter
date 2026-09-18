# Job Hunter — Roadmap

> هذا الملف يحول رؤية `job-hunter` إلى مراحل تنفيذ عملية.
> الهدف هو الوصول بسرعة إلى نسخة مفيدة فعليًا في البحث عن وظيفة، مع الحفاظ على قيمة المشروع كـ Portfolio قوي لوظائف `.NET Backend`.

---

# 1. الهدف العام

نبني النظام تدريجيًا بهذا الترتيب:

```text
Foundation
   ↓
ATS Integrations
   ↓
Normalization
   ↓
Persistence
   ↓
Filtering
   ↓
Scoring
   ↓
Real Job Search
   ↓
Tracking
   ↓
AI Assistance
   ↓
Analytics
   ↓
Learning Engine
```

---

# 2. قواعد تنفيذ الـRoadmap

## Rule 1 — لا نضيف Feature قبل الحاجة لها

ممنوع القفز مبكرًا إلى:

```text
Dashboard ضخم
Microservices
Scrapling Worker
Browser Automation
Auto Apply
Machine Learning
Kafka
RabbitMQ
Kubernetes
```

---

## Rule 2 — كل مرحلة يجب أن تنتج قيمة قابلة للاختبار

لا ننتقل للمرحلة التالية إلا إذا:

```text
Build passes
Tests pass
Feature works on real data
Documentation updated
```

---

## Rule 3 — Portfolio Value

أي Feature مهمة يجب أن تساعد أيضًا على إظهار مهارات:

```text
C#
ASP.NET Core
EF Core
HTTP APIs
Async programming
Testing
Architecture
Resilience
Logging
Docker
CI/CD
```

---

# PHASE 0 — Project Foundation

## الهدف

إنشاء Solution نظيفة وقابلة للتوسع.

## المهام

```text
[ ] إنشاء Git repository
[ ] إنشاء JobHunter.sln
[ ] إنشاء المشاريع الأربعة الرئيسية
[ ] إنشاء مشاريع الاختبارات
[ ] إضافة docs/
[ ] نقل ملفات التوثيق الحالية
[ ] إضافة .gitignore
[ ] إضافة Directory.Build.props إن احتجنا
[ ] تفعيل nullable reference types
[ ] تفعيل implicit usings
```

## الهيكل

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
└── README.md
```

## Definition of Done

```text
dotnet build
```

يعمل بدون أخطاء.

---

# PHASE 1 — Domain + Core Contracts

## الهدف

تعريف قلب النظام بدون أي اعتماد على Infrastructure.

## المهام

### Domain Entities

```text
[ ] Job
[ ] Company
[ ] CompanySource
[ ] Application
[ ] ApplicationEvent
[ ] CvVersion
[ ] JobScore
```

### Enums

```text
[ ] AtsType
[ ] JobStatus
[ ] ApplicationStatus
[ ] SeniorityLevel
[ ] EmploymentType
[ ] RemoteType
```

### Application Contracts

```text
[ ] IJobSource
[ ] IJobRepository
[ ] ICompanyRepository
[ ] IApplicationRepository
[ ] IJobScorer
```

## نموذج مبدئي

```csharp
public interface IJobSource
{
    string Name { get; }

    Task<IReadOnlyCollection<ExternalJob>> GetJobsAsync(
        CompanySource company,
        CancellationToken cancellationToken);
}
```

## Definition of Done

- لا يوجد EF Core داخل Domain.
- لا يوجد HttpClient داخل Domain.
- Unit tests للنماذج والقواعد الأساسية.

---

# PHASE 2 — Persistence with EF Core + SQLite

## الهدف

تخزين الوظائف والشركات والتطبيقات محليًا.

## المهام

```text
[ ] إضافة EF Core
[ ] إنشاء JobHunterDbContext
[ ] إعداد SQLite
[ ] Entity configurations
[ ] First migration
[ ] Database creation
[ ] Repositories
```

## الجداول الأولية

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

## Definition of Done

يمكن:

```text
Create company
Save job
Read job
Update job
Delete/test data
```

من خلال integration tests.

---

# PHASE 3 — Greenhouse Integration

## الهدف

أول مصدر حقيقي للوظائف.

## المهام

```text
[ ] GreenhouseClient
[ ] Greenhouse DTOs
[ ] GreenhouseJobSource
[ ] Pagination/response handling إن وجدت
[ ] CancellationToken
[ ] Logging
[ ] Error handling
[ ] Tests
```

## التدفق

```text
CompanySource
   ↓
GreenhouseClient
   ↓
ExternalJob[]
   ↓
Normalization
```

## Definition of Done

النظام يجلب وظائف حقيقية من شركة تستخدم Greenhouse.

---

# PHASE 4 — Lever Integration

## الهدف

إضافة ثاني ATS مع نفس الـcontract.

## المهام

```text
[ ] LeverClient
[ ] Lever DTOs
[ ] LeverJobSource
[ ] Error handling
[ ] Tests
```

## Definition of Done

بدون تغيير Scoring أو Domain يمكن جلب وظائف Lever.

---

# PHASE 5 — SmartRecruiters Integration

## الهدف

إضافة مصدر ثالث مهم في أوروبا.

## المهام

```text
[ ] SmartRecruitersClient
[ ] DTOs
[ ] JobSource
[ ] Query filters
[ ] Tests
```

## Definition of Done

يمكن تشغيل نفس pipeline على:

```text
Greenhouse
Lever
SmartRecruiters
```

---

# PHASE 6 — Normalization Layer

## الهدف

تحويل كل الوظائف إلى صيغة موحدة.

## المهام

```text
[ ] Normalize title
[ ] Normalize company
[ ] Normalize location
[ ] Normalize dates
[ ] Detect country
[ ] Detect seniority
[ ] Detect contract type
[ ] Detect remote type
[ ] Extract experience range
```

## مثال

```text
"Software Engineer - Junior"
"Junior Software Developer"
"Développeur logiciel junior"
```

يجب أن تصبح قابلة للمقارنة بشكل منطقي.

## Definition of Done

كل مصدر يعيد Job Domain Model متجانسًا.

---

# PHASE 7 — Deduplication

## الهدف

منع تخزين نفس الوظيفة أكثر من مرة.

## V1 Strategy

```text
NormalizedCompany
+
NormalizedTitle
+
NormalizedLocation
```

## المهام

```text
[ ] CanonicalJobId
[ ] Fingerprint generator
[ ] Duplicate check
[ ] Source references
[ ] Tests
```

## لاحقًا

```text
Description hash
Posting date
Fuzzy title similarity
ATS IDs
```

## Definition of Done

نفس الوظيفة القادمة من مصدرين لا تتحول إلى سجلين مستقلين.

---

# PHASE 8 — Company Discovery + ATS Detection

## الهدف

عدم إضافة كل شركة يدويًا.

## المهام

```text
[ ] CareersPageFinder
[ ] AtsDetector
[ ] Detect Greenhouse
[ ] Detect Lever
[ ] Detect SmartRecruiters
[ ] Detect SuccessFactors
[ ] Save CompanySource
```

## الناتج

```json
{
  "company": "Example",
  "careers_url": "...",
  "ats": "greenhouse",
  "identifier": "example"
}
```

## Definition of Done

إعطاء موقع شركة يؤدي إلى التعرف على ATS في الحالات المدعومة.

---

# PHASE 9 — Company Discovery Automation

## الهدف

التوقف تدريجيًا عن إضافة الشركات يدويًا، وتحويل إضافة الشركات إلى عملية شبه آلية ثم آلية بالكامل.

## المبدأ

الإدخال اليدوي في الـMVP هو فقط:

```text
Seed Dataset
```

وليس التصميم النهائي للنظام.

## V1 — Manual Seed

نبدأ بقائمة صغيرة من:

```text
20–50 شركة مستهدفة
```

يتم تخزينها في قاعدة البيانات، وليس Hard-coded داخل الكود.

مثال:

```text
Company
├── Name
├── Website
├── CareersUrl
├── Ats
├── AtsIdentifier
└── Enabled
```

---

## V2 — Careers Page Discovery

إعطاء النظام:

```text
https://company.com
```

ثم:

```text
Company Website
      ↓
Find Careers / Jobs / Recruitment links
      ↓
Resolve redirects
      ↓
Candidate Careers URLs
```

## المهام

```text
[ ] CareersPageFinder
[ ] Detect common careers paths
[ ] Parse navigation/footer links
[ ] Follow redirects safely
[ ] Score candidate careers URLs
[ ] Save best CareersUrl
```

---

## V3 — ATS Auto Detection

بعد العثور على صفحة الوظائف:

```text
Careers URL
      ↓
Inspect host / HTML / links / scripts
      ↓
Detect ATS
```

أمثلة:

```text
boards.greenhouse.io      → Greenhouse
jobs.lever.co             → Lever
smartrecruiters.com       → SmartRecruiters
SuccessFactors patterns   → SAP SuccessFactors
```

## الناتج

```json
{
  "company": "Example Company",
  "website": "https://example.com",
  "careers_url": "https://jobs.example.com",
  "ats": "greenhouse",
  "ats_identifier": "example",
  "confidence": 0.98
}
```

---

## V4 — Bulk Company Import

دعم إدخال:

```text
CSV
JSON
External dataset/API
```

ثم:

```text
500 companies
      ↓
Website validation
      ↓
Careers discovery
      ↓
ATS detection
      ↓
Company Registry
```

## المهام

```text
[ ] CSV importer
[ ] JSON importer
[ ] Deduplicate companies
[ ] Validate domains
[ ] Queue discovery jobs
[ ] Track discovery status
```

---

## V5 — Automated Company Discovery

لاحقًا يمكن اكتشاف شركات جديدة من مصادر عامة أو قوائم شركات مناسبة للسوق المستهدف.

```text
Company Sources
      ↓
Sector / Location filters
      ↓
Find Website
      ↓
Find Careers
      ↓
Detect ATS
      ↓
Add to Registry
```

## أمثلة لمصادر مستقبلية

```text
Public company datasets
Startup directories
Professional directories
Job aggregators
Search engines
Known ATS boards
```

## قاعدة مهمة

لا نعتمد على مصدر واحد لاكتشاف الشركات.

---

## Company Discovery Status

لكل شركة:

```text
Pending
WebsiteFound
CareersFound
AtsDetected
Ready
NeedsReview
Failed
```

---

## Confidence + Human Review

إذا كانت الثقة عالية:

```text
confidence >= threshold
→ save automatically
```

إذا كانت الثقة منخفضة:

```text
NeedsReview
```

ثم يراجعها المستخدم.

هذا يحقق:

```text
Automation-first
+
Human-assisted when uncertain
```

---

## Definition of Done

نعتبر هذه المرحلة ناجحة عندما يمكن:

1. إدخال Domain شركة فقط.
2. اكتشاف Careers page تلقائيًا.
3. اكتشاف ATS إن كان مدعومًا.
4. حفظ `CompanySource`.
5. تعليم الحالات الغامضة بـ `NeedsReview`.
6. إضافة عشرات الشركات دفعة واحدة بدون إدخال يدوي لكل ATS.

---

# PHASE 10 — SAP SuccessFactors

## الهدف

دعم الشركات الكبيرة مثل Atos.

## المهام

```text
[ ] Analyze SuccessFactors career page patterns
[ ] SuccessFactorsClient
[ ] Job listing extraction
[ ] Job detail extraction
[ ] Pagination
[ ] Tests
```

## ملاحظة

SuccessFactors قد يحتاج HTTP extraction أكثر من API بسيطة.

## Definition of Done

إدخال Atos كمصدر فعلي وتشغيله ضمن نفس الـpipeline.

---

# PHASE 11 — Master Profile

## الهدف

تعريف ما الذي نبحث عنه فعليًا.

## الملف

```text
profiles/master_profile.json
```

## يشمل

```text
Target roles
Secondary roles
Skills
Languages
Locations
Contract preferences
Remote preferences
Experience level
Hard exclusions
```

## Definition of Done

النظام يستطيع قراءة Profile واحد واستخدامه في filtering/scoring.

---

# PHASE 12 — Hard Filters

## الهدف

استبعاد الوظائف غير المناسبة قبل Scoring أو AI.

## الفلاتر

```text
[ ] Country
[ ] Location
[ ] Seniority
[ ] Experience
[ ] Language
[ ] Contract type
[ ] Hard excluded keywords
```

## أمثلة استبعاد

```text
Senior
Lead
Principal
Director
5+ years
```

مع عدم الاعتماد فقط على العنوان.

## Definition of Done

كل وظيفة لها سبب واضح إذا تم استبعادها.

---

# PHASE 13 — Deterministic Scoring

## الهدف

إعطاء Score أولي بدون LLM.

## الأوزان الأولية

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

## المهام

```text
[ ] RoleScorer
[ ] SkillScorer
[ ] ExperienceScorer
[ ] LocationScorer
[ ] LanguageScorer
[ ] ContractScorer
[ ] FreshnessScorer
[ ] Composite JobScorer
```

## Definition of Done

كل وظيفة تحصل على:

```text
Score
Breakdown
Reasons
```

---

# PHASE 14 — First Real Job Search MVP

## الهدف

استخدام النظام فعليًا في البحث اليومي.

## التدفق

```text
Companies
   ↓
ATS Sources
   ↓
Fetch Jobs
   ↓
Normalize
   ↓
Deduplicate
   ↓
Store
   ↓
Hard Filters
   ↓
Score
   ↓
Top Jobs
```

## Output V1

إما:

```text
CLI
```

أو API:

```text
GET /api/jobs/top
```

## Definition of Done

النظام يعرض يوميًا وظائف حقيقية مرتبة حسب الملاءمة.

---

# PHASE 15 — ASP.NET Core REST API

## الهدف

إظهار المشروع كـ Backend application حقيقية.

## Endpoints

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
```

## إضافات

```text
[ ] OpenAPI
[ ] Validation
[ ] Error responses
[ ] Pagination
[ ] Filtering
[ ] Sorting
```

---

# PHASE 16 — Background Job Discovery

## الهدف

تشغيل الاكتشاف دوريًا.

## V1

```text
BackgroundService
```

## المهام

```text
[ ] JobDiscoveryWorker
[ ] Configurable interval
[ ] Graceful cancellation
[ ] Failure isolation
[ ] Logging
```

## Definition of Done

النظام يحدث الوظائف دون تشغيل command يدوي كل مرة.

---

# PHASE 17 — Logging + Resilience

## الهدف

جعل integrations الخارجية موثوقة.

## المهام

```text
[ ] Structured logging
[ ] Serilog
[ ] HTTP timeout
[ ] Retry policy
[ ] Rate-limit handling
[ ] Failure isolation
[ ] Health checks
```

## مثال

```text
Lever failed
↓
Log
↓
Continue Greenhouse and SmartRecruiters
```

---

# PHASE 18 — Application Tracker

## الهدف

ربط البحث بالنتيجة الفعلية.

## الحالات

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

## Definition of Done

يمكن معرفة:

```text
أين قدمت؟
متى؟
بأي CV؟
ما النتيجة؟
```

---

# PHASE 19 — CV Version Tracking

## الهدف

معرفة أي نسخة CV تؤدي إلى نتائج أفضل.

## المهام

```text
[ ] CvVersion entity
[ ] Link CV version to application
[ ] Track response per version
[ ] Track interview rate per version
```

---

# PHASE 20 — AI Job Analysis

## الهدف

استخدام AI فقط على الوظائف الأعلى.

## شرط الدخول

مثال:

```text
Deterministic score >= 70
```

## المهام

```text
[ ] Extract requirements
[ ] Identify mandatory skills
[ ] Identify missing skills
[ ] Explain fit
[ ] Recommend Apply / Maybe / Skip
[ ] CV tailoring guidance
```

## قاعدة

AI لا يملك قرار نهائي مستقل.

---

# PHASE 21 — CV + Lettre de Motivation Tailoring

## الهدف

تخصيص التقديم لكل وظيفة.

## القيود

```text
No invented skills
No invented experience
No fake qualifications
No fake language level
```

## الناتج

```text
Tailored CV data
+
Tailored LM draft
```

بعد Human Review.

---

# PHASE 22 — Analytics

## الهدف

معرفة ما إذا كانت استراتيجية البحث تعمل.

## Metrics

```text
Applications
Replies
Interviews
Offers

Response Rate
Interview Rate
Offer Rate
```

## Breakdowns

```text
By Role
By Location
By ATS
By Source
By Company Size
By CV Version
By Match Score
```

---

# PHASE 23 — Learning Engine V1

## الهدف

تحسين Scoring باستخدام النتائج الحقيقية.

## لا نستخدم ML أولًا

V1:

```text
Rule-based adaptive weights
```

مثال:

```text
Backend .NET
Grand Est
SME
```

إذا أثبتت معدل مقابلات أعلى:

```text
future ranking boost
```

---

# PHASE 24 — PostgreSQL Migration

## الهدف

الانتقال من SQLite عند الحاجة.

## الشرط

لا يتم الانتقال إلا إذا أصبح:

```text
multi-process
larger dataset
remote deployment
```

مطلوبًا فعليًا.

---

# PHASE 25 — Docker

## الهدف

سهولة التشغيل والعرض.

## المهام

```text
[ ] Dockerfile
[ ] docker-compose
[ ] Environment configuration
[ ] PostgreSQL container later
```

---

# PHASE 26 — CI/CD

## GitHub Actions

```text
Restore
Build
Test
Publish
```

لاحقًا:

```text
Docker build
Deployment
```

---

# PHASE 27 — Scrapling Worker

## الحالة

اختياري فقط إذا ظهرت مصادر لا تغطيها الطرق السابقة.

## التقنية

```text
Python
+
Scrapling
```

## القاعدة

لا Business Logic في Python Worker.

فقط:

```text
Fetch
Extract
Return payload
```

كل:

```text
Filtering
Scoring
Tracking
Business rules
```

تبقى في .NET.

---

# PHASE 28 — Browser Automation

آخر fallback.

لا نضيفه إلا لمواقع مهمة فعلًا لا يمكن الوصول إليها بأي طريقة أخرى.

---

# 3. MVP الحقيقي الذي نريد الوصول إليه أولًا

لا نعتبر MVP هو كل ما سبق.

## MVP = Phases 0 → 14

أي:

```text
.NET solution
+
EF Core / SQLite
+
Greenhouse
+
Lever
+
SmartRecruiters
+
SuccessFactors
+
Company Discovery / ATS Detection
+
Normalization
+
Deduplication
+
Master Profile
+
Hard Filters
+
Deterministic Scoring
+
Real Top Jobs
```

عند هذه النقطة المشروع أصبح **مفيدًا فعليًا للبحث عن وظيفة**.

---

# 4. Priority Tiers

## P0 — Critical

```text
Foundation
Domain
Database
ATS adapters
Normalization
Deduplication
Master Profile
Filters
Scoring
```

## P1 — High Value

```text
REST API
Background jobs
Tracker
Logging
Resilience
SuccessFactors
```

## P2 — High Value Later

```text
AI analysis
CV tailoring
LM tailoring
Analytics
Learning Engine
```

## P3 — Optional

```text
Scrapling worker
Browser automation
Dashboard
Notifications
```

---

# 5. أول Sprint مقترح

## Sprint 1 — Foundation

### Tasks

```text
[ ] dotnet new sln
[ ] Create Domain project
[ ] Create Application project
[ ] Create Infrastructure project
[ ] Create Api project
[ ] Create test projects
[ ] Add project references
[ ] Add PROJECT_DECISIONS.md
[ ] Add ARCHITECTURE.md
[ ] Add ROADMAP.md
[ ] First build
[ ] First commit
```

### الهدف

نهاية Sprint 1:

```text
Repository builds cleanly
Architecture is visible
Tests project is ready
```

---

# 6. Sprint 2 — Greenhouse End-to-End

```text
Company
↓
Greenhouse API
↓
ExternalJob
↓
Normalize
↓
EF Core
↓
SQLite
↓
GET /api/jobs
```

إذا نجح هذا Sprint فقد أثبتنا الـvertical slice الأول بالكامل.

---

# 7. Sprint 3 — Multi-ATS

إضافة:

```text
Lever
SmartRecruiters
```

بدون تغيير Core business logic.

وهذه نقطة مهمة جدًا لاختبار جودة المعمارية.

---

# 8. Sprint 4 — Company Discovery

```text
Company Domain
↓
CareersPageFinder
↓
ATS Detector
↓
CompanySource
↓
Database
```

الهدف أن نبدأ من Domain الشركة بدل إدخال ATS يدويًا.

---

# 9. Sprint 5 — Real Matching

```text
Master Profile
+
Hard Filters
+
Scoring
+
Top Jobs
```

ومن هنا يبدأ استخدام النظام فعليًا يوميًا.

---

# 10. أهم Milestones

## M1 — Clean Build

المشروع يعمل.

## M2 — First Real Job

أول وظيفة حقيقية تدخل قاعدة البيانات.

## M3 — Multi-source

وظائف من 3 ATS مختلفة.

## M4 — First Auto-Discovered Company

أول شركة يتم اكتشاف Careers page وATS الخاص بها تلقائيًا.

## M5 — First Ranked Job

أول Score حقيقي.

## M6 — First Application

أول وظيفة تم التقديم عليها من خلال النظام.

## M7 — First Reply

أول رد مرتبط بالـtracker.

## M8 — First Interview

أول مقابلة ناتجة عن workflow المشروع.

## M9 — Learning Loop

النظام يبدأ تعديل الأولويات بناءً على نتائج حقيقية.

---

# 11. North Star Metric

المقياس الأساسي لنجاح المشروع ليس:

```text
Jobs scraped
```

بل:

```text
Interviews / Qualified Applications
```

ويجب أن تظل هذه القاعدة هي المرجع عند ترتيب أي Feature جديدة.

---

**Last updated:** 2026-09-18
