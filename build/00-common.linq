<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

const string VersionSuffix = "rc.9";

async Task Main()
{
	await SetupAsync(QueryCancelToken);
}

async Task SetupAsync(CancellationToken cancellationToken = default)
{
	Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
	Environment.CurrentDirectory = Util.CurrentQuery.Location;
	await EnsureNugetExe(cancellationToken);
}

void NuGetRun(string args) => Run(@".\win64\nuget.exe", args);
void DotNetRun(string args) => Run("dotnet", args);
void Run(string exe, string args) => Util.Cmd(exe, args, Encoding.GetEncoding("gb2312"));
string[] Projects = new[]
{
	"Sdcb.PaddleInference",
	"Sdcb.PaddleOCR",
	"Sdcb.PaddleOCR.KnownModels", 
};

async Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken = default)
{
	using HttpClient http = new();

	HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
	if (!resp.IsSuccessStatusCode)
	{
		throw new Exception($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
	}

	using (FileStream file = File.OpenWrite(localFile))
	{
		await resp.Content.CopyToAsync(file, cancellationToken);
	}
}

async Task<string> EnsureNugetExe(CancellationToken cancellationToken = default)
{
	Uri uri = new Uri(@"https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
	string localPath = @".\win64\nuget.exe";
	if (!File.Exists(localPath))
	{
		await DownloadFile(uri, localPath, cancellationToken);
	}
	return localPath;
}