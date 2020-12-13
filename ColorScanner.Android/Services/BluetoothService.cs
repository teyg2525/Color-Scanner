using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using ColorScanner.Services;
using Java.Interop;
using Java.IO;
using Java.Util;

namespace ColorScanner.Droid.Services
{
    public class BluetoothService : IBluetoothService
    {
        private static IBluetoothService _Instance;
        public static IBluetoothService Instance => _Instance ??= new BluetoothService();

		private string _currentData;

        public BluetoothService()
        {
        }

		private BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;

		private CancellationTokenSource _ct { get; set; }

		#region IBluetoothService implementation

		public void Start(string name, int sleepTime = 200)
		{
			Task.Run(() => Loop(name, sleepTime));
		}
		public void Cancel()
		{
			System.Diagnostics.Debug.WriteLine("Send a cancel to task!");
			_ct?.Cancel();
		}

		public Task<string> GetCurrentData()
        {
			return Task.FromResult(_currentData);
        }

		public Task<IEnumerable<string>> GetPairedDevices()
        {
			IEnumerable<string> result;

            try
			{
				result = Adapter.BondedDevices.Select(x => x.Name);
			}
			catch (Exception ex)
            {
				result = new List<string>();
            }

			return Task.FromResult(result);
        }

		#endregion

		#region -- Private Helpers --

		private async Task Loop(string name, int sleepTime)
		{
			BluetoothDevice device = null;
			BluetoothSocket BthSocket = null;

			_ct = new CancellationTokenSource();
			while (!_ct.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(sleepTime);

					foreach (var bd in Adapter.BondedDevices)
					{
						System.Diagnostics.Debug.WriteLine("Paired devices found: " + bd.Name.ToUpper());
						if (bd.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
						{

							System.Diagnostics.Debug.WriteLine("Found " + bd.Name + ". Try to connect with it!");
							device = bd;
							break;
						}
					}

					if (device == null)
					{
						System.Diagnostics.Debug.WriteLine("Named device not found.");
					}
					else
					{
						UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
						if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3 2.3.4
							BthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
						else
							BthSocket = device.CreateRfcommSocketToServiceRecord(uuid);

						if (BthSocket != null)
						{
							await BthSocket.ConnectAsync();

							if (BthSocket.IsConnected)
							{
								System.Diagnostics.Debug.WriteLine("Connected!");
								var mReader = new InputStreamReader(BthSocket.InputStream);
								var buffer = new BufferedReader(mReader);

								while (_ct.IsCancellationRequested == false)
								{
									if (buffer.Ready())
									{
										char[] chr = new char[100];
										_currentData = await buffer.ReadLineAsync();

										if (_currentData.Length > 0)
										{
											System.Diagnostics.Debug.WriteLine("Letto: " + _currentData);
										}
										else
										{
											System.Diagnostics.Debug.WriteLine("No data");
										}

									}
									else
									{
										System.Diagnostics.Debug.WriteLine("No data to read");
									}

									await Task.Delay(sleepTime);
								}

								System.Diagnostics.Debug.WriteLine("Exit the inner loop");
							}
						}
						else
						{
							System.Diagnostics.Debug.WriteLine("BthSocket = null");
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("EXCEPTION: " + ex.Message);
				}
				finally
				{
					if (BthSocket != null)
					{
						BthSocket.Close();
					}

					device = null;
				}
			}

			System.Diagnostics.Debug.WriteLine("Exit the external loop");
		}

		#endregion
	}
}
