# PowerLauncher Build Script
# Publishes the app, uninstaller, and creates the zip for release

Write-Host "=== PowerLauncher Build ===" -ForegroundColor Cyan

# Step 1: Publish main app
Write-Host "`n[1/4] Publishing launcher..." -ForegroundColor Yellow
dotnet publish MinecraftLauncher/MinecraftLauncher.csproj -c Release -r win-x64 --self-contained -o publish_output
if ($LASTEXITCODE -ne 0) { Write-Host "Failed!" -ForegroundColor Red; exit 1 }

# Step 2: Publish uninstaller
Write-Host "[2/4] Publishing uninstaller..." -ForegroundColor Yellow
dotnet publish PowerLauncherUninstaller/PowerLauncherUninstaller.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o publish_uninstall
if ($LASTEXITCODE -ne 0) { Write-Host "Failed!" -ForegroundColor Red; exit 1 }

# Step 3: Copy uninstaller into publish output
Write-Host "[3/4] Packaging..." -ForegroundColor Yellow
Copy-Item "publish_uninstall/Uninstall.exe" "publish_output/Uninstall.exe" -Force

# Step 4: Create zip
Write-Host "[4/4] Creating zip..." -ForegroundColor Yellow
Remove-Item "PowerLauncher.zip" -Force -ErrorAction SilentlyContinue
Compress-Archive -Path "publish_output/*" -DestinationPath "PowerLauncher.zip" -Force

Write-Host "`n=== Done ===" -ForegroundColor Green
Write-Host "Output: PowerLauncher.zip"
Write-Host "Upload to GitHub release with: gh release upload v1.0.0 PowerLauncher.zip --clobber"
