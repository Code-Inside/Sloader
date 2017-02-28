using System;
using System.Collections.Generic;

namespace Sloader.Result.Types
{
    public class GitHubEventResult : BaseResult
    {
        public class Event : IHaveRawContent
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Actor { get; set; }
            public string Repository { get; set; }
            public string Organization { get; set; }
            public string RawContent { get; set; }
            public string RelatedAction { get; set; }
            public string RelatedUrl { get; set; }
            public string RelatedDescription { get; set; }
            public string RelatedBody { get; set; }

        }

        public List<Event> Events { get; set; }

        public override KnownResultType ResultType
        {
            get { return KnownResultType.GitHubEvent; }
        }
    }
}