﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Effekseer.Internal
{
	[Serializable]
	public class EffekseerTextureResource
	{
		[SerializeField]
		public string path;
		[SerializeField]
		public Texture2D texture;
			
#if UNITY_EDITOR
		public static EffekseerTextureResource LoadAsset(string dirPath, string resPath) {
			Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(dirPath + "/" + resPath);

			var res = new EffekseerTextureResource();
			res.path = resPath;
			res.texture = texture;
			return res;
		}
		public static bool InspectorField(EffekseerTextureResource res) {
			EditorGUILayout.LabelField(res.path);
			var result = EditorGUILayout.ObjectField(res.texture, typeof(Texture2D), false) as Texture2D;
			if (result != res.texture) {
				res.texture = result;
				return true;
			}
			return false;
		}
#endif
	};
}

namespace Effekseer
{
	using Internal;

	public class EffekseerResourcePath
	{
		public int Version;
		public float Scale = 1.0f;

		public List<string> TexturePathList = new List<string>();
		public List<string> SoundPathList = new List<string>();
		public List<string> ModelPathList = new List<string>();
	}


	public class EffekseerEffectAsset : ScriptableObject
	{
		[SerializeField]
		public byte[] efkBytes;

		[SerializeField]
		public EffekseerTextureResource[] textureResources;		
		[SerializeField]
		public EffekseerSoundResource[] soundResources;
		[SerializeField]
		public EffekseerModelResource[] modelResources;

		[SerializeField]
		public float Scale = 1.0f;

		internal static Dictionary<int, WeakReference> enabledAssets = new Dictionary<int, WeakReference>();
		internal static System.Random keyGenerator = new System.Random();
		internal static int gcCounter = 0;
		internal static List<int> removingTargets = new List<int>();

		int dictionaryKey = 0;

		/// <summary xml:lang="en">
		/// Get a magnification combined with internal and external one
		/// </summary>
		/// <summary xml:lang="ja">
		/// 外部と内部の拡大率を合わせた拡大率を取得する。
		/// </summary>
		public float Magnification
		{
			get
			{
				if (EffekseerSystem.IsValid)
				{
					return EffekseerSystem.Instance.GetEffectMagnification(this);
				}
				return 0.0f;
			}
		}

		void OnEnable()
		{
			if(efkBytes != null && efkBytes.Length > 0)
			{
				LoadEffect();
			}

			//Debug.Log("EffekseerEffectAsset.OnEnable");
		}

		public void LoadEffect()
		{
			if (EffekseerSystem.IsValid)
			{
				EffekseerSystem.Instance.LoadEffect(this);
			}

			while (true)
			{
				dictionaryKey = keyGenerator.Next();
				if (!enabledAssets.ContainsKey(dictionaryKey))
				{
					enabledAssets.Add(dictionaryKey, new WeakReference(this));
					break;
				}
			}

			gcCounter++;

			// GC
			if (gcCounter > 20)
			{
				removingTargets.Clear();

				foreach (var kv in enabledAssets)
				{
					EffekseerEffectAsset target = kv.Value.Target as EffekseerEffectAsset;

					if (target == null)
					{
						removingTargets.Add(kv.Key);
					}
				}

				foreach (var k in removingTargets)
				{
					enabledAssets.Remove(k);
				}

				gcCounter = 0;
			}
		}


		void OnDisable()
		{
			enabledAssets.Remove(dictionaryKey);
			if (EffekseerSystem.IsValid) {
				EffekseerSystem.Instance.ReleaseEffect(this);
			}

			//Debug.Log("EffekseerEffectAsset.OnDisable");
		}

		public EffekseerTextureResource FindTexture(string path)
		{
			int index = Array.FindIndex(textureResources, (r) => (path == r.path));
			return (index >= 0) ? textureResources[index] : null;
		}
		
		public EffekseerSoundResource FindSound(string path)
		{
			int index = Array.FindIndex(soundResources, (r) => (path == r.path));
			return (index >= 0) ? soundResources[index] : null;
		}
		
		public EffekseerModelResource FindModel(string path)
		{
			int index = Array.FindIndex(modelResources, (r) => (path == r.path));
			return (index >= 0) ? modelResources[index] : null;
		}

		public static bool ReadResourcePath(byte[] data, ref EffekseerResourcePath resourcePath)
		{
			if (data.Length < 4 || data[0] != 'S' || data[1] != 'K' || data[2] != 'F' || data[3] != 'E')
			{
				return false;
			}

			int filepos = 4;

			// Get Format Version number
			resourcePath.Version = BitConverter.ToInt32(data, filepos);
			filepos += 4;

			resourcePath.TexturePathList = new List<string>();
			resourcePath.SoundPathList = new List<string>();
			resourcePath.ModelPathList = new List<string>();

			// Get color texture paths
			{
				int colorTextureCount = BitConverter.ToInt32(data, filepos);
				filepos += 4;
				for (int i = 0; i < colorTextureCount; i++)
				{
					resourcePath.TexturePathList.Add(ReadString(data, ref filepos));
				}
			}

			if (resourcePath.Version >= 9)
			{
				// Get normal texture paths
				int normalTextureCount = BitConverter.ToInt32(data, filepos);
				filepos += 4;
				for (int i = 0; i < normalTextureCount; i++)
				{
					resourcePath.TexturePathList.Add(ReadString(data, ref filepos));
				}

				// Get normal texture paths
				int distortionTextureCount = BitConverter.ToInt32(data, filepos);
				filepos += 4;
				for (int i = 0; i < distortionTextureCount; i++)
				{
					resourcePath.TexturePathList.Add(ReadString(data, ref filepos));
				}
			}

			if (resourcePath.Version >= 1)
			{
				// Get sound paths
				int soundCount = BitConverter.ToInt32(data, filepos);
				filepos += 4;
				for (int i = 0; i < soundCount; i++)
				{
					resourcePath.SoundPathList.Add(ReadString(data, ref filepos));
				}
			}

			if (resourcePath.Version >= 6)
			{
				// Get sound paths
				int modelCount = BitConverter.ToInt32(data, filepos);
				filepos += 4;
				for (int i = 0; i < modelCount; i++)
				{
					resourcePath.ModelPathList.Add(ReadString(data, ref filepos));
				}
			}

			return true;
		}

		private static string ReadString(byte[] data, ref int filepos)
		{
			int length = BitConverter.ToInt32(data, filepos);
			filepos += 4;
			string str = Encoding.Unicode.GetString(data, filepos, (length - 1) * 2);
			filepos += length * 2;
			return str;
		}

#if UNITY_EDITOR
		public static void CreateAsset(string path)
		{
			byte[] data = File.ReadAllBytes(path);
			CreateAsset(path, data);
		}

		public static void CreateAsset(string path, byte[] data)
		{
			EffekseerResourcePath resourcePath = new EffekseerResourcePath();
			if (!ReadResourcePath(data, ref resourcePath))
			{
				return;
			}

			float defaultScale = 1.0f;

			string assetPath = Path.ChangeExtension(path, ".asset");
			var asset = AssetDatabase.LoadAssetAtPath<EffekseerEffectAsset>(assetPath);
			if(asset != null)
			{
				defaultScale = asset.Scale;
			}

			string assetDir = assetPath.Substring(0, assetPath.LastIndexOf('/'));

			bool isNewAsset = false;
			if(asset == null)
			{
				isNewAsset = true;
				asset = CreateInstance<EffekseerEffectAsset>();
			}

			asset.efkBytes = data;
			
			asset.textureResources = new EffekseerTextureResource[resourcePath.TexturePathList.Count];
			for (int i = 0; i < resourcePath.TexturePathList.Count; i++) {
				asset.textureResources[i] = EffekseerTextureResource.LoadAsset(assetDir, resourcePath.TexturePathList[i]);
			}
			
			asset.soundResources = new EffekseerSoundResource[resourcePath.SoundPathList.Count];
			for (int i = 0; i < resourcePath.SoundPathList.Count; i++) {
				asset.soundResources[i] = EffekseerSoundResource.LoadAsset(assetDir, resourcePath.SoundPathList[i]);
			}
			
			asset.modelResources = new EffekseerModelResource[resourcePath.ModelPathList.Count];
			for (int i = 0; i < resourcePath.ModelPathList.Count; i++) {
				asset.modelResources[i] = EffekseerModelResource.LoadAsset(assetDir, resourcePath.ModelPathList[i]);
			}

			asset.Scale = defaultScale;

			if(isNewAsset)
			{
				AssetDatabase.CreateAsset(asset, assetPath);
			}
			else
			{
				EditorUtility.SetDirty(asset);
			}

			AssetDatabase.Refresh();
		}

#endif
	}
}
