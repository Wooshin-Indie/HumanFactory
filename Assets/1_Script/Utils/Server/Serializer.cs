using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HumanFactory.Util
{
	public static class Serializer
	{
		public static byte[] JsonToByteArray(string path)
		{
			string jsonData = File.ReadAllText(path);
			return Zip(jsonData.Trim());
		}

		public static byte[] JsonToByteArray<T>(T data)
		{
			string jsonData = JsonUtility.ToJson(data);
			return Zip(jsonData.Trim());
		}

		public static T ByteArrayToObject<T>(byte[] bytes)
		{
			string jsonData = Unzip(bytes);
			Debug.Log(jsonData);
			T objectData = JsonUtility.FromJson<T>(jsonData);
			return objectData;
		}

		/** Compressions **/
		private static byte[] Zip(string str)
		{
			Debug.Log("Unzip bytes : " + str.Length);
			var bytes = Encoding.UTF8.GetBytes(String.Concat(str.Where(c => !Char.IsWhiteSpace(c))));

			using (var msi = new MemoryStream(bytes))
			using (var mso = new MemoryStream())
			{
				using (var gs = new GZipStream(mso, CompressionMode.Compress))
				{
					msi.CopyTo(gs);
				}
				return mso.ToArray();
			}
		}
		private static string Unzip(byte[] bytes)
		{
			using (var msi = new MemoryStream(bytes))
			using (var gs = new GZipStream(msi, CompressionMode.Decompress))
			using (var mso = new MemoryStream())
			{
				gs.CopyTo(mso);
				return Encoding.UTF8.GetString(mso.ToArray());
			}
		}
	}
}