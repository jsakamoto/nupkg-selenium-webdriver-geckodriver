using OpenQA.Selenium;

// NOTE: This is a workaround for the case that Firefox is installed via "Snap" on Linux.
Environment.SetEnvironmentVariable("TMPDIR", AppDomain.CurrentDomain.BaseDirectory);

using var driver = new OpenQA.Selenium.Firefox.FirefoxDriver(AppDomain.CurrentDomain.BaseDirectory);

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
driver.Navigate().GoToUrl("https://www.nuget.org/");
await Task.Delay(1000);

driver.FindElement(By.Id("search")).SendKeys("Selenium WebDriver");
driver.FindElement(By.Id("search")).SendKeys(Keys.Enter);

Console.WriteLine("OK");
Console.ReadKey(intercept: true);
