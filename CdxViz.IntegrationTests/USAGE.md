# Guide d'utilisation des tests d'intégration CdxViz

## ?? Objectif

Ce projet de tests d'intégration permet de :
- ? Valider le fonctionnement de CdxViz avec différents paramètres
- ? Capturer la ligne de commande, les fichiers d'entrée/sortie pour chaque test
- ? Obtenir la couverture de code des différents appels
- ? Tester tous les formats supportés (Cytoscape et 3D Force Graph)

## ?? Démarrage rapide

### Prérequis
1. Compilez d'abord le projet CdxViz :
   ```bash
   dotnet build ..\CdxViz\CdxViz.csproj
   ```

2. Exécutez les tests :
   ```bash
   # Windows
   .\Run-Tests.ps1

   # Linux/Mac
   chmod +x run-tests.sh
   ./run-tests.sh
   ```

## ?? Options de test

### Tester un format spécifique

**Cytoscape uniquement :**
```powershell
# Windows
.\Run-Tests.ps1 -Formats cytoscape

# Linux/Mac
./run-tests.sh --formats cytoscape
```

**3D Force Graph uniquement :**
```powershell
# Windows
.\Run-Tests.ps1 -Formats 3dforce

# Linux/Mac
./run-tests.sh --formats 3dforce
```

### Spécifier le chemin de l'exécutable

```powershell
# Windows
.\Run-Tests.ps1 -ExecutablePath "D:\custom\path\CdxViz.exe"

# Linux/Mac
./run-tests.sh --executable-path /custom/path/CdxViz
```

### Activer la couverture de code

```powershell
# Windows
.\Run-Tests.ps1 -Coverage

# Linux/Mac
./run-tests.sh --coverage
```

Les rapports de couverture seront générés dans `TestResults/` au format OpenCover, Cobertura et JSON.

### Mode verbeux

```powershell
# Windows
.\Run-Tests.ps1 -Verbose

# Linux/Mac
./run-tests.sh --verbose
```

### Combiner les options

```powershell
# Windows
.\Run-Tests.ps1 -Coverage -Verbose -Formats cytoscape

# Linux/Mac
./run-tests.sh --coverage --verbose --formats cytoscape
```

## ?? Configuration via variables d'environnement

### Windows PowerShell
```powershell
# Spécifier l'exécutable
$env:CDXVIZ_EXECUTABLE_PATH = "D:\path\to\CdxViz.exe"

# Spécifier les formats à tester
$env:CDXVIZ_TEST_FORMATS = "cytoscape,3dforce"

# Exécuter les tests
dotnet test
```

### Linux/Mac Bash
```bash
# Spécifier l'exécutable
export CDXVIZ_EXECUTABLE_PATH=/path/to/CdxViz

# Spécifier les formats à tester
export CDXVIZ_TEST_FORMATS=cytoscape,3dforce

# Exécuter les tests
dotnet test
```

## ?? Structure des fichiers de sortie

Après l'exécution des tests, vous trouverez :

```
CdxViz.IntegrationTests/
??? bin/Debug/net9.0/TestsOutput/
?   ??? output-cytoscape-basic.json
?   ??? output-cytoscape-vulns.json
?   ??? output-cytoscape-licenses.json
?   ??? output-cytoscape-vulns-licenses.json
?   ??? output-cytoscape-show-groups.json
?   ??? output-cytoscape-only-vex.json
?   ??? output-cytoscape-only-vdr.json
?   ??? output-cytoscape-complex.json
?   ??? output-3dforce-basic.json
?   ??? output-3dforce-vulns.json
?   ??? ... (autres fichiers de test)
??? TestResults/
    ??? {guid}/
        ??? coverage.cobertura.xml
```

## ?? Scénarios de test couverts

Pour **chaque format** (Cytoscape et 3D Force Graph), les tests suivants sont exécutés :

| Scénario | Options | Description |
|----------|---------|-------------|
| Basic | aucune | Sortie de base sans options |
| WithVulnerabilities | `--with-vulns` | Inclut les vulnérabilités |
| WithLicenses | `--with-lics` | Inclut les licences |
| WithVulnsAndLicenses | `--with-vulns --with-lics` | Vulnérabilités + licences |
| ShowGroups | `--show-groups-in-nodes-labels` | Affiche les groupes dans les labels |
| OnlyVex | `--only-vex` | Mode VEX (vulnérabilités uniquement) |
| OnlyVdr | `--only-vdr` | Mode VDR (vulnérabilités + composants affectés) |
| Complex | `--with-vulns --with-lics --show-groups-in-nodes-labels` | Combinaison d'options |

## ?? Informations capturées par test

Chaque test capture et affiche :
- ? Nom du scénario
- ? Ligne de commande complète
- ? Fichier d'entrée
- ? Fichier de sortie
- ? Code de sortie
- ? Sortie standard
- ? Sortie d'erreur
- ? Taille du fichier généré

Exemple de sortie :
```
=== Test Scenario ===
Name: cytoscape_WithVulnerabilities
Command Line: D:\repos\...\CdxViz.exe --input "D:\...\test-dependency-tree-full.json" --output "D:\...\output-cytoscape-vulns.json" --format cytoscape --with-vulns
Input File: D:\...\test-dependency-tree-full.json
Output File: D:\...\TestsOutput\output-cytoscape-vulns.json
Exit Code: 0
? Test passed - Output file size: 12345 bytes
===================
```

## ? Validations effectuées

1. **Code de sortie** : Doit être 0 (succès)
2. **Existence du fichier** : Le fichier de sortie doit être créé
3. **JSON valide** : Le contenu doit être un JSON valide
4. **Structure Cytoscape** :
   - Présence de `elements`
   - Présence de `elements.nodes` (tableau)
   - Présence de `elements.edges` (tableau)
5. **Structure 3D Force Graph** :
   - Présence de `nodes` (tableau)
   - Présence de `links` (tableau)

## ?? Visualiser la couverture de code

Après avoir exécuté les tests avec l'option `-Coverage` :

### Avec ReportGenerator (recommandé)
```bash
# Installer ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Générer un rapport HTML
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html

# Ouvrir le rapport
# Windows
start TestResults/CoverageReport/index.html

# Linux
xdg-open TestResults/CoverageReport/index.html

# Mac
open TestResults/CoverageReport/index.html
```

### Dans Visual Studio
1. Extensions ? Manage Extensions
2. Installer "Fine Code Coverage"
3. Les résultats de couverture s'afficheront automatiquement après l'exécution des tests

## ?? Exécution depuis Visual Studio

1. Ouvrez le **Test Explorer** (Test ? Test Explorer)
2. Cliquez sur "Run All" pour exécuter tous les tests
3. Consultez les détails de chaque test dans la fenêtre de sortie

## ?? Conseils

- Les fichiers de sortie sont **conservés** pour inspection manuelle
- Exécutez d'abord `dotnet build ..\CdxViz\CdxViz.csproj` si vous modifiez le code source
- Utilisez `-Formats cytoscape` pour des tests plus rapides pendant le développement
- La couverture de code peut être combinée avec les autres options

## ?? Dépannage

### Erreur "CdxViz executable not found"
**Solution** : Compilez d'abord CdxViz ou spécifiez le chemin :
```bash
dotnet build ..\CdxViz\CdxViz.csproj
# OU
.\Run-Tests.ps1 -ExecutablePath "D:\path\to\CdxViz.exe"
```

### Tests qui échouent
1. Vérifiez les logs détaillés : `.\Run-Tests.ps1 -Verbose`
2. Inspectez les fichiers de sortie dans `bin/Debug/net9.0/TestsOutput/`
3. Vérifiez que le fichier de test existe : `TestData/test-dependency-tree-full.json`

## ?? Voir aussi

- [README.md](README.md) - Documentation technique
- [coverlet.runsettings](coverlet.runsettings) - Configuration de la couverture de code
