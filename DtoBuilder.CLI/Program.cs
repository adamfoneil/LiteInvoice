using System.Reflection;
using System.Runtime.Loader;

string assemblyPath = FindAssemblyPath(args[0]);


Console.WriteLine($"Loading assembly: {assemblyPath}");
var context = new CustomAssemblyLoadContext();
Assembly assembly = context.LoadFromAssemblyPath(assemblyPath);

string FindAssemblyPath(string path)
{
	


}

string FindCsProj(string path)
{
	if (File.Exists(path) && Path.GetExtension(path).Equals(".csproj")) return path;

	var files = Directory.GetFiles(path, "*.csproj", SearchOption.TopDirectoryOnly);

	return 
		(files.Length == 1) ? files[0] : throw new Exception("")
}


class CustomAssemblyLoadContext : AssemblyLoadContext
{
	public CustomAssemblyLoadContext() : base(isCollectible: true) { }

	protected override Assembly Load(AssemblyName assemblyName)
	{
		string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName.Name + ".dll");

		if (File.Exists(assemblyPath))
		{
			return LoadFromAssemblyPath(assemblyPath);
		}

		throw new Exception($"Unable to load assembly {assemblyName}");
	}
}

