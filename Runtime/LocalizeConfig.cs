using System;
using System.Collections.Generic;

using UnityEngine;

public class LocalizeConfig : ScriptableObject
{
    [Serializable]
    private struct Data
    {
        public string key;
        public string value;
    }

    [SerializeField] private LanguageCode _languageCode;
    [SerializeField] private List<Data> _datas;

    private Dictionary<string, string> _dataDic;

    public string this[string key]
    {
        get
        {
            if (_dataDic.TryGetValue(key, out var localizedText))
            {
                return localizedText;
            }
            return string.Empty;
        }
    }
    public LanguageCode languageCode => _languageCode;

    private void OnEnable()
    {
        _dataDic = new Dictionary<string, string>();
        for (int i = 0; _datas != null && i < _datas.Count; i++)
        {
            _dataDic[_datas[i].key] = _datas[i].value;
        }
    }

#if UNITY_EDITOR
    public void Init(LanguageCode languageCode)
    {
        _languageCode = languageCode;
        _datas = new List<Data>();
        _dataDic = new Dictionary<string, string>();
    }

    public void Add(string key, string value)
    {
        if (_dataDic.ContainsKey(key))
        {
            Debug.LogWarning($"{key}: localize key duplicated !!!");
            return;
        }
        _datas.Add(new Data() { key = key, value = value });
        _dataDic[key] = value;
    }
#endif
}
