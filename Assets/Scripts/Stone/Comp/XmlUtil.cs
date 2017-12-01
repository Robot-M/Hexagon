using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Stone.Core
{
	public class XmlUtil
	{
		public XmlUtil ()
		{
		}

		public static string SerializeObject(object obj, System.Type ty)
		{
			MemoryStream steam = new MemoryStream ();
			XmlSerializer xs = new XmlSerializer (ty);
			XmlTextWriter xmlTextWriter = new XmlTextWriter (steam, Encoding.UTF8);
			xs.Serialize (xmlTextWriter, obj);
			steam = (MemoryStream)xmlTextWriter.BaseStream;
			return UTF8ByteArrayToString (steam.ToArray ());
		}

		public static void CreateXml(string fileName, string dataStr)
		{
			StreamWriter writer = File.CreateText (fileName);
			writer.Write (dataStr);
			writer.Close ();
		}
			
		public static string UTF8ByteArrayToString(byte[] bytes)
		{
			UTF8Encoding encoding = new UTF8Encoding ();
			return encoding.GetString (bytes);
		}

		public static byte[] StringToUTF8ByteArray(string dataStr)
		{
			UTF8Encoding encoding = new UTF8Encoding ();
			return encoding.GetBytes (dataStr);
		}

		public static object DeserializeObject(string dataStr, System.Type ty)
		{
			XmlSerializer xs = new XmlSerializer (ty);
			MemoryStream steam = new MemoryStream (StringToUTF8ByteArray(dataStr));
			return xs.Deserialize (steam);
		}

		public static string LoadXml(string fileName)
		{
			StreamReader reader = File.OpenText (fileName);
			string dataStr = reader.ReadToEnd ();
			reader.Close ();
			return dataStr;
		}
	}
}

