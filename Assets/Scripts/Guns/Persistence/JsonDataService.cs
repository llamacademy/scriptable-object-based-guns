using System;
using System.IO;
using UnityEngine;

namespace LlamAcademy.Guns.Persistence
{
    public class JsonDataService : IDataService
    {
        public bool SaveData<T>(string RelativePath, T Data)
        {
            string path = Application.persistentDataPath + RelativePath;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing file for the first time!");
                }

                using FileStream stream = File.Create(path);

                stream.Close();
                // Normally I use Newtonsoft.Json but JsonUtility serializes and deserializes object references really nicely
                // which is very convenient for ScriptableObjects
                File.WriteAllText(path, JsonUtility.ToJson(Data));
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        public T LoadData<T>(string RelativePath)
        {
            string path = Application.persistentDataPath + RelativePath;

            if (!File.Exists(path))
            {
                Debug.LogError($"Cannot load file at {path}. File does not exist!");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try
            {
                // Normally I use Newtonsoft.Json but JsonUtility serializes and deserializes object references really nicely
                // which is very convenient for ScriptableObjects
                T data = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw e;
            }
        }
    }
}
