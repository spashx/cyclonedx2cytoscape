# Quick Start Test

This is a minimal test to verify that the integration tests are properly configured.

## Step 1: Build CdxViz
```bash
cd ..
dotnet build CdxViz/CdxViz.csproj
cd CdxViz.IntegrationTests
```

## Step 2: Run a single test
```bash
# Run only cytoscape basic test
$env:CDXVIZ_TEST_FORMATS = "cytoscape"
dotnet test --filter "Name~cytoscape_Basic"
```

## Expected Output

You should see something like:
```
=== Test Scenario ===
Name: cytoscape_Basic
Command Line: D:\repos\...\CdxViz.exe --input "..." --output "..." --format cytoscape
Input File: ...\test-dependency-tree-full.json
Output File: ...\TestsOutput\output-cytoscape-basic.json
Exit Code: 0
? Test passed - Output file size: XXXX bytes
===================

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1
```

## Step 3: Verify output file

Check that the file exists:
```bash
dir bin\Debug\net9.0\TestsOutput\output-cytoscape-basic.json
```

## Troubleshooting

If you see "CdxViz executable not found":
1. Make sure you built CdxViz first
2. Check the path: `CdxViz/bin/Debug/net9.0/CdxViz.exe`
3. Or specify the path manually:
   ```bash
   $env:CDXVIZ_EXECUTABLE_PATH = "D:\full\path\to\CdxViz.exe"
   dotnet test --filter "Name~cytoscape_Basic"
   ```
