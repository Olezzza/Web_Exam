using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Exam
{
    public  class HabrParser
    {
         WebDriver driver;

         string filterButton = "tm-navigation-filters__button";
         string bestButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul/li[2]/button";

         string dailyButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul[2]/li/button";
         string weeklyButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul[2]/li[2]/button";
         string monthlyButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul[2]/li[3]/button";
         string yearlyButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul[2]/li[4]/button";
         string allTimeButton = "//div[@id='app']/div/div[2]/main/div/div/div/div/div/div[2]/div[3]/div/div[2]/ul[2]/li[5]/button";

         string panelPagination = "tm-pagination";
         string applyButton = "tm-navigation-filters__apply";

         string titles = "tm-article-snippet__title-link";
         string authors = "tm-user-info__username";
         string dates = "tm-article-snippet__datetime-published";
         string rates = "tm-votes-meter__value_rating";
         string countOfViews = "tm-icon-counter__value";
         string descriptions = "article-formatted-body";

         List<string> titleList = new List<string>();
         List<string> authorsList = new List<string>();
         List<string> datesList = new List<string>();
         List<string> ratesList = new List<string>();
         List<string> countOfViewsList = new List<string>();
         List<string> descriptionList = new List<string>();

         IList<IWebElement> titlesOfArticles,
            authorsOfArticles,
            datesOfArticles,
            ratesOfArticles,
            countOfViewsOfArticles,
            descriptionOfArticles;

        List<Article> articles = new List<Article>();

        int countOfPages = 0, count = 0;

        public void parse() {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("log-level=3");
            driver = new ChromeDriver(options: chromeOptions);

            string[] arrButton = { dailyButton, weeklyButton/*, monthlyButton, yearlyButton, allTimeButton*/};
            driver.Navigate().GoToUrl(@"https://habr.com/ru/hub/machine_learning/");

            foreach (string btn in arrButton) {
                applyPeriod(btn);
                parseAllDataAboutArticles();
                transitionToNextPage();
                count++;
            }

            driver.Quit();
            count = 0;

            for (int i = 0; i < articles.Count; i++)
            {
                DatabaseWork.InsertArticleInDB(articles[i]);
                //Article.printArticle(articles[i]);
            }
        }

        public void dailyPrser()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("log-level=3");
            driver = new ChromeDriver(options: chromeOptions);

            driver.Navigate().GoToUrl(@"https://habr.com/ru/hub/machine_learning/");

            parseAllDataAboutArticles();
            //transitionToNextPage();
            driver.Quit();
            count = 0;

            for (int i = 0; i < articles.Count; i++)
            {
                DatabaseWork.InsertArticleInDB(articles[i]);
                //Article.printArticle(articles[i]);
            }
        }

        public void applyPeriod(string period)
        {
            driver.FindElement(By.ClassName(filterButton)).Click();

            if (count == 0) {
                Thread.Sleep(2000);
                driver.FindElement(By.XPath(bestButton)).Click();
            }

            Thread.Sleep(2000);
            driver.FindElement(By.XPath(period)).Click();

            Thread.Sleep(2000);
            driver.FindElement(By.ClassName(applyButton)).Click();
        }

        void parseAllDataAboutArticles()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(4000));

            Thread.Sleep(2000);
            titlesOfArticles = driver.FindElements(By.ClassName(titles));
            authorsOfArticles = driver.FindElements(By.ClassName(authors));
            datesOfArticles = driver.FindElements(By.ClassName(dates));
            ratesOfArticles = driver.FindElements(By.ClassName(rates));
            countOfViewsOfArticles = driver.FindElements(By.ClassName(countOfViews));
            descriptionOfArticles = driver.FindElements(By.ClassName(descriptions));

            addArticlesInList();
        }

        void transitionToNextPage() {
            Thread.Sleep(2000);
            bool isPresent = driver.FindElements(By.ClassName(panelPagination)).Count() > 0;

            Thread.Sleep(2000);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(4000));

            if (isPresent) {
                if (driver.FindElements(By.XPath("//div[2]/div[2]/div[2]/a")).Count() > 0)
                    countOfPages = Int32.Parse(driver.FindElement(By.XPath("//div[2]/div[2]/div[2]/a")).Text);
                else
                    countOfPages = Int32.Parse(driver.FindElement(By.XPath("//div[2]/div[2]/div/a[2]")).Text);

                do {
                    countOfPages--;
                    Thread.Sleep(3000);
                    try {
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("a[rel='next']"))).Click();
                        parseAllDataAboutArticles();
                    }
                    catch (WebDriverException e) {
                        Console.WriteLine(e.Message);
                    }
                } while (countOfPages != 1);
            }
        }

        void addArticlesInList() {
            for (int i = 0; i < titlesOfArticles.Count(); i++)
            {
                if (!articles.Exists(x => x.title == titlesOfArticles[i].Text))
                {
                    articles.Add(new Article(titlesOfArticles[i].Text, authorsOfArticles[i].Text, datesOfArticles[i].Text,
                        ratesOfArticles[i].Text, countOfViewsOfArticles[i].Text, descriptionOfArticles[i].Text));
                }
            }
        }
 
    }
}
