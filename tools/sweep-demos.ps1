<#
.SYNOPSIS
  Sweep every TinyLanguage demo (.tlg) through the published exe in file mode and report crashes,
  timeouts, and golden-output mismatches (stdout != the matching .expected file).

.DESCRIPTION
  The correct, reusable demo-sweep harness for the TinyLanguage orchestration (Build.md Phase 4.5
  demo convergence and Phase 5 final validation). It exists because two natural-looking PowerShell
  harness shapes produce FALSE results that have wasted real debugging cycles:

    * `Start-Process -PassThru` WITHOUT `-Wait`: after WaitForExit() the returned object's
      `.ExitCode` is null/blank, so `if ($p.ExitCode -ne 0)` is `$null -ne 0` = TRUE and EVERY
      demo is falsely reported as failed (seen as a spurious "519 of 520 failed" run).
    * Sharing ONE output file across demos: a previous process can still hold the redirect/output
      handle when the next opens it, yielding spurious "The process cannot access the file ...
      because it is being used by another process" failures for ~100 demos that actually pass.

  This harness avoids both: it uses System.Diagnostics.Process (reliable ExitCode), a UNIQUE output
  file per demo, async stdout/stderr drains (no pipe-buffer deadlock), and a per-demo timeout so an
  infinite-loop demo is reported as a TIMEOUT (Build.md TERMINATION rule) instead of hanging the run.

  Golden gate: when a <name>.expected file exists beside a <name>.tlg, the demo passes only if its
  file-mode stdout byte-matches .expected (CRLF->LF + trailing-newline normalized). Use -SkipGolden
  to check crash/timeout only.

  Exit code: 0 when every demo passes (FAILED=0 TIMEOUT=0 GOLD=0), else 1 - so callers can gate on it.

.PARAMETER Canonical
  The canonical solution root, e.g. Z:\repos\TinyLanguage.YYYY.MM.DD.HH

.PARAMETER TimeoutSeconds
  Per-demo timeout in seconds (default 5, matching the Build.md acceptance budget).

.PARAMETER SkipGolden
  Skip the golden-output comparison and check crash/timeout only.

.EXAMPLE
  powershell -File tools\sweep-demos.ps1 -Canonical Z:\repos\TinyLanguage.2026.05.29.02
#>
param(
  [Parameter(Mandatory = $true)] [string] $Canonical,
  [int] $TimeoutSeconds = 5,
  [switch] $SkipGolden
)

$dir = Join-Path $Canonical "TinyLanguage.DemoFiles"
$exe = Join-Path $dir "TinyLanguage.exe"
if (-not (Test-Path $exe)) {
  Write-Error "Published exe not found at $exe. Run: dotnet publish TinyLanguage -c Release"
  exit 2
}
$exeBytes = (Get-Item $exe).Length
$exeSizeMB = [math]::Round($exeBytes / 1MB, 2)
if ($exeBytes -lt 30MB) {
  Write-Error "$exe is only $exeSizeMB MB (expected ~36 MB self-contained single-file). This is likely the framework-dependent apphost - re-run: dotnet publish TinyLanguage -c Release"
  exit 2
}

$tmp = Join-Path $env:TEMP ("tlg_sweep_" + (Split-Path $Canonical -Leaf))
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $tmp | Out-Null

$timeoutMs = $TimeoutSeconds * 1000
$fail = New-Object System.Collections.ArrayList
$timeout = New-Object System.Collections.ArrayList
$gold = New-Object System.Collections.ArrayList
$tlgs = Get-ChildItem $dir -Filter *.tlg -File | Sort-Object Name

# Normalize before a golden compare: CRLF -> LF, drop trailing newlines.
function Get-NormalizedText([string] $s) {
  if ($null -eq $s) { return "" }
  return ($s -replace "`r`n", "`n").TrimEnd("`n")
}

foreach ($f in $tlgs) {
  $o = Join-Path $tmp ($f.BaseName + ".out")
  $psi = New-Object System.Diagnostics.ProcessStartInfo
  $psi.FileName = $exe
  $psi.Arguments = '"' + $f.FullName + '" "' + $o + '"'
  $psi.UseShellExecute = $false
  $psi.RedirectStandardError = $true
  $psi.RedirectStandardOutput = $true
  $psi.CreateNoWindow = $true
  $p = [System.Diagnostics.Process]::Start($psi)
  $errTask = $p.StandardError.ReadToEndAsync()
  $null = $p.StandardOutput.ReadToEndAsync()
  if (-not $p.WaitForExit($timeoutMs)) {
    try { $p.Kill($true) } catch { }
    [void]$timeout.Add($f.Name)
    continue
  }
  if ($p.ExitCode -ne 0) {
    [void]$fail.Add([pscustomobject]@{ Name = $f.Name; Err = (($errTask.Result -replace "\r?\n", " ").Trim()) })
    continue
  }
  # GOLDEN-OUTPUT GATE: when a <name>.expected golden file exists beside the .tlg,
  # the demo "passes" only if its file-mode stdout byte-matches .expected (after
  # CRLF->LF + trailing-newline normalization). Catches wrong-but-non-crashing demos
  # an exit-0-only gate ships silently. Disable with -SkipGolden.
  if (-not $SkipGolden) {
    $expPath = Join-Path $dir ($f.BaseName + ".expected")
    if (Test-Path $expPath) {
      $actualOut = if (Test-Path $o) { Get-Content $o -Raw } else { "" }
      $expectedOut = Get-Content $expPath -Raw
      if ((Get-NormalizedText $actualOut) -ne (Get-NormalizedText $expectedOut)) {
        [void]$gold.Add($f.Name)
      }
    }
  }
}

$report = Join-Path $tmp "failures.txt"
$fail | ForEach-Object { "$($_.Name) :: $($_.Err)" } | Set-Content -Encoding UTF8 $report

$goldReport = Join-Path $tmp "golden-mismatches.txt"
$gold | ForEach-Object { $_ } | Set-Content -Encoding UTF8 $goldReport

Write-Host ("TOTAL={0} FAILED={1} TIMEOUT={2} GOLD={3}  (exe {4} MB)" -f $tlgs.Count, $fail.Count, $timeout.Count, $gold.Count, $exeSizeMB)
Write-Host "REPORT=$report"
if ($gold.Count -gt 0) { Write-Host "GOLD_REPORT=$goldReport" }
if ($timeout.Count -gt 0) {
  Write-Host "=== TIMEOUTS ==="
  $timeout | ForEach-Object { Write-Host "TIMEOUT: $_" }
}
if ($fail.Count -gt 0) {
  Write-Host "=== FAILURES (crash / non-zero exit) ==="
  $fail | ForEach-Object { Write-Host ("{0} :: {1}" -f $_.Name, $_.Err) }
}
if ($gold.Count -gt 0) {
  Write-Host "=== GOLDEN MISMATCHES (ran OK but output != .expected) ==="
  $gold | ForEach-Object { Write-Host ("GOLD: {0}" -f $_) }
}

if ($fail.Count -gt 0 -or $timeout.Count -gt 0 -or $gold.Count -gt 0) { exit 1 } else { exit 0 }
