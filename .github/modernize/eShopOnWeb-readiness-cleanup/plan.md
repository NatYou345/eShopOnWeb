# eShopOnWeb Readiness Cleanup

Baseline: .NET 10 is already in place, so this plan stays on modernization cleanup and deployment-readiness work only.

## Scope

Use the fallback assessment findings as the input scope:

- Nullable is disabled in the production projects.
- Older package and build-tool references remain in the manifests.
- Web startup is still coupled to database and identity seeding.

## Tasks

1. Enable nullable reference types in the production projects.
   - Scope: `src/ApplicationCore`, `src/Infrastructure`, `src/Web`, `src/PublicApi`, `src/BlazorAdmin`, `src/BlazorShared`.
   - Outcome: nullable is enabled consistently, and any resulting warnings or compile breaks are corrected in-place.

2. Clean up legacy package and build-tool references.
   - Scope the production manifests first, especially `src/Web/Web.csproj` and `src/PublicApi/PublicApi.csproj`.
   - Review `Microsoft.Web.LibraryManager.Build`, `Microsoft.Azure.AppConfiguration.AspNetCore`, `Microsoft.FeatureManagement.AspNetCore`, `System.IdentityModel.Tokens.Jwt`, `MediatR`, `AutoMapper`, and `NuGet.Packaging` for actual usage, version alignment, or removal.
   - Outcome: the manifests contain only still-needed dependencies and the oldest nonessential tooling is removed or refreshed.

3. Decouple database and identity seeding from normal startup.
   - Scope: `src/Web/Program.cs`, `src/PublicApi/Program.cs`, and the seed helpers in `src/Infrastructure/Data`.
   - Outcome: startup no longer performs unconditional seeding; seeding is moved behind an explicit dev-only or initialization path so deployment is less fragile.

## Dependencies

- Task 1 should land first so nullable-related warnings and fixes are settled before package and startup changes.
- Tasks 2 and 3 can then proceed independently.

## Out Of Scope

- No framework retargeting.
- No broad feature rewrite.
- No database schema migration work beyond what is needed to preserve the current seeding flow.