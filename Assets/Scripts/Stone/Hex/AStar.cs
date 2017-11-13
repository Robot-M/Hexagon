using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stone.Core
{
	public class AStar
	{
		public AStar ()
		{
		}

		private static Cell getMin(Dictionary<Cell, int> function) {
			int min = int.MaxValue;
			Cell minHex = null;
			foreach(KeyValuePair<Cell, int> kvp in function) {
				if(kvp.Value <= min) {
					min = kvp.Value;
					minHex = kvp.Key;
				}
			}
			return minHex;
		}

		private static List<Cell> generatePath(Dictionary<Cell,Cell> cameFrom, Cell final, int size) {
			List<Cell> path = new List<Cell>(size);
			path.Add (final);
			for (int i = 1; i < size; i++) {
				Cell find;
				cameFrom.TryGetValue(path[i-1], out find);
				path.Add (find);
			}
			path.Reverse ();
			return path;
		}

		/// <summary>
		/// A*寻路
		/// </summary>
		/// <param name="map">所在map</param>
		/// <param name="start">起始点</param>
		/// <param name="goal">目标点</param>
		/// <param name="max">最大搜索距离</param>
		/// <param name="distance">攻击距离</param>
		public static List<Cell> search(Map map, Cell start, Cell goal, int max=64, int distance=0)
		{
			if (start.Distance (goal) <= distance) {
				// 在范围内就不需要寻找了
				return new List<Cell> ();
			}

			max += distance;

			Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();
			Dictionary<Cell, int> gScore = new Dictionary<Cell, int> ();
			Dictionary<Cell, int> fScore = new Dictionary<Cell, int> ();
			gScore.Add (start, 0);
			fScore.Add (start, start.Distance (goal));

			while (fScore.Count > 0) {
				Cell current = getMin(fScore);
				if(current.Distance (goal) <= distance) {
					int length = 0;
					gScore.TryGetValue(current, out length);
					return generatePath(cameFrom, current, length);
				}
				fScore.Remove(current);

				foreach(Cell neighbor in map.GetCellNeighbors(current)) {
					if(!neighbor.IsWalkable()) {
						continue;
					}
					if(gScore.ContainsKey(neighbor) && !fScore.ContainsKey(neighbor)) {
						continue;
					}

					int tentativeGScore = 0;
					gScore.TryGetValue(current, out tentativeGScore);
					++tentativeGScore;
					if(tentativeGScore > max) {
						continue;
					}

					int neighborGScore = 0;
					gScore.TryGetValue(neighbor, out neighborGScore);
					if(!fScore.ContainsKey(neighbor) || tentativeGScore < neighborGScore) {
						int newFScore = tentativeGScore + neighbor.Distance (goal);
						if(newFScore > max) {
							continue;
						}
						cameFrom.Add(neighbor, current);
						gScore.Add(neighbor, tentativeGScore);
						fScore.Add(neighbor, newFScore);
					}
				}
			}

			return new List<Cell> ();
		}
	}
}

