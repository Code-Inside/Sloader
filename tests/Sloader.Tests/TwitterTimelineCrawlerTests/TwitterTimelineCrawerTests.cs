﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Sloader.Crawler.Twitter;
using Sloader.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.TwitterTimelineCrawlerTests
{
    public class TwitterTimelineCrawerTests
    {
        private static async Task<List<TwitterTimelineCrawlerResult>> InvokeSut(TwitterTimelineCrawlerConfig config)
        {
            string responseData =
                TestHelperForCurrentProject.GetTestFileContent("TwitterTimelineCrawlerTests.Sample.user_timeline.json");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTimelineCrawler();
            sut.Config = config;
            var result = await sut.DoWorkAsync();
            return result;
        }

        [Fact]
        public async Task Crawler_Should_Return_Correct_Number_Of_Tweets_In_Timeline()
        {
            var result = await InvokeSut(new TwitterTimelineCrawlerConfig { OAuthToken = Guid.NewGuid().ToString(), Handles = "test"});
            Assert.Equal(5, result.First().Tweets.Count);
        }


        [Fact]
        public async Task Crawler_Should_Return_EmptyList_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(new TwitterTimelineCrawlerConfig());
            Assert.Equal(0, result.Count);
        }
    }
}