# Job Hunter — Project Decisions

> هذا الملف هو السجل الرسمي للقرارات المعمارية والوظيفية الأساسية في مشروع `job-hunter`.
> أي قرار جوهري جديد يجب إضافته هنا بدل الاعتماد على الذاكرة أو المحادثات السابقة فقط.

---

## معلومات المشروع

- **اسم المشروع:** `job-hunter`
- **الهدف الرئيسي:** مساعدة المستخدم في العثور على أول وظيفة مناسبة، ثم تحسين جودة الاستهداف والتقديم بناءً على النتائج الفعلية.
- **التقنية الأساسية:** `.NET 10 LTS / C# / ASP.NET Core`
- **ORM:** Entity Framework Core
- **قاعدة البيانات في MVP:** SQLite
- **قاعدة البيانات المستهدفة لاحقًا:** PostgreSQL
- **السوق الأولي المستهدف:** فرنسا وأوروبا، مع دعم الفرنسية والإنجليزية.
- **مرحلة المشروع الحالية:** التخطيط المعماري وبناء MVP.
- **هدف Portfolio:** المشروع نفسه يجب أن يثبت الكفاءة العملية في .NET Backend Engineering.

---

## D001 — إيقاف مشروع deal-hunter مؤقتًا

**الحالة:** Adopted

### القرار
إيقاف العمل على مشروع `deal-hunter` مؤقتًا، وتوجيه الجهد الحالي إلى `job-hunter`.

### السبب
الهدف الحالي ذو أولوية أعلى: الحصول على أول وظيفة، وليس بناء نظام عام لصيد الصفقات.

### الأثر
- عدم تشتيت وقت التطوير بين مشروعين.
- إعادة استخدام الأفكار والخبرة المكتسبة من scraping وdata extraction عند الحاجة.

---

## D002 — المصدر الأصلي للوظيفة له الأولوية

**الحالة:** Adopted

### القرار
نعطي الأولوية للمصادر الرسمية للوظائف قبل مواقع التجميع العامة.

### ترتيب مصادر البيانات

1. Official/Public ATS API
2. Structured Data / JSON-LD
3. Direct HTTP Extraction
4. Scrapling Python Worker
5. Browser Automation

### السبب
المصادر الرسمية عادةً:
- أكثر استقرارًا.
- أقل عرضة للحظر.
- توفر بيانات منظمة.
- تحتاج صيانة أقل.
- تقلل الاعتماد على CSS selectors الهشة.

---

## D003 — ATS-first Architecture

**الحالة:** Adopted

### القرار
يبنى نظام الاكتشاف حول أنظمة ATS وليس حول مواقع الشركات بشكل منفصل.

### الأنظمة ذات الأولوية

- Greenhouse
- Lever
- SmartRecruiters
- SAP SuccessFactors

### السبب
إذا كانت مئات الشركات تستخدم نفس ATS، فمن الأفضل بناء Adapter واحد للنظام بدل كتابة scraper منفصل لكل شركة.

### مثال

بدل:

```text
AtosScraper.cs
CompanyBScraper.cs
CompanyCScraper.cs
```

نستخدم:

```text
GreenhouseJobSource.cs
LeverJobSource.cs
SmartRecruitersJobSource.cs
SuccessFactorsJobSource.cs
```

---

## D004 — Company Discovery + ATS Detection

**الحالة:** Adopted

### القرار
يجب أن يحتوي المشروع على طبقة مستقلة لاكتشاف منصة التوظيف التي تستخدمها الشركة.

### المدخل المتوقع

```text
company.com
```

### المخرج المتوقع

```json
{
  "company": "Example Company",
  "careers_url": "https://jobs.example.com",
  "ats": "successfactors",
  "ats_identifier": "example",
  "collection_method": "public_api"
}
```

### الهدف
إتاحة إضافة شركات جديدة دون الحاجة إلى تعديل منطق الاستخراج الأساسي.

---

## D005 — .NET 10 هو التطبيق الأساسي

**الحالة:** Adopted

### القرار
التطبيق الرئيسي سيبنى باستخدام:

```text
.NET 10 LTS
C#
ASP.NET Core
Entity Framework Core
```

### السبب
المشروع يجب أن يخدم هدفين في نفس الوقت:

1. العثور على أول وظيفة.
2. العمل كـ Portfolio Project قوي لوظائف `.NET Backend Developer`.

### الأثر
يجب أن تظهر في المشروع ممارسات حقيقية من بيئة .NET مثل:

- Dependency Injection
- HttpClientFactory
- BackgroundService
- EF Core
- REST APIs
- Logging
- Testing
- Configuration
- Resilience
- Clean separation of concerns

---

## D006 — عدم اختيار NestJS كتقنية أساسية

**الحالة:** Adopted

### القرار
لن يكون NestJS هو التقنية الأساسية لـ `job-hunter`.

### السبب
هناك خبرة سابقة بالفعل في NestJS، بينما الهدف الوظيفي الحالي يركز على `.NET`.

### المبدأ
اختيار التقنية هنا يجب أن يزيد القيمة الوظيفية للمشروع، وليس فقط سرعة التطوير.

---

## D007 — Scrapling هو Python Worker اختياري

**الحالة:** Adopted

### القرار
Scrapling لن يكون جزءًا من التطبيق الأساسي في MVP.

إذا أصبح ضروريًا لاحقًا، سيتم عزله في Python Worker مستقل.

### المعمارية المستقبلية المحتملة

```text
ASP.NET Core
    ↓
Standard ATS / HTTP acquisition
    ↓
Failed/unsupported source
    ↓
Python Scrapling Worker
```

### السبب
Scrapling مكتبة Python، بينما التطبيق الأساسي C#/.NET.

### ممنوع في MVP
إنشاء Microservice للبنية الاحتياطية قبل وجود حاجة حقيقية لها.

---

## D008 — Schema موحّد لكل الوظائف

**الحالة:** Adopted

### القرار
كل `IJobSource` يجب أن ينتج نموذجًا داخليًا موحدًا مهما كان المصدر.

### الحد الأدنى

```text
SourceJobId
CanonicalJobId
Title
Company
Location
Country
Description
DatePosted
FirstSeen
LastSeen
Source
Ats
JobUrl
ApplyUrl
EmploymentType
Seniority
```

### الهدف
فصل Data Acquisition عن:

- Filtering
- Scoring
- Deduplication
- AI Analysis
- Tracking

---

## D009 — Deduplication إلزامي

**الحالة:** Adopted

### القرار
يجب إزالة الوظائف المكررة قبل التحليل والتقييم.

### الأسلوب المبدئي

```text
NormalizedCompany
+
NormalizedTitle
+
NormalizedLocation
```

مع إمكانية إضافة:

- ATS IDs
- Posting dates
- Description similarity
- Fuzzy matching

---

## D010 — Hard Filters قبل AI

**الحالة:** Adopted

### القرار
لا ترسل جميع الوظائف إلى LLM.

### Pipeline

```text
Raw Jobs
   ↓
Normalization
   ↓
Deduplication
   ↓
Hard Filters
   ↓
Deterministic Scoring
   ↓
Top Candidates
   ↓
AI Analysis
```

### السبب
تقليل التكلفة، زمن المعالجة، والأخطاء.

---

## D011 — دعم أول وظيفة وEarly Career

**الحالة:** Adopted

### القرار
يدعم النظام مصطلحات الوظائف المبكرة مهنيًا، ولا يعتمد فقط على كلمة `Junior`.

### أمثلة

```text
Junior
Entry Level
Graduate
Graduate Program
Débutant
Débutant accepté
Jeune diplômé
0-2 ans
0 à 2 ans
Sortie d'école
Trainee
```

### ملاحظة
طلب خبرة حتى سنتين لا يؤدي تلقائيًا إلى الاستبعاد إذا كانت بقية المتطلبات مناسبة.

---

## D012 — Seniority Exclusions

**الحالة:** Adopted

### أمثلة

```text
Senior
Lead
Principal
Staff
Head of
Director
Manager
5+ years
```

### ملاحظة
لا يعتمد الاستبعاد على العنوان فقط، بل على المتطلبات الفعلية أيضًا.

---

## D013 — Human-in-the-loop

**الحالة:** Adopted

### القرار
لن نبني النظام حول `mass auto-apply`.

### التدفق المطلوب

```text
Discover
→ Filter
→ Score
→ Analyze
→ Human Review
→ Tailor CV / LM
→ Apply
→ Track
```

---

## D014 — عدم اختلاق معلومات في CV

**الحالة:** Adopted

### ممنوع
- اختراع Skill.
- زيادة سنوات الخبرة.
- اختراع Project.
- اختراع شهادة.
- تغيير مستوى اللغة دون أساس.

### مسموح
- إعادة ترتيب المعلومات.
- إعادة الصياغة.
- إبراز خبرة حقيقية مرتبطة بالإعلان.
- تقليل المعلومات غير ذات الصلة.

---

## D015 — Success Metric

**الحالة:** Adopted

### المؤشر الرئيسي

```text
Interviews / Qualified Applications
```

### مؤشرات ثانوية

- Response Rate
- Interview Rate
- Applications per Offer
- Response Rate by Role
- Response Rate by Location
- Performance by CV Version
- Performance by Source

---

## D016 — Job Search Learning Engine

**الحالة:** Planned

### القرار
بعد توفر بيانات كافية، تستخدم نتائج التقديم الفعلية لتحسين Ranking الوظائف المستقبلية.

### بيانات محتملة

```text
Role
Location
Company Type
Company Size
Source
ATS
Match Score
CV Version
Outcome
```

---

## D017 — MVP صغير قبل التوسع

**الحالة:** Adopted

### MVP الأول

```text
ASP.NET Core
      ↓
3-4 ATS Providers
      ↓
Normalization
      ↓
Deduplication
      ↓
EF Core + SQLite
      ↓
Hard Filters
      ↓
Deterministic Score
      ↓
REST API / CLI output
```

### خارج MVP الأول

- Full Dashboard
- Scrapling Worker
- Browser Automation
- Auto Apply
- Telegram Bot
- Advanced AI Agent
- Complex ML ranking

---

## D018 — ATS Providers ذات الأولوية

**الحالة:** Adopted

```text
1. Greenhouse
2. Lever
3. SmartRecruiters
4. SAP SuccessFactors
```

---

## D019 — Architecture Style

**الحالة:** Adopted

### القرار
نستخدم Modular Monolith في البداية.

### هيكل مبدئي

```text
JobHunter.sln
│
├── JobHunter.Api
├── JobHunter.Application
├── JobHunter.Domain
├── JobHunter.Infrastructure
└── JobHunter.Tests
```

### السبب
تعلم وعرض مبادئ Clean Architecture دون الوقوع في overengineering أو Microservices مبكرة.

---

## D020 — HttpClientFactory للاتصالات الخارجية

**الحالة:** Adopted

### القرار
كل ATS integration تستخدم `HttpClientFactory`.

### الهدف
دعم:

- Timeout policies
- Retry
- Resilience
- Central configuration
- Testability

---

## D021 — Background Processing

**الحالة:** Planned

### القرار
التحديث الدوري للوظائف سيستخدم آليات .NET مثل:

```text
BackgroundService
IHostedService
```

في النسخ المبكرة.

إذا زاد التعقيد مستقبلًا يمكن تقييم أدوات مثل Hangfire أو Quartz.NET.

---

## D022 — قاعدة البيانات

**الحالة:** Adopted

### MVP

```text
SQLite
```

### Later

```text
PostgreSQL
```

### ORM

```text
Entity Framework Core
```

---

## D023 — المشروع نفسه Portfolio Project

**الحالة:** Adopted

يجب أن يظهر المشروع:

- ASP.NET Core
- C#
- EF Core
- REST API Design
- Dependency Injection
- Background Processing
- External API Integration
- Async Programming
- Logging
- Testing
- Docker
- GitHub Actions
- Resilience
- Clean Architecture
- Documentation

---

## D024 — قاعدة إضافة Features

**الحالة:** Adopted

لا نضيف Feature إلا إذا حسنت واحدًا على الأقل من:

1. Job discovery
2. Ranking quality
3. Application quality
4. User time
5. Response rate
6. Interview rate
7. Learning from outcomes
8. Portfolio value without creating major scope creep

---

# Decision Workflow

كل قرار جديد يضاف بالشكل التالي:

```text
## DXXX — Decision Name

Status:
Adopted / Planned / Rejected / Revisit

Decision:
...

Reason:
...

Alternatives considered:
...

Impact:
...
```

---

# القاعدة الأساسية للمشروع

> `job-hunter` ليس مشروع Web Scraping.
>
> هو نظام Decision Support للبحث عن وظيفة، مبني أساسًا بـ .NET، ويستخدم Web Data Acquisition كطبقة واحدة فقط.

---

# Current High-Level Pipeline

```text
Company Discovery
        ↓
ATS Detection
        ↓
Job Acquisition (.NET)
        ↓
Normalization
        ↓
Deduplication
        ↓
EF Core / Database
        ↓
Hard Filters
        ↓
Deterministic Scoring
        ↓
AI Analysis
        ↓
Human Review
        ↓
Tailored CV / LM
        ↓
Application
        ↓
Tracking
        ↓
Analytics
        ↓
Learning Engine
        └──────────────→ Future Scoring
```

---

**Last updated:** 2026-09-18
