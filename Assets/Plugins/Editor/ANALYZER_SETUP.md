# Unity Code Analyzers Setup Guide

## Overview

Unity doesn't provide standalone code analyzers like traditional .NET development. Instead, Unity relies on IDE integrations and built-in analysis tools. This guide explains what to set up for optimal code analysis in Unity projects.

## What You DON'T Need to Download

❌ **Unity-specific Roslyn Analyzers** - Unity doesn't provide these
❌ **Separate analyzer DLLs** - Not applicable for Unity
❌ **Third-party Unity analyzers** - Limited options available

## What You SHOULD Set Up

### 1. IDE-Specific Unity Support

#### Visual Studio (Windows)
```
Tools > Extensions and Updates > Install:
- Visual Studio Tools for Unity (VSTU)
- Unity Play Mode Tests Support
```

**Configuration**:
- `Tools > Options > Text Editor > C# > Advanced > Enable Unity integration`
- `Tools > Options > Text Editor > C# > Code Style > Configure Unity-specific rules`

#### JetBrains Rider
```
Settings > Plugins > Install/Enable:
- Unity Support (usually pre-installed)
- Unity Log Viewer
- Unity YAML Syntax Support
```

**Configuration**:
- `Settings > Editor > Inspections > Unity > Enable all Unity-specific inspections`
- `Settings > Editor > Code Style > C# > Import Unity code style`

#### Visual Studio Code
```
Extensions to install:
- C# (ms-dotnettools.csharp)
- Unity Code Snippets (kleber-swf.unity-code-snippets)
- Unity Tools (tobiah.unity-tools)
- Unity Meta Files Watcher (PTD.vscode-unitymeta)
```

### 2. Unity Built-in Analysis Tools

Unity provides several built-in code analysis features:

#### Console Warnings and Errors
- **Location**: `Window > General > Console`
- **Features**: Compile-time warnings, runtime errors, performance warnings
- **Usage**: Always keep console open during development

#### Script Inspector Warnings
- **Location**: Inspector panel when selecting scripts
- **Features**: Serialization warnings, component relationship issues
- **Usage**: Check inspector when adding components

#### Unity Analyzer (Built-in)
- **Location**: Automatically runs during compilation
- **Features**: Unity-specific rule checking
- **Common Warnings**:
  - `UNT0001`: Empty Unity message methods
  - `UNT0002`: Inefficient tag comparisons
  - `UNT0003`: Incorrect component access patterns

### 3. EditorConfig Setup (Already Included)

The project includes `.editorconfig` with Unity-specific settings:

```ini
# Unity-specific settings
[*.cs]
indent_style = space
indent_size = 4
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

# Unity script templates
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
```

## Recommended Setup Steps

### Step 1: Configure Your IDE

1. **Install Unity-specific extensions** (see above)
2. **Import Unity code style settings**
3. **Enable Unity-specific inspections/rules**

### Step 2: Enable Unity Analysis

1. **Enable Development Build warnings**:
   ```
   Edit > Project Settings > Player > Development Build ✓
   ```

2. **Enable Script Debugging**:
   ```
   Edit > Project Settings > Player > Script Debugging ✓
   ```

3. **Configure Console Settings**:
   ```
   Console Window > Right-click > 
   - Error Pause ✓
   - Warnings Pause (optional)
   - Log Entries > Stack Trace Logging > Script Only
   ```

### Step 3: Set Up Custom Code Analysis (Optional)

If you want additional static analysis:

#### SonarQube for Unity
```bash
# Install SonarQube scanner
dotnet tool install --global dotnet-sonarscanner

# Configure for Unity project
sonar-scanner -D"sonar.projectKey=summit-sprint" \
              -D"sonar.sources=Assets" \
              -D"sonar.exclusions=Assets/ThirdParty/**,Library/**"
```

#### Unity-specific Linting Rules

Create custom rules for your team:

```csharp
// Example custom analyzer rule
[System.AttributeUsage(System.AttributeTargets.Method)]
public class UnityMessageAttribute : System.Attribute
{
    // Mark Unity message methods to avoid "unused method" warnings
}
```

## IDE-Specific Configuration Files

### Visual Studio (.vs/ProjectSettings.json)
```json
{
  "CurrentProjectSetting": "Unity",
  "UnityIntegration": true,
  "ScriptDebugging": true
}
```

### Rider (.idea/projectSettingsUpdater.xml)
```xml
<application>
  <component name="ProjectSettingsUpdater">
    <option name="UNITY_PROJECT" value="true" />
    <option name="UNITY_AUTO_REFRESH" value="true" />
  </component>
</application>
```

### VS Code (.vscode/settings.json)
```json
{
  "unity.logLevel": "info",
  "unity.enableSolutionGeneration": true,
  "omnisharp.useGlobalMono": "never",
  "files.exclude": {
    "**/*.booproj": true,
    "**/*.pidb": true,
    "**/*.suo": true,
    "**/*.user": true,
    "**/*.userprefs": true,
    "**/*.unityproj": true,
    "**/*.dll": true,
    "**/*.exe": true,
    "**/Library": true,
    "**/obj": true,
    "**/Temp": true
  }
}
```

## Unity-Specific Code Quality Rules

### Naming Conventions (Enforced by CodingStandards.cs)
```csharp
// Public members - PascalCase
public int PlayerCount { get; set; }
public void StartGame() { }

// Private fields - camelCase
private float networkTickRate;
private bool isGameActive;

// Constants - UPPER_CASE
private const float MAX_CONNECTION_TIME = 30f;

// Unity events - PascalCase
private void Awake() { }
private void OnTriggerEnter(Collider other) { }
```

### Performance Rules
```csharp
// ✅ Good - Cached reference
private Transform myTransform;
void Awake() { myTransform = transform; }

// ❌ Bad - Repeated GetComponent calls
void Update() { 
    transform.position = newPosition; // Expensive!
}

// ✅ Good - Use CompareTag
if (gameObject.CompareTag("Player"))

// ❌ Bad - String comparison
if (gameObject.tag == "Player")
```

### Unity Message Optimization
```csharp
// ✅ Good - Only implement needed messages
void Update() {
    if (shouldUpdate) {
        // Logic here
    }
}

// ❌ Bad - Empty Unity messages (remove these)
void Start() { }
void Update() { }
```

## Verification

### Test Your Setup

1. **Create a test script with intentional issues**:
```csharp
public class AnalyzerTest : MonoBehaviour
{
    void Start() { } // Should warn: Empty Unity message
    
    void Update() {
        if (gameObject.tag == "Player") { } // Should warn: Use CompareTag
        transform.position = Vector3.zero; // Should warn: Cache transform
    }
}
```

2. **Check that your IDE shows**:
   - Warnings for empty Unity messages
   - Suggestions for performance improvements
   - Formatting issues
   - Unused variable warnings

### Expected Warnings/Suggestions

Your properly configured IDE should show:
- `UNT0001`: Empty Unity message 'Start()' can be removed
- `UNT0002`: Use 'CompareTag' instead of string comparison
- Performance suggestion: Cache transform reference
- Code style: Formatting inconsistencies

## Summary

Instead of downloadable analyzers, Unity development relies on:
1. **IDE Unity extensions** for intelligent code analysis
2. **Built-in Unity compiler warnings** for Unity-specific issues
3. **EditorConfig** for consistent code formatting
4. **Custom coding standards** (like our `CodingStandards.cs`)

The setup focuses on IDE configuration rather than separate analyzer downloads. This approach provides better Unity-specific analysis and integrates seamlessly with Unity's development workflow.
