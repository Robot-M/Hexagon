using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Stone.Core
{
	public class Piece
	{
		
		public List<Cell> cells;

		[XmlIgnore]
		public int count;

		[XmlIgnore]
		public bool isDirty = false;

		public Piece ()
		{
		}

		public Piece(List<Cell> cells)
		{
			this.cells = cells;
			this.count = cells.Count;
		}

		public static Piece GetRandomPiece(int radius)
		{
			List<Cell> cells = new List<Cell> ();
			Hex hex = new Hex ();

			for (int q = -radius; q <= radius; q++) {
				int r1 = Math.Max (-radius, -q - radius);
				int r2 = Math.Min (radius, -q + radius);
				for (int r = r1; r <= r2; r++) {
					Hex cHex = new Hex (q, r, -q - r);
					Cell cell = new Cell (hex, cHex);
					cells.Add (cell);
				}
			}

			return new Piece (cells);
		}
			
		public static Piece LoadPieceFromXml(string path)
		{
			Debug.Log ("LoadPieceFromXml path " + path);
			Piece piece = null;
			if (FileUtilEx.HasFile (path)) {
				string dataStr = XmlUtil.LoadXml (path);
				piece = XmlUtil.DeserializeObject (dataStr, typeof(Piece)) as Piece;
				piece.count = piece.cells.Count;
			} else {
				Debug.Log ("LoadPieceFromXml can't find");
			}
			return piece;
		}

		public static void SavePieceToXml(string path, Piece piece)
		{
			Debug.Log ("SavePieceToXml path " + path);
			if (piece.isDirty) {
				piece.isDirty = false;
				string dataStr = XmlUtil.SerializeObject (piece, typeof(Piece));
				XmlUtil.CreateXml (path, dataStr);
			} else {
				Debug.Log ("SavePieceToXml piece has nothing to save");
			}
		}
	}
}

