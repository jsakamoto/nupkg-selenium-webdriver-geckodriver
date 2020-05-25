using System;

namespace TestDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            // Please keep your IE configuration settings:
            // 1. Check on "Enable Protected Mode" at ALL zones in "Security" tab of Internet Options dialog.
            // 2. Browser zoom level keep to 100%.
            using (var driver = new OpenQA.Selenium.Firefox.FirefoxDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                driver.Navigate().GoToUrl("https://www.bing.com/");
                driver.FindElementById("sb_form_q").SendKeys("Selenium WebDriver");
                driver.FindElementByClassName("search").Click();

                Console.WriteLine("OK");
                Console.ReadKey(intercept: true);
            }
        }
    }
}
