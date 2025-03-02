using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WellReceivedData
{
    public string RawData;
    public string Scheme;
    public string Host;
    public DuplicateDictionary<string, string> Args;

    public WellReceivedData(string _RawData, string _scheme, string _host, string _args)
    {
        RawData = _RawData;
        Scheme = _scheme;
        Host = _host;
        Args = ParseStringToDuplicateDictionary(_args);
    }

    private DuplicateDictionary<string, string> ParseStringToDuplicateDictionary(string _sourceData)
    {
        if (string.IsNullOrEmpty(_sourceData))
            return new DuplicateDictionary<string, string>();
            
        DuplicateDictionary<string, string> tempDictionary = new DuplicateDictionary<string, string>();
        string[] chunk = _sourceData.Split(',');
        foreach (string tempChunk in chunk)
        {
            string[] pairs = tempChunk.Split('=');
            if (pairs.Length == 2)
                tempDictionary.Add(pairs[0], pairs[1]);
            else if (pairs.Length == 1)
                tempDictionary.Add(pairs[0], "");
            else
            {
                Debug.LogError("Parse DuplicateDictionary failed.source: " + _sourceData);
                return new DuplicateDictionary<string, string>();
            }
        }
        return tempDictionary;
    }
}

public class DuplicateDictionary<T, List>
{
    private Dictionary<string, List<string>> maps = new Dictionary<string, List<string>>();

    public string GetValue(string key)
    {
        if (maps != null && maps.ContainsKey(key))
        {
            return maps[key][0];
        }
        else
            return null;
    }

    public string GetValue(string key, int index)
    {
        if (maps != null && maps.ContainsKey(key) && maps[key].Count > index)
        {
            return maps[key][index];
        }
        else
            return null;
    }

    public List<string> GetValues(string key)
    {
        List<string> output = new List<string>();
        if (maps != null && maps.ContainsKey(key))
        {
            for (int i = 0; i < maps[key].Count; i++)
            {
                output.Add(maps[key][i]);
            }
        }
        else
            return null;
        return output;
    }

    public void Remove(string key)
    {
        if (maps != null && maps.ContainsKey(key))
        {
            maps.Remove(key);
        }
    }

    public void Remove(string key, string value)
    {
        if (maps != null && maps.ContainsKey(key))
        {
            for (int i = 0; i < maps[key].Count; i++)
            {
                if (maps[key][i] == value)
                {
                    maps[key].RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Remove(string key, int index)
    {
        if (maps != null && maps.ContainsKey(key) && maps[key].Count > index)
        {
            maps[key].RemoveAt(index);
        }
    }

    public void Add(string key, string value)
    {
        if (maps != null)
        {
            if (maps.ContainsKey(key))
            {
                maps[key].Add(value);
            }
            else
            {
                maps.Add(key, new List<string>() { value });
            }
        }
    }
}