namespace YTPTK.Core
{
    public record Video
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public int Duration { get; init; }

        public override string ToString() =>
            $"{{ Id: {Id}, Title: {Title}, Duration: {Duration} }}";
    }
}