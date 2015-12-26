using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VendingMachineSimulator.Simulator {
	/// <summary>
	/// Vending processor maintains products
	/// </summary>
	public class VendingProcessor {
		/// <summary>
		/// Product slots
		/// </summary>
		public Product[] ProductSlots { get; set; }

		internal VendingProcessor() {
			ProductSlots = new Product[] {
		                                 	new Product() {Name = "Coke", Price = 0.50, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 	new Product() {Name = "Pepsi", Price = 0.50, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 	new Product() {Name = "Tango", Price = 0.65, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 	new Product() {Name = "7up", Price = 0.75, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 	new Product() {Name = "Lilt", Price = 0.60, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 	new Product() {Name = "Ribena", Price = 0.85, OutOfStock = false, Count = 5, TotalSold = 0},
		                                 };			
		}
	}
}
