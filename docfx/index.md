# Idea & Goal

The goal of this library is to load data from "various" endpoints and store them in various "drops" in a structured way. 

The endpoint & drop configuration should very simple and developer focused.

To load data from those endpoints we have "Crawlers" and to store them we have "Drops".

## Scenarios

* Make "static pages" more dynamic - e.g. load your Twitter/GitHub/Feed data and store them in your GitHub Page

## Components

Sloader has these components:

* A config, which is defined by __Sloader.Config__ and is typically a .yml file
* The __Sloader.Engine__, which uses the config and implements the crawler & drops. Each crawler has it's own config & result "schema". When all endpoints have been crawled the result will be "dropped" to a configured destination.
* The output of the engine is a Key/Value json structure with the results of each crawler, this is defined under __Sloader.Result__.

# QuickStart

1. Create this Sloader.yml file:

```yml
Crawler:
  FeedsToCrawl:
  - Key: Blog
    Url: http://blog.codeinside.eu/feed
    LoadSocialLinkCounters: false
  GitHubEventsToCrawl:
  - Key: GitHubEvent
    Repository: code-inside/sloader
    
Drop:
  FileDrops:
  - FilePath: "result.json"
```

2. Save this file on your local drive or use [GitHubs Gist](https://gist.github.com/) or something else.

3. New Project -> C# Console App.

4. Install-Package Sloader.Engine

5. Code:

```csharp
class Program
    {
        static void Main(string[] args)
        {
            Sloader.Engine.SloaderRunner.AutoRun("https://gist.githubusercontent.com/.../gistfile1.txt", new Dictionary<string, string>()).GetAwaiter().GetResult();
        }
    }
```

6. Run.

7. Open bin/debug/result.json.

__Done.__

Note:

* There are several Drops & Crawlers available.
* You can also AppSettings to locate the Sloader.Config

# Sample Code

## Plain Console App:

```csharp
class Program
    {
        static void Main(string[] args)
        {
            Sloader.Engine.SloaderRunner.AutoRun("https://gist.githubusercontent.com/.../gistfile1.txt", new Dictionary<string, string>()).GetAwaiter().GetResult();
        }
    }
```

## Azure Function

This code runs currently in an Azure Function:

```
public static void Run(TimerInfo everyDay, TraceWriter log)
{
    log.Info($"Sloader run.csx invoked at: {DateTime.Now}");    

    var listener = new SystemTraceListener(log);

    log.Info($"SloaderRunner Version: {typeof(Sloader.Engine.SloaderRunner).Assembly.GetName().Version}");       

    Sloader.Engine.SloaderRunner.AutoRun().GetAwaiter().GetResult();

    log.Info($"Sloader run.csx done at: {DateTime.Now}");    

}
```

You need this AppSettings Key, which will point to your Sloader.yml file:

    Sloader.SloaderConfigFilePath

The Sloader.SloaderConfigFilePath can be a local file path or a URL.

# Working with Secrets

Some Crawlers or Drops need secrets - e.g. the Twitter API key. The complete config is defined in your Sloader.yml file. 
When the Sloader.Config loading kicks in, Sloader looks for placeholders and will replace them with values from the AppSettings.

If you have configured something like this:

    TwitterConsumerKey: $$Sloader.SecretTwitterConsumerKey$$

And a "Sloader.SecretTwitterConsumerKey" AppSettings Key is found, the value is injected.

# Full Sample Sloader.yml

```yml
Secrets:
  TwitterConsumerKey: $$Sloader.SecretTwitterConsumerKey$$
  TwitterConsumerSecret: $$Sloader.SecretTwitterConsumerSecret$$
  GitHubAccessToken: $$Sloader.SecretGitHubAccessToken$$

Crawler:
  FeedsToCrawl:
  - Key: Blog
    Url: http://blog.codeinside.eu/feed
    LoadSocialLinkCounters: false
  - Key: GitHub
    Url: https://github.com/robertmuehsig.atom; https://github.com/oliverguhr.atom
    LoadSocialLinkCounters: false
  TwitterTimelinesToCrawl:
  - Handle: codeinsideblog;robert0muehsig;oliverguhr
    IncludeRetweets: false
    Key: Twitter
  TwitterUsersToCrawl:
  - Handle: robert0muehsig
    Key: TwitterMe
  GitHubEventsToCrawl:
  - Key: GitHubEvent
    Repository: code-inside/sloader
    
Drop:
  FileDrops:
  - FilePath: "test.json"
  GitHubDrops:
  - Owner: "Code-Inside"
    Repo: "Hub"
    Branch: "gh-pages"
    FilePath: "_data/test.json"
```
