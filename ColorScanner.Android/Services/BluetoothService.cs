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

        public BluetoothService()
        {
        }

		private BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;

		private CancellationTokenSource _ct { get; set; }

		#region IBluetoothService implementation

		public void Start(string name, int sleepTime, Func<string, Task> resultTask)
		{
			Task.Run(() => Loop(name, sleepTime, resultTask));
		}

		public void Cancel()
		{
			_ct?.Cancel();
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

		private async Task Loop(string name, int sleepTime, Func<string, Task> resultTask)
		{
			BluetoothDevice device = null;
			BluetoothSocket BthSocket = null;

			_ct = new CancellationTokenSource();
			while (!_ct.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(sleepTime);

					device = Adapter.BondedDevices.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

					if (device != null)
					{
						UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

						if ((int)Android.OS.Build.VERSION.SdkInt >= 10)
						{
							BthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
						}
						else
						{
							BthSocket = device.CreateRfcommSocketToServiceRecord(uuid);
						}

						if (BthSocket != null)
						{
							await BthSocket.ConnectAsync();

							if (BthSocket.IsConnected)
							{
								var mReader = new InputStreamReader(BthSocket.InputStream);
								var buffer = new BufferedReader(mReader);
								var currentData = string.Empty;

								while (!_ct.IsCancellationRequested)
								{
									if (buffer.Ready())
									{
										char[] chr = new char[100];
										currentData = await buffer.ReadLineAsync();

										if (!string.IsNullOrEmpty(currentData))
										{
											await resultTask(currentData);
										}
									}
								}
							}
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
		}

		#endregion
	}
}
