using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TenLinks.Controllers;
using TenLinks.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TenLinks.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        KeywordController controller = new KeywordController(new KeywordContext(new DbContextOptions<KeywordContext>()));

        [Test]
        public void TestingSearchInGoogle()
        {
            Keyword keyword = new Keyword();
            keyword.Title = "Test";
            controller.SearchInGoogle(keyword);
            Assert.IsTrue(controller.links.Count > 0);
        }

        [Test]
        public void TestingSearchInBing()
        {
            Keyword keyword = new Keyword();
            keyword.Title = "Test";
            controller.SearchInBing(keyword);
            Assert.IsTrue(controller.links.Count > 0);
        }

        [Test]
        public void TestingSearchInYandex()
        {
            Keyword keyword = new Keyword();
            keyword.Title = "Yandex";
            controller.SearchInYandex(keyword);
            Assert.IsTrue(controller.links.Count > 0);
        }

        [Test]
        public void TestingAddLink()
        {
            Link firstLink = new Link();
            firstLink.Adress = "Standart adress";
            firstLink.Description = "Standart description";
            controller.AddLink(firstLink);
            Assert.IsTrue(controller.links.Count > 0);

            controller.links.Clear();

            Link secondLink = new Link();
            secondLink.Adress = new string('1', 401);
            secondLink.Description = "Standart description";
            controller.AddLink(secondLink);
            Assert.IsTrue(controller.links.Count == 0);

            controller.links.Clear();

            Link thirdLink = new Link();
            thirdLink.Adress = "Standart adress";
            thirdLink.Description = new string('1', 501);
            controller.AddLink(thirdLink);
            Assert.IsTrue(controller.links.Count == 0);

            controller.links.Clear();

            for (int i = 0; i < 20; i++)
            {
                controller.AddLink(firstLink);
            }
            Assert.IsTrue(controller.links.Count == 10);
        }
        [Test]
        public void TestingGetLinksAsync()
        {
            Keyword keyword = new Keyword();
            keyword.Title = "Test";
            controller.GetLinksAsync(keyword);

            Assert.AreEqual(controller.links.Count, 10);
        }

        [Test]
        public void TestingWorkWithDatabase() 
        {
            ActionResult result = controller.Create("Зима");
            Assert.AreEqual((result as ViewResult).ViewName, "~/Views/Keyword/TenLinksPage.cshtml");
        }

    }
}