using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Performance
{
    public class ListNewOrClear
    {
		List<int> lst = new List<int>();
		int[] arr = null;
		const int size = 10;
		int[] copy = new int[size];
		int copyIndex = 0;

		[Benchmark]
		public void NewList()
		{
			lst = new List<int>();
			AddToList();
		}

		[Benchmark]
		public void ClearList()
		{
			lst.Clear();
			AddToList();
			arr = lst.ToArray();
		}
		
		[Benchmark]
		public void CopyArr()
		{
			AddToCopy();
			arr = new int[copyIndex];
			Array.Copy(copy, arr, copyIndex);
			copyIndex = 0;
		}

		private void AddToCopy()
		{
			for (int i = 0; i < size; i++)
			{
				copy[i] = size;
			}
		}

		private void AddToList()
		{
			for (int i = 0; i < size; i++)
			{
				lst.Add(10);
			}
		}
	}
}
