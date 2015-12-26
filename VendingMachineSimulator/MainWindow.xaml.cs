using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VendingMachineSimulator.Simulator;

namespace VendingMachineSimulator {
	/// <summary>
	/// Vending machine interface
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		/// <summary>
		/// Event handler for maintenance window
		/// </summary>
		private void btnMaintenance_Click(object sender, RoutedEventArgs e) {
            var win = new MaintenanceWindow(); //When opened, it moves vending machine to maintenance state
            win.ShowDialog();



          
           
		}

		/// <summary>
		/// Updates product images
		/// </summary>
		private void ResetProducts() {
			for (int i = 0; i < 6; i++) {
				Product p = null;
				if (i < ControlUnit.Instance.VendingProcessor.ProductSlots.Length) {
					p = ControlUnit.Instance.VendingProcessor.ProductSlots[i];
				}

				Image imgDrink = null;

				switch (i) {
					case 0:
						imgDrink = imgDrink1;
						break;
					case 1:
						imgDrink = imgDrink2;
						break;
					case 2:
						imgDrink = imgDrink3;
						break;
					case 3:
						imgDrink = imgDrink4;
						break;
					case 4:
						imgDrink = imgDrink5;
						break;
					case 5:
						imgDrink = imgDrink6;
						break;
					default:
						return;
				}

				if (p == null || p.Count==0) { // Out of stock, remove image
					imgDrink.Source = null;
				} else {
					imgDrink.Source = new BitmapImage(new Uri("file:///" + (new FileInfo("Images/" + p.Name + ".png")).FullName.Replace("\\", "/")));  // Product image filename is "Images\<product_name>.png"
				}

			}
		}

		/// <summary>
		/// Initialize interface
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e) {

            
			ControlUnit.Instance.Dispatcher = Dispatcher;
			// Add event handlers
			ControlUnit.Instance.OnCashReturn += new CashReturnDelegate(Instance_CashReturn);
			ControlUnit.Instance.OnDisplayChanged += new DisplayChangedDelegate(Instance_DisplayChanged);
			ControlUnit.Instance.OnProductRelease += new ProductReleaseDelegate(Instance_ProductRelease);
			ControlUnit.Instance.OnOutOfStock += new OutOfStockDelegate(Instance_OutOfStock);
			ControlUnit.Instance.OnRestart += new RestartDelegate(Instance_OnRestart);

			// Initialize vending machine, get display text
			ControlUnit.Instance.Init();

			// Initialize product images
			ResetProducts();
		}

		/// <summary>
		/// Machine was restarted after maintenance
		/// </summary>
		private void Instance_OnRestart() {
			ResetProducts(); // Update products, it may have changed
		}

		/// <summary>
		/// Event when product at specified slot runs out
		/// </summary>
		/// <param name="slot"></param>
		private void Instance_OutOfStock(int slot) {
			Image imgDrink = null;
            
			switch (slot) {
				case 0:
					imgDrink = imgDrink1;
					break;
				case 1:
					imgDrink = imgDrink2;
					break;
				case 2:
					imgDrink = imgDrink3;
					break;
				case 3:
					imgDrink = imgDrink4;
					break;
				case 4:
					imgDrink = imgDrink5;
					break;
				case 5:
					imgDrink = imgDrink6;
					break;
				default:
                    
					return;
			}

			imgDrink.Source = null;
		}

		/// <summary>
		/// Event when product is released
		/// </summary>
		/// <param name="product"></param>
		private void Instance_ProductRelease(Product product) {
			new SoundPlayer(Properties.Resources.release).Play(); // play release sound
			imgRelease.Source = new BitmapImage(new Uri("file:///" + (new FileInfo("Images/" + product.Name + ".png")).FullName.Replace("\\", "/"))); //show product image in "pull" slot
		}

		/// <summary>
		/// Display has changed, update text
		/// </summary>
		/// <param name="text"></param>
		private void Instance_DisplayChanged(string text) {
			txtDisplay.Text = text;
		}

		/// <summary>
		/// One coin or banknote is returned
		/// </summary>
		/// <param name="amount"></param>
		private void Instance_CashReturn(int amount) {
			string amountStr = null;

			if(amount==5) {
				amountStr = "5pence";
			} else if(amount==10) {
				amountStr = "10pence";
			} else if(amount==20) {
				amountStr = "20pence";
			} else if(amount==50) {
				amountStr = "50pence";
			} else if(amount==100) {
				amountStr = "1pound";
			} else if(amount==200) {
				amountStr = "2pounds";
			} else if(amount==500) {
				amountStr = "5pounds";
			} else if(amount==1000) {
				amountStr = "10pounds";
			}

			new SoundPlayer(Properties.Resources.drop_coin).Play(); // Play return sound
			lbRefund.Items.Add("/VendingMachineSimulator;component/Images/" + amountStr + ".png"); // Show coin/banknote in change slot
		}

		/// <summary>
		/// User chooses drink
		/// </summary>
		private void btnGet1_Click(object sender, RoutedEventArgs e) {
			var tag = Int32.Parse(((Control) sender).Tag.ToString()); // Slot number is stored in "Tag" attribute of a button

			lbRefund.Items.Clear(); // Clear refund/change slot

			ControlUnit.Instance.ChooseDrink(tag - 1); // Choose drink (zero-based index)
		}

		/// <summary>
		/// User presses refund
		/// </summary>
		private void btnRefund_Click(object sender, RoutedEventArgs e) {
			ControlUnit.Instance.Refund(); // call refund function
		}

		/// <summary>
		/// User pays 5 pence coin
		/// </summary>
		private void btnPay5_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(5, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User pays 10 pence coin
		/// </summary>
		private void btnPay10_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(10, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User pays 20 pence coin
		/// </summary>
		private void btnPay20_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(20, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User pays 50 pence coin
		/// </summary>
		private void btnPay50_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(50, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User pays 1 pound note
		/// </summary>
		private void btnPay100_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(100, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User pays 2 pounds note
		/// </summary>
		private void btnPay200_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot

			new SoundPlayer(Properties.Resources.insert_coin).Play(); // play coin sound
			ControlUnit.Instance.Pay(200, false); // try to put coin to cashbox
		}

		/// <summary>
		/// User presses "Pull" area
		/// </summary>
		private void imgRelease_MouseDown(object sender, MouseButtonEventArgs e) {
			ControlUnit.Instance.SellFinished(); // Hide "Thank you!", move to Ready state
			lbRefund.Items.Clear(); // Clear refund/change slot
			imgRelease.Source = null; // Clear released product image
		}

		/// <summary>
		/// User pays 5 pounds note
		/// </summary>
		private void btnPay500_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot
			ControlUnit.Instance.Pay(500, true); //Try to put note to cashbox
		}

		/// <summary>
		/// User pays 10 pounds note
		/// </summary>
		private void btnPay1000_Click(object sender, RoutedEventArgs e) {
			lbRefund.Items.Clear(); // Clear refund/change slot
			ControlUnit.Instance.Pay(1000, true); //Try to put note to cashbox
		}

		/// <summary>
		/// User presses "Buy now" button
		/// </summary>
		private void btnBuy_Click(object sender, RoutedEventArgs e) {
			ControlUnit.Instance.ConfirmSell(); // Confirm sell, try to give change and release product
		}

        private void imgRelease_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void progressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void imgDrink2_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void imgDrink6_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

	}
}
