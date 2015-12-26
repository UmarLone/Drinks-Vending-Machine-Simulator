using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace VendingMachineSimulator.Simulator {

	/// <summary>
	/// Vending machine state
	/// </summary>
	public enum ControlUnitState {
		Maintenance,		// machine is in maintenance mode
		Ready,				// drink selection mode
		Transaction,		// payment mode
		Release,			// product release mode
		Finalize			// Thank you!
	}

	public delegate void RestartDelegate();
	public delegate void CashReturnDelegate(int amount);
	public delegate void DisplayChangedDelegate(string text);
	public delegate void ProductReleaseDelegate(Product product);
	public delegate void OutOfStockDelegate(int slot);

	/// <summary>
	/// Machine control unit.
	/// Contains all logic
	/// </summary>
	public class ControlUnit {
		#region Singletone
		/// <summary>
		/// Singleton instance
		/// </summary>
		private static ControlUnit _instance;
		public static ControlUnit Instance {
			get {
				if(_instance==null) {
					_instance = new ControlUnit();
				}
				return _instance;
			}
		}
		#endregion

		#region private fields
		/// <summary>
		/// Vending machine status
		/// </summary>
		private ControlUnitState _state;
		/// <summary>
		/// Cashbox object
		/// </summary>
		private CashBox _cashBox;
		/// <summary>
		/// Vending processor object
		/// </summary>
		private VendingProcessor _vendingProcessor;
		/// <summary>
		/// No change flag
		/// </summary>
		private bool NoChange;
		#endregion

		#region Constructor
		private ControlUnit() {
			_cashBox = new CashBox();
			_vendingProcessor = new VendingProcessor();

			_state = ControlUnitState.Ready;
		}

		#endregion

		#region events
		/// <summary>
		/// Event is called when one coin or note is returned
		/// </summary>
		public event CashReturnDelegate OnCashReturn;
		private void RaiseCashReturn(int amount) {
			if(OnCashReturn!=null) {
				OnCashReturn(amount);
			}
		}

		/// <summary>
		/// Event is called when vending machine display must be updated
		/// </summary>
		public event DisplayChangedDelegate OnDisplayChanged;
		private void RaiseDisplayChanged() {
			var sb = new StringBuilder();
			if(_state==ControlUnitState.Maintenance) {
				sb.AppendLine("OUT OF ORDER");
				sb.AppendLine("Sales report: ");

				double total = 0; // calculate sales report
				for (int i = 0; i < ControlUnit.Instance.VendingProcessor.ProductSlots.Length; i++) {
					var p = ControlUnit.Instance.VendingProcessor.ProductSlots[i];
					total += p.Price * p.TotalSold;
					sb.AppendLine(p.Name + ", sold " + p.TotalSold + "= £" + (p.Price * p.TotalSold).ToString("#0.00"));
				}

				sb.AppendLine("Total = £" + total.ToString("#0.00"));
			} else if(_state==ControlUnitState.Ready) {
				if (String.IsNullOrEmpty(Error)) {
					sb.AppendLine("CHOOSE DRINK");
				} else {
					sb.AppendLine(Error);
				}
			} else if(_state==ControlUnitState.Release || _state==ControlUnitState.Finalize) {
				sb.AppendLine("THANK YOU. . .");
			} else if(_state==ControlUnitState.Transaction) {
				if (NoChange) {
					sb.AppendLine("NO CHANGE");
				} else {
					if (String.IsNullOrEmpty(Error)) {
						sb.AppendLine("INSERT COINS PLEASE");
					} else {
						sb.AppendLine(Error);
					}
				}
			} else {
				sb.AppendLine("ERROR!");
			}

			if(_state == ControlUnitState.Transaction) { // Show product name, cost, and current transaction amount
				sb.AppendLine(ChosenProduct.Name);
				sb.AppendLine("Cost: £" + ChosenProduct.Price.ToString("#0.00"));
				sb.AppendLine("Cash Inserted : £" + TransactionAmount.ToString("#0.00"));
			}

			if(OnDisplayChanged!=null) {
				OnDisplayChanged(sb.ToString());
			}
		}

		/// <summary>
		/// Event is called when product is released
		/// </summary>
		public event ProductReleaseDelegate OnProductRelease;
		private void RaiseProductRelease(Product product) {
			if(OnProductRelease!=null) {
				OnProductRelease(product);
			}
		}

		/// <summary>
		/// Starts product release procedure to feel realistic process (takes 3 seconds)
		/// </summary>
		private void StartProductRelease() {
			_state = ControlUnitState.Release;
			var timer = new Timer(3000);
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Start();
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e) {
			((Timer)sender).Enabled = false;

			_state = ControlUnitState.Finalize;

			Dispatcher.Invoke(new Action(delegate() {
				RaiseProductRelease(ChosenProduct);
				RaiseDisplayChanged();
			}));
		}

		/// <summary>
		/// Event is called when one of products is finished
		/// </summary>
		public event OutOfStockDelegate OnOutOfStock;
		private void RaiseOutOfStock(int slot) {
			if(OnOutOfStock!=null) {
				OnOutOfStock(slot);

			}
		}

		/// <summary>
		/// Event is called when vending machine goes to operational mode from maintenance
		/// </summary>
		public event RestartDelegate OnRestart;
		private void RaiseOnRestart() {
			if(OnRestart!=null) {
				OnRestart();
			}
		}
		#endregion

		#region Public properties
		/// <summary>
		/// Threading dispatcher to make calls to UI from within another thread
		/// </summary>
		public Dispatcher Dispatcher { get; set; }

		/// <summary>
		/// Vending processor
		/// </summary>
		public VendingProcessor VendingProcessor {
			get { return _vendingProcessor; }
		}

		/// <summary>
		/// Cashbox
		/// </summary>
		public CashBox CashBox {
			get { return _cashBox; }
		}

		public string Error { get; set; }

		/// <summary>
		/// Transaction amount in pence
		/// </summary>
		public int TransactionAmount100 { get; set; }
		public double TransactionAmount {
			get { return (double)TransactionAmount100 / 100; }
		}

		/// <summary>
		/// Chosen product slot
		/// </summary>
		public int ProductSlot { get; set; }

		/// <summary>
		/// Chosen product
		/// </summary>
		public Product ChosenProduct {
			get {
				if(_vendingProcessor.ProductSlots==null) {
					return null;
				}

				if(ProductSlot>=0 && ProductSlot<_vendingProcessor.ProductSlots.Length) {
					return _vendingProcessor.ProductSlots[ProductSlot];
				}
				return null;
			}
		}
		#endregion

		public void Init() {
			RaiseDisplayChanged();
		}

		/// <summary>
		/// User chooses drink
		/// </summary>
		/// <param name="slot"></param>
		public void ChooseDrink(int slot) {
			Error = null;
			if(_state==ControlUnitState.Ready || (_state==ControlUnitState.Transaction && TransactionAmount100==0)) { // User can choose drink in appropriate mode or if he didn't start paying
				if(slot>=_vendingProcessor.ProductSlots.Length) {
					return;
				}
				var p = _vendingProcessor.ProductSlots[slot];
				if(p.OutOfStock || p.Count == 0) {
					Error = "OUT OF STOCK";
					RaiseDisplayChanged();
					return; 
				}

				ProductSlot = slot;

				_state = ControlUnitState.Transaction; // Move to payment mode
				TransactionAmount100 = 0;
				NoChange = false;

				RaiseDisplayChanged();
			} else if (_state == ControlUnitState.Transaction && TransactionAmount100 > 0) {
				Error = "PRESS REFUND \n FOR ANOTHER DRINK";
				RaiseDisplayChanged();
			}
		}

		/// <summary>
		/// Moves machine to maintenance mode
		/// </summary>
		public void ToMaintenance() {
			Refund();

			_state = ControlUnitState.Maintenance;
			RaiseDisplayChanged();
		}

		/// <summary>
		/// Moves machine to operational mode
		/// </summary>
		public void ToOperational() {
			Error = null;
			_state = ControlUnitState.Ready;
			RaiseOnRestart();
			RaiseDisplayChanged();
		}

		/// <summary>
		/// User took the product
		/// </summary>
		public void SellFinished() {
			Error = null;
			_state = ControlUnitState.Ready;
			RaiseDisplayChanged();
		}

		/// <summary>
		/// User presses refund button
		/// </summary>
		public void Refund() {
			Error = null;
			if(_state==ControlUnitState.Transaction) { // if was in payment mode
				var coins = _cashBox.GiveChange(TransactionAmount); // calculate minimum number of coins to return as if it was a change for full transaction amount

				if (coins != null) {
					foreach (var coin in coins) {
						RaiseCashReturn(coin); // return each coin
					}
				} else {
					//error!
				}

				_state = ControlUnitState.Ready;
				RaiseDisplayChanged();
			}
		}

		/// <summary>
		/// User pays one coin or one note of specified value in pence
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="note"></param>
		public void Pay(int amount, bool note) {
			Error = null;
			if(_state==ControlUnitState.Transaction) { // Accept money only in payment mode
				if(note) { // If it is note
					var change = _cashBox.GiveChange(TransactionAmount + (double)amount/100 - ChosenProduct.Price); //Try to check if we have change for him, if not - don't accept note
					if(change==null) {
						NoChange = true;
						Error = "NO CASH FOR CHANGE";
						RaiseCashReturn(amount); // return note back
						RaiseDisplayChanged();
					} else if(_cashBox.AddNote()) { // if we have enough room in notes box
						TransactionAmount100 += amount; // increment transaction amount by note value
					} else {
						Error = "CASHBOX FULL";
						RaiseCashReturn(amount); // return note, there's no space in notes box
						RaiseDisplayChanged();
					}

					RaiseDisplayChanged(); // update display
				} else if (_cashBox.AddCoin(amount)) { // if it is coin and we have enough space in cashbox
					TransactionAmount100 += amount; // increment transaction amount

					if(TransactionAmount100>ChosenProduct.Price100) { // Check if we have change
						var change = _cashBox.GiveChange(TransactionAmount - ChosenProduct.Price);
						if(change==null) {
							NoChange = true;
						}
					}

					RaiseDisplayChanged();
				} else {
					Error = "CASHBOX FULL";
					RaiseCashReturn(amount);
					RaiseDisplayChanged();
				}
			} else {
				Error = "CHOOSE DRINK FIRST";
				RaiseCashReturn(amount);
				RaiseDisplayChanged();
			}
		}

		/// <summary>
		/// User presses "buy now"
		/// </summary>
		public void ConfirmSell() {
			Error = null;
			if(_state!=ControlUnitState.Transaction) { // if not in payment mode - do nothing
				if(_state==ControlUnitState.Ready) {
					Error = "CHOOSE DRINK FIRST!";
					RaiseDisplayChanged();
				}
				return;
			}
			if(TransactionAmount100<ChosenProduct.Price100) { // if there's not enough money - do nothing
				Error = "NOT ENOUGH MONEY!";
				RaiseDisplayChanged();
				return;
			}

			var change = _cashBox.GiveChange(TransactionAmount - ChosenProduct.Price); // get change
			if (change == null) { //there no change
				change = _cashBox.GiveChange(TransactionAmount); // return transaction amount back
				if(change==null) {
					Error = "NO CASH FOR CHANGE";
					RaiseDisplayChanged();
				} else {
					foreach (var coin in change) {
						RaiseCashReturn(coin);
					}
				}

				_state = ControlUnitState.Ready;
			} else {
				foreach (var coin in change) { // return change
					RaiseCashReturn(coin);
				}

				ChosenProduct.TotalSold++; // update product statistics
				ChosenProduct.Count--;
				if (ChosenProduct.Count == 0) {
					ChosenProduct.OutOfStock = true;
					RaiseOutOfStock(ProductSlot);
				}

				StartProductRelease(); // start releasing product
				_state = ControlUnitState.Release;
			}
			
			RaiseDisplayChanged();
		}
	}
}
