using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VendingMachineSimulator.Simulator;
using Path = System.IO.Path;

namespace VendingMachineSimulator {
	/// <summary>
	/// Maintenance window
	/// </summary>
	public partial class MaintenanceWindow : Window {
		public MaintenanceWindow() {
			InitializeComponent();

			FillCombos(); // Fill product names popups
		}

		/// <summary>
		/// Fill product names popups using filenames from "Images" folder
		/// </summary>
		private void FillCombos() {
			var files =
				Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images"));

			foreach (var file in files) {
				string name = Path.GetFileNameWithoutExtension(file);

				cmbDrink1.Items.Add(name);
				cmbDrink2.Items.Add(name);
				cmbDrink3.Items.Add(name);
				cmbDrink4.Items.Add(name);
				cmbDrink5.Items.Add(name);
				cmbDrink6.Items.Add(name);
			}
		}

		/// <summary>
		/// Checks integer value (e.g., coin count) to be valid
		/// </summary>
		private bool CheckInt(string name, string valStr, int max, out int val) {
			val = -1;
			if(!Int32.TryParse(valStr, out val)) {
				MessageBox.Show("Invalid value for " + name);
				return false;
			}
			if(val>max) {
				MessageBox.Show("Value for " + name + " is bigger than " + max);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Checks price value to be valid
		/// </summary>
		private bool CheckPrice(string name, string valStr, out double val) {
			val = 0;
			if (!Double.TryParse(valStr, out val)) {
				MessageBox.Show("Invalid value for " + name);
				return false;
			}
			int intVal = (int) Math.Round(val*100);
			if((intVal%5)>0) {
				MessageBox.Show("Invalid price for " + name);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Finds popup index for given string
		/// </summary>
		private int FindComboValue(ComboBox cmb, string value) {
			int ind = 0;
			foreach (var item in cmb.Items) {
				if(item.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
					return ind;
				}
				ind++;
			}
			return -1;
		}

		/// <summary>
		/// Window is loaded.
		/// Initializes control values, moves vending machine to maintenance mode
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			ControlUnit.Instance.ToMaintenance();

			txtCash5.Text = ControlUnit.Instance.CashBox.Slots[0].ToString();
			txtCash10.Text = ControlUnit.Instance.CashBox.Slots[1].ToString();
			txtCash20.Text = ControlUnit.Instance.CashBox.Slots[2].ToString();
			txtCash50.Text = ControlUnit.Instance.CashBox.Slots[3].ToString();
			txtCash100.Text = ControlUnit.Instance.CashBox.Slots[4].ToString();
			txtCash200.Text = ControlUnit.Instance.CashBox.Slots[5].ToString();

			txtCashNotes.Text = ControlUnit.Instance.CashBox.NotesBox.ToString();

			chkDrink1.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[0].OutOfStock;
			chkDrink2.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[1].OutOfStock;
			chkDrink3.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[2].OutOfStock;
			chkDrink4.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[3].OutOfStock;
			chkDrink5.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[4].OutOfStock;
			chkDrink6.IsChecked = !ControlUnit.Instance.VendingProcessor.ProductSlots[5].OutOfStock;

			cmbDrink1.SelectedIndex = FindComboValue(cmbDrink1, ControlUnit.Instance.VendingProcessor.ProductSlots[0].Name);
			cmbDrink2.SelectedIndex = FindComboValue(cmbDrink2, ControlUnit.Instance.VendingProcessor.ProductSlots[1].Name);
			cmbDrink3.SelectedIndex = FindComboValue(cmbDrink3, ControlUnit.Instance.VendingProcessor.ProductSlots[2].Name);
			cmbDrink4.SelectedIndex = FindComboValue(cmbDrink4, ControlUnit.Instance.VendingProcessor.ProductSlots[3].Name);
			cmbDrink5.SelectedIndex = FindComboValue(cmbDrink5, ControlUnit.Instance.VendingProcessor.ProductSlots[4].Name);
			cmbDrink6.SelectedIndex = FindComboValue(cmbDrink6, ControlUnit.Instance.VendingProcessor.ProductSlots[5].Name);

			txtDrinkCount1.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[0].Count.ToString();
			txtDrinkCount2.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[1].Count.ToString();
			txtDrinkCount3.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[2].Count.ToString();
			txtDrinkCount4.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[3].Count.ToString();
			txtDrinkCount5.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[4].Count.ToString();
			txtDrinkCount6.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[5].Count.ToString();

			txtDrink1.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[0].Price.ToString("#0.00");
			txtDrink2.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[1].Price.ToString("#0.00");
			txtDrink3.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[2].Price.ToString("#0.00");
			txtDrink4.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[3].Price.ToString("#0.00");
			txtDrink5.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[4].Price.ToString("#0.00");
			txtDrink6.Text = ControlUnit.Instance.VendingProcessor.ProductSlots[5].Price.ToString("#0.00");
		}

		/// <summary>
		/// When maintenance window is closed, move vending machine back to operational mode
		/// </summary>
		private void Window_Closed(object sender, EventArgs e) {
			ControlUnit.Instance.ToOperational();
		}

		private void btnDec5_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash5.Text) == 0) {
				return;
			}
			txtCash5.Text = (Int32.Parse(txtCash5.Text) - 1).ToString();
		}

		private void btnDec10_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash10.Text) == 0) {
				return;
			}
			txtCash10.Text = (Int32.Parse(txtCash10.Text) - 1).ToString();
		}

		private void btnDec20_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash20.Text) == 0) {
				return;
			}
			txtCash20.Text = (Int32.Parse(txtCash20.Text) - 1).ToString();
		}

		private void btnDec50_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash50.Text) == 0) {
				return;
			}
			txtCash50.Text = (Int32.Parse(txtCash50.Text) - 1).ToString();
		}

		private void btnDec100_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash100.Text) == 0) {
				return;
			}
			txtCash100.Text = (Int32.Parse(txtCash100.Text) - 1).ToString();
		}

		private void btnDec200_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash200.Text)==0) {
				return;
			}
			txtCash200.Text = (Int32.Parse(txtCash200.Text) - 1).ToString();
		}

		private void btnAdd5_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash5.Text) >= 20) {
				return;
			}
			txtCash5.Text = (Int32.Parse(txtCash5.Text) + 1).ToString();
		}

		private void btnAdd10_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash10.Text) >= 20) {
				return;
			}
			txtCash10.Text = (Int32.Parse(txtCash10.Text) + 1).ToString();
		}

		private void btnAdd20_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash20.Text) >= 20) {
				return;
			}
			txtCash20.Text = (Int32.Parse(txtCash20.Text) + 1).ToString();
		}

		private void btnAdd50_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash50.Text) >= 20) {
				return;
			}
			txtCash50.Text = (Int32.Parse(txtCash50.Text) + 1).ToString();
		}

		private void btnAdd100_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash100.Text) >= 20) {
				return;
			}
			txtCash100.Text = (Int32.Parse(txtCash100.Text) + 1).ToString();
		}

		private void btnAdd200_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtCash200.Text) >= 20) {
				return;
			}
			txtCash200.Text = (Int32.Parse(txtCash200.Text) + 1).ToString();
		}

		private void btnDrinkDec1_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount1.Text) == 0) {
				return;
			}
			txtDrinkCount1.Text = (Int32.Parse(txtDrinkCount1.Text) - 1).ToString();
		}

		private void btnDrinkDec2_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount2.Text) == 0) {
				return;
			}
			txtDrinkCount2.Text = (Int32.Parse(txtDrinkCount2.Text) - 1).ToString();
		}

		private void btnDrinkDec3_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount3.Text) == 0) {
				return;
			}
			txtDrinkCount3.Text = (Int32.Parse(txtDrinkCount3.Text) - 1).ToString();
		}

		private void btnDrinkDec4_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount4.Text) == 0) {
				return;
			}
			txtDrinkCount4.Text = (Int32.Parse(txtDrinkCount4.Text) - 1).ToString();
		}

		private void btnDrinkDec5_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount5.Text) == 0) {
				return;
			}
			txtDrinkCount5.Text = (Int32.Parse(txtDrinkCount5.Text) - 1).ToString();
		}

		private void btnDrinkDec6_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount6.Text) == 0) {
				return;
			}
			txtDrinkCount6.Text = (Int32.Parse(txtDrinkCount6.Text) - 1).ToString();
		}

		private void btnDrinkAdd1_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount1.Text) >= 5) {
				return;
			}
			txtDrinkCount1.Text = (Int32.Parse(txtDrinkCount1.Text) + 1).ToString();
		}

		private void btnDrinkAdd2_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount2.Text) >= 5) {
				return;
			}
			txtDrinkCount2.Text = (Int32.Parse(txtDrinkCount2.Text) + 1).ToString();
		}

		private void btnDrinkAdd3_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount3.Text) >= 5) {
				return;
			}
			txtDrinkCount3.Text = (Int32.Parse(txtDrinkCount3.Text) + 1).ToString();
		}

		private void btnDrinkAdd4_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount4.Text) >= 5) {
				return;
			}
			txtDrinkCount4.Text = (Int32.Parse(txtDrinkCount4.Text) + 1).ToString();
		}

		private void btnDrinkAdd5_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount5.Text) >= 5) {
				return;
			}
			txtDrinkCount5.Text = (Int32.Parse(txtDrinkCount5.Text) + 1).ToString();
		}

		private void btnDrinkAdd6_Click(object sender, RoutedEventArgs e) {
			if (Int32.Parse(txtDrinkCount6.Text) >= 5) {
				return;
			}
			txtDrinkCount6.Text = (Int32.Parse(txtDrinkCount6.Text) + 1).ToString();
		}

		/// <summary>
		/// "Apply" button pressed.
		/// Verifies control values, applies products and cashbox state
		/// </summary>
		private void Button_Click(object sender, RoutedEventArgs e) {
			if(!CheckInt("5p count", txtCash5.Text, 20, out ControlUnit.Instance.CashBox.Slots[0])) {
				return;
			}
			if(!CheckInt("10p count", txtCash10.Text, 20, out ControlUnit.Instance.CashBox.Slots[1])) {
				return;
			}
			if(!CheckInt("20p count", txtCash20.Text, 20, out ControlUnit.Instance.CashBox.Slots[2])) {
				return;
			}
			if(!CheckInt("50p count", txtCash50.Text, 20, out ControlUnit.Instance.CashBox.Slots[3])) {
				return;
			}
			if (!CheckInt("£1 count", txtCash100.Text, 20, out ControlUnit.Instance.CashBox.Slots[4])) {
				return;
			}
			if (!CheckInt("£2 count", txtCash200.Text, 20, out ControlUnit.Instance.CashBox.Slots[5])) {
				return;
			}

			for (int i = 0; i < 6; i++) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[i].TotalSold = 0;
			}

			ControlUnit.Instance.VendingProcessor.ProductSlots[0].OutOfStock = !chkDrink1.IsChecked.GetValueOrDefault(false);
			ControlUnit.Instance.VendingProcessor.ProductSlots[1].OutOfStock = !chkDrink2.IsChecked.GetValueOrDefault(false);
			ControlUnit.Instance.VendingProcessor.ProductSlots[2].OutOfStock = !chkDrink3.IsChecked.GetValueOrDefault(false);
			ControlUnit.Instance.VendingProcessor.ProductSlots[3].OutOfStock = !chkDrink4.IsChecked.GetValueOrDefault(false);
			ControlUnit.Instance.VendingProcessor.ProductSlots[4].OutOfStock = !chkDrink5.IsChecked.GetValueOrDefault(false);
			ControlUnit.Instance.VendingProcessor.ProductSlots[5].OutOfStock = !chkDrink6.IsChecked.GetValueOrDefault(false);

			ControlUnit.Instance.VendingProcessor.ProductSlots[0].Name = cmbDrink1.SelectedItem.ToString();
			ControlUnit.Instance.VendingProcessor.ProductSlots[1].Name = cmbDrink2.SelectedItem.ToString();
			ControlUnit.Instance.VendingProcessor.ProductSlots[2].Name = cmbDrink3.SelectedItem.ToString();
			ControlUnit.Instance.VendingProcessor.ProductSlots[3].Name = cmbDrink4.SelectedItem.ToString();
			ControlUnit.Instance.VendingProcessor.ProductSlots[4].Name = cmbDrink5.SelectedItem.ToString();
			ControlUnit.Instance.VendingProcessor.ProductSlots[5].Name = cmbDrink6.SelectedItem.ToString();

			int val;
			if (CheckInt("Drink#1 count", txtDrinkCount1.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[0].Count = val;
			} else {
				return;
			}
			if (CheckInt("Drink#2 count", txtDrinkCount2.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[1].Count = val;
			} else {
				return;
			}
			if (CheckInt("Drink#3 count", txtDrinkCount3.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[2].Count = val;
			} else {
				return;
			}
			if (CheckInt("Drink#4 count", txtDrinkCount4.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[3].Count = val;
			} else {
				return;
			}
			if (CheckInt("Drink#5 count", txtDrinkCount5.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[4].Count = val;
			} else {
				return;
			}
			if (CheckInt("Drink#6 count", txtDrinkCount6.Text, 20, out val)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[5].Count = val;
			} else {
				return;
			}

			double dblVal;
			if(CheckPrice("Drink#1", txtDrink1.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[0].Price = dblVal;
			} else {
				return;
			}
			if(CheckPrice("Drink#2", txtDrink2.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[1].Price = dblVal;
			} else {
				return;
			}
			if(CheckPrice("Drink#3", txtDrink3.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[2].Price = dblVal;
			} else {
				return;
			}
			if(CheckPrice("Drink#4", txtDrink4.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[3].Price = dblVal;
			} else {
				return;
			}
			if(CheckPrice("Drink#5", txtDrink5.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[4].Price = dblVal;
			} else {
				return;
			}
			if(CheckPrice("Drink#6", txtDrink6.Text, out dblVal)) {
				ControlUnit.Instance.VendingProcessor.ProductSlots[5].Price = dblVal;
			} else {
				return;
			}

			if("0".Equals(txtCashNotes.Text)) {
				ControlUnit.Instance.CashBox.NotesBox = 0;
			}

			Close();
		}

		private void btnReset_Click(object sender, RoutedEventArgs e) {
			txtCashNotes.Text = "0";
		}
	}
}
