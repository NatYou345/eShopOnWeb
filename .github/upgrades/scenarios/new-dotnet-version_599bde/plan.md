# eShopOnWeb .NET 10.0 Upgrade Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario
Upgrade **eShopOnWeb** solution from **.NET 8.0** to **.NET 10.0 (LTS)** to leverage the latest framework features, performance improvements, security updates, and long-term support.

### Scope
- **Projects Affected**: 10 active .NET projects (1 docker-compose project excluded from upgrade)
  - 3 class libraries (ApplicationCore, BlazorShared, Infrastructure)
  - 3 ASP.NET Core applications (BlazorAdmin, PublicApi, Web)
  - 4 test projects (FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests)
- **Current State**: All projects target .NET 8.0
- **Target State**: All projects target .NET 10.0

### Discovered Metrics
- **Total Issues**: 147 (36 mandatory, 104 potential, 7 optional)
- **Affected Files**: 35 across the solution
- **Dependency Depth**: 5 levels (BlazorShared → ApplicationCore → Infrastructure → PublicApi/Web → Test projects)
- **Package Updates Required**: 18 packages need version updates
- **Deprecated Packages**: 4 packages require attention (AutoMapper.Extensions.Microsoft.DependencyInjection, Azure.Identity, Microsoft.AspNetCore.Mvc, System.IdentityModel.Tokens.Jwt)
- **Breaking Changes**: 26 binary incompatibilities, 20 source incompatibilities, 52 behavioral changes

### Complexity Classification
**Medium Complexity** with All-at-Once execution strategy

**Rationale**:
- Solution size manageable (10 projects)
- Clear dependency structure with no circular dependencies
- All projects on same starting framework (.NET 8.0)
- 2 high-risk projects (Web: 40 issues, PublicApi: 27 issues) but well-isolated
- Deprecated packages can be addressed during upgrade
- Good test coverage (4 test projects) enables comprehensive validation

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in single atomic operation

**Why This Approach**:
- Small-to-medium solution (10 projects) is ideal for all-at-once
- All projects currently on .NET 8.0 (homogeneous starting point)
- No multi-targeting complexity needed
- Faster completion with single coordinated upgrade
- Clean dependency resolution in one pass
- All package updates applied together for consistency

### Critical Issues
🔴 **Mandatory Actions**:
- 36 mandatory issues must be resolved (API breaking changes, target framework updates)
- 4 deprecated packages need investigation and potential replacement
- 26 binary incompatible APIs require code changes or recompilation
- All projects must update TargetFramework property to net10.0

⚠️ **Potential Risks**:
- 52 behavioral changes in APIs may affect runtime behavior
- 20 source incompatible changes may require code modifications
- Web project (largest) has 40 issues including 7 binary breaking changes
- Test projects depend on upgraded projects and may need adjustments

### Recommended Approach
Execute single atomic upgrade operation:
1. **Phase 0**: Prerequisites verification (SDK, build environment)
2. **Phase 1**: Atomic upgrade of all projects (target frameworks + packages + compilation fixes)
3. **Phase 2**: Test execution and validation
4. **Phase 3**: Final verification and documentation

### Expected Iterations
Following All-at-Once strategy with consolidated task structure:
- **Foundation iterations** (2-3): Dependency analysis, migration strategy, project classification
- **Detail iterations** (1-2): All project specifications in batched groups
- **Finalization** (1): Success criteria, source control strategy

---

## Migration Strategy

### All-At-Once Strategy

**Selected Approach**: All projects in the solution upgrade simultaneously in a single coordinated operation. All project files are updated to net10.0, all package references are updated, and all compilation errors are fixed in one atomic upgrade task.

### Strategy Rationale

**Why All-At-Once is Optimal for eShopOnWeb**:

✅ **Solution Size**: 10 projects - well within the ideal range for all-at-once (<15 projects)

✅ **Homogeneous Codebase**: All projects currently on .NET 8.0, creating consistent starting point

✅ **Clear Dependency Structure**: No circular dependencies, clean 5-level hierarchy

✅ **Assessment Clarity**: All NuGet packages have known versions or clear compatibility status

✅ **Framework Jump**: .NET 8 → .NET 10 is two versions, but both are modern .NET (not Framework → Core transition)

✅ **Test Coverage**: 4 test projects provide comprehensive validation after upgrade

### Strategy Principles

**1. Simultaneity**
All projects upgrade in the same operation:
- All TargetFramework properties change to net10.0 together
- All package versions update together
- All compilation errors addressed in single pass
- No intermediate states with mixed framework versions

**2. Atomic Operation**
The upgrade is indivisible:
- Cannot partially succeed (either all projects build or none are committed)
- All changes in single commit (if feasible)
- Rollback is clean (revert single commit)

**3. Dependency-Aware Execution**
Even though all projects update simultaneously, execution respects dependencies:
- MSBuild automatically builds in dependency order
- Compilation errors propagate from foundation (BlazorShared) to top (test projects)
- Fixes applied bottom-up where possible

**4. Unified Package Updates**
All package references update to compatible versions:
- Microsoft.* packages align with .NET 10.0 (version 10.0.3)
- Third-party packages remain on current compatible versions
- Deprecated packages addressed during upgrade (not deferred)

**5. Single Validation Phase**
Testing happens after complete upgrade:
- No intermediate testing of partially-upgraded solution
- All 4 test projects run after atomic upgrade completes
- Final validation confirms entire solution works on .NET 10.0

### Execution Approach

The All-at-Once strategy executes in clear phases:

#### Phase 0: Prerequisites (if applicable)
Verify environment readiness:
- ✅ .NET 10 SDK installed and available
- ✅ Branch created (upgrade-to-NET10)
- ✅ No pending changes blocking commit strategy
- ⚠️ Note: global.json validation may be needed

#### Phase 1: Atomic Upgrade
**Single coordinated operation** covering all projects:

**Step 1: Update All Project Files**
Update `<TargetFramework>` in all 10 project files:
- src\ApplicationCore\ApplicationCore.csproj: net8.0 → net10.0
- src\BlazorAdmin\BlazorAdmin.csproj: net8.0 → net10.0
- src\BlazorShared\BlazorShared.csproj: net8.0 → net10.0
- src\Infrastructure\Infrastructure.csproj: net8.0 → net10.0
- src\PublicApi\PublicApi.csproj: net8.0 → net10.0
- src\Web\Web.csproj: net8.0 → net10.0
- tests\FunctionalTests\FunctionalTests.csproj: net8.0 → net10.0
- tests\IntegrationTests\IntegrationTests.csproj: net8.0 → net10.0
- tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj: net8.0 → net10.0
- tests\UnitTests\UnitTests.csproj: net8.0 → net10.0

**Step 2: Update All Package References**
Update all packages requiring version changes (see §Package Update Reference for complete matrix):
- 18 packages across projects updating to .NET 10 compatible versions
- Focus areas: ASP.NET Core packages (8.0.3 → 10.0.3), Entity Framework packages (8.0.3 → 10.0.3), System packages (8.0.x → 10.0.3)

**Step 3: Restore Dependencies**
`dotnet restore eShopOnWeb.sln`

**Step 4: Build Solution and Fix Compilation Errors**
Build entire solution to identify all compilation errors:
`dotnet build eShopOnWeb.sln`

Fix compilation errors by addressing breaking changes (see §Breaking Changes Catalog):
- Binary incompatible APIs (26 occurrences)
- Source incompatible APIs (20 occurrences)
- Behavioral changes requiring code updates

**Step 5: Rebuild and Verify**
Rebuild solution to confirm 0 errors:
`dotnet build eShopOnWeb.sln --no-incremental`

**Expected Outcome**: Solution builds successfully with 0 errors, 0 warnings

#### Phase 2: Test Validation
Execute all test projects after successful build:
- `tests\UnitTests` - Core domain tests
- `tests\IntegrationTests` - Data layer integration tests
- `tests\PublicApiIntegrationTests` - API integration tests
- `tests\FunctionalTests` - End-to-end functional tests

Address test failures if any (may be due to behavioral changes in .NET 10).

**Expected Outcome**: All tests pass

#### Phase 3: Final Verification
- Confirm no package vulnerabilities
- Verify no compiler warnings introduced
- Document any behavioral changes requiring monitoring
- Update README or documentation with .NET 10.0 requirement

### Dependency-Based Ordering

While all projects update simultaneously, understanding dependency order helps anticipate where issues may appear:

**Build Order** (MSBuild follows this automatically):
1. Level 0: BlazorShared
2. Level 1: ApplicationCore, BlazorAdmin
3. Level 2: Infrastructure
4. Level 3: PublicApi, Web
5. Level 4: FunctionalTests, PublicApiIntegrationTests, UnitTests
6. Level 5: IntegrationTests

**Error Propagation**:
- Errors in BlazorShared block everything
- Errors in ApplicationCore block most projects
- Errors in Infrastructure block Web, PublicApi, tests
- Errors in Web/PublicApi only affect their test projects

**Fix Strategy**:
- Address compilation errors starting from lowest level (BlazorShared, ApplicationCore)
- Move up dependency chain as lower levels build successfully
- Test projects fixed last (Level 4-5)

### Parallel vs Sequential Execution

**Within All-At-Once Strategy**:
- File edits can happen in any order (all edited before build)
- Build execution is sequential by dependency (MSBuild determines)
- Testing can be parallelized (test projects are independent)
- No human coordination needed between projects (automated process)

### Risk Management Within Strategy

All-at-Once has higher initial risk but clear mitigation:

**Risk**: Larger testing surface (entire solution must work)
**Mitigation**: 4 comprehensive test projects validate functionality

**Risk**: All developers must adapt simultaneously
**Mitigation**: Single team working on eShopOnWeb, documentation provided

**Risk**: Compilation errors in multiple projects
**Mitigation**: Breaking changes catalog guides fixes, dependency-ordered troubleshooting

**Risk**: Behavioral changes affect production functionality
**Mitigation**: Behavioral changes documented, comprehensive testing validates

### Strategy Advantages for eShopOnWeb

✅ **Fastest Completion**: Single atomic operation, no multi-phase coordination
✅ **No Multi-Targeting**: No need for `<TargetFrameworks>` (plural) complexity
✅ **Clean Dependency Resolution**: NuGet resolves all dependencies for net10.0 together
✅ **Simplified Testing**: Test once after upgrade, not per phase
✅ **Simple Source Control**: Single commit captures entire upgrade
✅ **Immediate Benefits**: All projects benefit from .NET 10 features simultaneously

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The eShopOnWeb solution has a clear 5-level dependency hierarchy with no circular dependencies. All projects upgrade simultaneously in the All-at-Once approach, but understanding the dependency structure helps anticipate build order and integration points.

```
Level 0 (Foundation - no dependencies):
├─ BlazorShared.csproj (1 issue)
└─ docker-compose.dcproj (excluded)

Level 1 (depends on Level 0):
├─ ApplicationCore.csproj (2 issues) → depends on BlazorShared
└─ BlazorAdmin.csproj (15 issues) → depends on BlazorShared

Level 2 (depends on Levels 0-1):
└─ Infrastructure.csproj (9 issues) → depends on ApplicationCore

Level 3 (depends on Levels 0-2):
├─ PublicApi.csproj (27 issues) → depends on ApplicationCore, Infrastructure
└─ Web.csproj (40 issues) → depends on BlazorAdmin, ApplicationCore, BlazorShared, Infrastructure

Level 4 (depends on Levels 0-3):
├─ FunctionalTests.csproj (36 issues) → depends on Web, ApplicationCore, PublicApi
├─ PublicApiIntegrationTests.csproj (13 issues) → depends on Web, PublicApi
└─ UnitTests.csproj (2 issues) → depends on Web, ApplicationCore

Level 5 (depends on Levels 0-4):
└─ IntegrationTests.csproj (2 issues) → depends on Infrastructure, UnitTests
```

### Project Groupings for All-At-Once Migration

Since all projects upgrade simultaneously, grouping is primarily organizational (not execution phases):

**Core Libraries Group** (Foundation):
- `BlazorShared` - Shared Blazor components (1 issue - minimal complexity)
- `ApplicationCore` - Domain logic (2 issues - low complexity)

**Data & Infrastructure Group**:
- `Infrastructure` - Data access layer (9 issues - medium complexity, includes deprecated package)

**Application Group**:
- `BlazorAdmin` - Blazor admin UI (15 issues - medium complexity)
- `PublicApi` - REST API (27 issues - high complexity, deprecated packages)
- `Web` - Main Razor Pages web app (40 issues - **highest complexity**, deprecated packages)

**Test Group**:
- `UnitTests` - Unit tests (2 issues - low complexity, deprecated package)
- `IntegrationTests` - Integration tests (2 issues - low complexity)
- `FunctionalTests` - Functional tests (36 issues - high complexity due to dependencies)
- `PublicApiIntegrationTests` - API integration tests (13 issues - medium complexity)

### Critical Path Identification

In the All-at-Once strategy, the critical path represents the most complex/risky upgrade sequence that determines overall success:

**Primary Critical Path**:
```
BlazorShared → ApplicationCore → Infrastructure → Web (40 issues - most complex)
```

**Secondary Critical Path**:
```
ApplicationCore → Infrastructure → PublicApi (27 issues - API breaking changes)
```

**Why These Matter**:
- `Web` has the most issues (40) and highest risk - requires careful attention
- `PublicApi` has 27 issues with multiple API breaking changes
- Both depend on `Infrastructure`, which has deprecated package (AutoMapper.Extensions.Microsoft.DependencyInjection)
- Compilation errors will cascade from lower levels to higher levels

### Dependency Highlights

**Key Dependency Relationships**:

1. **BlazorShared** is foundational - used by:
   - ApplicationCore
   - BlazorAdmin
   - Web (directly)

2. **ApplicationCore** is core domain - used by:
   - Infrastructure
   - Web
   - PublicApi
   - UnitTests
   - FunctionalTests

3. **Infrastructure** provides data access - used by:
   - Web
   - PublicApi
   - IntegrationTests

4. **Web** is the main application - used by:
   - UnitTests
   - FunctionalTests
   - PublicApiIntegrationTests

**Implications for All-At-Once Upgrade**:
- All projects update TargetFramework simultaneously
- Build order follows dependency hierarchy (MSBuild handles automatically)
- Compilation errors in foundation projects will block dependent projects
- Test projects are the final validation point (depend on all application projects)

### No Circular Dependencies

✅ **Confirmed**: No circular dependencies detected. Clean dependency graph enables:
- Straightforward build order
- Clear error propagation (bottom-up)
- Simplified rollback if needed
- Predictable test execution order

---

## Project-by-Project Migration Plans

All projects migrate simultaneously in the All-at-Once strategy. Details below provide specific guidance for each project.

---

### Project: BlazorShared
**Path**: `src\BlazorShared\BlazorShared.csproj`
**Current State**: net8.0, Razor class library for shared Blazor components, 1 issue (1 mandatory)
**Target State**: net10.0
**Complexity**: Low
**Dependencies**: None (Level 0 - foundation)
**Used By**: ApplicationCore, BlazorAdmin, Web

#### Migration Steps

**1. Prerequisites**
- No dependencies to upgrade first (foundation project)
- No external packages requiring updates

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `BlazorShared.csproj`

**3. Package Updates**
None - no explicit package references

**4. Expected Breaking Changes**
None identified in assessment

**5. Code Modifications**
None expected - pure target framework change

**6. Testing Strategy**
- Build project successfully
- Verify no compilation errors
- Dependent projects (ApplicationCore, BlazorAdmin, Web) will validate integration

**7. Validation Checklist**
- [ ] `BlazorShared.csproj` targets net10.0
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] No package dependency conflicts

---

### Project: ApplicationCore
**Path**: `src\ApplicationCore\ApplicationCore.csproj`
**Current State**: net8.0, domain logic class library, 2 issues (1 mandatory, 1 potential)
**Target State**: net10.0
**Complexity**: Low
**Dependencies**: BlazorShared
**Used By**: Infrastructure, Web, PublicApi, UnitTests, FunctionalTests

#### Migration Steps

**1. Prerequisites**
- BlazorShared upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `ApplicationCore.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| System.Text.Json | 8.0.5 | 10.0.3 | Framework compatibility |

Other packages (compatible, no update):
- Ardalis.GuardClauses 4.5.0 ✅
- Ardalis.Specification 8.0.0 ✅
- MediatR 12.2.0 ✅

**4. Expected Breaking Changes**
None identified - System.Text.Json update is compatible

**5. Code Modifications**
None expected - package update is binary compatible

**6. Testing Strategy**
- Build project successfully
- Verify System.Text.Json 10.0.3 resolves correctly
- UnitTests will validate domain logic still works

**7. Validation Checklist**
- [ ] `ApplicationCore.csproj` targets net10.0
- [ ] System.Text.Json updated to 10.0.3
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] No package dependency conflicts

---

### Project: Infrastructure
**Path**: `src\Infrastructure\Infrastructure.csproj`
**Current State**: net8.0, data access layer (EF Core), 9 issues (5 mandatory, 3 potential, 1 optional)
**Target State**: net10.0
**Complexity**: Medium
**Dependencies**: ApplicationCore
**Used By**: Web, PublicApi, IntegrationTests

#### Migration Steps

**1. Prerequisites**
- ApplicationCore upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `Infrastructure.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.3 | Framework compatibility |
| System.IdentityModel.Tokens.Jwt | 7.4.1 | 8.1.0 (or latest 8.x) | Deprecated - upgrade to LTS version |

Other packages (compatible, no update):
- Ardalis.Specification.EntityFrameworkCore 8.0.0 ✅

**Deprecated Package Note**:
- **System.IdentityModel.Tokens.Jwt 7.4.1** is deprecated
- Assessment recommends version 8.16.0 (latest LTS)
- Used in `IdentityTokenClaimService.cs`
- Update to latest 8.x version: https://aka.ms/IdentityModel/LTS

**4. Expected Breaking Changes**

**JwtSecurityTokenHandler API Changes** (4 occurrences in `IdentityTokenClaimService.cs`):
- Line 24: `new JwtSecurityTokenHandler()` - constructor may have changes
- Line 41: `CreateToken()` method - may have signature changes
- Line 42: `WriteToken()` method - may have signature changes

**Impact**: Binary incompatible - requires code review and potential updates

**Recommended Actions**:
1. Review Microsoft.IdentityModel breaking changes documentation
2. Check if `JwtSecurityTokenHandler` API changed in version 8.x
3. Update code in `src\Infrastructure\Identity\IdentityTokenClaimService.cs` as needed
4. Common pattern: API may require different token validation parameters

**5. Code Modifications**

**File**: `src\Infrastructure\Identity\IdentityTokenClaimService.cs`
- Review lines 24, 41, 42 for `JwtSecurityTokenHandler` usage
- Consult breaking changes: https://go.microsoft.com/fwlink/?linkid=2262679
- Update token creation/writing calls if API changed
- Test JWT token generation and validation thoroughly

**6. Testing Strategy**
- Build project successfully
- Run IntegrationTests (depends on Infrastructure)
- Validate Entity Framework queries work correctly
- **Critical**: Test JWT token generation and validation
- Verify Identity/authentication flows work

**7. Validation Checklist**
- [ ] `Infrastructure.csproj` targets net10.0
- [ ] All Entity Framework packages updated to 10.0.3
- [ ] System.IdentityModel.Tokens.Jwt updated to 8.16.0 or latest 8.x
- [ ] JwtSecurityTokenHandler code updated for API changes
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] IntegrationTests pass
- [ ] JWT token generation verified
- [ ] No package dependency conflicts

---

### Project: BlazorAdmin
**Path**: `src\BlazorAdmin\BlazorAdmin.csproj`
**Current State**: net8.0, Blazor WebAssembly admin UI, 15 issues (2 mandatory, 13 potential)
**Target State**: net10.0
**Complexity**: Medium
**Dependencies**: BlazorShared
**Used By**: Web

#### Migration Steps

**1. Prerequisites**
- BlazorShared upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `BlazorAdmin.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Components.WebAssembly | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 8.0.3 | 10.0.3 | Framework compatibility |
| System.Net.Http.Json | 8.0.0 | 10.0.3 | Framework compatibility |

Other packages (compatible, no update):
- Blazored.LocalStorage 4.5.0 ✅
- BlazorInputFile 0.2.0 ✅ (may be old, verify compatibility)

**4. Expected Breaking Changes**

Assessment flags 13 potential behavioral changes (no specific details provided). Common areas for Blazor upgrades:

**Potential Areas**:
- Component lifecycle method changes
- Render mode differences (WebAssembly-specific)
- JavaScript interop API updates
- HTTP client configuration changes
- LocalStorage API behavioral differences

**Recommended Actions**:
1. Review .NET 10 Blazor breaking changes: https://go.microsoft.com/fwlink/?linkid=2262679
2. Test all Blazor components after upgrade
3. Verify JavaScript interop still works
4. Check LocalStorage functionality

**5. Code Modifications**

Specific files not identified by assessment. After build, address:
- Compilation errors from API changes
- Obsolete API warnings
- Component render issues

**6. Testing Strategy**
- Build project successfully
- Run BlazorAdmin UI in browser
- Test admin functionality manually (component rendering, navigation, data binding)
- Verify JavaScript interop works
- Check LocalStorage persistence

**7. Validation Checklist**
- [ ] `BlazorAdmin.csproj` targets net10.0
- [ ] All ASP.NET Core packages updated to 10.0.3
- [ ] System.Net.Http.Json updated to 10.0.3
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] BlazorAdmin UI loads in browser
- [ ] Admin components render correctly
- [ ] JavaScript interop functions work
- [ ] No package dependency conflicts

---

### Project: PublicApi
**Path**: `src\PublicApi\PublicApi.csproj`
**Current State**: net8.0, REST API with JWT authentication, 27 issues (7 mandatory, 18 potential, 2 optional)
**Target State**: net10.0
**Complexity**: High
**Dependencies**: ApplicationCore, Infrastructure
**Used By**: FunctionalTests, PublicApiIntegrationTests

#### Migration Steps

**1. Prerequisites**
- ApplicationCore upgraded to net10.0
- Infrastructure upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `PublicApi.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.2 | 10.0.2 | Framework compatibility |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | **Keep or Remove** | **Deprecated** - see note below |
| System.IdentityModel.Tokens.Jwt | 7.4.1 | 8.16.0 (latest 8.x) | **Deprecated** - upgrade to LTS |

Other packages (compatible, no update):
- Ardalis.ApiEndpoints 4.1.0 ✅
- MediatR 12.2.0 ✅
- MinimalApi.Endpoint 1.3.0 ✅
- Swashbuckle.AspNetCore 6.5.0 ✅
- Swashbuckle.AspNetCore.Annotations 6.5.0 ✅
- Swashbuckle.AspNetCore.SwaggerUI 6.5.0 ✅

**Deprecated Packages Investigation**:

1. **AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1**
   - Deprecated: Functionality now in AutoMapper directly
   - **Action**: Check if AutoMapper (base package) is referenced. If yes, remove this package and use `AddAutoMapper()` from base package. If not, add AutoMapper base package.
   - Assessment says "keep 12.0.1" - verify compatibility or use newer approach

2. **System.IdentityModel.Tokens.Jwt 7.4.1**
   - Deprecated: Upgrade to version 8.16.0 (LTS)
   - **Action**: Update to 8.16.0 - used in JWT authentication

**4. Expected Breaking Changes**

**Critical: JWT Bearer Authentication API Changes** (6 occurrences in `Program.cs`):
- Line 55-70: JWT authentication configuration
- Line 57: `JwtBearerDefaults.AuthenticationScheme` (source incompatible)
- Line 61-63: `JwtBearerOptions` properties (source incompatible)
  - `RequireHttpsMetadata`
  - `SaveToken`
  - `TokenValidationParameters`
- Line 86: `AddJwtBearer()` extension method (source incompatible)

**Impact**: Source incompatible - code will not compile without updates

**Other Breaking Changes**:
- Line 86: `AddMediatR()` - ServiceCollectionExtensions binary incompatible
- Line 87: `AddAutoMapper()` - ServiceCollectionExtensions binary incompatible (also related to deprecated package)

**5. Code Modifications**

**File**: `src\PublicApi\Program.cs`

**Lines 55-70: JWT Bearer Authentication Configuration**
Current pattern (NET 8):
```csharp
builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
```

**Expected Changes for .NET 10**:
- Review JWT Bearer API changes: https://go.microsoft.com/fwlink/?linkid=2262679
- `JwtBearerDefaults` may be in different namespace or have different structure
- `JwtBearerOptions` properties may have changed or require different initialization
- May need to use new authentication configuration patterns

**Line 86-87: Dependency Injection**
- `AddMediatR()` - May require different registration pattern in .NET 10
- `AddAutoMapper()` - Related to deprecated package, may need different approach

**Recommended Approach**:
1. Build project after package updates
2. Address compilation errors from JWT authentication first (most critical)
3. Fix MediatR registration
4. Fix AutoMapper registration (or remove deprecated package)
5. Test authentication thoroughly (JWT token generation and validation)

**6. Testing Strategy**
- Build project successfully
- Run PublicApiIntegrationTests
- **Critical**: Test JWT authentication endpoints
  - Token generation
  - Token validation
  - Authenticated API calls
- Verify Swagger UI works
- Test API endpoints with authentication required
- Verify Entity Framework queries
- Check AutoMapper mappings

**7. Validation Checklist**
- [ ] `PublicApi.csproj` targets net10.0
- [ ] All ASP.NET Core packages updated to 10.0.3
- [ ] System.IdentityModel.Tokens.Jwt updated to 8.16.0
- [ ] AutoMapper deprecated package addressed (removed or updated)
- [ ] JWT authentication code updated for .NET 10 API changes
- [ ] MediatR registration updated
- [ ] AutoMapper registration updated
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] PublicApiIntegrationTests pass
- [ ] JWT token generation works
- [ ] JWT token validation works
- [ ] API endpoints respond correctly
- [ ] Swagger UI loads
- [ ] No package dependency conflicts

---

### Project: Web
**Path**: `src\Web\Web.csproj`
**Current State**: net8.0, main Razor Pages web application with Identity, 40 issues (8 mandatory, 29 potential, 3 optional)
**Target State**: net10.0
**Complexity**: Very High (highest in solution)
**Dependencies**: BlazorAdmin, ApplicationCore, BlazorShared, Infrastructure (4 dependencies - most complex)
**Used By**: UnitTests, FunctionalTests, PublicApiIntegrationTests

#### Migration Steps

**1. Prerequisites**
- BlazorShared upgraded to net10.0
- ApplicationCore upgraded to net10.0
- Infrastructure upgraded to net10.0
- BlazorAdmin upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `Web.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.Server | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.2 | 10.0.2 | Framework compatibility |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | **Keep or Remove** | **Deprecated** |
| System.IdentityModel.Tokens.Jwt | 7.4.1 | 8.16.0 | **Deprecated** |
| Azure.Identity | 1.13.2 | 1.18.0 | **Deprecated** - depends on deprecated MSAL version |

Other packages (compatible, no update):
- Ardalis.ListStartupServices 1.1.4 ✅
- Ardalis.Specification 8.0.0 ✅
- MediatR 12.2.0 ✅
- Microsoft.Azure.AppConfiguration.AspNetCore 7.1.0 ✅
- Microsoft.FeatureManagement.AspNetCore 3.2.0 ✅

**Deprecated Packages Investigation**:

1. **AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1**
   - Same as PublicApi - functionality in AutoMapper directly

2. **System.IdentityModel.Tokens.Jwt 7.4.1**
   - Upgrade to 8.16.0 (LTS version)

3. **Azure.Identity 1.13.2**
   - Assessment recommends 1.18.0 (current version depends on deprecated MSAL)
   - **Action**: Update to 1.18.0 to remove deprecated MSAL dependency

**4. Expected Breaking Changes**

**Highest Concentration of Breaking Changes in Solution**:
- 7 binary incompatible APIs (mandatory fixes)
- 11 source incompatible APIs (compilation issues)
- 9 behavioral changes (runtime behavior differences)

**Critical Areas**:

**A. Configuration/DependencyInjection** (`ConfigureWebServices.cs`, `ConfigureCoreServices.cs`):
- Line 10 (ConfigureWebServices.cs): `AddMediatR()` - binary incompatible
- Line 14 (ConfigureWebServices.cs): `Configure<CatalogSettings>()` - binary incompatible
- Line 22 (ConfigureCoreServices.cs): `configuration.Get<CatalogSettings>()` - binary incompatible

**B. Cookie Settings** (`ConfigureCookieSettings.cs`):
- Line 25: `TimeSpan.FromMinutes()` - source incompatible (may require explicit typing)

**C. Health Checks** (`HomePageHealthCheck.cs`, `ApiHealthCheck.cs`):
- Line 26 (HomePageHealthCheck.cs): `HttpContent.ReadAsStringAsync()` - behavioral change
- Line 25 (ApiHealthCheck.cs): `HttpContent.ReadAsStringAsync()` - behavioral change
- **Impact**: May behave differently at runtime

**D. Exception Handling** (`Program.cs`):
- Line 197: `UseExceptionHandler("/Error")` - behavioral change

**5. Code Modifications**

**File**: `src\Web\Configuration\ConfigureWebServices.cs`
- Line 10: Update `AddMediatR()` call for .NET 10 API
- Line 14: Update `Configure<CatalogSettings>()` call for .NET 10 API

**File**: `src\Web\Configuration\ConfigureCoreServices.cs`
- Line 22: Update `configuration.Get<CatalogSettings>()` for .NET 10 API
- May need to use `configuration.Get<CatalogSettings>(options => ...)` pattern

**File**: `src\Web\Configuration\ConfigureCookieSettings.cs`
- Line 25: Review `TimeSpan.FromMinutes()` - may need explicit double cast

**File**: `src\Web\HealthChecks\HomePageHealthCheck.cs` and `ApiHealthCheck.cs`
- Lines 26/25: Review `ReadAsStringAsync()` behavioral change
- **Action**: Test health check endpoints, verify string reading works correctly

**File**: `src\Web\Program.cs`
- Line 197: Review `UseExceptionHandler()` behavioral change
- **Action**: Test error handling, verify error page displays correctly

**Recommended Approach**:
1. Build project after all package updates
2. Address compilation errors in order:
   - Configuration services (ConfigureWebServices, ConfigureCoreServices)
   - Cookie settings
   - Other compilation errors
3. After successful build, test for behavioral changes:
   - Health checks (may behave differently)
   - Exception handling (error pages)
   - HTTP content reading
4. Thoroughly test Identity authentication/authorization
5. Test Blazor components integration (BlazorAdmin embedded)

**6. Testing Strategy**

**Build Validation**:
- Build project successfully
- Address all compilation errors
- Ensure 0 warnings

**Functional Testing**:
- Run application locally
- Test authentication flows (login, logout, register)
- Test authorization (role-based access)
- Test Razor Pages rendering
- Test Blazor admin component integration
- **Critical**: Test health check endpoints (`/health`)
- Test error handling (trigger errors, verify error page)
- Test catalog browsing
- Test basket functionality
- Test order placement

**Automated Testing**:
- Run UnitTests (depends on Web)
- Run FunctionalTests (depends on Web)
- Run PublicApiIntegrationTests (depends on Web)

**7. Validation Checklist**
- [ ] `Web.csproj` targets net10.0
- [ ] All ASP.NET Core packages updated to 10.0.3
- [ ] System.IdentityModel.Tokens.Jwt updated to 8.16.0
- [ ] Azure.Identity updated to 1.18.0
- [ ] AutoMapper deprecated package addressed
- [ ] MediatR registration updated
- [ ] Configuration binding updated
- [ ] Cookie settings updated
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Application starts successfully
- [ ] Authentication works (login/logout/register)
- [ ] Authorization works (role-based pages)
- [ ] Razor Pages render correctly
- [ ] Blazor admin components work
- [ ] Health checks respond correctly
- [ ] Error handling works (error page displays)
- [ ] Catalog browsing works
- [ ] Basket functionality works
- [ ] UnitTests pass
- [ ] FunctionalTests pass
- [ ] PublicApiIntegrationTests pass
- [ ] No package dependency conflicts

---

### Project: UnitTests
**Path**: `tests\UnitTests\UnitTests.csproj`
**Current State**: net8.0, unit tests for core domain logic, 2 issues (1 mandatory, 1 optional)
**Target State**: net10.0
**Complexity**: Low
**Dependencies**: Web, ApplicationCore
**Used By**: IntegrationTests

#### Migration Steps

**1. Prerequisites**
- ApplicationCore upgraded to net10.0
- Web upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `UnitTests.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Mvc (if explicit) | 2.2.0 | Remove | Very old, deprecated, likely unnecessary |

Test framework packages (compatible):
- MSTest.TestAdapter 3.2.2 ✅
- MSTest.TestFramework 3.2.2 ✅
- Moq 4.20.70 ✅
- Microsoft.NET.Test.Sdk 17.9.0 ✅

**Deprecated Package**:
- **Microsoft.AspNetCore.Mvc 2.2.0** (if present) - very old version from .NET Core 2.2
- **Action**: Remove this package reference - functionality is built into ASP.NET Core framework

**4. Expected Breaking Changes**
Minimal - mostly related to test target (ApplicationCore, Web) API changes

**5. Code Modifications**
- Update test code if testing APIs that changed in ApplicationCore or Web
- Remove any references to obsolete ASP.NET MVC 2.2 APIs

**6. Testing Strategy**
- Build project successfully
- Run all unit tests
- Fix any test failures due to API changes in tested code

**7. Validation Checklist**
- [ ] `UnitTests.csproj` targets net10.0
- [ ] Deprecated Microsoft.AspNetCore.Mvc package removed
- [ ] Project builds without errors
- [ ] All unit tests pass
- [ ] No package dependency conflicts

---

### Project: IntegrationTests
**Path**: `tests\IntegrationTests\IntegrationTests.csproj`
**Current State**: net8.0, integration tests for data layer, 2 issues (1 mandatory, 1 potential)
**Target State**: net10.0
**Complexity**: Low
**Dependencies**: Infrastructure, UnitTests
**Used By**: None

#### Migration Steps

**1. Prerequisites**
- Infrastructure upgraded to net10.0
- UnitTests upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `IntegrationTests.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.3 | Framework compatibility |

Test framework packages (compatible):
- xUnit 2.7.0 ✅
- xunit.runner.console 2.7.0 ✅
- xunit.runner.visualstudio 2.5.7 ✅
- Microsoft.NET.Test.Sdk 17.9.0 ✅

**4. Expected Breaking Changes**
Minimal - mostly related to Infrastructure (EF Core) API changes

**5. Code Modifications**
- Update test code if Entity Framework queries changed behavior
- Address any logging configuration changes

**6. Testing Strategy**
- Build project successfully
- Run all integration tests
- Verify database operations work correctly with EF Core 10.0.3

**7. Validation Checklist**
- [ ] `IntegrationTests.csproj` targets net10.0
- [ ] Microsoft.Extensions.Logging.Configuration updated to 10.0.3
- [ ] Project builds without errors
- [ ] All integration tests pass
- [ ] EF Core queries work correctly
- [ ] No package dependency conflicts

---

### Project: FunctionalTests
**Path**: `tests\FunctionalTests\FunctionalTests.csproj`
**Current State**: net8.0, end-to-end functional tests, 36 issues (5 mandatory, 31 potential)
**Target State**: net10.0
**Complexity**: High (highest for test projects)
**Dependencies**: Web, ApplicationCore, PublicApi
**Used By**: None

#### Migration Steps

**1. Prerequisites**
- ApplicationCore upgraded to net10.0
- PublicApi upgraded to net10.0
- Web upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `FunctionalTests.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Mvc.Testing | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Identity.Core | 8.0.3 | 10.0.3 | Framework compatibility |

Test framework packages (compatible):
- xUnit 2.7.0 ✅
- xunit.runner.visualstudio 2.5.7 ✅
- Microsoft.NET.Test.Sdk 17.9.0 ✅

**4. Expected Breaking Changes**

**High Issue Count (36 issues)** primarily due to:
- Testing APIs from Web (40 issues) and PublicApi (27 issues)
- API behavioral changes may affect test assertions
- Test host configuration may need updates

**Common Areas**:
- `WebApplicationFactory` configuration
- HTTP client usage in tests
- Authentication/authorization testing
- API endpoint testing

**5. Code Modifications**
- Update test code for API changes in Web and PublicApi
- Fix assertions that may fail due to behavioral changes
- Update test host configuration if `AspNetCore.Mvc.Testing` API changed
- Review authentication testing (JWT bearer changes in PublicApi)

**6. Testing Strategy**
- Build project successfully
- Run all functional tests (may have failures initially)
- Triage failures:
  - Test code needs updating (fix test)
  - Application behavior changed (validate change is correct, update assertion)
  - Actual regression (fix application code)

**7. Validation Checklist**
- [ ] `FunctionalTests.csproj` targets net10.0
- [ ] Microsoft.AspNetCore.Mvc.Testing updated to 10.0.3
- [ ] Microsoft.Extensions.Identity.Core updated to 10.0.3
- [ ] Project builds without errors
- [ ] All functional tests pass
- [ ] Test host configuration works
- [ ] HTTP client tests work
- [ ] Authentication tests work
- [ ] No package dependency conflicts

---

### Project: PublicApiIntegrationTests
**Path**: `tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj`
**Current State**: net8.0, API integration tests, 13 issues (5 mandatory, 8 potential)
**Target State**: net10.0
**Complexity**: Medium
**Dependencies**: Web, PublicApi
**Used By**: None

#### Migration Steps

**1. Prerequisites**
- Web upgraded to net10.0
- PublicApi upgraded to net10.0

**2. Update Target Framework**
```xml
<TargetFramework>net10.0</TargetFramework>
```
Change from `net8.0` to `net10.0` in `PublicApiIntegrationTests.csproj`

**3. Package Updates**

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Mvc.Testing | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.Authentication | 8.0.3 | 10.0.3 | Framework compatibility |

Test framework packages (compatible):
- xUnit 2.7.0 ✅
- xunit.runner.visualstudio 2.5.7 ✅
- Microsoft.NET.Test.Sdk 17.9.0 ✅

**4. Expected Breaking Changes**

**Medium Issue Count (13 issues)** primarily due to:
- Testing JWT authentication in PublicApi (many API changes there)
- Testing Web integration with PublicApi
- WebAssembly authentication component testing

**Focus Areas**:
- JWT Bearer authentication testing
- API endpoint testing
- Token generation/validation tests

**5. Code Modifications**
- Update test code for JWT authentication API changes
- Fix test authentication configuration if needed
- Update API endpoint test assertions

**6. Testing Strategy**
- Build project successfully
- Run all API integration tests
- Verify JWT authentication tests work
- Verify API endpoint tests pass

**7. Validation Checklist**
- [ ] `PublicApiIntegrationTests.csproj` targets net10.0
- [ ] Microsoft.AspNetCore.Mvc.Testing updated to 10.0.3
- [ ] Microsoft.AspNetCore.Components.WebAssembly.Authentication updated to 10.0.3
- [ ] Project builds without errors
- [ ] All integration tests pass
- [ ] Authentication tests work
- [ ] API endpoint tests work
- [ ] No package dependency conflicts

---

---

## Package Update Reference

This section consolidates all NuGet package updates across the solution for the All-at-Once upgrade.

### Common Package Updates (Affecting Multiple Projects)

These packages appear in multiple projects and should be updated consistently:

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.3 | 10.0.3 | 2 (Web, PublicApi) | Framework compatibility - ASP.NET Core 10 |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.3 | 3 (Infrastructure, Web, PublicApi) | Framework compatibility - ASP.NET Core 10 |
| Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.3 | 2 (Web, PublicApi) | Framework compatibility - ASP.NET Core 10 |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.3 | 10.0.3 | 2 (Web, PublicApi) | Framework compatibility - ASP.NET Core 10 |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.3 | 10.0.3 | 3 (Infrastructure, Web, PublicApi) | Framework compatibility - EF Core 10 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.3 | 3 (Infrastructure, Web, PublicApi) | Framework compatibility - EF Core 10 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.3 | 3 (Infrastructure, Web, PublicApi) | Framework compatibility - EF Core 10 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.2 | 10.0.2 | 2 (Web, PublicApi) | Framework compatibility - code generation |
| System.Text.Json | 8.0.5 | 10.0.3 | 1 (ApplicationCore) | Framework compatibility |
| System.Net.Http.Json | 8.0.0 | 10.0.3 | 1 (BlazorAdmin) | Framework compatibility |

### Category-Specific Updates

**Blazor/WebAssembly Projects** (BlazorAdmin):
| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Components.WebAssembly | 8.0.3 | 10.0.3 | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 8.0.3 | 10.0.3 | Framework compatibility |

**Test Projects**:
| Package | Current | Target | Projects | Reason |
|---------|---------|--------|----------|--------|
| Microsoft.AspNetCore.Mvc.Testing | 8.0.3 | 10.0.3 | FunctionalTests, PublicApiIntegrationTests | Test host compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.Authentication | 8.0.3 | 10.0.3 | PublicApiIntegrationTests | WebAssembly auth testing |
| Microsoft.Extensions.Identity.Core | 8.0.3 | 10.0.3 | FunctionalTests | Identity testing |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.3 | IntegrationTests | Logging compatibility |

**Web Application Specific** (Web):
| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Components.WebAssembly.Server | 8.0.3 | 10.0.3 | Blazor WebAssembly hosting |

### Deprecated Packages Requiring Action

🔴 **Critical: Deprecated packages that need investigation and potential replacement**

| Package | Current | Suggested | Projects Affected | Action Required | Priority |
|---------|---------|-----------|-------------------|-----------------|----------|
| **AutoMapper.Extensions.Microsoft.DependencyInjection** | 12.0.1 | Keep or Remove | Web, PublicApi | Functionality moved to AutoMapper base. Check if base package referenced, remove extension if redundant | **HIGH** |
| **System.IdentityModel.Tokens.Jwt** | 7.4.1 | 8.16.0 (LTS) | Infrastructure, Web, PublicApi | Upgrade to latest LTS version (8.16.0). See https://aka.ms/IdentityModel/LTS | **CRITICAL** |
| **Azure.Identity** | 1.13.2 | 1.18.0 | Web | Upgrade to remove deprecated MSAL dependency | **MEDIUM** |
| **Microsoft.AspNetCore.Mvc** | 2.2.0 | Remove | UnitTests (likely) | Very old (.NET Core 2.2), remove explicit reference - built into framework | **MEDIUM** |

### Packages Staying at Current Version (Compatible)

The following packages are compatible with .NET 10.0 and do not require updates:

**Third-Party Libraries**:
- Ardalis.ApiEndpoints 4.1.0 ✅
- Ardalis.GuardClauses 4.5.0 ✅
- Ardalis.ListStartupServices 1.1.4 ✅
- Ardalis.Specification 8.0.0 ✅
- Ardalis.Specification.EntityFrameworkCore 8.0.0 ✅
- Blazored.LocalStorage 4.5.0 ✅
- BlazorInputFile 0.2.0 ✅ (verify compatibility)
- FluentValidation 11.9.0 ✅
- MediatR 12.2.0 ✅
- Microsoft.Azure.AppConfiguration.AspNetCore 7.1.0 ✅
- Microsoft.FeatureManagement.AspNetCore 3.2.0 ✅
- MinimalApi.Endpoint 1.3.0 ✅
- Moq 4.20.70 ✅
- Swashbuckle.AspNetCore 6.5.0 ✅
- Swashbuckle.AspNetCore.Annotations 6.5.0 ✅
- Swashbuckle.AspNetCore.SwaggerUI 6.5.0 ✅

**Test Frameworks**:
- xUnit 2.7.0 ✅
- xunit.runner.console 2.7.0 ✅
- xunit.runner.visualstudio 2.5.7 ✅
- MSTest.TestAdapter 3.2.2 ✅
- MSTest.TestFramework 3.2.2 ✅
- Microsoft.NET.Test.Sdk 17.9.0 ✅
- coverlet.collector 6.0.2 ✅

### Package Update Summary by Project

| Project | Total Packages | Packages to Update | Deprecated to Address | Priority |
|---------|----------------|-------------------|----------------------|----------|
| **ApplicationCore** | 4 | 1 | 0 | Low |
| **BlazorShared** | 0 | 0 | 0 | Low |
| **BlazorAdmin** | 5 | 3 | 0 | Medium |
| **Infrastructure** | 5 | 3 | 1 (JWT) | High |
| **PublicApi** | 16 | 8 | 2 (JWT, AutoMapper) | **Very High** |
| **Web** | 17 | 9 | 3 (JWT, AutoMapper, Azure.Identity) | **Critical** |
| **UnitTests** | 4+ | 0 | 1 (ASP.NET MVC 2.2) | Low |
| **IntegrationTests** | 5+ | 1 | 0 | Low |
| **FunctionalTests** | 5+ | 2 | 0 | Medium |
| **PublicApiIntegrationTests** | 5+ | 2 | 0 | Medium |

### Package Update Execution Order

In All-at-Once strategy, all packages update simultaneously, but priority indicates focus areas:

1. **Critical**: Web, PublicApi (most deprecated packages, most breaking changes)
2. **High**: Infrastructure (JWT deprecated package)
3. **Medium**: Test projects, BlazorAdmin
4. **Low**: Foundation libraries (ApplicationCore, BlazorShared)

---

## Breaking Changes Catalog

This section documents all identified breaking changes from the assessment, organized by category and severity.

### Breaking Changes Summary

- **Binary Incompatible (Mandatory)**: 26 occurrences - require recompilation, may need code changes
- **Source Incompatible (Potential)**: 20 occurrences - require code changes to compile
- **Behavioral Changes (Potential)**: 52 occurrences - may affect runtime behavior without compilation errors

### Category 1: Configuration & Dependency Injection API Changes

**Severity**: Binary Incompatible (Mandatory)

These APIs changed in .NET 10 and will cause compilation errors.

| API | Location | Impact | Recommended Action |
|-----|----------|--------|-------------------|
| `OptionsConfigurationServiceCollectionExtensions.Configure<T>()` | Web/ConfigureWebServices.cs:14 | Binary incompatible | Review Options configuration API in .NET 10 docs |
| `ConfigurationBinder.Get<T>()` | Web/ConfigureCoreServices.cs:22 | Binary incompatible | May need explicit binding options parameter |
| `ServiceCollectionExtensions` (MediatR) | Web/ConfigureWebServices.cs:10, PublicApi/Program.cs:86 | Binary incompatible | Update MediatR registration pattern for .NET 10 |
| `ServiceCollectionExtensions` (AutoMapper) | PublicApi/Program.cs:87 | Binary incompatible | Related to deprecated package, use new AutoMapper DI pattern |

**Documentation**: https://go.microsoft.com/fwlink/?linkid=2262679

**Common Resolution Pattern**:
```csharp
// Old (.NET 8):
services.Configure<Settings>(configuration);

// New (.NET 10) - may require:
services.Configure<Settings>(configuration, options => { ... });
```

---

### Category 2: JWT Bearer Authentication API Changes

**Severity**: Source Incompatible (Potential) + Binary Incompatible (Mandatory)

**Critical for PublicApi and Infrastructure projects**.

| API | Location | Severity | Impact |
|-----|----------|----------|--------|
| `JwtSecurityTokenHandler` constructor | Infrastructure/IdentityTokenClaimService.cs:24 | Binary incompatible | Constructor may have changed |
| `JwtSecurityTokenHandler.CreateToken()` | Infrastructure/IdentityTokenClaimService.cs:41 | Binary incompatible | Method signature may differ |
| `JwtSecurityTokenHandler.WriteToken()` | Infrastructure/IdentityTokenClaimService.cs:42 | Binary incompatible | Method signature may differ |
| `JwtBearerDefaults.AuthenticationScheme` | PublicApi/Program.cs:57 | Source incompatible | Constant may be in different namespace or structure |
| `JwtBearerOptions` properties | PublicApi/Program.cs:61-63 | Source incompatible | Property API may have changed |
| `AddJwtBearer()` extension | PublicApi/Program.cs:55 | Source incompatible | Extension method signature may differ |

**Impact**: JWT authentication may not compile or function without updates.

**Affected Functionality**:
- Token generation (Infrastructure)
- Token validation (Infrastructure)
- JWT Bearer authentication configuration (PublicApi)
- API authentication/authorization (PublicApi)

**Recommended Actions**:
1. Review System.IdentityModel.Tokens.Jwt 8.16.0 migration guide
2. Update token handler instantiation and usage
3. Update JWT Bearer configuration in PublicApi
4. Thoroughly test authentication after changes
5. See: https://aka.ms/IdentityModel/LTS

---

### Category 3: HTTP Content & Network API Changes

**Severity**: Behavioral Change (Potential)

These APIs changed behavior - code compiles but may behave differently at runtime.

| API | Location | Behavioral Change | Potential Impact |
|-----|----------|-------------------|------------------|
| `HttpContent.ReadAsStringAsync()` | Web/HealthChecks/HomePageHealthCheck.cs:26 | Changed behavior in .NET 10 | String reading may have different encoding or error handling |
| `HttpContent.ReadAsStringAsync()` | Web/HealthChecks/ApiHealthCheck.cs:25 | Changed behavior in .NET 10 | Same as above |

**Impact**: Health checks may read HTTP responses differently, potentially affecting health status reporting.

**Recommended Actions**:
1. Test health check endpoints after upgrade
2. Verify string content reads correctly
3. Check if encoding handling changed
4. Monitor health check logs for unexpected behavior

**Documentation**: https://go.microsoft.com/fwlink/?linkid=2262679

---

### Category 4: TimeSpan API Changes

**Severity**: Source Incompatible (Potential)

| API | Location | Impact | Recommended Action |
|-----|----------|--------|-------------------|
| `TimeSpan.FromMinutes(double)` | Web/ConfigureCookieSettings.cs:25 | Source incompatible | May require explicit double cast or different overload |

**Example**:
```csharp
// May need explicit typing:
options.ExpireTimeSpan = TimeSpan.FromMinutes((double)ValidityMinutesPeriod);
```

---

### Category 5: Exception Handling Middleware

**Severity**: Behavioral Change (Potential)

| API | Location | Behavioral Change | Potential Impact |
|-----|----------|-------------------|------------------|
| `UseExceptionHandler(string)` | Web/Program.cs:197 | Middleware behavior changed | Error page handling may differ in .NET 10 |

**Impact**: Error pages may render differently or use different error handling pipeline.

**Recommended Actions**:
1. Test error handling (trigger errors deliberately)
2. Verify error page displays correctly
3. Check error logging still works
4. Verify production error handling is secure

---

### Category 6: Other Behavioral Changes

**Severity**: Potential

**Count**: 52 total behavioral changes flagged by assessment

The assessment identified 52 potential behavioral changes but many lack specific details. These are distributed across:
- Web: 9 behavioral changes
- PublicApi: 1 behavioral change
- Other projects: API usage from Web/PublicApi

**General Areas to Monitor**:
- ASP.NET Core middleware pipeline changes
- Entity Framework query translation differences
- Authentication/authorization behavior
- Blazor component lifecycle changes
- JSON serialization behavior (System.Text.Json)

**Mitigation Strategy**:
- Comprehensive testing after upgrade (all 4 test projects)
- Manual smoke testing of key functionality
- Monitor application logs after deployment
- Review .NET 10 release notes for known behavioral changes
- Phased production rollout to detect issues early

---

### Breaking Changes by Project

#### Infrastructure
- **Binary Incompatible**: 4 (all JWT-related)
- **Focus**: JWT token handler API changes

#### PublicApi
- **Binary Incompatible**: 6
- **Source Incompatible**: 9 (JWT authentication configuration)
- **Behavioral Changes**: 1
- **Focus**: JWT authentication, AutoMapper, MediatR

#### Web
- **Binary Incompatible**: 7
- **Source Incompatible**: 11
- **Behavioral Changes**: 9
- **Focus**: Configuration, DI, HTTP clients, exception handling, cookies

#### Test Projects
- **Various**: API usage from Web/PublicApi may need updates in test code

---

### Breaking Changes Resolution Priority

**Priority 1 (Critical - Blocks Compilation)**:
1. JWT authentication APIs (Infrastructure, PublicApi)
2. Configuration/DI APIs (Web, PublicApi)
3. MediatR registration (Web, PublicApi)

**Priority 2 (High - Compilation Warnings/Errors)**:
1. AutoMapper registration (Web, PublicApi)
2. TimeSpan API (Web)
3. Source incompatible APIs

**Priority 3 (Medium - Runtime Issues)**:
1. Behavioral changes (health checks, exception handling)
2. HTTP content reading
3. Test code updates

**Priority 4 (Low - Monitor After Deployment)**:
1. Other behavioral changes (no specific details)
2. Framework-level behavior differences

---

### External Resources

**Microsoft Breaking Changes Documentation**:
- Main: https://go.microsoft.com/fwlink/?linkid=2262679
- .NET 10 Breaking Changes: https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0

**Package-Specific Migration Guides**:
- System.IdentityModel.Tokens.Jwt: https://aka.ms/IdentityModel/LTS
- AutoMapper: Check https://docs.automapper.org for DI changes
- MediatR: Check https://github.com/jbogard/MediatR for .NET 10 compatibility

**ASP.NET Core 10.0 Migration**:
- ASP.NET Core migration guide (when available)
- Entity Framework Core 10.0 breaking changes

---

### Tracking Breaking Changes During Upgrade

**During Atomic Upgrade**:
1. Address compilation errors in dependency order (Level 0 → Level 5)
2. Document each breaking change fix applied
3. Note any workarounds or temporary solutions
4. Track behavioral changes observed during testing

**Post-Upgrade Monitoring**:
1. Monitor application logs for unexpected exceptions
2. Track performance metrics (behavioral changes may affect performance)
3. User-reported issues may reveal behavioral changes
4. Security audit (especially JWT authentication changes)

---

## Risk Management

### High-Level Risk Assessment

The All-at-Once strategy for eShopOnWeb presents manageable risks given the solution's size and structure:

**Overall Risk Level**: **Medium**

**Risk Factors**:
- ✅ **Mitigating**: Small solution size (10 projects), clear dependencies, good test coverage
- ⚠️ **Elevating**: 2 high-complexity projects (Web: 40 issues, PublicApi: 27 issues), 4 deprecated packages, 147 total issues

### Risk Categories

#### 1. Breaking Changes Risk - **HIGH**

**Description**: 26 binary incompatible APIs and 20 source incompatible APIs may cause compilation failures or runtime errors.

**Impact**: Build failures, runtime exceptions, behavioral changes

**Affected Projects**:
- Web (7 binary, 11 source incompatibilities)
- PublicApi (6 binary, 9 source incompatibilities)
- FunctionalTests (API usage from Web/PublicApi)
- PublicApiIntegrationTests (API usage)
- Infrastructure (binary incompatibilities)

**Mitigation Strategies**:
- Comprehensive breaking changes catalog documents expected issues
- Address compilation errors in dependency order (foundation first)
- Reference Microsoft breaking changes documentation: https://go.microsoft.com/fwlink/?linkid=2262679
- Test suite validates behavioral correctness after fixes

**Contingency**:
- If breaking changes block upgrade, document incompatible APIs and research alternatives
- Consider temporary workarounds using reflection or compatibility shims
- Defer non-critical features if APIs have no .NET 10 equivalent

#### 2. Deprecated Packages Risk - **MEDIUM**

**Description**: 4 packages are deprecated and may have security issues or missing .NET 10 support.

**Affected Packages & Projects**:
1. **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.1)
   - Projects: PublicApi, Web
   - Concern: Deprecated dependency injection extension
   - Alternative: Check AutoMapper docs for replacement package or use manual registration

2. **Azure.Identity** (1.13.2)
   - Projects: Web
   - Concern: Marked deprecated but may be false positive (active package)
   - Alternative: Verify on NuGet.org, likely compatible despite flag

3. **Microsoft.AspNetCore.Mvc** (2.2.0)
   - Projects: Web (implied from assessment)
   - Concern: Very old version (2.2), likely obsolete reference
   - Alternative: Remove explicit reference (included in ASP.NET Core framework)

4. **System.IdentityModel.Tokens.Jwt** (7.4.1)
   - Projects: Web, PublicApi
   - Concern: May have newer recommended version
   - Alternative: Check for Microsoft.IdentityModel.Tokens or keep current if compatible

**Mitigation Strategies**:
- Research each deprecated package before upgrade
- Check NuGet.org for deprecation messages and recommended alternatives
- Test functionality thoroughly after any package replacement
- Monitor security advisories for deprecated packages

**Contingency**:
- If replacement not available, document risk and plan separate upgrade
- If functionality critical, keep deprecated package temporarily with plan to replace
- Add security scanning to CI/CD pipeline

#### 3. Behavioral Changes Risk - **MEDIUM**

**Description**: 52 potential behavioral changes may alter runtime behavior without compilation errors.

**Impact**: Subtle bugs, performance changes, different runtime behavior

**Affected Areas**:
- Web: 9 behavioral changes
- FunctionalTests: API behavioral differences
- PublicApi: 1 behavioral change
- All projects: Framework-level behavior differences

**Mitigation Strategies**:
- Run full test suite (4 test projects) after upgrade
- Perform smoke testing of key functionality
- Review behavioral change documentation: https://go.microsoft.com/fwlink/?linkid=2262679
- Monitor application logs after deployment for unexpected behavior

**Contingency**:
- Document all behavioral changes discovered during testing
- Add regression tests for changed behaviors
- Implement feature flags for critical changed functionality
- Plan gradual rollout to production

#### 4. Build Complexity Risk - **LOW**

**Description**: Simultaneous upgrade of 10 projects creates larger compilation surface.

**Impact**: More compilation errors to resolve, longer troubleshooting

**Mitigation Strategies**:
- Follow dependency-ordered fix approach (Level 0 → Level 5)
- Use breaking changes catalog to anticipate common errors
- Build incrementally by level if needed (build foundation, then dependents)
- MSBuild error messages guide to specific files/lines

**Contingency**:
- If overwhelming errors, temporarily revert specific projects to isolate issues
- Use `#if NET10_0` conditional compilation for framework-specific code
- Reference .NET API browser for correct .NET 10 APIs

#### 5. Test Failures Risk - **MEDIUM**

**Description**: 4 test projects (36+13+2+2 = 53 issues total) may have failures after upgrade.

**Impact**: Unknown regression, blocked deployment, reduced confidence

**Affected Test Projects**:
- FunctionalTests (36 issues - highest test complexity)
- PublicApiIntegrationTests (13 issues)
- UnitTests (2 issues)
- IntegrationTests (2 issues)

**Mitigation Strategies**:
- Expect test failures due to API/behavior changes
- Fix test code using same breaking changes guidance as application code
- Distinguish between "test needs updating" vs "application has bug"
- Run tests in isolation to identify specific failures

**Contingency**:
- If test failures due to test framework issues, update xUnit/MSTest packages
- If many failures, triage by priority (unit → integration → functional)
- Document expected test changes in breaking changes catalog
- Consider temporary test disabling for non-critical scenarios (with tracking)

### Security Vulnerabilities

**Current Status**: Assessment did not flag CVEs, but deprecated packages warrant attention.

**Deprecated Packages Requiring Security Review**:
- AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
- Azure.Identity (1.13.2) - likely false positive
- Microsoft.AspNetCore.Mvc (2.2.0) - very old version
- System.IdentityModel.Tokens.Jwt (7.4.1)

**Security Mitigation**:
- Scan solution with `dotnet list package --vulnerable` after upgrade
- Check NuGet.org advisories for each deprecated package
- Prioritize replacement of packages with known vulnerabilities
- Add automated vulnerability scanning to CI/CD

### Contingency Plans

#### If Atomic Upgrade Fails

**Scenario**: Compilation errors too complex to resolve in single pass

**Fallback Plan**:
1. Revert all changes (git checkout main)
2. Analyze most problematic projects from error logs
3. Create isolated branch to fix high-complexity projects (Web, PublicApi)
4. Research breaking changes specific to those projects
5. Retry atomic upgrade with pre-identified fixes

#### If Tests Fail After Upgrade

**Scenario**: Application builds but tests reveal regressions

**Fallback Plan**:
1. Categorize failures: test code issue vs application code issue
2. Fix critical path tests first (unit tests, core functionality)
3. Document behavioral changes requiring application updates
4. Consider temporary test exclusion for non-critical failing tests
5. Create issues to track deferred test fixes

#### If Deprecated Package Blocks Functionality

**Scenario**: Deprecated package has no .NET 10 compatible replacement

**Fallback Plan**:
1. Research package maintainer status and community alternatives
2. Evaluate extracting/reimplementing limited needed functionality
3. Check if functionality can be removed (feature flag)
4. Document technical debt and plan future replacement
5. Accept risk if functionality is non-critical

#### If Performance Degrades

**Scenario**: .NET 10 behavioral changes cause performance issues

**Fallback Plan**:
1. Profile application to identify slow paths
2. Review .NET 10 performance guidance and JIT changes
3. Apply performance best practices for .NET 10
4. Tune GC settings if memory behavior changed
5. Consider reverting specific code patterns if necessary

### Rollback Strategy

**Clean Rollback** (before commit):
```bash
git checkout main
git branch -D upgrade-to-NET10
```

**Post-Commit Rollback** (if issues found later):
```bash
git revert <commit-hash>
```

**Rollback Risk**: Low - All-at-Once in single commit enables clean revert

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

The All-at-Once strategy requires comprehensive testing after the atomic upgrade completes. Testing occurs in phases, progressing from quick validation to thorough verification.

---

### Phase 1: Build Validation (Immediate)

**Objective**: Verify solution builds successfully after all changes applied.

**Steps**:
1. Restore NuGet packages: `dotnet restore eShopOnWeb.sln`
2. Clean build: `dotnet clean eShopOnWeb.sln`
3. Full build: `dotnet build eShopOnWeb.sln --no-incremental`

**Success Criteria**:
- ✅ All 10 projects build successfully
- ✅ 0 compilation errors
- ✅ 0 warnings (or only acceptable warnings documented)
- ✅ No package dependency conflicts

**If Build Fails**:
- Review compilation errors in dependency order (fix Level 0 first)
- Consult Breaking Changes Catalog for known issues
- Address errors project-by-project following dependency hierarchy

---

### Phase 2: Unit Testing (Quick Validation)

**Objective**: Validate core domain logic still works correctly.

**Test Project**: UnitTests

**Execution**:
```bash
dotnet test tests\UnitTests\UnitTests.csproj
```

**Focus Areas**:
- Domain model logic (ApplicationCore)
- Business rules validation
- Service layer functionality

**Success Criteria**:
- ✅ All unit tests pass
- ✅ No test failures due to framework changes
- ✅ Test execution time comparable to .NET 8

**If Tests Fail**:
- Distinguish between:
  - Test code needs updating (API changes in test assertions)
  - Domain logic broken (actual regression - fix immediately)
- Update test code for .NET 10 API changes
- Re-run after fixes

---

### Phase 3: Integration Testing (Data Layer)

**Objective**: Validate Entity Framework and data access work correctly.

**Test Project**: IntegrationTests

**Execution**:
```bash
dotnet test tests\IntegrationTests\IntegrationTests.csproj
```

**Focus Areas**:
- Entity Framework Core queries
- Database context initialization
- Repository patterns
- Data seeding

**Success Criteria**:
- ✅ All integration tests pass
- ✅ EF Core queries work correctly with 10.0.3
- ✅ No database connection issues
- ✅ Data access patterns functional

**If Tests Fail**:
- Check EF Core breaking changes (query translation, behavior)
- Verify connection strings still valid
- Review LINQ query differences in EF Core 10
- Update queries if translation changed

---

### Phase 4: API Integration Testing

**Objective**: Validate REST API and authentication work correctly.

**Test Projects**: PublicApiIntegrationTests

**Execution**:
```bash
dotnet test tests\PublicApiIntegrationTests\PublicApiIntegrationTests.csproj
```

**Focus Areas**:
- JWT Bearer authentication (critical - many breaking changes)
- API endpoint responses
- Authorization rules
- Token generation and validation
- Swagger/OpenAPI integration

**Success Criteria**:
- ✅ All API integration tests pass
- ✅ JWT authentication works (token generation, validation)
- ✅ API endpoints respond correctly
- ✅ Authorization enforced correctly

**If Tests Fail**:
- **Priority**: JWT authentication issues (many breaking changes)
- Review JWT Bearer configuration changes
- Verify token generation matches expected format
- Check authorization policies still applied correctly

---

### Phase 5: Functional Testing (End-to-End)

**Objective**: Validate entire application workflows work correctly.

**Test Project**: FunctionalTests

**Execution**:
```bash
dotnet test tests\FunctionalTests\FunctionalTests.csproj
```

**Focus Areas**:
- End-to-end user workflows
- Web application functionality
- Razor Pages rendering
- Blazor component integration
- Full authentication flows
- Complete business scenarios (catalog, basket, ordering)

**Success Criteria**:
- ✅ All functional tests pass
- ✅ Web application workflows complete successfully
- ✅ No behavioral regressions detected
- ✅ Performance acceptable

**If Tests Fail**:
- High failure count expected initially (36 issues in this project)
- Triage failures:
  - Test code using changed APIs
  - Behavioral changes requiring test updates
  - Actual application regressions
- Update tests systematically
- Re-run until all pass or only acceptable failures remain

---

### Phase 6: Manual Smoke Testing

**Objective**: Validate functionality not covered by automated tests.

**Applications to Test**:
1. **Web** (main Razor Pages application)
2. **PublicApi** (REST API)
3. **BlazorAdmin** (admin UI)

#### Web Application Smoke Tests

**Authentication & Authorization**:
- [ ] User registration works
- [ ] Login works (credentials, JWT)
- [ ] Logout works
- [ ] Password reset flow works
- [ ] Role-based page access enforced

**Core Functionality**:
- [ ] Home page loads
- [ ] Catalog browsing works
- [ ] Product details display
- [ ] Search functionality works
- [ ] Basket add/remove/update works
- [ ] Checkout process completes
- [ ] Order history displays

**Health Checks** (behavioral changes flagged):
- [ ] `/health` endpoint responds
- [ ] Health check status accurate

**Error Handling** (behavioral changes flagged):
- [ ] Trigger error (e.g., invalid URL)
- [ ] Error page displays correctly
- [ ] Errors logged appropriately

#### PublicApi Smoke Tests

**Authentication**:
- [ ] POST `/api/token` generates valid JWT token
- [ ] Authenticated endpoints require valid token
- [ ] Invalid token returns 401

**API Endpoints**:
- [ ] GET `/api/catalog-items` returns data
- [ ] Swagger UI loads at `/swagger`
- [ ] API documentation displays correctly

#### BlazorAdmin Smoke Tests

**UI Rendering**:
- [ ] Admin UI loads in browser
- [ ] Blazor components render
- [ ] JavaScript interop works
- [ ] LocalStorage persistence works
- [ ] Navigation functions

---

### Phase 7: Performance & Behavioral Validation

**Objective**: Ensure no performance regressions or unexpected behavioral changes.

**Performance Checks**:
- [ ] Application startup time comparable to .NET 8
- [ ] Page load times acceptable
- [ ] API response times acceptable
- [ ] Memory usage stable
- [ ] No obvious performance degradation

**Behavioral Validation**:
- [ ] Review application logs for unexpected warnings/errors
- [ ] Verify no new exceptions logged
- [ ] Check behavioral changes noted in Breaking Changes Catalog:
  - HttpContent.ReadAsStringAsync() (health checks)
  - UseExceptionHandler (error handling)
  - Other flagged behavioral changes
- [ ] Confirm expected behaviors still occur

---

### Phase 8: Security Validation

**Objective**: Ensure security posture maintained or improved.

**Security Checks**:
- [ ] Run `dotnet list package --vulnerable` - 0 vulnerabilities
- [ ] JWT authentication secure (updated package, proper validation)
- [ ] Authorization rules enforced
- [ ] HTTPS enforced where required
- [ ] No sensitive data in logs
- [ ] Azure.Identity updated (removed deprecated MSAL dependency)
- [ ] Deprecated packages addressed (AutoMapper, JWT, Azure.Identity, ASP.NET MVC 2.2)

---

### Comprehensive Test Execution

**Run All Tests**:
```bash
# All test projects
dotnet test eShopOnWeb.sln

# With detailed output
dotnet test eShopOnWeb.sln --verbosity normal

# With coverage (if desired)
dotnet test eShopOnWeb.sln --collect:"XPlat Code Coverage"
```

**Expected Test Counts**:
- UnitTests: ~X tests (check baseline before upgrade)
- IntegrationTests: ~Y tests
- PublicApiIntegrationTests: ~Z tests
- FunctionalTests: ~W tests

**Success Criteria for All Tests**:
- ✅ 100% pass rate (or documented acceptable failures)
- ✅ No new test failures vs .NET 8 baseline
- ✅ Test execution time acceptable

---

### Testing Checklist Summary

**Build Phase**:
- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds with 0 errors
- [ ] 0 warnings (or acceptable documented warnings)

**Automated Testing Phase**:
- [ ] UnitTests: All pass
- [ ] IntegrationTests: All pass
- [ ] PublicApiIntegrationTests: All pass
- [ ] FunctionalTests: All pass

**Manual Testing Phase**:
- [ ] Web application: All smoke tests pass
- [ ] PublicApi: All smoke tests pass
- [ ] BlazorAdmin: All smoke tests pass

**Validation Phase**:
- [ ] No performance regressions
- [ ] No unexpected behavioral changes
- [ ] Security validated (0 vulnerabilities)
- [ ] Logs clean (no unexpected errors/warnings)

**Documentation Phase**:
- [ ] Document any deferred issues
- [ ] Update README with .NET 10 requirement
- [ ] Note any configuration changes needed for deployment

---

### Test Failure Triage Process

When tests fail:

1. **Categorize Failure**:
   - **Test Code Issue**: Test uses changed API → Update test
   - **Behavioral Change**: Expected behavior changed → Validate change is correct, update test
   - **Actual Regression**: Application logic broken → Fix application code immediately

2. **Prioritize Fixes**:
   - **Critical**: Security, authentication, data corruption
   - **High**: Core functionality broken
   - **Medium**: Non-critical feature broken
   - **Low**: Edge case, minor issue

3. **Document**:
   - Note failing test
   - Root cause analysis
   - Fix applied
   - Re-test result

4. **Regression Prevention**:
   - Add new tests if gaps found
   - Update test documentation
   - Consider additional coverage

---

### Post-Upgrade Testing Recommendations

**Continuous Monitoring** (after deployment):
- Run tests regularly to catch regressions
- Monitor production logs for unexpected errors
- Track performance metrics
- Gather user feedback

**Regression Suite**:
- Keep .NET 8 baseline test results for comparison
- Document expected behavior changes
- Maintain test coverage as features evolve

---

## Complexity & Effort Assessment

### Overall Solution Complexity: **MEDIUM**

**Factors**:
- 10 projects (manageable size)
- 147 total issues (moderate issue density)
- Clean dependency structure (no circular dependencies)
- 2 high-complexity projects (Web, PublicApi)
- All-at-Once strategy reduces coordination complexity

### Project Complexity Ratings

Complexity based on issue count, dependency count, risk factors, and codebase size:

| Project | Complexity | Issues | Mandatory | Dependencies | Risk Factors |
|---------|------------|--------|-----------|--------------|--------------|
| **BlazorShared** | **Low** | 1 | 1 | 0 | None - minimal changes |
| **ApplicationCore** | **Low** | 2 | 1 | 1 | Package update only |
| **UnitTests** | **Low** | 2 | 1 | 2 | Deprecated package |
| **IntegrationTests** | **Low** | 2 | 1 | 2 | None - minimal changes |
| **Infrastructure** | **Medium** | 9 | 5 | 1 | Deprecated AutoMapper package |
| **PublicApiIntegrationTests** | **Medium** | 13 | 5 | 2 | API breaking changes |
| **BlazorAdmin** | **Medium** | 15 | 2 | 1 | API breaking changes |
| **PublicApi** | **High** | 27 | 7 | 2 | 6 binary breaks, deprecated packages |
| **FunctionalTests** | **High** | 36 | 5 | 3 | High dependency count, API changes |
| **Web** | **Very High** | 40 | 8 | 4 | 7 binary breaks, 3 deprecated packages, most files |

### Complexity by Dependency Level

Understanding complexity by dependency level helps anticipate where effort concentrates:

**Level 0-1 (Foundation)**: **Low Complexity**
- BlazorShared (1 issue), ApplicationCore (2 issues), BlazorAdmin (15 issues)
- **Total**: 18 issues
- **Characteristics**: Few dependencies, mostly package updates
- **Estimated Relative Effort**: 15% of total

**Level 2 (Infrastructure)**: **Medium Complexity**
- Infrastructure (9 issues)
- **Characteristics**: Deprecated AutoMapper package, binary incompatibilities
- **Estimated Relative Effort**: 10% of total

**Level 3 (Applications)**: **Very High Complexity**
- PublicApi (27 issues), Web (40 issues)
- **Total**: 67 issues (46% of all issues!)
- **Characteristics**: Most binary breaks, deprecated packages, most files affected
- **Estimated Relative Effort**: 50% of total

**Level 4-5 (Tests)**: **Medium Complexity**
- FunctionalTests (36 issues), PublicApiIntegrationTests (13 issues), UnitTests (2 issues), IntegrationTests (2 issues)
- **Total**: 53 issues
- **Characteristics**: Test code updates, API usage changes
- **Estimated Relative Effort**: 25% of total

### Relative Effort Distribution

Based on All-at-Once strategy, effort distributes across activities:

| Activity | Relative Effort | Complexity Factors |
|----------|----------------|-------------------|
| **Project File Updates** | 5% | Mechanical - 10 files, simple TargetFramework changes |
| **Package Updates** | 10% | 18 packages across projects, mostly automated |
| **Compilation Fixes** | 50% | 26 binary breaks, 20 source breaks - most effort here |
| **Deprecated Package Resolution** | 15% | Research and replace 4 deprecated packages |
| **Test Fixes** | 15% | Update test code for API changes, 4 test projects |
| **Final Validation** | 5% | Smoke testing, documentation updates |

### High-Complexity Projects Detail

#### Web (40 issues) - **Very High Complexity**

**Why Very High**:
- Most issues in solution (40)
- 8 mandatory issues (7 binary breaks, 1 TFM change)
- 185 total files (largest project)
- 3 deprecated packages (AutoMapper, Azure.Identity, System.IdentityModel.Tokens.Jwt)
- 4 project dependencies (most complex dependency set)
- Main Razor Pages application (critical path)

**Effort Concentration**:
- 7 binary incompatible APIs requiring code changes
- 11 source incompatible APIs requiring compilation fixes
- 9 behavioral changes requiring validation
- 9 package updates + 3 deprecated package resolutions

**Recommendation**: Allocate 30-35% of total effort to Web project

#### PublicApi (27 issues) - **High Complexity**

**Why High**:
- 27 issues (second most in solution)
- 7 mandatory issues (6 binary breaks, 1 TFM change)
- 38 files (medium size)
- 2 deprecated packages (AutoMapper, System.IdentityModel.Tokens.Jwt)
- REST API with authentication (security-sensitive)

**Effort Concentration**:
- 6 binary incompatible APIs
- 9 source incompatible APIs
- 8 package updates + 2 deprecated package resolutions

**Recommendation**: Allocate 20% of total effort to PublicApi project

#### FunctionalTests (36 issues) - **High Complexity**

**Why High**:
- 36 issues (high for test project)
- Depends on Web, ApplicationCore, PublicApi (most dependencies)
- API usage from upgraded applications may break tests
- Functional tests are end-to-end (broader scope)

**Effort Concentration**:
- Test code updates for API changes
- Potential test framework compatibility issues
- Behavioral validation in tests

**Recommendation**: Allocate 15% of total effort to FunctionalTests project

### Resource Requirements

**Skill Levels Needed**:

1. **.NET Framework Expertise** (High)
   - Understanding .NET 8 → .NET 10 breaking changes
   - API compatibility knowledge
   - Framework behavior differences

2. **ASP.NET Core Expertise** (High)
   - Web and PublicApi are ASP.NET Core applications
   - Middleware, authentication, dependency injection changes

3. **Entity Framework Expertise** (Medium)
   - Infrastructure uses EF Core
   - Package updates may affect data access

4. **Blazor Expertise** (Medium)
   - BlazorAdmin and BlazorShared projects
   - Component API changes

5. **Testing Expertise** (Medium)
   - 4 test projects need updates
   - xUnit, MSTest familiarity

**Team Composition** (for manual execution):
- 1-2 senior .NET developers (lead upgrade, handle complex projects)
- 1 developer for test updates
- 1 QA for validation testing

**Parallel Capacity**:
With All-at-Once strategy, parallelization is limited:
- File updates can happen in parallel (different developers, different files)
- Build must be sequential (dependency order)
- Testing can be partially parallel (independent test projects)

### Effort Notes

**Important**: This plan uses **relative complexity ratings** (Low/Medium/High/Very High) and **percentage effort distribution**, not absolute time estimates.

**Why No Time Estimates**:
- Upgrade execution is often automated (tools handle mechanical changes)
- Actual duration depends on tooling, developer experience, and interruptions
- Breaking change fixes vary wildly (some are 1-line, others require design changes)
- Test failures can be quick fixes or deep investigations

**How to Use Complexity Ratings**:
- **Low**: Straightforward, minimal intervention needed
- **Medium**: Some manual fixes expected, manageable scope
- **High**: Significant manual effort, careful attention required
- **Very High**: Major effort concentration, highest risk, most scrutiny

**Percentage Effort Distribution** indicates where to focus attention and allocate resources, not calendar time.

---

## Source Control Strategy

### Branching Strategy

**Current Status**:
- **Source Branch**: `main` (starting point)
- **Upgrade Branch**: `upgrade-to-NET10` (created, currently active)
- **Pending Changes**: None at upgrade start

**Branch Purpose**:
- `upgrade-to-NET10` isolates all .NET 10 upgrade changes
- Enables clean rollback if needed (delete branch)
- Allows code review before merging to `main`
- Preserves `main` branch stability during upgrade

---

### Commit Strategy (All-at-Once Approach)

The All-at-Once strategy favors **single atomic commit** for the entire upgrade when feasible.

#### Recommended: Single Atomic Commit

**Rationale**:
- ✅ All changes logically related (one upgrade)
- ✅ Clean rollback (`git revert <commit>`)
- ✅ Clear history (one commit = one upgrade)
- ✅ No intermediate broken states
- ✅ Easier to cherry-pick or merge

**Commit Structure**:
```
Upgrade solution to .NET 10.0

- Update all 10 project files: net8.0 → net10.0
- Update 18 NuGet packages to .NET 10 compatible versions
- Address 4 deprecated packages (JWT, AutoMapper, Azure.Identity, ASP.NET MVC)
- Fix 26 binary incompatible APIs
- Fix 20 source incompatible APIs
- Update code for JWT authentication API changes (Infrastructure, PublicApi)
- Update configuration/DI patterns (Web, PublicApi)
- Update test code for API changes
- All tests passing (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)

Breaking changes addressed:
- JWT Bearer authentication (System.IdentityModel.Tokens.Jwt 7.4.1 → 8.16.0)
- Configuration binding APIs (Web, PublicApi)
- MediatR registration (Web, PublicApi)
- AutoMapper registration (Web, PublicApi - deprecated package)
- Azure.Identity (1.13.2 → 1.18.0 - removed deprecated MSAL)

See .github/upgrades/scenarios/new-dotnet-version_599bde/plan.md for complete details.
```

**When to Use**:
- Upgrade completes in reasonable time (hours, not days)
- All breaking changes addressed in single session
- All tests passing

#### Alternative: Phased Commits

If atomic commit not feasible (e.g., upgrade takes multiple days, complex fixes needed):

**Commit 1: Project Files & Package Updates**
```
Upgrade: Update project files and packages to .NET 10

- Update TargetFramework in all 10 projects: net8.0 → net10.0
- Update 18 NuGet packages to .NET 10 compatible versions
- Address deprecated packages

Note: Build currently broken, compilation fixes in next commit.
```

**Commit 2: Compilation Fixes**
```
Upgrade: Fix compilation errors for .NET 10

- Fix JWT authentication API changes (Infrastructure, PublicApi)
- Fix configuration/DI API changes (Web, PublicApi)
- Fix MediatR/AutoMapper registration
- Other breaking change fixes

Build succeeds. Tests may have failures (next commit).
```

**Commit 3: Test Fixes**
```
Upgrade: Update test code for .NET 10

- Update test code for API changes
- Fix test assertions for behavioral changes
- All tests passing

Upgrade complete.
```

**When to Use**:
- Upgrade spans multiple work sessions
- Need to checkpoint progress
- Team collaboration requires intermediate commits

---

### Code Review & Merge Process

#### Pull Request Requirements

**PR Title**: "Upgrade eShopOnWeb solution to .NET 10.0"

**PR Description Template**:
```markdown
## Upgrade Summary
Upgrades eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

## Changes
- **Projects Upgraded**: 10 projects (all .NET projects)
- **Package Updates**: 18 packages to .NET 10 compatible versions
- **Deprecated Packages**: 4 addressed (JWT, AutoMapper, Azure.Identity, ASP.NET MVC)
- **Breaking Changes Fixed**: 46 (26 binary, 20 source incompatible)
- **Behavioral Changes**: 52 potential changes reviewed

## Critical Changes
- JWT authentication APIs updated (Infrastructure, PublicApi)
- Configuration binding patterns updated (Web, PublicApi)
- Deprecated packages upgraded or removed
- Test code updated for API changes

## Testing
- ✅ All 10 projects build successfully (0 errors, 0 warnings)
- ✅ UnitTests: All passing
- ✅ IntegrationTests: All passing
- ✅ PublicApiIntegrationTests: All passing
- ✅ FunctionalTests: All passing
- ✅ Manual smoke testing completed
- ✅ No package vulnerabilities (`dotnet list package --vulnerable`)

## Documentation
- Upgrade plan: `.github/upgrades/scenarios/new-dotnet-version_599bde/plan.md`
- Assessment: `.github/upgrades/scenarios/new-dotnet-version_599bde/assessment.md`

## Deployment Notes
- Requires .NET 10 SDK on all environments
- No configuration changes needed
- No database migration needed
- Review deprecated package replacements (see plan.md §Package Update Reference)

## Rollback Plan
If issues discovered post-merge:
```bash
git revert <this-commit-sha>
```
```

#### Review Checklist

Reviewers should verify:

**Code Quality**:
- [ ] All breaking changes addressed correctly (not just compiling)
- [ ] No TODO comments left in code
- [ ] No commented-out code (unless documented)
- [ ] Deprecated package replacements appropriate
- [ ] JWT authentication changes secure
- [ ] Configuration changes correct

**Testing**:
- [ ] All automated tests passing in PR
- [ ] Test coverage maintained or improved
- [ ] Manual testing documented

**Documentation**:
- [ ] plan.md accurately reflects changes made
- [ ] README updated with .NET 10 requirement (if applicable)
- [ ] Breaking changes documented
- [ ] Deployment notes included

**Security**:
- [ ] No vulnerabilities introduced (`dotnet list package --vulnerable`)
- [ ] Deprecated security packages addressed
- [ ] Authentication/authorization working correctly

**Build & CI/CD**:
- [ ] CI build passing
- [ ] No build warnings
- [ ] Deployment pipeline compatible with .NET 10

---

### Merge Criteria

**Merge to `main` when**:
- ✅ PR approved by required reviewers
- ✅ All CI checks passing
- ✅ All tests passing (100%)
- ✅ Manual smoke testing completed
- ✅ No unresolved review comments
- ✅ Documentation complete
- ✅ Deployment plan ready

**Merge Method**: 
- **Recommended**: Squash and merge (clean history)
- **Alternative**: Merge commit (preserves detailed commit history)

**After Merge**:
1. Delete `upgrade-to-NET10` branch (cleanup)
2. Tag release (optional): `git tag v1.0.0-net10`
3. Deploy to staging environment for validation
4. Monitor for issues
5. Deploy to production (phased if preferred)

---

### Rollback Strategy

#### Pre-Merge Rollback (Branch Level)

If upgrade needs to be abandoned:
```bash
# Switch back to main
git checkout main

# Delete upgrade branch
git branch -D upgrade-to-NET10

# Start fresh if retrying
git checkout -b upgrade-to-NET10-v2
```

#### Post-Merge Rollback (Commit Level)

If issues discovered after merge to `main`:

**Option 1: Revert Commit (Recommended)**
```bash
# Revert the merge commit
git revert <merge-commit-sha>

# Push revert
git push origin main
```
✅ Safe - preserves history, creates new commit undoing changes

**Option 2: Reset Branch (Dangerous)**
```bash
# Only if merge just happened and not deployed
git reset --hard HEAD~1
git push --force origin main
```
⚠️ Use with caution - rewrites history, affects other developers

#### Post-Deployment Rollback (Application Level)

If issues discovered in production:

1. **Immediate**: Redeploy previous version (from backup/tag)
2. **Permanent**: Revert commit in `main` and redeploy
3. **Investigate**: Determine root cause of issues
4. **Fix Forward** (if minor): Create hotfix branch, fix issue, deploy fix

---

### Source Control Best Practices

**During Upgrade**:
- ✅ Commit message describes specific changes
- ✅ Commits are atomic (each compiles successfully) or clearly marked as WIP
- ✅ Regular pushes to remote (backup progress)
- ✅ Branch kept up-to-date with `main` (if long-running)

**Branching**:
- ✅ Clear branch naming (`upgrade-to-NET10`, not `test` or `temp`)
- ✅ Delete branches after merge (keep repo clean)
- ✅ Protect `main` branch (require PR, reviews)

**History**:
- ✅ Clean commit history (squash if many small commits)
- ✅ Meaningful commit messages (future reference)
- ✅ Tag important milestones (e.g., `net8-final`, `net10-initial`)

---

### All-at-Once Strategy Alignment

The single atomic commit approach aligns perfectly with All-at-Once strategy:
- **All projects change together** → One commit
- **No intermediate states** → No partial commits
- **Clean rollback** → Revert one commit
- **Clear history** → "Upgraded to .NET 10" is unambiguous

This source control strategy supports the All-at-Once migration approach and enables safe, trackable upgrade execution.

---

## Success Criteria

The .NET 10.0 upgrade is considered **complete and successful** when ALL of the following criteria are met.

---

### Technical Criteria

#### 1. Framework Migration ✅

- [ ] **All 10 projects** target `net10.0` in their `.csproj` files
  - ApplicationCore, BlazorShared, BlazorAdmin, Infrastructure, PublicApi, Web
  - UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests
- [ ] No projects remain on `net8.0`
- [ ] No multi-targeting (`<TargetFrameworks>`) unless intentional

#### 2. Package Updates ✅

- [ ] **All 18 recommended package updates** applied:
  - Microsoft.AspNetCore.* packages → 10.0.3
  - Microsoft.EntityFrameworkCore.* packages → 10.0.3
  - System.Text.Json → 10.0.3
  - System.Net.Http.Json → 10.0.3
  - Microsoft.Extensions.* packages → 10.0.3
  - Testing packages → 10.0.x

- [ ] **All 4 deprecated packages addressed**:
  - System.IdentityModel.Tokens.Jwt: 7.4.1 → 8.16.0 ✅
  - AutoMapper.Extensions.Microsoft.DependencyInjection: Removed or explained ✅
  - Azure.Identity: 1.13.2 → 1.18.0 ✅
  - Microsoft.AspNetCore.Mvc 2.2.0: Removed ✅

- [ ] No package dependency conflicts
- [ ] `dotnet restore` succeeds without warnings

#### 3. Build Success ✅

- [ ] `dotnet build eShopOnWeb.sln` succeeds
- [ ] **0 compilation errors** across all projects
- [ ] **0 warnings** (or only documented acceptable warnings)
- [ ] All projects build in dependency order without issues
- [ ] No deprecated API usage warnings (or documented/tracked)

#### 4. Breaking Changes Resolved ✅

- [ ] **All 26 binary incompatible APIs** addressed
- [ ] **All 20 source incompatible APIs** addressed
- [ ] **52 behavioral changes** reviewed and validated
- [ ] JWT authentication APIs updated correctly (Infrastructure, PublicApi)
- [ ] Configuration/DI patterns updated (Web, PublicApi)
- [ ] MediatR registration updated
- [ ] AutoMapper registration updated (or deprecated package removed)
- [ ] No compilation errors from breaking changes

#### 5. Testing Success ✅

**Automated Tests**:
- [ ] **UnitTests**: 100% passing
- [ ] **IntegrationTests**: 100% passing
- [ ] **PublicApiIntegrationTests**: 100% passing
- [ ] **FunctionalTests**: 100% passing
- [ ] Total test pass rate: 100% (or documented acceptable failures with justification)
- [ ] No new test failures vs .NET 8 baseline

**Manual Testing**:
- [ ] Web application smoke tests pass (auth, catalog, basket, checkout)
- [ ] PublicApi smoke tests pass (JWT auth, endpoints, Swagger)
- [ ] BlazorAdmin smoke tests pass (UI rendering, components)
- [ ] Health check endpoints work (`/health`)
- [ ] Error handling works (error pages display correctly)

---

### Quality Criteria

#### 6. Code Quality ✅

- [ ] No `// TODO: .NET 10 upgrade` comments remaining
- [ ] No commented-out code (unless documented)
- [ ] No temporary workarounds without tracking issues
- [ ] Code follows existing project conventions
- [ ] No security anti-patterns introduced

#### 7. Test Coverage ✅

- [ ] Test coverage maintained or improved vs .NET 8 baseline
- [ ] New tests added for changed behaviors (if applicable)
- [ ] No critical code paths lost coverage

#### 8. Documentation ✅

- [ ] `README.md` updated with .NET 10 requirement (if applicable)
- [ ] `plan.md` accurately reflects changes made
- [ ] `assessment.md` available for reference
- [ ] Breaking changes documented
- [ ] Deprecated package replacements documented
- [ ] Deployment notes documented (if configuration changes needed)

---

### Process Criteria

#### 9. All-at-Once Strategy Principles Applied ✅

- [ ] All projects upgraded simultaneously (not piecemeal)
- [ ] All package updates applied together
- [ ] No intermediate multi-targeting states
- [ ] Solution builds as unified .NET 10 codebase
- [ ] Single atomic upgrade operation completed

#### 10. Source Control ✅

- [ ] Upgrade changes committed to `upgrade-to-NET10` branch
- [ ] Commit message(s) clear and descriptive
- [ ] Branch ready for PR or already merged to `main`
- [ ] Clean commit history (no unnecessary commits)
- [ ] All changes tracked in version control

#### 11. Code Review ✅

- [ ] Pull request created (if team workflow)
- [ ] PR description includes upgrade summary, changes, testing results
- [ ] PR approved by required reviewers (if applicable)
- [ ] All review comments addressed or documented

---

### Security Criteria

#### 12. Security Posture ✅

- [ ] `dotnet list package --vulnerable` returns **0 vulnerabilities**
- [ ] All deprecated packages addressed (no known security issues)
- [ ] JWT authentication secure and functional
- [ ] Authorization rules enforced correctly
- [ ] No sensitive data exposed in logs or code
- [ ] Azure.Identity updated (deprecated MSAL dependency removed)
- [ ] System.IdentityModel.Tokens.Jwt updated to LTS (8.16.0)

---

### Performance Criteria

#### 13. Performance Maintained ✅

- [ ] Application startup time comparable to .NET 8 (no major regression)
- [ ] Page load times acceptable
- [ ] API response times acceptable
- [ ] Memory usage stable (no memory leaks)
- [ ] No obvious performance degradation
- [ ] Test execution time comparable

---

### Deployment Criteria

#### 14. Deployment Readiness ✅

- [ ] .NET 10 SDK requirement documented
- [ ] Deployment pipeline updated for .NET 10 (if applicable)
- [ ] Configuration changes documented (if any)
- [ ] Database migrations not needed (EF Core version compatible)
- [ ] Deployment tested in staging/pre-production environment
- [ ] Rollback plan documented and tested

---

### Acceptance Criteria

#### 15. Stakeholder Acceptance ✅

- [ ] Development team confident in upgrade stability
- [ ] QA validation completed (if applicable)
- [ ] Product owner/stakeholder approved (if required)
- [ ] No outstanding blocking issues
- [ ] Production deployment approved

---

## Success Validation Checklist

Use this checklist to verify upgrade success before considering complete:

### Build Phase
- [ ] All projects build (0 errors, 0 warnings)
- [ ] All packages restored successfully
- [ ] No deprecated APIs in use (or tracked)

### Testing Phase
- [ ] All automated tests passing (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)
- [ ] Manual smoke testing completed
- [ ] Performance validated

### Security Phase
- [ ] 0 vulnerabilities (`dotnet list package --vulnerable`)
- [ ] Deprecated packages addressed
- [ ] Authentication/authorization working

### Documentation Phase
- [ ] README updated (if needed)
- [ ] Plan and assessment documents complete
- [ ] Deployment notes documented

### Source Control Phase
- [ ] Changes committed and pushed
- [ ] PR created and reviewed (if applicable)
- [ ] Clean commit history

### Deployment Phase
- [ ] Staging deployment successful (if applicable)
- [ ] Rollback plan ready
- [ ] Production deployment approved

---

## Definition of Done

The upgrade is **DONE** when:

1. ✅ **All 15 success criteria** above are met
2. ✅ **All checklist items** are checked
3. ✅ **All tests passing** (100%)
4. ✅ **No blockers** remaining
5. ✅ **Code merged** to `main` (or ready to merge)
6. ✅ **Team confident** in production deployment

---

## Post-Upgrade Success Monitoring

After deployment, continue monitoring for 1-2 weeks:

**Monitor**:
- [ ] Application logs (no unexpected errors/warnings)
- [ ] Performance metrics (no degradation)
- [ ] User-reported issues (no increase)
- [ ] Security alerts (no new vulnerabilities)
- [ ] Error rates (comparable to .NET 8 baseline)

**If Issues Arise**:
1. Assess severity (critical, high, medium, low)
2. Determine if related to .NET 10 upgrade
3. Fix forward (hotfix) or rollback (if critical)
4. Document issue and resolution
5. Update plan.md lessons learned (if applicable)

---

## Success Declaration

**When can you declare success?**

✅ **Immediately After Upgrade**: If all 15 criteria met, tests passing, no blockers

✅ **After Staging Validation**: If deployment tested in staging, no issues found

✅ **After Production Soak**: After 1-2 weeks in production with no upgrade-related issues

**Final Success Statement**:
> "eShopOnWeb solution successfully upgraded from .NET 8.0 to .NET 10.0 (LTS). All 10 projects migrated, 18 packages updated, 4 deprecated packages addressed, 46 breaking changes resolved. All tests passing, no vulnerabilities, no regressions detected. Upgrade complete."

---

## Lessons Learned (Post-Upgrade)

After upgrade completion, document:
- What went well?
- What was challenging?
- Unexpected issues encountered?
- How long did it take vs estimate?
- Would All-at-Once strategy be used again?
- Recommendations for future upgrades?

(Add notes here after completion)
