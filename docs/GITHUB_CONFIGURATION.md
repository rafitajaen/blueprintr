# GitHub Configuration Guide

> Complete guide for configuring GitHub repository, NuGet publishing, and CI/CD workflows for Blueprintr.

## Table of Contents

1. [GitHub Repository Settings](#1-github-repository-settings)
2. [NuGet Account Configuration](#2-nuget-account-configuration)
3. [Versioning Strategy](#3-versioning-strategy)
4. [Workflow Testing](#4-workflow-testing)
5. [Troubleshooting](#5-troubleshooting)

---

## 1. GitHub Repository Settings

### 1.1 Enable GitHub Pages

GitHub Pages hosts your project documentation automatically via GitHub Actions.

**Steps:**
1. Navigate to your repository: `https://github.com/rafitajaen/blueprintr`
2. Click `Settings` (top menu)
3. In the left sidebar, click `Pages`
4. Under "Build and deployment":
   - **Source:** Select `GitHub Actions`
5. Click `Save`

**Result:** Documentation will be available at `https://rafitajaen.github.io/blueprintr/` after first successful deployment.

**Note:** First deployment happens when you push to main with documentation changes or create a git tag.

---

### 1.2 Configure Branch Protection Rules

Branch protection prevents broken code from reaching main and enforces code review.

**Steps:**
1. Navigate to: `Settings` > `Branches`
2. Click `Add branch protection rule`
3. **Branch name pattern:** `main`
4. Configure the following settings:

#### Protect Matching Branches:

**Pull Request Requirements:**
- ✅ **Require a pull request before merging**
  - ✅ **Require approvals:** 1 (recommended, adjustable based on team size)
  - ✅ **Dismiss stale pull request approvals when new commits are pushed**
  - ✅ **Require approval of the most recent reviewable push**

**Status Check Requirements:**
- ✅ **Require status checks to pass before merging**
  - ✅ **Require branches to be up to date before merging**
  - **Search for status checks:** Type `build-and-test` and select it
    - Note: Status checks appear only after running at least once

**Additional Settings:**
- ✅ **Do not allow bypassing the above settings**
  - This applies to administrators too (recommended for production)
- [ ] **Allow force pushes** (keep UNCHECKED)
- [ ] **Allow deletions** (keep UNCHECKED)

5. Click `Create` or `Save changes`

**What this does:**
- Prevents direct pushes to main (requires PRs)
- Requires all tests to pass before merge
- Requires code review approval
- Ensures branch is up-to-date with main

**Testing Protection:**
```bash
# This should fail:
git checkout main
echo "test" >> README.md
git commit -am "test"
git push origin main
# Error: protected branch

# This is the correct workflow:
git checkout -b feature/my-change
# make changes
git push origin feature/my-change
# Create PR via GitHub UI
```

---

### 1.3 Configure Repository Secrets

Secrets store sensitive information like API keys securely.

**Steps:**
1. Navigate to: `Settings` > `Secrets and variables` > `Actions`
2. Click `New repository secret`
3. Create the following secrets:

#### Required Secrets:

| Secret Name | Purpose | How to Obtain | Required? |
|-------------|---------|---------------|-----------|
| `NUGET_USERNAME` | NuGet.org username for Trusted Publishing | Your NuGet.org profile name | Recommended (for Trusted Publishing) |
| `NUGET_API_KEY` | Legacy API key for package publishing | See [Section 2.3](#23-generate-api-key-legacy-method) | Optional (if not using Trusted Publishing) |
| `CODECOV_TOKEN` | Upload test coverage reports | From [codecov.io](https://codecov.io/) | Optional |

**Authentication Methods:**
- **Recommended:** Use `NUGET_USERNAME` + configure Trusted Publishing (see [Section 2.2](#22-trusted-publishing-recommended))
- **Legacy:** Use `NUGET_API_KEY` (long-lived API key)

**For each secret:**
1. Click `New repository secret`
2. **Name:** Enter exact name (case-sensitive)
3. **Secret:** Paste the value
4. Click `Add secret`

**Security Notes:**
- Never commit secrets to code
- Rotate secrets regularly
- Use separate keys for different repositories
- Set calendar reminders for expiration dates

---

## 2. NuGet Account Configuration

### 2.1 Create NuGet Account

**Steps:**
1. Go to [https://www.nuget.org/](https://www.nuget.org/)
2. Click `Sign in` (top right)
3. Choose sign-in method:
   - Microsoft Account
   - GitHub Account (recommended for consistency)
   - Google Account
4. Complete account creation
5. Verify your email address

**Profile Setup:**
- Add profile picture (optional)
- Add description
- Verify email is confirmed
- **Note your username** (displayed in top-right corner) - you'll need this for Trusted Publishing

---

### 2.2 Trusted Publishing (RECOMMENDED)

**🔐 Modern, Keyless Authentication Method**

Trusted Publishing is the **secure, recommended way** to publish NuGet packages. It eliminates long-lived API keys by using short-lived OIDC tokens from GitHub Actions.

#### Benefits vs Traditional API Keys

| Feature | Trusted Publishing | API Keys |
|---------|-------------------|----------|
| **Security** | ✅ Short-lived (1 hour) | ⚠️  Long-lived (365 days) |
| **Management** | ✅ Automated | ⚠️  Manual rotation needed |
| **Secret Storage** | ✅ No API key storage | ⚠️  Must store in GitHub Secrets |
| **Token Reuse** | ✅ Single-use only | ⚠️  Can be reused indefinitely |
| **Compromise Risk** | ✅ Low (expires quickly) | ⚠️  High if leaked |
| **Rotation** | ✅ Automatic per workflow | ⚠️  Manual every 365 days |

**Industry Standard:** Part of the [OpenSSF Trusted Publishers initiative](https://repos.openssf.org/trusted-publishers-for-all-package-repositories)

---

#### How Trusted Publishing Works

```
1. GitHub Actions workflow runs
2. GitHub issues OIDC token (cryptographically signed)
3. Token includes: repository name, workflow file, environment
4. Workflow sends token to NuGet.org
5. NuGet validates token with GitHub
6. NuGet issues short-lived API key (valid 1 hour, single use)
7. Workflow publishes package with temporary key
```

**No long-lived secrets needed!**

---

#### Configure Trusted Publishing on NuGet.org

**Prerequisites:**
- NuGet account created (see [Section 2.1](#21-create-nuget-account))
- Know your NuGet.org **username** (NOT email)
- Repository: `rafitajaen/blueprintr`
- Workflow file: `publish-nuget.yml`

**Steps:**

1. **Log into NuGet.org**
   - Visit: [https://www.nuget.org/](https://www.nuget.org/)
   - Sign in with your account

2. **Navigate to Trusted Publishing**
   - Click your **username** (top right)
   - Select **"Trusted Publishing"** from dropdown
   - Or visit: [https://www.nuget.org/account/publishing](https://www.nuget.org/account/publishing)

3. **Add New Trusted Publishing Policy**
   - Click **"Add"** or **"Create new policy"**
   - Fill in the form:

**Policy Configuration:**

| Field | Value | Notes |
|-------|-------|-------|
| **Repository Owner** | `rafitajaen` | Your GitHub username (case-insensitive) |
| **Repository** | `blueprintr` | Repository name (case-insensitive) |
| **Workflow File** | `publish-nuget.yml` | Filename ONLY (no `.github/workflows/` path) |
| **Environment** | Leave empty | Optional; restricts to GitHub Actions environment |

4. **Save the Policy**
   - Click **"Create"** or **"Save"**
   - Policy status: **"Pending"** (temporary 7-day activation)
   - Becomes **"Active"** after first successful publish

**Policy Ownership:**
- Applies to ALL packages you own
- For organizations: Must be an active member
- Leaving organization deactivates policy

---

#### Configure GitHub Secrets

**Required Secret:**

1. Go to: `Settings` > `Secrets and variables` > `Actions`
2. Click `New repository secret`
3. Create:
   - **Name:** `NUGET_USERNAME`
   - **Secret:** Your NuGet.org username (visible in top-right on nuget.org)
     - **Use username, NOT email address**
     - Example: `rafitajaen` (not `rafitajaen@example.com`)
4. Click `Add secret`

**Workflow Configuration:**
- Already configured in `.github/workflows/publish-nuget.yml`
- Uses `NuGet/login@v1` action to get temporary API key
- Automatically falls back to API key if Trusted Publishing not configured

---

#### Verify Trusted Publishing Setup

After configuring policy and secret:

1. **Push change to main** with src/ modifications
2. **Watch workflow:** Actions > Publish to NuGet
3. **Check logs:**
   - Should show: `✅ Using Trusted Publishing (OIDC - short-lived token)`
   - If shows: `⚠️  Using legacy API key` - Trusted Publishing not configured correctly

**First Publish:**
- Policy status changes from "Pending" to "Active"
- Provides GitHub repository/owner IDs to prevent resurrection attacks

**Troubleshooting:**
- Ensure username is correct (NOT email)
- Verify workflow file name is exact: `publish-nuget.yml`
- Check repository owner/name are correct
- Ensure you're an active organization member (if using org)

---

#### Limitations & Considerations

- **Temporary Activation:** New policies are "Pending" for 7 days, become "Active" after first publish
- **GitHub Actions Only:** Currently only supports GitHub Actions (Azure DevOps, GitLab not documented)
- **1 Hour Validity:** Temporary API keys expire in 1 hour
- **Single Use:** Each OIDC token creates one API key, usable once
- **Username Required:** Must use NuGet.org username, NOT email

---

#### Migration from API Keys

**You can use both methods simultaneously:**
1. Configure Trusted Publishing (recommended for new workflows)
2. Keep API key as fallback (workflow auto-detects)
3. Once Trusted Publishing works, remove `NUGET_API_KEY` secret

**Workflow automatically:**
- Tries Trusted Publishing first (if `NUGET_USERNAME` exists)
- Falls back to API key (if `NUGET_API_KEY` exists)
- Fails with helpful message if neither configured

---

### 2.3 Generate API Key (Legacy Method)

**⚠️  Legacy Method - Consider using [Trusted Publishing](#22-trusted-publishing-recommended) instead**

API keys authenticate GitHub Actions to publish packages.

**Steps:**
1. Go to [https://www.nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
2. Click `+ Create` (top right)
3. Configure the API key:

**Settings:**
- **Key name:** `blueprintr-github-actions`
  - Use descriptive names for multiple keys
- **Expiration:** 365 days (maximum)
  - Set calendar reminder for 30 days before expiration
- **Package owner:** Select your account
  - If organization: Select organization instead
- **Glob pattern:** `Blueprintr.*`
  - Limits key to only your packages
  - Increases security if key is compromised
- **Scopes:** Select the following
  - ✅ **Push new packages and package versions**
  - [ ] **Push new package versions only** (don't select)
  - [ ] **Unlist packages** (optional, only if needed)

4. Click `Create`
5. **COPY THE KEY IMMEDIATELY**
   - This is shown only once
   - Store temporarily in password manager
6. Go to GitHub repository settings
7. Add as `NUGET_API_KEY` secret (see [Section 1.3](#13-configure-repository-secrets))
8. Test the key by pushing to main with src/ changes

**Key Format:** Starts with `oy2...` (long string of characters)

---

### 2.4 API Key Renewal (Legacy)

**Note:** With Trusted Publishing, no key renewal needed (automatic per workflow).

NuGet API keys expire. Plan ahead to avoid publishing failures.

**Renewal Process:**

**30 days before expiration:**
1. Set calendar reminder
2. Generate new API key (same settings)
3. Update GitHub Secret `NUGET_API_KEY` with new value
4. Test by triggering publish workflow
5. After confirming new key works, delete old key from NuGet

**If key expires:**
1. Publishing workflows will fail with 403 Forbidden error
2. Generate new key immediately
3. Update GitHub Secret
4. Re-run failed workflow

**Best Practices:**
- Use 365-day expiration (maximum)
- Set reminder 30 days before expiration
- Document expiration date in team calendar
- Consider using organization API keys for team projects

---

### 2.5 Package Ownership

**Individual Ownership:**
- Default: You own all packages you publish
- You can transfer ownership to organizations

**Organization Ownership:**

**Benefits:**
- Shared package management
- Team member access
- Continuity if members leave

**Setup:**
1. Create NuGet organization:
   - Go to [https://www.nuget.org/organization/add](https://www.nuget.org/organization/add)
   - Enter organization name
   - Add members
2. Transfer existing packages:
   - Go to package page on NuGet
   - Click `Manage Package` > `Owners`
   - Add organization as owner
   - Remove individual owner (optional)
3. Generate API key under organization account
4. Update GitHub Secret with organization key

---

## 3. Versioning Strategy

Blueprintr uses **MinVer** for automatic semantic versioning based on git tags and commit history.

### 3.1 How MinVer Works

MinVer calculates package versions automatically from:
- Git tags (stable versions)
- Commit history (pre-release versions)
- Branch names (ignored, tags only)

**No manual version management needed!**

---

### 3.2 Version Calculation

| Scenario | Version Format | Example | When Used |
|----------|----------------|---------|-----------|
| No tags, N commits | `0.0.0-alpha.0.N` | `0.0.0-alpha.0.5` | Early development |
| After tag `1.0.0`, N commits | `1.0.1-alpha.0.N` | `1.0.1-alpha.0.3` | Post-release development |
| Exactly on tag `1.0.0` | `1.0.0` | `1.0.0` | Stable release |
| Pre-release tag | `1.0.0-beta.1` | `1.0.0-beta.1` | Beta releases |

**Key Points:**
- Commits without tags produce pre-release versions (alpha)
- Pre-release versions auto-increment on each commit
- Stable versions require explicit git tags
- Minor version auto-bumps for post-release commits

---

### 3.3 Development Workflow (Pre-release)

During active development before the first stable release:

**What happens:**
- Every push to main publishes `0.0.0-alpha.0.X`
- X increments with each commit (1, 2, 3, ...)
- These versions are marked as "pre-release" on NuGet
- Users must check "Include prerelease" to see them

**Advantages:**
- No version management needed
- Automatic publishing on every change
- No conflicts (each commit = unique version)
- Clear indication these are unstable builds

**Check current version:**
```bash
minver
```

**Example output:**
```
0.0.0-alpha.0.5
```

---

### 3.4 Stable Release Process

When ready to publish a stable version:

**Steps:**
```bash
# 1. Ensure all changes are committed and pushed
git status  # Should be clean
git push origin main

# 2. Check current calculated version
minver
# Output: 0.0.0-alpha.0.X or 1.0.1-alpha.0.X

# 3. Create semantic version tag
git tag 1.0.0

# 4. Push the tag
git push origin 1.0.0
```

**What happens:**
1. GitHub Actions detects the tag
2. CI workflow runs (build + test)
3. If tests pass:
   - NuGet publish workflow publishes `1.0.0`
   - Documentation workflow publishes docs
4. Package appears on NuGet as stable (no pre-release flag)

**Verification:**
- Check GitHub Actions for successful workflow
- Visit NuGet package page (may take 5-15 minutes to index)
- Visit documentation site for updated version

---

### 3.5 Version Increment Rules

Follow semantic versioning (SemVer):

| Change Type | From | To | Command | When to Use |
|-------------|------|-----|---------|-------------|
| Patch (Bug fix) | 1.0.0 | 1.0.1 | `git tag 1.0.1` | Bug fixes, no new features |
| Minor (New feature) | 1.0.0 | 1.1.0 | `git tag 1.1.0` | New features, backwards compatible |
| Major (Breaking change) | 1.0.0 | 2.0.0 | `git tag 2.0.0` | Breaking API changes |
| Pre-release (Beta) | N/A | 1.0.0-beta.1 | `git tag 1.0.0-beta.1` | Testing before stable |
| Pre-release (RC) | N/A | 1.0.0-rc.1 | `git tag 1.0.0-rc.1` | Release candidate |

**Examples:**

```bash
# Bug fix release
git tag 1.0.1
git tag 1.0.2
git tag 1.0.3

# Feature releases
git tag 1.1.0
git tag 1.2.0

# Breaking change
git tag 2.0.0

# Pre-release sequence
git tag 1.0.0-beta.1
git tag 1.0.0-beta.2
git tag 1.0.0-rc.1
git tag 1.0.0  # Final stable
```

---

### 3.6 Avoiding Version Conflicts

MinVer + `--skip-duplicate` prevent version conflicts:

**Scenario 1: Version already exists**
- NuGet publish workflow uses `--skip-duplicate` flag
- If version exists, publish silently skips (no error)
- Workflow succeeds without re-publishing

**Scenario 2: Pre-release auto-increment**
- Each commit generates unique pre-release version
- `0.0.0-alpha.0.1`, `0.0.0-alpha.0.2`, `0.0.0-alpha.0.3`, ...
- No conflicts possible

**Scenario 3: Multiple tags on same commit**
- Don't do this - MinVer picks the highest version
- Create tags on separate commits

---

### 3.7 Check Current Version

**Install MinVer CLI (first time):**
```bash
# Install globally to check versions locally
dotnet tool install --global minver-cli

# Verify installation
minver
# Output: 0.0.0-alpha.0.X
```

**View calculated version:**
```bash
# Basic version
minver

# With detailed info
minver -v d

# For specific project
cd src/Blueprintr
minver
```

**Example output:**
```bash
$ minver
0.0.0-alpha.0.7

$ minver -v d
MinVer: Using working directory '/home/user/blueprintr'
MinVer: Calculating version from commit history
MinVer: No tags found
MinVer: Commit height: 7
MinVer: Version: 0.0.0-alpha.0.7
```

**Check tags:**
```bash
# Local tags
git tag -l

# Remote tags
git ls-remote --tags origin

# Show tag details
git show 1.0.0
```

---

## 4. Workflow Testing

Test each workflow to ensure proper configuration.

### 4.1 Testing CI Workflow

**Triggers:** Push to any branch, or create PR to main/develop

**Expected Behavior:**
1. Checkout code with full git history
2. Setup .NET 10.0
3. Restore dependencies
4. Build in Release configuration
5. Run all tests
6. Publish test results
7. Generate code coverage report
8. Upload coverage to Codecov

**Test Steps:**
```bash
# Create test branch
git checkout -b test/ci-verification
echo "# Test" >> README.md
git commit -am "test: verify CI workflow"
git push origin test/ci-verification

# Watch workflow:
# 1. Go to GitHub Actions tab
# 2. Find "CI - Build and Test" workflow
# 3. Click on the running workflow
# 4. Verify all steps pass
```

**Verify Success:**
- ✅ Green checkmark on workflow run
- ✅ All steps completed successfully
- ✅ Test results show pass count (e.g., "45 tests passed")
- ✅ Coverage report uploaded (check Codecov dashboard)

**If CI Fails:**
- Click on failed step to see error
- Check build logs for compilation errors
- Verify all tests pass locally: `dotnet test`
- See [Section 5.1](#51-ci-workflow-issues)

---

### 4.2 Testing NuGet Publishing

**Triggers:** Push to main with changes in `src/` directory

**Expected Behavior:**
1. CI workflow runs first
2. If CI passes, build-and-publish job starts
3. Detects changed projects in src/
4. Packs changed projects
5. Calculates version with MinVer
6. Publishes to NuGet (with --skip-duplicate)
7. Uploads packages as artifacts

**Test Steps:**
```bash
# 1. Create feature branch
git checkout -b feat/test-publish

# 2. Make small change in src/
cd src/Blueprintr
echo "// Test comment" >> Extensions/StringExtensions.cs
cd ../..

# 3. Commit and push
git add .
git commit -m "feat: test NuGet publish workflow"
git push origin feat/test-publish

# 4. Create PR via GitHub UI

# 5. Watch CI pass

# 6. Merge PR

# 7. Watch "Publish to NuGet" workflow
# Go to Actions tab > Publish to NuGet
```

**Verify Success:**
- ✅ CI job passes first
- ✅ build-and-publish job starts only after CI succeeds
- ✅ Changed projects detected
- ✅ Package published (check workflow logs)
- ✅ Package appears on nuget.org (5-15 min delay)
  - Visit: https://www.nuget.org/packages/Blueprintr.Endpoints/
  - Check version matches MinVer calculation

**If Publishing Fails:**
- Check workflow logs for authentication method used
- Trusted Publishing: Verify NUGET_USERNAME secret and NuGet.org policy configured
- Legacy API Key: Verify NUGET_API_KEY secret exists and hasn't expired
- See [Section 5.2](#52-nuget-publishing-issues)

---

### 4.3 Testing Documentation Workflow

**Triggers:**
- Push to main with changes in docs/, src/, README.md, etc.
- Push git tag
- Manual trigger via workflow_dispatch

**Expected Behavior:**
1. CI workflow runs first
2. If CI passes, build-and-deploy job starts
3. Builds .NET projects
4. Generates DocFX API metadata
5. Builds documentation site
6. Uploads artifact to GitHub Pages
7. Deploys to GitHub Pages

**Test Steps:**
```bash
# Option A: Test via main commit
git checkout -b docs/test-deploy
echo "## Test" >> docs/index.md
git add docs/index.md
git commit -m "docs: test documentation workflow"
git push origin docs/test-deploy
# Create PR, merge to main, watch workflow

# Option B: Test via tag
git tag 0.1.0-test
git push origin 0.1.0-test
# Watch workflow

# Option C: Manual trigger
# Go to Actions > Build and Deploy Documentation > Run workflow
```

**Verify Success:**
- ✅ CI job passes first
- ✅ build-and-deploy job starts only after CI succeeds
- ✅ DocFX metadata generated
- ✅ Documentation site built
- ✅ Artifact uploaded
- ✅ Deployed to GitHub Pages
- ✅ Documentation accessible at: https://rafitajaen.github.io/blueprintr/

**Check Documentation:**
1. Visit: https://rafitajaen.github.io/blueprintr/
2. Verify:
   - Home page loads
   - API documentation shows classes/methods
   - Articles render correctly
   - Navigation works
   - Search functionality works

**If Documentation Fails:**
- Check GitHub Pages is enabled (Settings > Pages)
- Verify pages permission in workflow
- See [Section 5.3](#53-documentation-issues)

---

### 4.4 Testing Branch Protection

**Verify:** Cannot push directly to main, PR requires passing tests

**Test 1: Direct Push (Should Fail)**
```bash
git checkout main
git pull origin main
echo "test" >> README.md
git commit -am "test: direct push"
git push origin main
# Expected: ERROR - protected branch
```

**Test 2: PR with Failing Test (Should Block Merge)**
```bash
# 1. Create branch with failing test
git checkout -b test/failing
cd tests/Blueprintr.Tests
# Edit a test file to make it fail
git add .
git commit -m "test: intentional failure"
git push origin test/failing

# 2. Create PR via GitHub UI

# 3. Observe:
# - CI runs and fails
# - Merge button is disabled
# - Status shows "Some checks were not successful"
```

**Test 3: PR with Passing Tests (Should Allow Merge)**
```bash
# 1. Create branch with passing change
git checkout -b feat/small-change
echo "# Small change" >> README.md
git commit -am "docs: small change"
git push origin feat/small-change

# 2. Create PR via GitHub UI

# 3. Observe:
# - CI runs and passes
# - Merge button enabled (if approvals met)
# - Status shows "All checks have passed"

# 4. Get approval (if required)

# 5. Merge PR
```

**What to Check:**
- ✅ Cannot push directly to main
- ✅ PR created successfully
- ✅ CI runs automatically on PR
- ✅ Failing tests block merge
- ✅ Passing tests allow merge (with approvals)
- ✅ Merge button shows "Merging is blocked" when tests fail

---

### 4.5 Complete End-to-End Flow Test

**Scenario:** Add a new feature with tests, verify complete pipeline

**Steps:**
```bash
# 1. Create feature branch
git checkout -b feat/add-feature
git pull origin main

# 2. Make changes in src/
cd src/Blueprintr
# Add new utility class or method
cat > Utils/NewFeature.cs << 'EOF'
namespace Blueprintr.Utils;

/// <summary>
/// Example new feature
/// </summary>
public static class NewFeature
{
    /// <summary>
    /// Example method
    /// </summary>
    public static string GetMessage() => "Hello from new feature!";
}
EOF
cd ../..

# 3. Add tests
cd tests/Blueprintr.Tests
cat > NewFeatureTests.cs << 'EOF'
using Blueprintr.Utils;
using NUnit.Framework;

namespace Blueprintr.Tests;

[TestFixture]
public class NewFeatureTests
{
    [Test]
    public void GetMessage_ReturnsCorrectMessage()
    {
        var result = NewFeature.GetMessage();
        Assert.That(result, Is.EqualTo("Hello from new feature!"));
    }
}
EOF
cd ../..

# 4. Test locally
dotnet build
dotnet test

# 5. Commit and push
git add .
git commit -m "feat: add new feature with tests"
git push origin feat/add-feature

# 6. Create PR via GitHub UI
# 7. Watch CI workflow run
# 8. Request review (if needed)
# 9. Merge PR after approval

# 10. Watch main workflows:
# - Go to Actions tab
# - Observe "Publish to NuGet" running
# - Observe "Build and Deploy Documentation" running

# 11. Verify results:
# - Check NuGet for new version
# - Check GitHub Pages for updated docs
```

**Expected Timeline:**
1. PR created → CI runs (2-3 min)
2. PR merged → Publish workflow starts (3-5 min)
3. Package on NuGet (5-15 min for indexing)
4. Documentation deployed (2-3 min)
5. Docs accessible (1-2 min for CDN)

**Verification Checklist:**
- [ ] Feature branch created
- [ ] Changes made in src/
- [ ] Tests added for new feature
- [ ] Local build passes
- [ ] Local tests pass
- [ ] PR created
- [ ] CI passes on PR
- [ ] PR approved
- [ ] PR merged to main
- [ ] Publish workflow runs
- [ ] Package published to NuGet
- [ ] Documentation workflow runs
- [ ] Documentation deployed to GitHub Pages
- [ ] New feature documented in API docs

---

## 5. Troubleshooting

### 5.1 CI Workflow Issues

#### Problem: Workflow Not Triggering

**Symptoms:**
- Push to branch but no workflow runs
- PR created but no checks appear

**Solutions:**
1. Check workflow is enabled:
   - Go to Actions tab
   - Find "CI - Build and Test"
   - If disabled, click "Enable workflow"

2. Verify branch name matches triggers:
   - Workflow triggers on: main, develop
   - PRs to: main, develop
   - Other branches: No CI (by design)

3. Check workflow file syntax:
   ```bash
   # Validate YAML syntax
   yamllint .github/workflows/ci.yml
   ```

4. Verify file exists in repository:
   ```bash
   git ls-files .github/workflows/
   ```

---

#### Problem: Tests Pass Locally But Fail in CI

**Symptoms:**
- `dotnet test` passes on your machine
- CI workflow shows test failures

**Common Causes & Solutions:**

**1. Environment Differences**
```bash
# Check .NET version
dotnet --version  # Should be 10.0.x

# Ensure dependencies restored
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

**2. File Path Issues**
- Windows uses `\`, Linux uses `/`
- Use `Path.Combine()` instead of string concatenation
- Avoid hardcoded paths

**3. Case Sensitivity**
- Linux is case-sensitive, Windows is not
- Filenames must match exactly: `readme.md` ≠ `README.md`

**4. Missing Test Data**
- Ensure test files are committed
- Check `.gitignore` isn't excluding test data

**5. Timezone Differences**
- CI runs in UTC
- Use `DateTimeOffset` for time-aware tests
- Don't assume local timezone

**6. Async Test Issues**
- Use `async/await` properly
- Don't use `.Result` or `.Wait()` (can cause deadlocks)
- Increase timeout for slow operations

---

#### Problem: Build Warnings Treated as Errors

**Symptoms:**
- Warning appears in build output
- Build fails with "error" status

**Explanation:**
- This is expected behavior (quality gate)
- `TreatWarningsAsErrors=true` in Directory.Build.props

**Solutions:**

**Option 1: Fix the warning** (recommended)
- Address the underlying issue
- Improves code quality

**Option 2: Suppress specific warning**
- Add to `WarningsNotAsErrors` in Directory.Build.props
```xml
<WarningsNotAsErrors>NU1510;NU1608;CS1591;CS1234</WarningsNotAsErrors>
```

**Option 3: Disable for specific code**
```csharp
#pragma warning disable CS1591 // Missing XML comment
public void MyMethod() { }
#pragma warning restore CS1591
```

---

### 5.2 NuGet Publishing Issues

#### Problem: Workflow Runs But Doesn't Publish

**Symptoms:**
- Push to main succeeds
- Publish workflow runs
- No package published

**Diagnostic Steps:**

**1. Check if changes were in src/**
```bash
git log -1 --name-only
# Verify files changed are in src/ directory
```

**2. View workflow logs:**
- Go to Actions > Publish to NuGet
- Click on workflow run
- Check "Detect changed projects" step
- Should show: "Changes detected in [project name]"

**3. Check authentication method in logs:**
- Expand "Pack and Publish to NuGet" step
- Look for: `✅ Using Trusted Publishing` or `⚠️ Using legacy API key`
- If neither: Authentication not configured

**4. Verify secrets exist:**
- Settings > Secrets > Actions
- For Trusted Publishing: Confirm `NUGET_USERNAME` exists
- For Legacy: Confirm `NUGET_API_KEY` exists

**Solutions:**
- If no src/ changes: No publish needed (by design)
- If authentication not configured: Add `NUGET_USERNAME` (recommended) or `NUGET_API_KEY` (legacy)
- If changes not detected: Check git diff logic

---

#### Problem: "Package version already exists"

**Symptoms:**
- Publish fails with version conflict error

**Explanation:**
- Shouldn't happen with `--skip-duplicate` flag
- Possible causes:
  1. Custom version in .csproj
  2. MinVer not calculating version correctly
  3. --skip-duplicate not working

**Solutions:**

**1. Check for hardcoded version:**
```xml
<!-- DON'T DO THIS in .csproj: -->
<Version>1.0.0</Version>

<!-- MinVer handles versioning automatically -->
```

**2. Verify MinVer calculation:**
```bash
minver
# Should show auto-calculated version
```

**3. Check git history:**
```bash
git log --oneline
git tag -l
# MinVer needs full git history
```

**4. Verify fetch-depth:**
```yaml
# In workflow, ensure:
- uses: actions/checkout@v4
  with:
    fetch-depth: 0  # Required for MinVer
```

---

#### Problem: Trusted Publishing Not Working

**Symptoms:**
- Workflow uses legacy API key instead of Trusted Publishing
- Log shows: `⚠️ Using legacy API key`
- Or: `❌ No NuGet authentication configured!`

**Diagnostic Steps:**

**1. Check NUGET_USERNAME secret:**
```bash
# In GitHub: Settings > Secrets > Actions
# Verify NUGET_USERNAME exists and contains correct username
```

**2. Verify Trusted Publishing policy on NuGet.org:**
- Visit: https://www.nuget.org/account/publishing
- Check policy exists for `rafitajaen/blueprintr`
- Verify:
  - Repository Owner: `rafitajaen`
  - Repository: `blueprintr`
  - Workflow File: `publish-nuget.yml` (no path, just filename)
  - Status: "Pending" or "Active"

**3. Check workflow permissions:**
```yaml
# In publish-nuget.yml, job should have:
permissions:
  id-token: write  # Required for OIDC token
  contents: read
```

**4. View workflow logs:**
- Actions > Publish to NuGet > Recent run
- Expand "NuGet Login (Trusted Publishing)" step
- Look for error messages

**Common Issues & Solutions:**

| Issue | Cause | Solution |
|-------|-------|----------|
| "Username is email" | Used email instead of username | Use profile name from nuget.org (NOT email) |
| "Policy not found" | Policy not configured | Add policy on nuget.org Trusted Publishing page |
| "Workflow file mismatch" | Wrong filename in policy | Use `publish-nuget.yml` (no `.github/workflows/` path) |
| "Repository mismatch" | Wrong owner/repo name | Verify exact repository name (case-insensitive) |
| "Permission denied" | Missing id-token permission | Add `id-token: write` to job permissions |
| "Policy pending" | First publish not completed | Publish once to activate policy |

**Migration Tip:**
- Keep `NUGET_API_KEY` as fallback while testing Trusted Publishing
- Once working, remove API key secret
- Workflow auto-detects and uses best available method

---

#### Problem: API Key Expired (Legacy)

**Symptoms:**
- Publish fails with 403 Forbidden
- Error: "The specified API key is invalid"

**Solution:**
1. Go to https://www.nuget.org/account/apikeys
2. Check expiration date
3. Generate new API key (see [Section 2.2](#22-generate-api-key))
4. Update GitHub Secret: NUGET_API_KEY
5. Re-run failed workflow

---

#### Problem: Package Takes Long to Appear

**Symptoms:**
- Workflow succeeded
- Package not visible on nuget.org

**Explanation:**
- NuGet indexing takes 5-15 minutes
- Package exists but not searchable yet

**Solutions:**
1. Wait 15 minutes
2. Check direct URL: `https://www.nuget.org/packages/Blueprintr.Endpoints/`
3. Click "Include prerelease" for alpha versions
4. Check "Package History" tab for recent versions

---

### 5.3 Documentation Issues

#### Problem: Docs Not Deploying

**Symptoms:**
- Documentation workflow runs
- GitHub Pages shows 404

**Diagnostic Steps:**

**1. Check GitHub Pages is enabled:**
- Settings > Pages
- Source should be "GitHub Actions"

**2. Check workflow permissions:**
```yaml
# In documentation.yml:
permissions:
  contents: read
  pages: write
  id-token: write
```

**3. Check workflow logs:**
- Actions > Build and Deploy Documentation
- Look for errors in "Deploy to GitHub Pages" step

**4. Verify artifact uploaded:**
- Check "Upload artifact" step
- Artifact should be named "github-pages"
- Size should be > 0 bytes

**Solutions:**
- Enable GitHub Pages if not enabled
- Verify permissions in workflow file
- Check for DocFX build errors

---

#### Problem: API Docs Missing Classes

**Symptoms:**
- Documentation builds successfully
- API reference is empty or incomplete

**Causes & Solutions:**

**1. XML documentation not generated:**
```xml
<!-- Verify in .csproj: -->
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

**2. DocFX configuration:**
```bash
# Test locally
docfx metadata docfx.json
# Should generate YAML files in api/ directory
```

**3. Classes not public:**
- Only public classes appear in API docs
- Make classes public if they should be documented

**4. XML comments missing:**
```csharp
/// <summary>
/// This comment will appear in docs
/// </summary>
public class MyClass { }
```

---

#### Problem: 404 on GitHub Pages

**Symptoms:**
- Workflow succeeded
- URL shows 404 Not Found

**Solutions:**

**1. Wait a few minutes:**
- First deployment takes 5-10 minutes
- CDN propagation adds 1-2 minutes

**2. Check deployment status:**
- Actions > Build and Deploy Documentation
- Look for "Deploy to GitHub Pages" step
- Check "View deployment" link

**3. Verify URL:**
- Correct: `https://rafitajaen.github.io/blueprintr/`
- Incorrect: `https://rafitajaen.github.io/blueprintr` (missing trailing slash sometimes matters)

**4. Check Pages settings:**
- Settings > Pages
- Custom domain (if any) should be correct
- Enforcement of HTTPS

---

### 5.4 Branch Protection Issues

#### Problem: Can't Find Status Check

**Symptoms:**
- Configuring branch protection
- Status check "build-and-test" doesn't appear in search

**Cause:**
- Status checks appear only after running at least once

**Solution:**
1. Push a change to trigger CI
2. Wait for CI to complete
3. Return to branch protection settings
4. Search for "build-and-test"
5. Select it from dropdown

**Alternative:**
- Create PR first
- CI runs
- Then configure branch protection

---

#### Problem: Admin Can Bypass Protection

**Symptoms:**
- Admin can push directly to main
- Protection rules seem ignored

**Explanation:**
- By default, admins can bypass rules

**Solution:**
- Enable: "Do not allow bypassing the above settings"
- Checkbox in branch protection rules
- Applies to administrators too

---

### 5.5 Version Issues

#### Problem: Wrong Version Calculated

**Symptoms:**
- Expected version: 1.0.1
- MinVer calculates: 1.0.0-alpha.0.5

**Diagnostic:**
```bash
# Debug MinVer with verbose output
minver -v d

# Check all tags
git tag -l

# Check tags on remote
git ls-remote --tags origin

# Show commits since last tag
git log --oneline $(git describe --tags --abbrev=0)..HEAD
```

**Common Issues:**

**1. Tag not pushed to remote:**
```bash
git tag -l  # Shows tag locally
git ls-remote --tags origin  # Tag missing on remote

# Solution:
git push origin 1.0.0
```

**2. Tag on different branch:**
```bash
# Check which branch contains tag
git branch --contains 1.0.0

# If not on main, merge or cherry-pick
```

**3. Fetch depth too shallow:**
```yaml
# In workflow, must have:
- uses: actions/checkout@v4
  with:
    fetch-depth: 0  # Gets all history
```

---

#### Problem: Version Stuck at 0.0.0-alpha.0.X

**Explanation:**
- No stable tags created yet
- This is expected for early development

**Solution:**
- Create first stable tag when ready:
```bash
git tag 1.0.0
git push origin 1.0.0
```

**When to create first stable tag:**
- API is stable
- Core functionality complete
- Ready for production use
- Documentation complete

---

### 5.6 Common Error Messages

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "Resource not accessible by integration" | Missing workflow permissions | Add required permissions to workflow YAML |
| "The process '/usr/bin/dotnet' failed with exit code 1" | Build or test failure | Check build logs for specific compilation/test error |
| "No test assemblies found" | Tests not building or wrong path | Verify test project builds, check test discovery |
| "fatal: bad revision 'HEAD^'" | First commit, no parent | Add fallback in change detection: `\|\| echo ""` |
| "Invalid API key" | Expired or wrong NUGET_API_KEY | Generate new key, update secret |
| "Package version already exists" | Duplicate version without --skip-duplicate | Verify --skip-duplicate flag in publish command |
| "Pages deployment failed" | GitHub Pages not configured | Enable Pages in Settings with GitHub Actions source |
| "Reference to undefined environment 'github-pages'" | Environment doesn't exist yet | GitHub creates on first deployment, re-run workflow |
| "Trusted publishing policy not found" | Policy not configured on nuget.org | Add policy in Trusted Publishing settings |
| "Username must not be an email address" | Used email instead of username in NUGET_USERNAME | Use profile name from nuget.org (top-right corner) |
| "Workflow file does not match" | Wrong filename in Trusted Publishing policy | Use exact filename: `publish-nuget.yml` (no path) |
| "Repository not found for trusted publishing" | Wrong owner/repo in policy | Verify exact repository name (rafitajaen/blueprintr) |
| "Token exchange failed" | Missing id-token: write permission | Add permission to job in workflow |
| "No NuGet authentication configured!" | No secrets set | Add NUGET_USERNAME (recommended) or NUGET_API_KEY (legacy) |

---

### 5.7 Getting Additional Help

**If issues persist:**

1. **Check workflow logs:**
   - Actions tab
   - Click on failed workflow
   - Expand failed step
   - Read error messages carefully

2. **Enable debug logging:**
   - Settings > Secrets > Actions
   - Add: `ACTIONS_RUNNER_DEBUG` = `true`
   - Add: `ACTIONS_STEP_DEBUG` = `true`
   - Re-run workflow

3. **Search existing issues:**
   - https://github.com/rafitajaen/blueprintr/issues

4. **Create new issue:**
   - Include workflow logs
   - Include error messages
   - Include steps to reproduce

---

## Summary

### Quick Reference

**GitHub Repository Setup:**
1. Enable GitHub Pages (Settings > Pages > GitHub Actions)
2. Configure branch protection (Settings > Branches)
3. Add secrets (see authentication options below)

**NuGet Setup (Recommended - Trusted Publishing):**
1. Create account at nuget.org
2. Configure Trusted Publishing policy:
   - Owner: `rafitajaen`
   - Repository: `blueprintr`
   - Workflow: `publish-nuget.yml`
3. Add GitHub Secret: `NUGET_USERNAME` (your nuget.org username)

**NuGet Setup (Legacy - API Key):**
1. Create account at nuget.org
2. Generate API key with `Blueprintr.*` pattern (365 days)
3. Add GitHub Secret: `NUGET_API_KEY`

**Versioning:**
- Pre-release: Auto-increments on each commit (`0.0.0-alpha.0.X`)
- Stable: Create git tag (`git tag 1.0.0 && git push origin 1.0.0`)

**Workflows:**
- CI: Runs on all pushes and PRs
- NuGet Publish: Runs on main with src/ changes (after CI passes)
- Documentation: Runs on main with docs changes, or tags (after CI passes)

**Key Commands:**
```bash
# Check version
minver

# Create stable release
git tag 1.0.0
git push origin 1.0.0

# Test locally
dotnet build
dotnet test
```

**Important URLs:**
- NuGet Packages: https://www.nuget.org/packages/Blueprintr.Endpoints/
- Documentation: https://rafitajaen.github.io/blueprintr/
- GitHub Repo: https://github.com/rafitajaen/blueprintr

---

## Next Steps

1. Follow [Section 1](#1-github-repository-settings) to configure repository
2. Follow [Section 2](#2-nuget-account-configuration) to set up NuGet
3. Run tests from [Section 4](#4-workflow-testing) to verify setup
4. Create first stable release when ready (see [Section 3.4](#34-stable-release-process))

For questions or issues, see [Section 5.7](#57-getting-additional-help).

---

**Last Updated:** 2026-01-27
**Version:** 2.0 (Added Trusted Publishing support)
**Maintainer:** Blueprintr Team
