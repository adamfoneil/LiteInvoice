namespace DtoBuilder.Abstractions;

public interface IDtoTypes
{
	string OutputPath { get; }
	Type[] SourceTypes { get; }
}
