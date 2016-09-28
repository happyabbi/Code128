using System;

namespace Code128
{
	class MainClass
	{
		public static void Main(string[] args)
		{			 
			string code = string.Empty;



			code = "(00)007406172000000737(422)840(30)999(241)990KIN-KGEN/14023456";

			Console.WriteLine("Input: "+code);

			string barcode = GS1_128.StringToBarcode(code);
			if (barcode.Length > 0)
			{
				Console.WriteLine("Output:" + barcode);
			}
			else
			{
				Console.Write("Invalid string \"" + code + "\"");
			}
		}
	}
}
