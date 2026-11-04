# GitHub Actions Workflows

## Workflows

### 1. Build and Test (`build.yml`)
- **Trigger**: Push to any branch (except tags) or pull request to main
- **Purpose**: Builds the project and runs a dry-run of the package build
- **Does NOT publish** to NuGet

### 2. Build and Publish NuGet Package (`build-and-publish-nuget.yml`)
- **Trigger**: 
  - Push to main branch
  - Push to a tag (e.g., `v1.0.0`)
  - Manual workflow dispatch (via GitHub UI)
- **Purpose**: Builds the package and publishes to NuGet.org

## Setup

### Required Secrets

To publish to NuGet.org, you need to add the following GitHub repository secret:

1. **`NUGET_API_KEY`**: Your NuGet.org API key
   - Go to [NuGet.org](https://www.nuget.org/) and sign in
   - Click on your profile picture → API Keys
   - Create a new key with "Push" scope
   - Copy the key value
   - In your GitHub repository: Settings → Secrets → Actions → New repository secret
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key

### Manual Publishing

You can manually trigger a publish by:
1. Going to GitHub Actions tab
2. Selecting "Build and Publish NuGet Package" workflow
3. Clicking "Run workflow"
4. Optionally specifying a version number (if not using tag or nuspec version)

### Version Management

The package version is determined in this order:
1. If manually triggered with a version input → use that version
2. If triggered by a git tag (e.g., `v1.2.3`) → use the tag name (without 'v' prefix)
3. Otherwise → use the version from `Noesis.MonoGame.nuspec`

## Notes

- Both workflows run on Windows runners since the project uses native Noesis libraries
- The workflows require .NET 10.0.x SDK
- The `build_package.bat` script must be run in a Windows environment (cmd shell)
