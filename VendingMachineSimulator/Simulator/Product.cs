using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VendingMachineSimulator.Simulator {
	/// <summary>
	/// Contains product description, price and sell statistics
	/// </summary>
	public class Product {
		public int Count { get; set; }
		public string Name { get; set; }
		public double Price { get; set; }

		public int Price100 {
			get { return (int) Math.Round(Price*100); }
		}
		public bool OutOfStock { get; set; }

		public int TotalSold { get; set; }
	}
}
