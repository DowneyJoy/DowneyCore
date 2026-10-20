using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Data;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.Utils;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AT.Core.Runtime.Manager
{
    public class AssetsManager : AutoSingleton<AssetsManager>,ISingleton
    {
        [SerializeField] private PreloadAssetsData assets;
        
        private PreloadAssetsData _preloadAssets;
        
        private readonly Dictionary<string, AudioClip> _cachedAudioClips = new Dictionary<string, AudioClip>();
        private readonly Dictionary<string, GameObject> _cachedPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Sprite> _cachedSprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Object> _cachedMisc = new Dictionary<string, Object>();

        void ISingleton.Single(string id)
        {
            PreparePreloadAssets();
        }

        void PreparePreloadAssets()
        {
            CleanupCache();
            LoadPreloadAssetsData();

        }

        void LoadPreloadAssetsData()
        {
            string path = ContentDefinitions.GetPreloadAssetsPath();
            if (!File.Exists(path))
            {
                Log.Fatal($"Object: {id}, Type: {GetType().FullName}, Action: Load Preload Assets Data, State: Failed at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, Reason: Missing File:{path}");

                Bootstrap.ForceCrash();
            }
            string json = File.ReadAllText(path);
            PreloadAssetsData assets = PreloadAssetsData.FromJson(json);
            _preloadAssets = assets.Clone() as PreloadAssetsData;

            if (!_preloadAssets)
            {
                Log.Error($"Object: {id}, Type: {this.GetType().FullName}, Action: Copy Preload Assets Data, State: Failed at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, Reason: Invalid Data.");

                Bootstrap.ForceCrash();
                return;
            }
        }

        void CleanupCache()
        {
            _cachedAudioClips.Clear();
            _cachedPrefabs.Clear();
            _cachedSprites.Clear();
            _cachedMisc.Clear();
        }

        public T LoadAsset<T>(string path, AssetType type) where T : Object
        {
            T asset;
            switch (type)
            {
                case AssetType.AudioClip:
                    asset = Cache(path,_cachedAudioClips) as T;
                    break;
                case AssetType.GameObject:
                    asset = Cache(path,_cachedPrefabs) as T;
                    break;
                case AssetType.Sprite:
                    asset = Cache(path,_cachedSprites) as T;
                    break;
                default:
                    asset = Cache(path,_cachedMisc) as T;
                    break;
            }
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Load Assets(Cached), State: {asset.GetInstanceID()}, Type: {asset.GetType().FullName}");
            return asset;
        }

        T Cache<T>(string key, Dictionary<string, T> dict) where T : Object
        {
            return dict.GetValueOrDefault(key);
        }

        public IEnumerator PreloadAssets()
        {
            int total = _preloadAssets.entries.Count;
            int loaded = 0;
            LoadingManager.instance.StartLoading(TaskDefinitions.PRELOAD,null,null, f =>
            {
                Log.Info($"[{f:P}] Object: {id}, Type: {GetType().FullName}, Action: Preload Assets");
            });
            foreach (var entry in _preloadAssets.entries)
            {
                switch (entry.type)
                {
                    case AssetType.AudioClip:
                        yield return LoadSingleAsset<AudioClip>(entry, asset =>
                        {
                            loaded++;
                            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Preload Assets, State: {asset.GetInstanceID()}, Type: {asset.GetType().FullName}");
                        });
                        break;
                    case AssetType.GameObject:
                        yield return LoadSingleAsset<GameObject>(entry, asset =>
                        {
                            loaded++;
                            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Preload Assets, State: {asset.GetInstanceID()}, Type: {asset.GetType().FullName}");
                        });
                        break;
                    case AssetType.Sprite:
                        yield return LoadSingleAsset<Sprite>(entry, asset =>
                        {
                            loaded++;
                            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Preload Assets, State: {asset.GetInstanceID()}, Type: {asset.GetType().FullName}");
                        });
                        break;
                    default:
                        yield return LoadSingleAsset<Object>(entry, asset =>
                        {
                            loaded++;
                            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Preload Assets, State: {asset.GetInstanceID()}, Type: {asset.GetType().FullName}");
                        });
                        break;
                }
                            
                float progress = (float)loaded / total;
                LoadingManager.instance.Report(TaskDefinitions.PRELOAD,progress);
            }
            yield return new WaitForEndOfFrame();
            LoadingManager.instance.StopLoading(TaskDefinitions.PRELOAD);
        }

        IEnumerator LoadSingleAsset<T>(AssetEntry asset, Action<T> callback, Func<T> alt=null) where T : Object
        {
            // 因为使用了自动化加载，所以没有在场景中挂载并声明PreloadAssets引用
            // 使用MonoSingleton<T>可以解决此问题，需要序列化一个PreloadAssetsData并克隆其数据
            // if (Application.isEditor)
            // {
            //     callback(asset.value as T);
            //     yield break;
            // }

            AsyncOperationHandle operation =
                asset.type switch
                {
                    AssetType.AudioClip => Addressables.LoadAssetAsync<AudioClip>(asset.key),
                    AssetType.GameObject => Addressables.LoadAssetAsync<GameObject>(asset.key),
                    AssetType.Sprite => Addressables.LoadAssetAsync<Sprite>(asset.key),
                    _ => Addressables.LoadAssetAsync<Object>(asset.key)
                };
            while (!operation.IsDone)
                yield return null;

            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                T cache = operation.Result as T;
                switch (asset.type)
                {
                    case AssetType.AudioClip:
                        AddCache<AudioClip>(asset.key,cache as AudioClip,_cachedAudioClips);
                        break;
                    case AssetType.GameObject:
                        AddCache<GameObject>(asset.key, cache as GameObject, _cachedPrefabs);
                        break;
                    case AssetType.Sprite:
                        AddCache<Sprite>(asset.key, cache as Sprite, _cachedSprites);
                        break;
                    default:
                        AddCache<Object>(asset.key, cache, _cachedMisc);
                        break;
                }

                if (cache == null)
                {
                    callback(alt());
                    yield break;
                }
                callback(cache);
            }
            else
            {
                callback(alt());
            }
        }

        void AddCache<T>(string path, T asset, Dictionary<string, T> dict) where T : Object
        {
            dict[path] = asset;
        }

        void Clear<T>(Dictionary<string, T> dict) where T : Object
        {
            foreach (var pair in dict)
            {
                Addressables.Release(pair.Value);
            }
        }

        public void ClearCache()
        {
            Clear(_cachedAudioClips);
            Clear(_cachedPrefabs);
            Clear(_cachedSprites);
            Clear(_cachedMisc);
            
            CleanupCache();
        }
    }
}