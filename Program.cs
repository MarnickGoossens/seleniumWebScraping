using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            foreach (IWebElement productElement in productElements.Take(5))
            {
                string title = productElement.FindElement(By.Id("video-title")).Text;
                string uploaded = productElement.FindElement(By.CssSelector("#metadata-line > span:nth-child(4)")).Text;
                items.Add(new string[] { title, uploaded });
                string csvFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\YouTube.csv";
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Title,Uploaded");
                    foreach (string[] item in items)
                    {
                        writer.WriteLine(string.Join(",", item));
                    }
                }
            }

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
            Console.WriteLine(productElements.Count);
            foreach (IWebElement productElement in productElements.Take(5))
            {
                string title = productElement.FindElement(By.ClassName("job-title")).Text;
                string company = productElement.FindElement(By.ClassName("job-company")).Text;
                items.Add(new string[] { title, company });
                string csvFilePath = "C:\\Users\\marni\\source\\repos\\webscraping\\items.csv";
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Title,Company");
                    foreach (string[] item in items)
                    {
                        writer.WriteLine(string.Join(",", item));
                    }
                }
            }

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

            foreach (IWebElement productElement in productElements.Take(3))
            {
                string title = productElement.FindElement(By.ClassName("product-title--inline")).Text;
                string price = productElement.FindElement(By.ClassName("promo-price")).Text.Replace("\r", ".").Replace("\n", "");
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

            driver.Quit();
        }
    }
}
