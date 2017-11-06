using System;  
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Stone.Comp  
{  
	public static class FileUtilEx
	{
		/// <summary>
		/// 获取所有文件夹中包含后缀为 extension 的文件路径
		/// </summary>
		/// <returns>The dirs.</returns>
		/// <param name="dirPath">Dir path.</param>
		/// <param name="extension">Extension.</param>
		public static List<string> GetDirs(string dirPath, string extension)
		{  
			Debug.Log("GetDirs dirPath = " + dirPath);
			List<string> dirs = new List<string> ();
			foreach (string path in Directory.GetFiles(dirPath))
			{  
				//获取所有文件夹中包含后缀为 extension 的路径
				if (System.IO.Path.GetExtension(path) == extension)
				{  
					dirs.Add(path);
//					Debug.Log("GetDirs path = " + path);
				}  
			}
			return dirs;
		}

		/// <summary>
		/// Gets the forlds.
		/// </summary>
		/// <returns>The forlds.</returns>
		/// <param name="dirPath">Dir path.</param>
		public static List<string> GetForlds(string dirPath)
		{  
			Debug.Log("GetForlds dirPath = " + dirPath);
			List<string> forlds = new List<string> ();
			foreach (string path in Directory.GetDirectories(dirPath))
			{  
				forlds.Add (path);
			}
			return forlds;
		}

		/// <summary>
		/// Gets the forlds.
		/// </summary>
		/// <returns>The forlds.</returns>
		/// <param name="dirPath">Dir path.</param>
		public static List<string> GetForldNames(string dirPath)
		{  
			Debug.Log("GetForldNames dirPath = " + dirPath);
			List<string> forlds = new List<string> ();

			DirectoryInfo info = new DirectoryInfo (dirPath);
			foreach (DirectoryInfo folder in info.GetDirectories()) {
				forlds.Add (folder.Name);
			}
			return forlds;
		}

		/// <summary>
		/// 获取所有文件夹中包含后缀为 extension 的文件名
		/// </summary>
		/// <returns>The file names.</returns>
		/// <param name="dirPath">Dir path.</param>
		/// <param name="extension">Extension.</param>
		public static List<string> GetFileNames(string dirPath, string extension)
		{  
			Debug.Log("GetFileNames dirPath = " + dirPath);
			List<string> names = new List<string> ();

			//获取文件信息
			DirectoryInfo direction = new DirectoryInfo(dirPath);
			FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

			for (int i = 0; i < files.Length; i++)
			{  
				//过滤掉临时文件
				if (files[i].Name.EndsWith(extension))
				{
					string name = Path.GetFileNameWithoutExtension(files[i].ToString());
					names.Add(name);
//					Debug.Log("GetFileNames name = " + name);
				}
			}
			return names;
		}

		public static bool HasFile(string fileName)
		{
			return File.Exists (fileName);
		}

		public static bool HasDirectory(string path)
		{
			return Directory.Exists (path);
		}

		public static void CreateDirectory(string path)
		{
			if (!Directory.Exists (path)) {
				Directory.CreateDirectory (path);
			}
		}
	}  
} 