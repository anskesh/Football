using UnityEditor;
using UnityEngine;

namespace Football.Core
{
    public class ResourceLoader
    {
        private string _path = "Assets/Resources/Configurations";
        private string _pathForSearch = "Configurations";
        
        public T GetConfiguration<T>() where T:Configuration
        {
            var nameDef =  $"{_pathForSearch}/{typeof(T).Name}";
            var asset = Resources.Load<T>(nameDef);
            
            if (!asset)
            {
                var name =  $"{_path}/{typeof(T).Name}";
                asset = ScriptableObject.CreateInstance<T>();
                
                #if UNITY_EDITOR
                AssetDatabase.CreateAsset(asset, $"{name}.asset");
                AssetDatabase.SaveAssets();
                #endif
            }
            
            return asset;
        }
    }
}