﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using DSharpPlus;

namespace DiscordBot
{
    public class PageChecker
    {
        private readonly DebugLogger debugLogger;
        private readonly Config config;
        public PageChecker(DebugLogger debugLogger, Config config)
        {
            this.debugLogger = debugLogger;
            this.config = config;
        }
        
        public bool Check(string type)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.124 Safari/537.36 Vivaldi/3.3.2022.47";
            string lastUserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 85.0.4183.106 Safari / 537.36";
            //chromeOptions.AddArguments(new string[] {
            //    "start-maximized",
            //    "window-size=1920,1080",
            //    "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.106 Safari/537.36"
            //});
            chromeOptions.AddArguments(new string[] {
                "headless",
                "window-size=1920,1080",
                $"user-agent={userAgent}"
            });
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            chromeOptions.AddExcludedArguments(new List<string>() { "enable-automation" });
            ChromeDriver driver = null;
            bool result = false;
            try
            {
                driver = new ChromeDriver(chromeDriverService, chromeOptions);
                switch (type)
                {
                    case "evga":
                        result = evga(driver);
                        break;
                    case "amazon":
                        result = amazon(driver);
                        break;
                    case "newegg":
                        result = newegg(driver);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                debugLogger.LogMessage(DSharpPlus.LogLevel.Critical, $"PageChecker:Check() WebPage {type} ", e.Message, DateTime.Now);
            }
            if(driver != null)
            {
                try
                {
                    driver.Close();
                    driver.Quit();
                    driver.Dispose();
                }
                catch (Exception e)
                {
                    debugLogger.LogMessage(DSharpPlus.LogLevel.Critical, $"PageChecker:Check() CleanUp {type}", e.Message, DateTime.Now);
                }
            }
            return result;
        }

        private bool evga(ChromeDriver driver)
        {
            //driver.Url = "https://www.evga.com/products/product.aspx?pn=10G-P5-3897-KR";
            driver.Url = config.Urls["evga"];
            string titleXPath = "//span[text()='EVGA GeForce RTX 3080 FTW3 ULTRA GAMING, 10G-P5-3897-KR, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate']";
            new WebDriverWait(driver, TimeSpan.FromMinutes(1)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            IWebElement outOfStockButton = driver.FindElement(By.XPath("//div[@id='LFrame_pnlOutOfStock']/p"));
            IWebElement autoNotifyButton = driver.FindElement(By.ClassName("btnBigAutoNotify"));
            if (outOfStockButton.Displayed && autoNotifyButton.Displayed) return false;
            else return true;
        }

        private bool amazon(ChromeDriver driver)
        {
            //driver.Url = "https://www.amazon.com/EVGA-10G-P5-3897-KR-GeForce-Technology-Backplate/dp/B08HR3Y5GQ";
            driver.Url = config.Urls["amazon"];
            string titleXPath = "//div[@id='titleSection']/h1[@id='title']/span[contains(text(),'EVGA 10G-P5-3897-KR GeForce RTX 3080 FTW3 ULTRA GAMING, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate')]";
            driver.GetScreenshot().SaveAsFile("SS.png");
            new WebDriverWait(driver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            IWebElement currentlyUnavailable = driver.FindElement(By.XPath("//div[@id='availability_feature_div']/div[@id='availability']/span[contains(text(),'Currently unavailable.')]"));
            if (currentlyUnavailable.Displayed) return false;
            else return true;
        }

        private bool newegg(ChromeDriver driver)
        {
            //driver.Url = "https://www.newegg.com/evga-geforce-rtx-3080-10g-p5-3897-kr/p/N82E16814487518";
            driver.Url = config.Urls["newegg"];
            string titleXPath = "//span[contains(text(),'EVGA GeForce RTX 3080 FTW3 ULTRA GAMING Video Card, 10G-P5-3897-KR, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate')]";
            new WebDriverWait(driver, TimeSpan.FromMinutes(1)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            //IWebElement comingSoon = driver.FindElement(By.XPath("//div[@class='aside']//span[contains(text(),'COMING SOON')]"));
            //IWebElement preRelease = driver.FindElement(By.XPath("//div[@class='grpNote']//span[contains(text(),'PRE-RELEASE.')]"));
            //IWebElement outOfStock = driver.FindElement(By.XPath("//div[@class='aside']/div[@id='landingpage-topshipping']/div/div/span[contains(text(),'OUT OF STOCK')]"));
            IWebElement outOfStock2 = driver.FindElement(By.XPath("//div[@class='grpLayout']/div[@class='grpNote']//span[contains(text(),'OUT OF STOCK')]"));
            //if (comingSoon.Displayed && preRelease.Displayed) return false;
            if (outOfStock2.Displayed) return false;
            else return true;
        }

        public Dictionary<string, bool> CheckAll()
        {
            CleanOldProcesses();
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            //sites.ForEach(site => results.Add(site, Check(site)));
            Parallel.ForEach(config.ValidChoices, site => results.Add(site, Check(site)));
            //results.Add("test", true);
            return results;
        }

        private void CleanOldProcesses()
        {
            foreach (var process in Process.GetProcessesByName("chrome")) process.Kill();
            foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill();
        }
    }
}
