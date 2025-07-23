namespace Hayson.EditorKit
{
    public record ComponentInfo(string Name)
    {
        public string Description { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string Version { get; init; } = string.Empty;
        public string ReadmePath { get; init; } = string.Empty;
    }
}