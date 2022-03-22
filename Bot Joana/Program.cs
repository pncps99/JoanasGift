using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Bot
{
    class Waits
    {
        public static IWebElement HoldUntil(ChromeDriver driver, int timeout=10000, string xpath="", string id="", string name="")
        {
            if (!String.IsNullOrEmpty(xpath))
                return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(driver => driver.FindElement(By.XPath(xpath)));
            else if (!String.IsNullOrEmpty(id))
                return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(driver => driver.FindElement(By.Id(id)));
            else
                return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(driver => driver.FindElement(By.Name(name)));
        }
    }

    class Program
    {
        private static void Login(ChromeDriver driver, string[] data, int timeout = 0)
        {
            driver.Navigate().GoToUrl("https://www.zara.com/pt/pt/logon");

            // Because cookies
            Thread.Sleep(timeout);

            Waits.HoldUntil(driver, id: "onetrust-accept-btn-handler").Click();

            Waits.HoldUntil(driver, name: "logonId").SendKeys(data[0]);

            Waits.HoldUntil(driver, name: "password").SendKeys(data[1]);

            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div/div[2]/div[1]/section/form/div[2]/button").Submit();
        }

        private static void FillCardInfo(ChromeDriver driver,
                                            string number,
                                            string expirationMonth,
                                            string expirationYear,
                                            string name,
                                            string cvv2)
        {
            // fill number
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[1]/div[1]/div/div/div[1]/input").SendKeys(number);

            // fill expiration Month
            new SelectElement(Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[1]/fieldset/div/div[1]/div/div[1]/div/select")).SelectByText("0" + expirationMonth);

            // fill expiration Year
            new SelectElement(Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[1]/fieldset/div/div[2]/div/div[1]/div/select")).SelectByText(expirationYear);

            // fill name
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[1]/div[2]/div/div/div[1]/input").SendKeys(name);

            // fill cvv2
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[1]/div[3]/div/div/div[1]/input").SendKeys(cvv2);
        }

        private static void Checkout(ChromeDriver driver, string[] data, int timeout = 0)
        {
            driver.Navigate().GoToUrl("https://www.zara.com/pt/pt/shop/cart");

            if (timeout > 0)
                Thread.Sleep(timeout);

            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/div[2]/section/div[2]/div[2]/div[2]/button").Click();

            if (timeout > 0)
                Thread.Sleep(timeout);

            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div/section/div[2]/div[2]/div[2]/button").Click();

            var OrderID = new Uri(driver.Url).AbsolutePath.Split("/").ElementAt(4);

            driver.Navigate().GoToUrl("https://www.zara.com/pt/pt/shop/" + OrderID + "/payment/selection");

            if (timeout > 0)
                Thread.Sleep(timeout);

            // selects payment (Mastercard)
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/section/fieldset/div/div[1]/div[2]").Click();

            if (timeout > 0)
                Thread.Sleep(timeout);

            // proceeds
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/div/section/div[2]/div[2]/div[2]/button").Click();

            FillCardInfo(driver, data[2], data[3], data[4], data[5], data[6]);

            if (timeout > 0)
                Thread.Sleep(timeout);

            // proceed x2
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div[2]/form/div[2]/div/section/div[2]/div[2]/div[2]/button").Click();

            if (timeout > 0)
                Thread.Sleep(timeout);

            // comply with terms
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div/form/div[2]/section/div[2]/div[1]/div/div/div/div/label/div/input").Click();

            // authorize payment
            Waits.HoldUntil(driver, xpath: "//*[@id=\"main\"]/article/div[2]/div/form/div[2]/section/div[2]/div[2]/div[2]/button").Click();
        }

        static void Main(string[] args)
        {
            string[] data = System.IO.File.ReadAllLines("./info.txt");
            /*
             0 -> email
             1 -> password
             2 -> card number
             3 -> card expiration month
             4 -> card expiration year
             5 -> card name
             6 -> card cvv2
             */

            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();
            options.AddArgument("--disable-gpu");
            options.AddArgument("--enable-automation");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--disable-blink-features=AutomationControlled");

            var driver = new ChromeDriver(options: options);

            driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            Login(driver, data, 2000);

            Thread.Sleep(2000);

            Checkout(driver, data, 1000);

            driver.Quit();
        }
    }
}