<#
.SYNOPSIS
	Runs the d20Tek.DiceNotation benchmarks and appends trimmed Mean/Allocated results to a
	tracked history CSV, for lightweight regression detection over time.

.DESCRIPTION
	BenchmarkDotNet writes full diagnostic reports to BenchmarkDotNet.Artifacts/ (gitignored,
	since it is large and regenerated per machine/run). This script runs the benchmarks, reads
	the resulting "*-report.csv" files BenchmarkDotNet already produces, and appends one row per
	benchmark/parameter combination to benchmarks\BenchmarkHistory.csv (tracked in git) with just
	the fields needed to spot speed or allocation regressions: timestamp, git commit, class,
	method, params, mean (ns), and allocated bytes/op.

.PARAMETER Filter
	Optional BenchmarkDotNet --filter glob (for example "*NotationRollBenchmarks*"). Defaults to
	running all benchmarks.

.PARAMETER SkipRun
	If set, skips running the benchmarks and only parses the most recent artifacts already on
	disk. Useful for re-appending history from a run you already have.

.EXAMPLE
	.\Update-BenchmarkHistory.ps1
	.\Update-BenchmarkHistory.ps1 -Filter "*NotationRollBenchmarks*"
#>
param(
	[string]$Filter = "*",
	[switch]$SkipRun
)

$ErrorActionPreference = "Stop"

$scriptDir = $PSScriptRoot
$projectDir = $scriptDir
$solutionDir = Split-Path (Split-Path $projectDir -Parent) -Parent
$artifactsResultsDir = Join-Path $solutionDir "BenchmarkDotNet.Artifacts\results"
$historyPath = Join-Path (Split-Path $projectDir -Parent) "BenchmarkHistory.csv"

if (-not $SkipRun) {
	Write-Host "Running benchmarks (filter: $Filter)..."
	dotnet run -c Release --project $projectDir -- --filter $Filter
	if ($LASTEXITCODE -ne 0) {
		throw "Benchmark run failed with exit code $LASTEXITCODE."
	}
}

if (-not (Test-Path $artifactsResultsDir)) {
	throw "No BenchmarkDotNet artifacts found at '$artifactsResultsDir'. Run without -SkipRun first."
}

$gitCommit = "unknown"
if (Get-Command git -ErrorAction SilentlyContinue) {
	$result = git rev-parse --short HEAD 2>$null
	if ($LASTEXITCODE -eq 0 -and $result) { $gitCommit = $result }
}
$timestamp = (Get-Date).ToUniversalTime().ToString("o")

if (-not (Test-Path $historyPath)) {
	"Timestamp,GitCommit,Class,Method,Params,Mean_ns,Allocated_bytes" | Out-File -FilePath $historyPath -Encoding utf8
}

$reportFiles = Get-ChildItem -Path $artifactsResultsDir -Filter "*-report.csv"
if ($reportFiles.Count -eq 0) {
	throw "No '*-report.csv' files found in '$artifactsResultsDir'."
}

function ConvertTo-Nanoseconds([string]$value) {
	if ($value -match '^\s*([\d.,]+)\s*([a-zA-Z\u00b5\u03bc]+)\s*$') {
		$number = [double]::Parse($matches[1], [System.Globalization.CultureInfo]::InvariantCulture)
		$unit = $matches[2]
		switch -Regex ($unit) {
			'^ns$'        { return $number }
			'^ms$'        { return $number * 1000000 }
			'^s$'         { return $number * 1000000000 }
			'^.?s$'       { return $number * 1000 }   # matches "us"/"µs"/"μs" (any single lead char + s)
		}
	}
	return $null
}

function ConvertTo-Bytes([string]$value) {
	if ($value -match '^\s*([\d.,]+)\s*(B|KB|MB|GB)\s*$') {
		$number = [double]::Parse($matches[1], [System.Globalization.CultureInfo]::InvariantCulture)
		switch ($matches[2]) {
			"B" { return $number }
			"KB" { return $number * 1024 }
			"MB" { return $number * 1024 * 1024 }
			"GB" { return $number * 1024 * 1024 * 1024 }
		}
	}
	return $null
}

$rows = @()
foreach ($file in $reportFiles) {
	$className = $file.BaseName -replace '-report$', ''
	$records = Import-Csv -Path $file.FullName
	foreach ($record in $records) {
		$meanNs = ConvertTo-Nanoseconds $record.Mean
		$allocatedBytes = ConvertTo-Bytes $record.Allocated

		# Collect any extra [Params]-driven columns (anything not a known BenchmarkDotNet column)
		$knownColumns = @(
			"Method","Job","AnalyzeLaunchVariance","EvaluateOverhead","MaxAbsoluteError",
			"MaxRelativeError","MinInvokeCount","MinIterationTime","OutlierMode","Affinity",
			"EnvironmentVariables","Jit","LargeAddressAware","Platform","PowerPlanMode","Runtime",
			"AllowVeryLargeObjects","Concurrent","CpuGroups","Force","HeapAffinitizeMask",
			"HeapCount","NoAffinitize","RetainVm","Server","Arguments","BuildConfiguration","Clock",
			"EngineFactory","NuGetReferences","Toolchain","IsMutator","InvocationCount",
			"IterationCount","IterationTime","LaunchCount","MaxIterationCount",
			"MaxWarmupIterationCount","MemoryRandomization","MinIterationCount",
			"MinWarmupIterationCount","RunStrategy","UnrollFactor","WarmupCount","Mean","Error",
			"StdDev","StdErr","Median","Min","Max","Q1","Q3","Op/s","Ratio","RatioSD","Gen0","Gen1",
			"Gen2","Allocated","Alloc Ratio","Completed Workitems","Lock Contentions"
		)
		$paramColumns = $record.PSObject.Properties.Name | Where-Object { $_ -notin $knownColumns }
		$paramsText = ($paramColumns | ForEach-Object { "$_=$($record.$_)" }) -join ";"

		$rows += [PSCustomObject]@{
			Timestamp       = $timestamp
			GitCommit       = $gitCommit
			Class           = $className
			Method          = $record.Method
			Params          = $paramsText
			Mean_ns         = $meanNs
			Allocated_bytes = $allocatedBytes
		}
	}
}

function ConvertTo-CsvField([string]$value) {
	if ($null -eq $value) { return "" }
	if ($value -match '[,"\r\n]') {
		return '"' + ($value -replace '"', '""') + '"'
	}
	return $value
}

$rows | ForEach-Object {
	$line = "{0},{1},{2},{3},{4},{5},{6}" -f `
		(ConvertTo-CsvField $_.Timestamp), (ConvertTo-CsvField $_.GitCommit), (ConvertTo-CsvField $_.Class), `
		(ConvertTo-CsvField $_.Method), (ConvertTo-CsvField $_.Params), $_.Mean_ns, $_.Allocated_bytes
	$line | Out-File -FilePath $historyPath -Append -Encoding utf8
}

Write-Host "Appended $($rows.Count) row(s) to $historyPath"
