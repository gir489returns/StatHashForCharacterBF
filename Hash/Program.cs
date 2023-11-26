using System;
using System.Collections;
using System.Text;
using System.Xml;

class Program
{
	static uint JenkinsOneAtATimeHash(string input)
	{
		uint hash = 0;

		foreach (char c in input.ToLower())
		{
			hash += c;
			hash += hash << 10;
			hash ^= hash >> 6;
		}

		hash += hash << 3;
		hash ^= hash >> 11;
		hash += hash << 15;

		return hash;
	}

	static void Main(string[] args)
	{
		Hashtable lookupTable = new Hashtable();

		string[] filePaths = Directory.GetFiles(@"D:\Backup\Stuff\GTA V\MPSTATS");

		foreach (string filePath in filePaths)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);

			XmlNodeList nodes = doc.DocumentElement.SelectNodes("/StatsSetup/stats/stat");

			//using (StreamWriter outputFile = new StreamWriter(@"D:\Backup\Stuff\GTA V\output.txt"))
			{
				foreach (XmlNode node in nodes)
				{
					string name = node.Attributes["Name"].Value;
					string to_output = name;
					string to_hash = name;
					bool isCharacterStat = false;
					if (node.Attributes["characterStat"] != null)
					{
						isCharacterStat = Convert.ToBoolean(node.Attributes["characterStat"].Value);
					}

					if (isCharacterStat)
					{
						to_output = "MPX_" + name;
						to_hash = "MP0_" + name;
					}
					else
					{
						continue;
					}

					StringBuilder sb = new StringBuilder();

					var hash = JenkinsOneAtATimeHash(to_hash);

					lookupTable[hash] = to_output;
				}
			}
		}

		// Read the text file content into a string array
		string[] lines = File.ReadAllLines(@"D:\Backup\Stuff\GTA V\stats\bool_stats.txt");

		// Create a dictionary of key-value pairs from the array
		Dictionary<int, uint> dict = new Dictionary<int, uint>();

		// Loop through each line in the array
		foreach (string line in lines)
		{
			// Split the line by the equal sign and get the key and value
			string[] parts = line.Split(" // ");

			if (parts.Length == 2)
			{
				// Add the key-value pair to the dictionary
				int key;
				int.TryParse(parts[0], out key);
				uint value = uint.Parse(parts[1], System.Globalization.NumberStyles.HexNumber);
				dict.Add(key, value);
			}
		}

		using (StreamWriter outputFile = new StreamWriter(@"D:\Backup\Stuff\GTA V\bool_stats.txt"))
		{
			foreach (KeyValuePair<int, uint> kvp in dict)
			{
				outputFile.WriteLine("{0}: {1}", kvp.Key, lookupTable[kvp.Value]);
			}
		}
	}
}