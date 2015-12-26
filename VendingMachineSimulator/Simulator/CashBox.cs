using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VendingMachineSimulator.Simulator {

	/// <summary>
	/// Class for cash box. Stores coins and notes
	/// </summary>
	public class CashBox {
		/// <summary>
		/// Maximum amount of coins to fit into each coin slot
		/// </summary>
		private const int MAX_AMOUNT = 20;
		/// <summary>
		/// Maximum amount of notes in notes box (all values)
		/// </summary>
		private const int MAX_NOTES = 20;

		/// <summary>
		/// Slots with coins, each array element is amount of coins in appropriate slot
		/// </summary>
		private readonly int[] CoinSlots = {10, 10, 10, 10, 10, 10};
		/// <summary>
		/// Coin values for each slot
		/// </summary>
		private readonly int[] SlotValues = {5, 10, 20, 50, 100, 200};

		/// <summary>
		/// Amount of notes in notes box
		/// </summary>
		public int NotesBox { get; set; }

		/// <summary>
		/// Cashobox is full flag
		/// </summary>
		public bool Full { get; private set; }

		internal CashBox() {
			NotesBox = 0;

			CheckFull();
		}

		/// <summary>
		/// Check if cashbox is full and cannot accept anything else
		/// </summary>
		private void CheckFull() {
			for(int i=0;i<CoinSlots.Length;i++) {
				if(CoinSlots[i]<MAX_AMOUNT) {
					Full = false;
					return;
				}
			}
			Full = true;
		}

		/// <summary>
		/// Returns coin slots
		/// </summary>
		public int[] Slots {
			get { return CoinSlots; }
		}

		/// <summary>
		/// Calculates coin slot number by it's value
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		private int GetSlotIndex(int amount) {
			switch (amount) {
				case 5:
					return 0;
				case 10:
					return 1;
				case 20:
					return 2;
				case 50:
					return 3;
				case 100:
					return 4;
				case 200:
					return 5;
				default:
					return -1;
			}
		}

		/// <summary>
		/// Initialize slot for coin of given value with given count
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="count"></param>
		public void InitSlot(int amount, int count) {
			int index = GetSlotIndex(amount);
			CoinSlots[index] = count;

			CheckFull();
		}

		/// <summary>
		/// Check if cashbox can accept one more note, adds note to NoteBox 
		/// </summary>
		/// <returns></returns>
		public bool AddNote() {
			if(NotesBox>=MAX_NOTES) {
				return false;
			}
			NotesBox++;

			return true;
		}

		/// <summary>
		/// Checks if cashbox can accept coin of given value, adds to appropriate slot
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool AddCoin(int amount) {
			int index = GetSlotIndex(amount);
			if(index<0) {
				return false;
			}
			if(CoinSlots[index]>=MAX_AMOUNT) {
				return false;
			}

			CoinSlots[index]++;

			CheckFull();
			return true;
		}

		/// <summary>
		/// Calculates change for given double amount, returns array of coin values to return
		/// </summary>
		/// <param name="total"></param>
		/// <returns></returns>
		public int[] GiveChange(double total) {
			var result = new List<int>();

			int totalInt = (int) (Math.Round(total*100));
			int amount = 0;

			var slots = (int[])CoinSlots.Clone();

			int index = slots.Length - 1;
			while(amount<totalInt) {
				if (slots[index] > 0) {
					if (amount + SlotValues[index] <= totalInt) {
						slots[index]--;
						amount += SlotValues[index];
						result.Add(SlotValues[index]);
					} else {
						index--;
					}
				} else {
					index--;
				}
				if(index<0) {
					break;
				}
			}

			if (amount != totalInt) {
				return null;
			}

			for (int i = 0; i < slots.Length;i++ ) {
				CoinSlots[i] = slots[i];
			}

			if (Full) {
				Full = false;
			}

			return result.ToArray();
		}
	}
}
