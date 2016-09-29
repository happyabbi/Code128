using System;
namespace Code128
{
	public static class GS1_128
	{
		//private static string FNC1String="Ê";
		//private static string startWithTableB = "Ì";
		//private static string startWithTableC = "Í";
		//private static string switchToTableC = "Ç";
		//private static string switchToTableB = "È";

		private static string FNC1 = ">8";
		private static string startWithTableB = ">:";
		private static string startWithTableC = ">;";
		private static string switchToTableC = ">5";
		private static string switchToTableB = ">6";



		public static string StringToBarcode(string value)
		{
			string barcode = string.Empty;
			int num = 0;
			bool tableB = true;
			bool tswCl, tswBl;

			// Remove unnecessary characters
			value = value.Replace(Environment.NewLine, string.Empty);
			// Split string to parts if it contains application identifiers
			string[] parts = value.Split('(');

			// Validate and process string parts and construct barcode
			foreach (string part in parts)
			{
				if (IsValid(part))
				{
					if (part.Length > 0)
					{
						string code = ProcessBarcodePart(part.Replace(")", string.Empty), num, tableB);
						if (part.Contains(")"))
						{
							tswCl = (switchToTableC+startWithTableC).Contains(code.Substring(0, 2));
							tswBl = startWithTableB == code.Substring(0, 2);
							if (tswCl || tswBl)
							{
								code = code.Substring(0, 2) + FNC1 + code.Substring(2);

								if (tswCl)
								{
									tableB = false;
								}

								if (tswBl)
								{
									tableB = true;
								}
							}
							else {
								code = FNC1 + code;
							}

							tableB = code.Contains(switchToTableB);
						}
						barcode += code;
						num++;
					}
				}
				else
				{
					return string.Empty;
				}
			}

			if (barcode.Length > 0)
			{
				return barcode;
			}

			return string.Empty;
		}




		private static string ProcessBarcodePart(string value, int num, bool isTableB)
		{

			int charPos, minCharPos;
			string returnValue = string.Empty;

			if (value.Length > 0)
			{
				charPos = 0;
				while (charPos < value.Length)
				{
					if (isTableB)
					{
						// See if interesting to switch to table C
						// yes for 4 digits at start or end, else if 6 digits
						if (charPos == 0 || charPos + 4 == value.Length)
							minCharPos = 4;
						else
							minCharPos = 6;


						minCharPos = IsNumber(value, charPos, minCharPos);

						if (minCharPos < 0)
						{
							// Choice table C
							if (charPos == 0 && num == 0)
							{
								// Starting with table C
								returnValue = startWithTableC;
							}
							else
							{
								// Switch to table C
								returnValue = returnValue + switchToTableC;
							}
							isTableB = false;
						}
						else
						{
							if (charPos == 0 && num == 0)
							{
								// Starting with table B
								returnValue = startWithTableB;
							}

						}
					}

					if (!isTableB)
					{
						// We are on table C, try to process 2 digits
						minCharPos = 2;
						minCharPos = GS1_128.IsNumber(value, charPos, minCharPos);
						if (minCharPos < 0) // OK for 2 digits, process it
						{
							var currentChar =  value.Substring(charPos, 2);
						 
							returnValue = returnValue + currentChar;
							charPos += 2;
						}
						else
						{
							// We haven't 2 digits, switch to table B
							returnValue = returnValue + switchToTableB;
							isTableB = true;
						}
					}
					if (isTableB)
					{
						// Process 1 digit with table B
						returnValue += value.Substring(charPos, 1);
						charPos++;
					}
				}
			}

			return returnValue;
		}

		private static bool IsValid(string value)
		{
			int currentChar;
			// Check for valid characters
			for (int charCount = 0; charCount < value.Length; charCount++)
			{
				currentChar = (int)char.Parse(value.Substring(charCount, 1));
				if (!(currentChar >= 32 && currentChar <= 126))
				{
					return false;
				}
			}
			return true;

		}

		private static int IsNumber(string InputValue, int CharPos, int MinCharPos)
		{
			// if the MinCharPos characters from CharPos are numeric, then MinCharPos = -1
			MinCharPos--;
			if (CharPos + MinCharPos < InputValue.Length)
			{
				while (MinCharPos >= 0)
				{
					if ((int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) < 48
						|| (int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) > 57)
					{
						break;
					}
					MinCharPos--;
				}
			}
			return MinCharPos;
		}

	}
}

