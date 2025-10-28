#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using UnityEditor;

using UnityEngine;

namespace CSVToSO
{
    [CreateAssetMenu(fileName = "CSVToSOExecutor", menuName = "CSVToSO/CSVToSOExecutor")]
    public partial class CSVToSOExecutor : ScriptableObject
    {

        [SerializeField, FolderPath] private string _CSVFolder;
        [SerializeField, FolderPath] private string _CSFolder;
        [SerializeField, FolderPath] private string _SOFolder;
        [SerializeField] private SearchOption _searchOption = SearchOption.AllDirectories;
        [SerializeField] private AbsCompileSetup _compileSetup;

        public void GenAllCS(Predicate<string> fileNameFilter, ISupportGenCS genCS)
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    IncludePrivateMembers = true,
                };
                string projectPath = Path.GetDirectoryName(Application.dataPath);
                Dictionary<string, string> csFileDic = new Dictionary<string, string>();
                string[] filePaths = Directory.GetFiles(_CSVFolder, "*.csv", _searchOption);
                genCS.GenCSPrepare(filePaths);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filePath = Path.Combine(projectPath, filePaths[i]);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (!fileNameFilter(fileName)) continue;
                    string relativeFilePath = Path.GetRelativePath(_CSVFolder, filePaths[i]).Replace(".csv", string.Empty);
                    using var reader = new StreamReader(filePath);
                    using var csv = new CsvReader(reader, config);
                    genCS.GenCS(csv, relativeFilePath, fileName, csFileDic);
                }

                bool compileResult = CheckCompilationErrors(csFileDic.Values, out List<string> errors);
                if (compileResult)
                {
                    genCS.GenCSFinalize(csFileDic);
                    Debug.Log($"Generate complete: {csFileDic.Count} .cs file(s)");
                    string csFolderPath = Path.Combine(projectPath, _CSFolder);
                    if (!Directory.Exists(csFolderPath))
                    {
                        Directory.CreateDirectory(csFolderPath);
                    }
                    foreach (KeyValuePair<string, string> csFile in csFileDic)
                    {
                        string subFolder = Path.GetDirectoryName(csFile.Key);
                        if (!string.IsNullOrEmpty(subFolder))
                        {
                            subFolder = Path.Combine(csFolderPath, subFolder);
                            if (!Directory.Exists(subFolder))
                            {
                                Directory.CreateDirectory(subFolder);
                            }
                        }
                        File.WriteAllText(Path.Combine(csFolderPath, csFile.Key + ".cs"), csFile.Value);
                    }
                }
                else
                {
                    foreach (string error in errors)
                    {
                        Debug.LogError(error);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Generate fail");
                throw e;
            }
            finally
            {
                genCS.GenCSClear();
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }

        public void GenAllSO(Predicate<string> fileNameFilter, ISupportGenSO genSO)
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                string projectPath = Path.GetDirectoryName(Application.dataPath);
                string soFolderPath = Path.Combine(projectPath, _SOFolder);
                if (!Directory.Exists(soFolderPath))
                {
                    Directory.CreateDirectory(soFolderPath);
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null,
                    IncludePrivateMembers = true,
                };
                Dictionary<string, ScriptableObject> assetDic = new Dictionary<string, ScriptableObject>();
                string[] filePaths = Directory.GetFiles(_CSVFolder, "*.csv", _searchOption);
                genSO.GenSOPrepare(filePaths);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filePath = filePaths[i];
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (!fileNameFilter(fileName)) continue;
                    string relativeFilePath = Path.GetRelativePath(_CSVFolder, filePaths[i]).Replace(".csv", string.Empty);
                    using var reader = new StreamReader(filePath);
                    using var csv = new CsvReader(reader, config);
                    genSO.GenSO(csv, relativeFilePath, fileName, assetDic, _compileSetup);
                }
                genSO.GenSOFinalize(assetDic);
                foreach (KeyValuePair<string, ScriptableObject> soAsset in assetDic)
                {
                    string subFolder = Path.GetDirectoryName(soAsset.Key);
                    if (!string.IsNullOrEmpty(subFolder))
                    {                        
                        subFolder = Path.Combine(soFolderPath, subFolder);
                        if (!Directory.Exists(subFolder))
                        {
                            Directory.CreateDirectory(subFolder);
                        }
                    }
                    AssetDatabase.CreateAsset(soAsset.Value, Path.Combine(_SOFolder, soAsset.Key + ".asset"));
                }
                Debug.Log($"Sync CSV to SO complete");
            }
            catch (Exception e)
            {
                Debug.LogError($"Sync CSV to SO fail");
                throw e;
            }
            finally
            {
                genSO.GenSOClear();
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }

        private bool CheckCompilationErrors(ICollection<string> sourceCodes, out List<string> errors)
        {
            _compileSetup.GetRoslynConfig(out var references, out var parseOptions, out var compilationOptions);

            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (string sourceCode in sourceCodes)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(
                    text: sourceCode,
                    options: parseOptions
                ));
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: null,
                syntaxTrees: syntaxTrees,
                references: references,
                options: compilationOptions
            );

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                var diagnostics = result.Diagnostics
                    .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error)
                    .ToList();

                if (!result.Success)
                {
                    errors = new List<string>();
                    foreach (var diagnostic in diagnostics)
                    {
                        // Format the error message to include line, and column
                        var location = diagnostic.Location.GetLineSpan();
                        errors.Add(
                            $"Error: {diagnostic.Id}: {diagnostic.GetMessage()}\n" +
                            $"Line: {location.StartLinePosition.Line + 1}, Col: {location.StartLinePosition.Character + 1}), File:\n" + (diagnostic.Location.SourceTree == null ? "" :
                            $"{diagnostic.Location.SourceTree.GetText()}")
                        );
                    }

                    return false;
                }
                else
                {
                    errors = null;
                    return true;
                }
            }
        }
    }
}
#endif