#if UNITY_EDITOR

using System.Collections.Generic;

using CsvHelper;

using Scriban;
using Scriban.Runtime;

using UnityEngine;

namespace CSVToSO
{
    [CreateAssetMenu(fileName = "NornalConfigParser", menuName = "CSVToSO/Parser/NornalConfigParser")]
    public class NornalConfigParser : AbsConfigParser, ISupportGenCS, ISupportGenSO
    {
        [SerializeField, Tooltip(@"Flatten the path make it far more easier when you want to build your Asset Bundle/Addressables tool chain.
Because the class name and the asset name are the same for normal config. But I still let it be an option for you.
For localization text config, it is forced to flatten out, as it has no meaning for keeping the csv folder structure.
Because all text of the same language code will end up in the same localization text config asset file, no matter if you use one csv file or separated csv files for that."
        )] 
        private bool _isFlattenFolder = true;
        [SerializeField] private TextAsset _template;
        private Template _parsedTemplate;
        private Template template
        {
            get
            {
                _parsedTemplate ??= Template.Parse(_template.text);
                return _parsedTemplate;
            }
        }

        [SerializeField] private List<string> _decDatas = new List<string>();

        public void GenCS(CsvReader csv, string relativeFilePath, string fileName, Dictionary<string, string> csFileDic)
        {
            csFileDic[_isFlattenFolder ? fileName : relativeFilePath] = template.Render(ParseHeader(csv, fileName));
        }

        public void GenCSClear()
        {
        }

        public void GenCSFinalize(Dictionary<string, string> csFileDic)
        {
        }

        public void GenCSPrepare(string[] allValidFilePath)
        {
        }

        public void GenSO(CsvReader csv, string relativeFilePath, string fileName, Dictionary<string, ScriptableObject> assetDic, AbsCompileSetup compileSetup)
        {
            SkipHeader(csv);
            NormalConfigBase configBase = CreateInstance(compileSetup.GetConfigTypeByName(fileName)) as NormalConfigBase;
            configBase.SyncFromCSV(csv);
            assetDic[_isFlattenFolder ? fileName : relativeFilePath] = configBase;
        }

        public void GenSOClear()
        {
        }

        public void GenSOFinalize(Dictionary<string, ScriptableObject> assetDic)
        {
        }

        public void GenSOPrepare(string[] allValidFilePath)
        {
        }

        private TemplateContext ParseHeader(CsvReader csv, string fileName)
        {
            TemplateContext templateContext = new TemplateContext();
            List<ScriptObject> columnDatas = new List<ScriptObject>();
            for (int rowIdx = 0; rowIdx < _decDatas.Count; rowIdx++)
            {
                csv.Read();
                int collumeIndex = 0;
                string data = csv.GetField(collumeIndex);
                while (data != null || collumeIndex < columnDatas.Count)
                {
                    switch (rowIdx)
                    {
                        case 0:
                            columnDatas.Add(new ScriptObject());
                            if (!string.IsNullOrEmpty(data))
                            {
                                columnDatas[collumeIndex][_decDatas[rowIdx]] = data;
                            }
                            break;
                        default:
                            if (!string.IsNullOrEmpty(data))
                            {
                                columnDatas[collumeIndex][_decDatas[rowIdx]] = data;
                            }
                            break;
                    }
                    collumeIndex++;
                    data = csv.GetField(collumeIndex);
                }
            }
            ScriptObject columns = new ScriptObject();
            columns["columns"] = new ScriptArray(columnDatas);
            templateContext.PushGlobal(columns);
            ScriptObject fileNameData = new ScriptObject();
            fileNameData["file_name"] = fileName;
            templateContext.PushGlobal(fileNameData);
            return templateContext;
        }        

        private void SkipHeader(CsvReader csv)
        {
            for (int i = 0; i < _decDatas.Count; i++) csv.Read();
        }
    }
}
#endif