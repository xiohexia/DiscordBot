﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace DiscordBot
{
    static class PageChecker
    {
        static public bool Check(string type)
        {
            bool result = false;
            try
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriverService.SuppressInitialDiagnosticInformation = true;
                //chromeOptions.AddArgument("start-maximized");
                chromeOptions.AddArgument("headless");
                chromeOptions.AddArgument("no-sandbox");
                chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                ChromeDriver driver = new ChromeDriver(chromeDriverService, chromeOptions);
                
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
                driver.Close();
                driver.Quit();
                driver.Dispose();
            }
            catch (Exception e)
            {
                Program.discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Critical, $"PageChecker:Check() {type}", e.Message, DateTime.Now);
            }
            return result;
        }

        static private bool evga(ChromeDriver driver)
        {
            //driver.Url = "https://www.evga.com/products/product.aspx?pn=10G-P5-3897-KR";
            driver.Url = Program.Urls["evga"];
            string titleXPath = "//span[text()='EVGA GeForce RTX 3080 FTW3 ULTRA GAMING, 10G-P5-3897-KR, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate']";
            new WebDriverWait(driver, TimeSpan.FromMinutes(1)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            IWebElement outOfStockButton = driver.FindElement(By.XPath("//div[@id='LFrame_pnlOutOfStock']/p"));
            IWebElement autoNotifyButton = driver.FindElement(By.ClassName("btnBigAutoNotify"));
            if (outOfStockButton.Displayed && autoNotifyButton.Displayed) return false;
            else return true;
        }

        static private bool amazon(ChromeDriver driver)
        {
            //driver.Url = "https://www.amazon.com/EVGA-10G-P5-3897-KR-GeForce-Technology-Backplate/dp/B08HR3Y5GQ";
            driver.Url = Program.Urls["amazon"];
            string titleXPath = "//div[@id='titleSection']/h1[@id='title']/span[contains(text(),'EVGA 10G-P5-3897-KR GeForce RTX 3080 FTW3 ULTRA GAMING, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate')]";
            new WebDriverWait(driver, TimeSpan.FromMinutes(1)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            IWebElement currentlyUnavailable = driver.FindElement(By.XPath("//div[@id='availability_feature_div']/div[@id='availability']/span[contains(text(),'Currently unavailable.')]"));
            if (currentlyUnavailable.Displayed) return false;
            else return true;
        }

        static private bool newegg(ChromeDriver driver)
        {
            //driver.Url = "https://www.newegg.com/evga-geforce-rtx-3080-10g-p5-3897-kr/p/N82E16814487518";
            driver.Url = Program.Urls["newegg"];
            string titleXPath = "//span[contains(text(),'EVGA GeForce RTX 3080 FTW3 ULTRA GAMING Video Card, 10G-P5-3897-KR, 10GB GDDR6X, iCX3 Technology, ARGB LED, Metal Backplate')]";
            new WebDriverWait(driver, TimeSpan.FromMinutes(1)).Until(ExpectedConditions.ElementIsVisible(By.XPath(titleXPath)));
            //IWebElement comingSoon = driver.FindElement(By.XPath("//div[@class='aside']//span[contains(text(),'COMING SOON')]"));
            //IWebElement preRelease = driver.FindElement(By.XPath("//div[@class='grpNote']//span[contains(text(),'PRE-RELEASE.')]"));
            IWebElement outOfStock = driver.FindElement(By.XPath("//div[@class='aside']/div[@id='landingpage-topshipping']/div/div/span[contains(text(),'OUT OF STOCK')]"));
            IWebElement outOfStock2 = driver.FindElement(By.XPath("//div[@class='grpLayout']/div[@class='grpNote']//span[contains(text(),'OUT OF STOCK')]"));
            //if (comingSoon.Displayed && preRelease.Displayed) return false;
            if (outOfStock.Displayed && outOfStock2.Displayed) return false;
            else return true;
        }

        static public Dictionary<string, bool> CheckAll()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            //sites.ForEach(site => results.Add(site, Check(site)));
            Parallel.ForEach(Program.ValidChoices, site => results.Add(site, Check(site)));
            //results.Add("test", true);
            return results;
        }
    }
}
