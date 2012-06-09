<Query Kind="Program">
  <Connection>
    <ID>7db35e7c-4a61-4d41-970f-5b3008a44339</ID>
    <Driver>AstoriaAuto</Driver>
    <Server>http://odata.netflix.com/Catalog/</Server>
  </Connection>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	Dictionary<string,bool> collection = new Dictionary<string,bool>();
	IEnumerable<string> commonWords = File.ReadAllLines(@"G:\LINQPad4\words.txt")
		.Where(x => !string.IsNullOrEmpty(x)).Select(t => t.Trim());
	
	string magna_carta = File.ReadAllText(@"G:\LINQPad4\magna-carta.txt");
	
	Parallel.ForEach(commonWords,
	() => new Dictionary<string,bool>(),
	(word, loopState, localState) =>
	{
		RabinKarpAlgo rbAlgo = new RabinKarpAlgo(magna_carta,word);
		localState.Add(word,rbAlgo.Match());
		return localState;
	},
	(localState) =>
	{
		lock(collection){
			foreach(var item in localState)
			{
				collection.Add(item.Key, item.Value);
			}
		}
	});
	
	collection.Dump();
}

public class RabinKarpAlgo
{
	private readonly string inputString;
	private readonly string pattern;
	private ulong siga = 0;
	private ulong sigb = 0;
	private readonly ulong Q = 100007;
	private readonly ulong D = 256;
	
	public RabinKarpAlgo(string inputString, string pattern)
	{
		this.inputString = inputString;
		this.pattern = pattern;
	}
	
	public bool Match()
	{
		for (int i = 0; i < pattern.Length; i++)
		{
			siga = (siga * D + (ulong)inputString[i]) % Q;
			sigb = (sigb * D + (ulong)pattern[i]) % Q;
		}
		
		if(siga == sigb)
			return true;
		
		ulong pow = 1;
		for (int k = 1; k <= pattern.Length - 1; k++)
			pow = (pow * D) % Q;
			
		for (int j = 1; j <= inputString.Length - pattern.Length; j++)
		{
			siga = (siga + Q - pow * (ulong)inputString[j - 1] %Q) % Q;
			siga = (siga * D + (ulong)inputString[j + pattern.Length - 1]) % Q;
			
			if (siga == sigb)
			{
				if (inputString.Substring(j, pattern.Length) == pattern)
				{
					return true;
				}
			}
		}
		
		return false;
	}
}

// Define other methods and classes here