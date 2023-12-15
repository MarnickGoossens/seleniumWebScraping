using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Text;
using System.Diagnostics;
class Program
{
    static void Main()
    {
        Console.WriteLine("Kies uit de volgende keuzes:");
        Console.WriteLine("1. De 5 meest recent geüploade YouTube video’s op basis van een zoekterm");
        Console.WriteLine("2. De 5 nieuwste jobs te vinden op www.ictjob.be met zoekterm");
        Console.WriteLine("3. De eerste 3 producten die bol.com geeft via een zoekterm");
        Console.Write("Geef je keuze: ");
        int keuze = Convert.ToInt32(Console.ReadLine());
        Console.Write("Geef je zoekterm: ");
        string zoekterm = Console.ReadLine();

        if (keuze == 1)
        {
            IWebDriver driver = new ChromeDriver();
            String url = "https://www.youtube.com/";
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            List<string[]> items = new List<string[]>();
            IWebElement acceptAll = driver.FindElement(By.CssSelector("div.body.style-scope.ytd-consent-bump-v2-lightbox > div.eom-buttons.style-scope.ytd-consent-bump-v2-lightbox > div:nth-child(1) > ytd-button-renderer:nth-child(1) > yt-button-shape > button"));
            acceptAll.Click();
            Thread.Sleep(2000);
            IWebElement searchBox = driver.FindElement(By.Name("search_query"));
            searchBox.SendKeys(zoekterm);
            Thread.Sleep(1000);
            searchBox.SendKeys(Keys.Enter);
            // Wacht enkele seconden om de zoekresultaten te laden.
            Thread.Sleep(2000);

            IWebElement filter = driver.FindElement(By.CssSelector("#filter-button > ytd-button-renderer > yt-button-shape > button > yt-touch-feedback-shape > div > div.yt-spec-touch-feedback-shape__fill"));
            filter.Click();
            Thread.Sleep(2000);
            IWebElement releaseDate = driver.FindElement(By.CssSelector("#label > yt-formatted-string"));
            releaseDate.Click();
            Thread.Sleep(2000);

            IReadOnlyCollection<IWebElement> productElements = driver.FindElements(By.CssSelector("#contents > ytd-video-renderer"));
            List<video> videos = new List<video> {};
            foreach (IWebElement productElement in productElements.Take(5))
            {
                // Het blok tekst splitsen in een array van strings op basis van nieuwe regeltekens
                string[] regels = productElement.Text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Een lijst maken en de regels toevoegen
                List<string> regelLijst = new List<string>(regels);

                // Inhoud van de lijst afdrukken
                Console.WriteLine("Inhoud van de lijst:");
                foreach (string regel in regelLijst)
                {
                    Console.WriteLine(regel);
                }
                string title = productElement.FindElement(By.Id("video-title")).Text;
                string link = productElement.FindElement(By.Id("video-title")).GetAttribute("href");
                string uploaded = productElement.FindElement(By.CssSelector("#metadata-line > span:nth-child(4)")).Text; 
                
                items.Add(new string[] { title, uploaded, link });
                videos.Add(new video { videoTitle = title, videoUploaded = uploaded, videoLink = link });
                string csvFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\YouTube.csv";
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Title,uploader,uploaded,link");
                    foreach (string[] item in items)
                    {   
                        writer.WriteLine(string.Join(",", item));
                    }
                }
            }

            // Serialize de lijst naar een JSON-string
            string jsonString = JsonSerializer.Serialize(videos);

            // Schrijf de JSON-string naar een bestand
            string jsonFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\youtube.json";
            File.WriteAllText(jsonFilePath, jsonString, Encoding.UTF8);

            // Sluit de browser.
            driver.Quit();
        }
        else if (keuze == 2)
        {
            IWebDriver driver = new ChromeDriver();
            String url = "https://www.ictjob.be/nl/";
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            List<string[]> items = new List<string[]>();
            IWebElement searchField = driver.FindElement(By.Id("keywords-input"));
            IWebElement searchButton = driver.FindElement(By.Id("main-search-button"));
            searchField.SendKeys(zoekterm);
            Thread.Sleep(1000);
            searchButton.Click();
            Thread.Sleep(20000);

            IReadOnlyCollection<IWebElement> productElements = driver.FindElements(By.ClassName("job-info"));
            List<vacature> vacatures = new List<vacature> { };
            foreach (IWebElement productElement in productElements.Take(5))
            {
                string title = productElement.FindElement(By.ClassName("job-title")).Text;
                string company = productElement.FindElement(By.ClassName("job-company")).Text;
                string location = productElement.FindElement(By.ClassName("job-location")).Text;
                string keywords = productElement.FindElement(By.CssSelector("span:nth-child(4)")).Text.Replace(",", ";");
                string link = productElement.FindElement(By.ClassName("job-title")).GetAttribute("href");
                vacatures.Add(new vacature { jobTitle = title, jobCompany = company, jobLocation = location, jobKeywords = keywords, jobLink = link, });
                items.Add(new string[] { title, company, location, keywords, link });
                string csvFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\vacatures.csv";
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Title,Company,location,keywords,link");
                    foreach (string[] item in items)
                    {
                        writer.WriteLine(string.Join(",", item));
                    }
                }
            }
            // Serialize de lijst naar een JSON-string
            string jsonString = JsonSerializer.Serialize(vacatures);

            // Schrijf de JSON-string naar een bestand
            string jsonFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\vacatures.json";
            File.WriteAllText(jsonFilePath, jsonString, Encoding.UTF8);

            driver.Quit();
        }
        else if (keuze == 3) {
            IWebDriver driver = new ChromeDriver();
            String url = "https://www.bol.com/nl/nl/";
            driver.Navigate().GoToUrl(url);
            List<string[]> items = new List<string[]>();
            Thread.Sleep(2000);
            IWebElement acceptAll = driver.FindElement(By.Id("js-first-screen-accept-all-button"));
            acceptAll.Click();
            Thread.Sleep(2000);
            IWebElement doorgaan = driver.FindElement(By.ClassName("js-country-language-btn"));
            doorgaan.Click();
            Thread.Sleep(2000);
            IWebElement searchField = driver.FindElement(By.Id("searchfor"));
            searchField.SendKeys(zoekterm);
            searchField.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            IReadOnlyCollection<IWebElement> productElements = driver.FindElements(By.ClassName("product-item--row"));
            List<product> producten = new List<product> { };
            foreach (IWebElement productElement in productElements.Take(3))
            {
                string title = productElement.FindElement(By.ClassName("product-title--inline")).Text;
                string price = productElement.FindElement(By.ClassName("promo-price")).Text.Replace("\r", ".").Replace("\n", "");
                producten.Add(new product { productTitle = title, productPrice = price });
                items.Add(new string[] { title, price });
                string csvFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\bol.csv";
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Title,Price");
                    foreach (string[] item in items)
                    {
                        writer.WriteLine(string.Join(",", item));
                    }
                }
            }

            // Serialize de lijst naar een JSON-string
            string jsonString = JsonSerializer.Serialize(producten);

            // Schrijf de JSON-string naar een bestand
            string jsonFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\producten.json";
            File.WriteAllText(jsonFilePath, jsonString, Encoding.UTF8);

            driver.Quit();
        }
    }
}

// Define a simple class for demonstration
// Define een eenvoudige klasse voor demonstratie

public class video
{
    public string videoTitle { get; set; }
    public string videoUploaded { get; set; }
    public string videoLink { get; set; }
}

public class vacature
{
    public string jobTitle { get; set; }
    public string jobCompany { get; set; }
    public string jobLocation { get; set; }
    public string jobKeywords { get; set; }
    public string jobLink { get; set; }
}

public class product
{
    public string productTitle { get; set; }
    public string productPrice { get; set; }
}