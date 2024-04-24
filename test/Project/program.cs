using OpenQA.Selenium;

// NOTE: This is a workaround for the case that Firefox is installed via "Snap" on Linux.
Environment.SetEnvironmentVariable("TMPDIR", AppDomain.CurrentDomain.BaseDirectory);

using var driver = new OpenQA.Selenium.Firefox.FirefoxDriver(AppDomain.CurrentDomain.BaseDirectory);

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
driver.Navigate().GoToUrl("https://www.bing.com/");
driver.FindElement(By.Id("sb_form_q")).SendKeys("Selenium WebDriver");
await Task.Delay(1000);
driver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Enter);

Console.WriteLine("OK");
Console.ReadKey(intercept: true);
