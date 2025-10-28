#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using UnityEditor.Compilation;

using UnityEngine;

namespace CSVToSO
{
    [CreateAssetMenu(fileName = "DefaultCompileSetup", menuName = "CSVToSO/CompileSetup/DefaultCompileSetup")]
    public class DefaultCompileSetup : AbsCompileSetup
    {
        [SerializeField, Tooltip(
@"Assembly name of the assembly that the config will be built into.
Default is Assembly-CSharp, CSVToSO and CSVToSO_Editor.
If you do not make custom assembly definition for config, just leave it be. If you do, replace Assembly-CSharp with yours")]
        private string[] _assemblyNames = Const.CSVToCSSetting.DEFAULT_ASSEMBLY_NAME;
        [SerializeField, Tooltip(
@"namespace of the config if your config use one")]
        private string _namespace = "";

        private Dictionary<string, Type> _cachedType = new Dictionary<string, Type>();

        public override Type GetConfigTypeByName(string typeName)
        {
            if (_cachedType.TryGetValue(typeName, out Type type)) return type;
            string[] assemblyNames = (_assemblyNames == null) ? Const.CSVToCSSetting.DEFAULT_ASSEMBLY_NAME : _assemblyNames;
            string fullTypeName = string.IsNullOrEmpty(_namespace) ? typeName : _namespace + "." + typeName;
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Array.IndexOf(assemblyNames, assembly.GetName().Name) < 0) continue;
                type = assembly.GetType(fullTypeName);
                if (type == null) continue;
                _cachedType[typeName] = type;
                return type;
            }
            Debug.LogError($"Can not find type {fullTypeName} inside {string.Join(", ", assemblyNames)}");
            return null;
        }

        public override void GetRoslynConfig(
            out HashSet<MetadataReference> references,
            out CSharpParseOptions parseOptions,
            out CSharpCompilationOptions compilationOptions)
        {
            references = new HashSet<MetadataReference>();
            string[] assemblyNames = (_assemblyNames == null) ? Const.CSVToCSSetting.DEFAULT_ASSEMBLY_NAME : _assemblyNames;
            parseOptions = null;
            bool allowUnsafe = false;
            LanguageVersion langVersion = LanguageVersion.Latest;
            List<string> defines = new List<string>(){
                "DEBUG", "TRACE", "UNITY_EDITOR", "ENABLE_BURST_AOT"
            };
            foreach (Assembly assembly in CompilationPipeline.GetAssemblies(AssembliesType.Player | AssembliesType.Editor))
            {
                if (Array.IndexOf(assemblyNames, assembly.name) < 0) continue;
                allowUnsafe = allowUnsafe || assembly.compilerOptions.AllowUnsafeCode;
                defines.AddRange(assembly.defines);                
                if (assembly.name != "CSVToSO")
                {                    
                    Enum.TryParse(assembly.compilerOptions.LanguageVersion, out langVersion);
                }
                references.Add(MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(Application.dataPath), assembly.outputPath)));
                CollectAllRefAssembly(references, assembly);
            }

            compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,
                allowUnsafe: allowUnsafe,
                concurrentBuild: true
            );            
            parseOptions = new CSharpParseOptions(
                languageVersion: langVersion,
                preprocessorSymbols: defines
            );
        }

        private void CollectAllRefAssembly(HashSet<MetadataReference> references, Assembly assembly)
        {
            foreach (string compiledAssemblyPath in assembly.compiledAssemblyReferences)
            {
                references.Add(MetadataReference.CreateFromFile(compiledAssemblyPath));
            }
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            foreach (Assembly refAssembly in assembly.assemblyReferences)
            {
                references.Add(MetadataReference.CreateFromFile(Path.Combine(projectPath, refAssembly.outputPath)));
                if (refAssembly.assemblyReferences != null)
                {
                    foreach (Assembly refAssembly_2nd in refAssembly.assemblyReferences)
                    {
                        CollectAllRefAssembly(references, refAssembly_2nd);
                    }
                }
            }
        }
    }
}
#endif