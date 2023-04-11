using System.Text;

var tokenData = File.ReadAllLines(args[0])[0];

var tokenSplit = tokenData.Split('.');

var partNum = 0;
foreach(var part in tokenSplit){
    Console.WriteLine($"---- Part {++partNum} ----");
    Console.WriteLine(DecodeBase64UrlString(part));
}


static string DecodeBase64UrlString(string input)
{
    string base64 = input.Replace('-', '+').Replace('_', '/');
    while (base64.Length % 4 != 0)
    {
        base64 += '=';
    }
    var base64Bytes = Convert.FromBase64String(base64);
    return Encoding.UTF8.GetString(base64Bytes);
}