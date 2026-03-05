# eShopOnWeb .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the eShopOnWeb solution upgrade from .NET 8.0 to .NET 10.0. All 10 projects will be upgraded simultaneously in a single atomic operation, followed by comprehensive testing and validation.

**Progress**: 3/4 tasks complete (75%) ![0%](https://progress-bar.xyz/75)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-05 15:58)*
**References**: Plan §Phase 0, Plan §Implementation Timeline

- [✓] (1) Verify .NET 10 SDK installed and available
- [✓] (2) .NET 10 SDK meets minimum requirements (**Verify**)
- [✓] (3) Check global.json compatibility with .NET 10 (if file exists)
- [✓] (4) global.json compatible or absent (**Verify**)

---

### [✓] TASK-002: Atomic framework and dependency upgrade with compilation fixes *(Completed: 2026-03-05 16:06)*
**References**: Plan §Phase 1, Plan §Implementation Timeline, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Update TargetFramework to net10.0 in all 10 project files per Plan §Phase 1 (ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, PublicApi, Web, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)
- [✓] (2) All project files updated to net10.0 (**Verify**)
- [✓] (3) Update all package references per Plan §Package Update Reference (18 packages: Microsoft.AspNetCore.* → 10.0.3, Microsoft.EntityFrameworkCore.* → 10.0.3, System.Text.Json → 10.0.3, System.Net.Http.Json → 10.0.3, Microsoft.Extensions.* → 10.0.3)
- [✓] (4) Address 4 deprecated packages per Plan §Package Update Reference (System.IdentityModel.Tokens.Jwt → 8.16.0, Azure.Identity → 1.18.0, remove AutoMapper.Extensions.Microsoft.DependencyInjection if AutoMapper base package present, remove Microsoft.AspNetCore.Mvc 2.2.0)
- [✓] (5) All package references updated (**Verify**)
- [✓] (6) Restore all dependencies
- [✓] (7) Dependencies restored successfully (**Verify**)
- [✓] (8) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus: JWT authentication APIs in Infrastructure/PublicApi, Configuration/DI patterns in Web/PublicApi, MediatR registration, AutoMapper registration, TimeSpan API, HttpContent behavioral changes)
- [✓] (9) Solution builds with 0 errors (**Verify**)

---

### [✓] TASK-003: Run full test suite and validate upgrade *(Completed: 2026-03-05 17:06)*
**References**: Plan §Phase 2, Plan §Testing & Validation Strategy, Plan §Breaking Changes Catalog

- [✓] (1) Run all test projects per Plan §Testing Strategy (UnitTests, IntegrationTests, PublicApiIntegrationTests, FunctionalTests)
- [✓] (2) Fix any test failures (reference Plan §Breaking Changes Catalog for API changes and behavioral differences)
- [✓] (3) Re-run all test projects after fixes
- [✓] (4) All tests pass with 0 failures (**Verify**)

---

### [▶] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [▶] (1) Commit all changes with message: "Upgrade solution to .NET 10.0\n\n- Update all 10 project files: net8.0 → net10.0\n- Update 18 NuGet packages to .NET 10 compatible versions\n- Address 4 deprecated packages (JWT, AutoMapper, Azure.Identity, ASP.NET MVC)\n- Fix 26 binary incompatible APIs\n- Fix 20 source incompatible APIs\n- Update code for JWT authentication API changes (Infrastructure, PublicApi)\n- Update configuration/DI patterns (Web, PublicApi)\n- Update test code for API changes\n- All tests passing (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)\n\nBreaking changes addressed:\n- JWT Bearer authentication (System.IdentityModel.Tokens.Jwt 7.4.1 → 8.16.0)\n- Configuration binding APIs (Web, PublicApi)\n- MediatR registration (Web, PublicApi)\n- AutoMapper registration (Web, PublicApi - deprecated package)\n- Azure.Identity (1.13.2 → 1.18.0 - removed deprecated MSAL)\n\nSee .github/upgrades/scenarios/new-dotnet-version_599bde/plan.md for complete details."

---





