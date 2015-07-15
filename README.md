# cake-skeleton-app
Building a C# application with Cake:
 * build solution via MSBuild
 * run tests with NUnit
 * create zip archive

## Steps
 * create `src` folder, create solution
 * adjust `build.cake`
 * build: `.\build.ps1`
 * add `SolutionInfo.cs` to Solution and and links from all projects, remove duplicated info from `AssemblyInfo.cs` files
 * build again

## Stumbling Blocks
 * powershell v3+ is required to run the `build.ps1` file (`$PSVersionTable.PSVersion`)
 * doesn't run from a network share without further work (`CasPol.exe` ...)
