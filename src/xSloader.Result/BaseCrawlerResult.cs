namespace Sloader.Results
{
    public abstract class BaseCrawlerResult
    {
        public abstract KnownCrawlerResultType ResultType { get; }
        public string ResultIdentifier { get; set; }
    }
}