#if UNITY_EDITOR
using System.Collections.Generic;

using CsvHelper;

using UnityEngine;

namespace CSVToSO
{
    public abstract class AbsConfigParser : ScriptableObject { }

    //why those extra interfaces when I can just make some simple funciton or abtract funciton?
    //for the posibility of 1-n or n-1 or even n-n csv to cs/so converting.
    public interface ISupportGenCS
    {
        void GenCSPrepare(string[] allValidFilePath);
        void GenCS(CsvReader csv, string relativeFilePath, string fileName, Dictionary<string, string> csFileDic);
        void GenCSFinalize(Dictionary<string, string> csFileDic);
        void GenCSClear();
    }

    public interface ISupportGenSO
    {
        void GenSOPrepare(string[] allValidFilePath);
        void GenSO(CsvReader csv, string relativeFilePath, string fileName, Dictionary<string, ScriptableObject> assetDic, AbsCompileSetup compileSetup);
        void GenSOFinalize(Dictionary<string, ScriptableObject> assetDic);
        void GenSOClear();
    }
}
#endif