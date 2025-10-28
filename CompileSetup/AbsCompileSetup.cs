#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using UnityEngine;

namespace CSVToSO
{
    public abstract class AbsCompileSetup : ScriptableObject
    {
        public abstract Type GetConfigTypeByName(string typeName);
        public abstract void GetRoslynConfig(
            out HashSet<MetadataReference> references,
            out CSharpParseOptions parseOptions,
            out CSharpCompilationOptions compilationOptions);
    }
}
#endif