#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using CsvHelper;

using UnityEngine;

namespace CSVToSO
{
    [CreateAssetMenu(fileName = "LocalizeConfigParser", menuName = "CSVToSO/Parser/LocalizeConfigParser")]
    public class LocalizeConfigParser : AbsConfigParser, ISupportGenSO
    {
        private Dictionary<LanguageCode, LocalizeConfig> _localizeCfgDic;

        private LocalizeConfig this[LanguageCode languageCode]
        {
            get
            {
                if (!_localizeCfgDic.TryGetValue(languageCode, out var localizeConfig))
                {
                    localizeConfig = CreateInstance<LocalizeConfig>();
                    localizeConfig.Init(languageCode);
                    _localizeCfgDic[languageCode] = localizeConfig;
                }
                return localizeConfig;
            }
        }

        public void GenSO(CsvReader csv, string relativeFilePath, string fileName, Dictionary<string, ScriptableObject> assetDic, AbsCompileSetup compileSetup)
        {
            csv.Read();
            int collumeIndex = 1;
            string data = csv.GetField(collumeIndex);
            List<LocalizeConfig> localizeConfigs = new List<LocalizeConfig>();
            while (data != null)
            {
                if (int.TryParse(data, out int enumInt))
                {
                    localizeConfigs.Add(this[(LanguageCode)enumInt]);
                }
                else
                {
                    localizeConfigs.Add(this[(LanguageCode)Enum.Parse(typeof(LanguageCode), data)]);
                }
                collumeIndex++;
                data = csv.GetField(collumeIndex);
            }
            while (csv.Read())
            {
                data = csv.GetField(0);
                string key = data;
                for (int i = 1; i <= localizeConfigs.Count; i++)
                {
                    localizeConfigs[i - 1].Add(key, csv.GetField(i));
                }
            }
        }

        public void GenSOClear()
        {
            _localizeCfgDic = null;
        }

        public void GenSOFinalize(Dictionary<string, ScriptableObject> assetDic)
        {
            assetDic.Clear();
            foreach (var localizeData in _localizeCfgDic)
            {
                assetDic[localizeData.Key.ToString()] = localizeData.Value;
            }
        }

        public void GenSOPrepare(string[] allValidFilePath)
        {
            _localizeCfgDic = new Dictionary<LanguageCode, LocalizeConfig>();
        }
    }
}
#endif