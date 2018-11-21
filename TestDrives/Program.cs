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
            //using (var driver = new OpenQA.Selenium.Chrome.ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://www.bing.com/");
                driver.FindElementById("sb_form_q").SendKeys("Selenium WebDriver");
                driver.FindElementById("sb_form_go").Click();

                Console.WriteLine("OK");
                Console.ReadKey(intercept: true);
            }
        }
    }
}
