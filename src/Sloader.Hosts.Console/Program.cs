using System;
using System.Threading.Tasks;
using Sloader.Engine;

namespace Sloader.Hosts.Console
{
	class Program
	{
		public static async Task Main(string[] args)
		{
			System.Console.WriteLine("Sloader Console App started.");

			await SloaderRunner.AutoRun();
		}
	}
}
