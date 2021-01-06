using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Fizzler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TenLinks.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System.Threading;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Net;
using AngleSharp.Dom;
using System.Web;

namespace TenLinks.Controllers
{
    public class KeywordController : Controller
    {
        KeywordContext Database;
        public KeywordController(KeywordContext context)
        {
            Database = context;
        }

        public ActionResult Index()
        {
            return View();
        }


        //Общий лист
        public List<Link> links = new List<Link>();

        [HttpPost]
        public ActionResult Create(string currentKeyword)
        {
            //Проверка наличия ключевого слова в базе данных

            foreach (var keyword in Database.Keyword.ToList())
            {
                if (keyword.Title == currentKeyword)
                {
                    links = Database.Link.ToList().Where(l => l.KeywordId == keyword.Id).ToList();
                    if (links.Count > 10)
                    {
                        links.RemoveRange(10, links.Count - 10);
                    }
                    ViewBag.Links = links;
                    return View("~/Views/Keyword/TenLinksPage.cshtml");
                }
            }

            //Парсинг

            //Так как myKeyword используется и в try и в catch, он был объявлен снаружи конструкции
            var myKeyword = new Keyword();
            try
            {
                myKeyword.Title = currentKeyword;
                Database.Keyword.Add(myKeyword);
                Database.SaveChanges();

                GetLinksAsync(myKeyword);

                foreach (var link in links)
                {
                    Database.Link.Add(link);
                }
                Database.SaveChanges();

                ViewBag.Links = links;
                return View("~/Views/Keyword/TenLinksPage.cshtml");
            }
            catch (Exception ex)
            {
                //Каскадное удаление добавленных ключевых слов и ссылок
                if (myKeyword != null && Database.Keyword.Find(myKeyword.Id) != null)
                {
                    Database.Keyword.Remove(myKeyword);
                }
                Database.SaveChanges();

                ViewBag.Message = ex.Message.ToString();
                return View("~/Views/Keyword/Error.cshtml");
            }
        }

        public async void GetLinksAsync(Keyword myKeyword)
        {
            //Яндекс
            Task firstTask = Task.Run(() => SearchInYandex(myKeyword));

            //Bing
            Task secondTask = Task.Run(() => SearchInBing(myKeyword));

            //Google
            Task thirdTask = Task.Run(() => SearchInGoogle(myKeyword));

            Task.WaitAll(firstTask, secondTask, thirdTask);
        }

        public void SearchInGoogle(Keyword myKeyword)
        {
            //Поиск всех блоков с ссылками и их описанием
            var client = new HtmlWeb();
            var document = client.Load($"https://www.google.com/search?q={myKeyword.Title}");
            var page = document.DocumentNode;
            var items = page.SelectNodes("//div[@class=\"rc\"]");

            //Добавление ссылок и их описания в общий лист
            foreach (var item in items)
            {
                try
                {
                    Link newLink = new Link();
                    newLink.Adress = HttpUtility.UrlDecode(item.SelectSingleNode("div[1]/a").Attributes["href"].Value);
                    newLink.Description = item.SelectSingleNode("div[2]/div/span").InnerText.Replace("&nbsp;", "");
                    newLink.KeywordId = myKeyword.Id;
                    AddLink(newLink);
                }
                catch
                {
                    continue;
                }
            }
        }

        public void SearchInYandex(Keyword myKeyword)
        {
            //Поиск всех блоков с ссылками и их описанием
            var parser = new HtmlParser();
            var document = parser.ParseDocument(new WebClient().DownloadString($"https://yandex.ru/search/?text={myKeyword.Title}&lr=1&p=0"));
            var items = document.QuerySelectorAll("li.serp-item");

            //Добавление ссылок и их описания в общий лист
            foreach (var item in items)
            {
                try
                {
                    Link newLink = new Link();
                    newLink.Adress = HttpUtility.UrlDecode(item.GetElementsByTagName("a").First().GetAttribute("href")).Trim();
                    newLink.Description = item.GetElementsByClassName("organic__content-wrapper").First().TextContent;
                    newLink.KeywordId = myKeyword.Id;
                    AddLink(newLink);
                }
                catch
                {
                    continue;
                }
            }
        }

        //Для анализа поисковой системы Bing был выбран Selenium, так как иные инструменты парсинга, 
        //например, HtmlAgilityPack выдавали пустую страницу с сообщением "Результаты не найдены".
        public void SearchInBing(Keyword myKeyword)
        {
            //Открытие браузера Chrome
            IWebDriver driver = new ChromeDriver(Environment.CurrentDirectory);

            driver.Url = $"https://www.bing.com/search?q={myKeyword.Title}";

            //Задержка для прогрузки браузера
            Thread.Sleep(3000);

            var items = driver.FindElements(By.ClassName("b_algo"));

            foreach (var item in items)
            {
                try
                {
                    Link newLink = new Link();
                    newLink.Adress = HttpUtility.UrlDecode(item.FindElement(By.TagName("a")).GetAttribute("href"));
                    newLink.Description = item.FindElement(By.XPath(".//div/p")).Text;
                    newLink.KeywordId = myKeyword.Id;
                    AddLink(newLink);
                }
                catch
                {
                    continue;
                }
            }

            //Закрытие браузера после отработки цикла
            driver.Close();
        }

        //Вспомогательный метод
        public void AddLink(Link link)
        {
            if (link.Description.Length <= 500 && link.Adress.Length <= 400 && links.Count < 10)
            {
                links.Add(link);

                //Задержка для поддержки асинхронного добавления в общий лист
                Thread.Sleep(100);
            }
        }

    }
}