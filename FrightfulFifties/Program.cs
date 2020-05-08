﻿using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrightfulFifties
{
	internal class Program
	{
		private static int validSolutions = 0;

		private static void Main()
		{
			var allTiles = new List<Tile>
			{
				new Tile(22, 8),
				new Tile(9, 20),
				new Tile(10, 8),
				new Tile(7, 12),
				new Tile(18, 13),
				new Tile(5, 16),
				new Tile(17, 15),
				new Tile(19, 11),
				new Tile(20, 21),
				new Tile(19, 23),
				new Tile(4, 5),
				new Tile(14, 6),
				new Tile(17, 3),
				new Tile(6, 2),
				new Tile(1, 18),
				new Tile(24, 7)
			};
			GroupsOfFour(allTiles);
		}

		private static void GroupsOfFour(IList<Tile> allTiles)
		{
			Console.WriteLine("Getting all stateful tiles...");
			var allStatefullTiles = GetAllStatefulTiles(allTiles);
			Console.WriteLine("Got all stateful tiles.");

			Console.WriteLine("Getting rows...");
			var rows = new List<RowNode>();
			foreach (var statefulTileCombination in new Combinations<StatefulTile>(allStatefullTiles, 4))
			{
				if (ValidRow(statefulTileCombination))
				{
					var rowNode = new RowNode(statefulTileCombination);
					rows.Add(rowNode);
					foreach (var tile in rowNode.Row)
					{
						tile.RowNodes.Add(rowNode);
					}
				}
			}
			Console.WriteLine($"Got {rows.Count} rows.");

			//Console.WriteLine($"Computing uniqueness graph...");
			//foreach (var row in rows)
			//{
			//	foreach (var otherRow in rows)
			//	{
			//		if (row.Unique(otherRow))
			//		{
			//			row.UniqueEdges.Add(otherRow);
			//		}
			//	}
			//}
			//Console.WriteLine($"Computed uniqueness graph.");

			//PrintRowCount(allStatefullTiles);

			Console.WriteLine("Finding valid grids...");
			foreach (var rowA in rows)
			{
				var tileA = rowA.Row[0];
				var validColAs = tileA.RowNodes.Where(x =>
					x != rowA && !x.Row.Any(y =>
						y != tileA && rowA.Row.Any(z =>
							z != tileA && IsSameTile(y, z))));
				foreach (var colA in validColAs)
				{
					var tileB = rowA.Row[1];
					var validColBs = tileB.RowNodes.Where(x =>
						x != rowA && !x.Row.Any(y =>
							y != tileB && rowA.Row.Any(z =>
								z != tileB && IsSameTile(y, z))
							|| colA.Row.Any(z =>
								IsSameTile(y, z))));
					foreach (var colB in validColBs)
					{
						var tileC = rowA.Row[2];
						var validColCs = tileC.RowNodes.Where(x =>
							x != rowA && !x.Row.Any(y =>
								y != tileC && rowA.Row.Any(z =>
									z != tileC && IsSameTile(y, z))
								|| colA.Row.Any(z =>
									IsSameTile(y, z))
								|| colB.Row.Any(z =>
									IsSameTile(y, z))));
						foreach (var colC in validColCs)
						{
							var tileD = rowA.Row[3];
							var validColDs = tileD.RowNodes.Where(x =>
								x != rowA && !x.Row.Any(y =>
									y != tileD && rowA.Row.Any(z =>
										z != tileD && IsSameTile(y, z))
									|| colA.Row.Any(z =>
										IsSameTile(y, z))
									|| colB.Row.Any(z =>
										IsSameTile(y, z))
									|| colC.Row.Any(z =>
										IsSameTile(y, z))));
							foreach (var colD in validColDs)
							{
								// create new objects and remove row A from the colums
								var colARowB = CopyRow(colA);
								colARowB.Row.Remove(tileA);
								var colBRowB = CopyRow(colB);
								colBRowB.Row.Remove(tileB);
								var colCRowB = CopyRow(colC);
								colCRowB.Row.Remove(tileC);
								var colDRowB = CopyRow(colD);
								colDRowB.Row.Remove(tileD);
								var validRowBs = rows.Except(new List<RowNode>
								{ rowA, colA, colB, colC, colD })
									.Where(x =>
										colARowB.Row.Any(y => x.Row.Contains(y))
										&& colBRowB.Row.Any(y => x.Row.Contains(y))
										&& colCRowB.Row.Any(y => x.Row.Contains(y))
										&& colDRowB.Row.Any(y => x.Row.Contains(y)));
								foreach (var validRowB in validRowBs)
								{
									var colARowC = CopyRow(colARowB);
									var colBRowC = CopyRow(colBRowB);
									var colCRowC = CopyRow(colCRowB);
									var colDRowC = CopyRow(colDRowB);
									foreach (var tile in validRowB.Row)
									{
										colARowC.Row.Remove(tile);
										colBRowC.Row.Remove(tile);
										colCRowC.Row.Remove(tile);
										colDRowC.Row.Remove(tile);
									}
									var validRowCs = rows.Except(new List<RowNode>
									{ rowA, validRowB, colA, colB, colC, colD })
										.Where(x =>
										colARowC.Row.Any(y => x.Row.Contains(y))
										&& colBRowC.Row.Any(y => x.Row.Contains(y))
										&& colCRowC.Row.Any(y => x.Row.Contains(y))
										&& colDRowC.Row.Any(y => x.Row.Contains(y)));
									foreach (var validRowC in validRowCs)
									{
										var colARowD = CopyRow(colARowC);
										var colBRowD = CopyRow(colBRowC);
										var colCRowD = CopyRow(colCRowC);
										var colDRowD = CopyRow(colDRowC);
										foreach (var tile in validRowC.Row)
										{
											colARowD.Row.Remove(tile);
											colBRowD.Row.Remove(tile);
											colCRowD.Row.Remove(tile);
											colDRowD.Row.Remove(tile);
										}
										var validRowD = rows.Except(new List<RowNode>
										{ rowA, validRowB, validRowC, colA, colB, colC, colD })
											.SingleOrDefault(x =>
												colARowD.Row.Any(y => x.Row.Contains(y))
												&& colBRowD.Row.Any(y => x.Row.Contains(y))
												&& colCRowD.Row.Any(y => x.Row.Contains(y))
												&& colDRowD.Row.Any(y => x.Row.Contains(y)));
										if (validRowD != null)
										{
											//PrintRows(new List<RowNode> { rowA, validRowB, validRowC, validRowD });
											//PrintRows(new List<RowNode>
											//{ rowA, validRowB, validRowC, validRowD, columnA, columnB, columnC, columnD });
											// create and organise board as 2D array
											var board = new StatefulTile[4, 4];
											var i = 0;
											// apply row A
											foreach (var tile in rowA.Row)
											{
												board[0, i++] = tile;
											}
											// apply row B
											board[1, 0] = colA.Row.Single(x => validRowB.Row.Contains(x));
											board[1, 1] = colB.Row.Single(x => validRowB.Row.Contains(x));
											board[1, 2] = colC.Row.Single(x => validRowB.Row.Contains(x));
											board[1, 3] = colD.Row.Single(x => validRowB.Row.Contains(x));
											// apply row C
											board[2, 0] = colA.Row.Single(x => validRowC.Row.Contains(x));
											board[2, 1] = colB.Row.Single(x => validRowC.Row.Contains(x));
											board[2, 2] = colC.Row.Single(x => validRowC.Row.Contains(x));
											board[2, 3] = colD.Row.Single(x => validRowC.Row.Contains(x));
											// apply row D
											board[3, 0] = colA.Row.Single(x => validRowD.Row.Contains(x));
											board[3, 1] = colB.Row.Single(x => validRowD.Row.Contains(x));
											board[3, 2] = colC.Row.Single(x => validRowD.Row.Contains(x));
											board[3, 3] = colD.Row.Single(x => validRowD.Row.Contains(x));
											//PrintBoard(board);
											var possibleDiagAs = rows.Except(new List<RowNode>
											{ rowA, validRowB, validRowC, validRowD, colA, colB, colC, colD });
											foreach (var possibleDiagA in possibleDiagAs)
											{
												CheckDiag(
													rows,
													rowA,
													validRowB,
													validRowC,
													validRowD,
													colA,
													colB,
													colC,
													colD,
													possibleDiagA,
													(StatefulTile[,]) board.Clone());
											}
										}
									}
								}
							}
						}
					}
				}
			}
			Console.WriteLine($"Found {validSolutions} valid grids.");
		}

		private static void CheckDiag(
			List<RowNode> rows,
			RowNode rowA,
			RowNode validRowB,
			RowNode validRowC,
			RowNode validRowD,
			RowNode colA,
			RowNode colB,
			RowNode colC,
			RowNode colD,
			RowNode possibleDiagA,
			StatefulTile[,] board)
		{
			var counter = 0;
			//PrintBoard(diagABoard);
			foreach (var tileDiagA in possibleDiagA.Row)
			{
				//PrintTile(tileDiagA);
				int i;
				int j;
				(i, j) = GetTileLocation(board, tileDiagA);
				if (i == -1 || i < counter || j < counter)
				{
					return;
				}
				SwapRow(ref board, counter, i);
				SwapCol(ref board, counter, j);
				counter++;
			}
			//PrintBoard(diagABoard);
			// check all possible diagBs
			// can use heaps algorithm here
			for (var i = 0; i < 4; i++)
			{
				CheckAndSwapRowAndCol(ref board, 0, 1);
				CheckAndSwapRowAndCol(ref board, 0, 2);
				CheckAndSwapRowAndCol(ref board, 0, 1);
				CheckAndSwapRowAndCol(ref board, 0, 2);
				CheckAndSwapRowAndCol(ref board, 0, 1);
				CheckAndSwapRowAndCol(ref board, i, 3);
			}
		}

		private static void SwapCol(ref StatefulTile[,] board, int a, int b)
		{
			var temp = new StatefulTile[4];
			for (var k = 0; k < 4; k++)
			{
				temp[k] = board[k, b];
				board[k, b] = board[k, a];
				board[k, a] = temp[k];
			}
		}

		private static void SwapRow(ref StatefulTile[,] board, int a, int b)
		{
			var temp = new StatefulTile[4];
			for (var k = 0; k < 4; k++)
			{
				temp[k] = board[b, k];
				board[b, k] = board[a, k];
				board[a, k] = temp[k];
			}
		}

		private static void CheckAndSwapRowAndCol(ref StatefulTile[,] board, int a, int b)
		{
			var diagB = new List<StatefulTile>();
			for (var i = 0; i < 4; i++)
			{
				diagB.Add(board[i, 3 - i]);
			}
			if (ValidRow(diagB))
			{
				// every row, col and diag is valid
				// inner and outer lines can be swaped
				RemainingChecks(ref board);
				SwapRowAndCol(ref board, 0, 3);
				RemainingChecks(ref board);
				SwapRowAndCol(ref board, 1, 2);
				RemainingChecks(ref board);
				SwapRowAndCol(ref board, 0, 3);
				RemainingChecks(ref board);
				//PrintBoard(board);
				//validSolutions++;
			}
			SwapRowAndCol(ref board, a, b);
		}

		private static void RemainingChecks(ref StatefulTile[,] board)
		{
			// check corners
			if (ValidRow(new List<StatefulTile> { board[0,0], board[0,3], board[3,0], board[3,3] }))
			{
				// check squares 
			}
		}

		private static void  SwapRowAndCol(ref StatefulTile[,] board, int a, int b)
		{
			SwapRow(ref board, a, b);
			SwapCol(ref board, a, b);
		}

		private static void PrintTile(StatefulTile tileDiagA)
		{
			PrintFace(tileDiagA, 0);
			Console.Write("| ");
			PrintFace(tileDiagA, 1);
			Console.WriteLine();
		}

		private static (int, int) GetTileLocation(StatefulTile[,] board, StatefulTile tile)
		{
			for (var i = 0; i < 4; i++)
			{
				for (var j = 0; j < 4; j++)
				{
					if (board[i, j] == tile)
					{
						return (i, j);
					}
				}
			}
			return (-1, -1);
		}

		private static void PrintBoard(StatefulTile[,] board)
		{
			for (var i = 0; i < 4; i++)
			{
				for (var j = 0; j < 4; j++)
				{
					PrintFace(board[i, j], 0);
				}
				Console.Write("| ");
				for (var j = 0; j < 4; j++)
				{
					PrintFace(board[i, j], 1);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static RowNode CopyRow(RowNode columnA) =>
			new RowNode(columnA.Row.ToList());
		private static bool IsSameTile(StatefulTile a, StatefulTile b) =>
			a == b || a == b.StatefulTileTwin;

		private static void PrintRowCount(IList<StatefulTile> allStatefullTiles)
		{
			foreach (var tile in allStatefullTiles)
			{
				PrintFace(tile, 0);
				Console.Write("| ");
				PrintFace(tile, 1);
				Console.WriteLine($": {tile.RowNodes.Count} Rows");
			}
		}

		private static IList<StatefulTile> GetAllStatefulTiles(IList<Tile> allTiles)
		{
			var allStatefulTiles = new List<StatefulTile>();
			foreach (var tile in allTiles)
			{
				var a = new StatefulTile(tile, new List<int> { 0, 1 });
				var b = new StatefulTile(tile, new List<int> { 1, 0 });
				a.StatefulTileTwin = b;
				b.StatefulTileTwin = a;
				allStatefulTiles.Add(a);
				allStatefulTiles.Add(b);
			}
			return allStatefulTiles;
		}

		private static void PrintRows(IList<RowNode> fourGroupCombination)
		{
			foreach (var fourGroup in fourGroupCombination)
			{
				PrintRow(fourGroup, 0);
				Console.Write("| ");
				PrintRow(fourGroup, 1);
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static bool CheckUniqueTiles(IList<IList<IList<int>>> fourGroupCombination)
		{
			foreach (var fourGroup in fourGroupCombination)
			{
				foreach (var tile in fourGroup)
				{
					var tempFourGroupCombination = fourGroupCombination.ToList();
					tempFourGroupCombination.Remove(fourGroup);
					foreach (var tempFourGroup in tempFourGroupCombination)
					{
						foreach (var tempTile in tempFourGroup)
						{
							if (tile[0] == tempTile[0] && tile[1] == tempTile[1]
								|| tile[0] == tempTile[1] && tile[1] == tempTile[0])
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private static bool ValidRow(IList<StatefulTile> tileSet) =>
			tileSet.Any(x => tileSet.Any(y => x.StatefulTileTwin == y))
			? false
			: tileSet.Select(x => x.GetFaceValue(0)).Sum() == 50
				&& tileSet.Select(x => x.GetFaceValue(1)).Sum() == 50;

		private static void PrintGroup(RowNode rowNode)
		{
			PrintRow(rowNode, 0);
			Console.WriteLine();
			PrintRow(rowNode, 1);
			Console.WriteLine();
			Console.WriteLine();
		}

		private static void PrintRow(RowNode rowNode, int i)
		{
			foreach (var tile in rowNode.Row)
			{
				PrintFace(tile, i);
			}
		}

		private static void PrintFace(StatefulTile tile, int i) =>
			Console.Write(tile.GetFaceValue(i).ToString().PadRight(3));

		static IEnumerable<IList<T>> GetCombinations<T>(IList<T> list, int k) =>
			k == 1
			? list.Select(x => new List<T> { x })
			: list.SelectMany((e, i) =>
				GetCombinations(list.Skip(i + 1).ToList(), k - 1).Select(c => (new[] { e }).Concat(c).ToList()));

		private static int GetState(int i, int j) =>
			(i & (1 << j)) >> j;

		private static void PrintBruteBoard(IList<Tile> list)
		{
			for (var i = 0; i < list.Count() / 4; i++)
			{
				foreach (var value in list.Skip(i * 4).Take(4).Select(x => x.Faces.First()))
				{
					Console.Write(value.ToString().PadRight(3));
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static bool BruteCheck(IList<Tile> list)
		{
			for (var i = 0; i < list.Count() / 4; i++)
			{
				// rows
				if (list.Skip(i * 4).Take(4).Select(x => x.Faces.First()).Sum() != 50)
				{
					return false;
				}
				// columns
				var columnSum = 0;
				for (var j = i; j < list.Count() / 4; j++)
				{
					columnSum += list[j].Faces.First();
				}
				if (columnSum != 50)
				{
					return false;
				}
			}
			// diag
			var leftDiagSum = 0;
			for (var i = 0; i < list.Count(); i += 5)
			{
				leftDiagSum += list[i].Faces.First();
			}
			if (leftDiagSum != 50)
			{
				return false;
			}
			var rightDiagSum = 0;
			for (var i = 3; i < list.Count(); i += 3)
			{
				rightDiagSum += list[i].Faces.First();
			}
			return rightDiagSum == 50;
		}

		private static IEnumerable<IList<Tile>> GetPermutations(IList<Tile> list, int pointer, int max)
		{
			if (pointer == max)
			{
				yield return list;
			}
			else
			{
				for (var i = pointer; i <= max; i++)
				{
					Swap(list[pointer], list[i]);
					foreach (var result in GetPermutations(list, pointer + 1, max))
					{
						yield return result;
					}
					Swap(list[pointer], list[i]);
				}
			}
		}

		private static void Swap(Tile a, Tile b)
		{
			var temp = a;
			a = b;
			b = temp;
		}

		private static long GetFactoral(int n) =>
			Enumerable.Range(1, n).Aggregate(1L, (x, y) => x * y);
	}
}
