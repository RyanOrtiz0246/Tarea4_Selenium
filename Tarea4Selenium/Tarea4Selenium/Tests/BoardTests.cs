using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;

namespace Tarea4Selenium.Tests
{
    [TestFixture]
    [Order(2)]
    public class BoardTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private const string UserEmail = "ryan142310@gmail.com";
        private const string UserPassword = "HolasoyRyan";
        private const string BoardUrl = "https://trello.com/b/BWCewGuc/pruebas-selenium";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--start-maximized", "--disable-infobars");
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            LoginToTrello();
            driver.Navigate().GoToUrl(BoardUrl);
            WaitForBoardToLoad();
        }

        private void LoginToTrello()
        {
            driver.Navigate().GoToUrl("https://trello.com/login");

            var emailField = wait.Until(d =>
                d.FindElement(By.CssSelector("input[id^='username-uid'][data-testid='username']")));
            emailField.Clear();
            emailField.SendKeys(UserEmail);

            var continueButton = wait.Until(d =>
                d.FindElement(By.CssSelector("button[id='login-submit'][data-testid='login-submit-idf-testid']")));
            continueButton.Click();

            wait.Until(d => d.FindElement(By.CssSelector("input#password[data-testid='password']")).Displayed);

            var passwordField = wait.Until(d =>
                d.FindElement(By.CssSelector("input#password[data-testid='password']")));
            passwordField.SendKeys(UserPassword);

            var loginButton = wait.Until(d =>
                d.FindElement(By.CssSelector("button#login-submit span.css-178ag6o")));
            loginButton.Click();

            wait.Until(d => d.Url.Contains("boards"));
        }

        private void WaitForBoardToLoad()
        {
            wait.Until(d =>
                d.FindElements(By.CssSelector("div[data-testid='list']")).Count > 0 ||
                d.FindElements(By.CssSelector("button[data-testid='list-add-card-button']")).Count > 0);
        }

        [Test]
        public void CRUD_Tarjeta_Completo()
        {
            try
            {
                TakeScreenshot("01_board_loaded");

                // Crear tarjeta
                var addCardBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='list-add-card-button']")));
                addCardBtn.Click();
                TakeScreenshot("02_add_card_click");

                var cardTitle = wait.Until(d =>
                    d.FindElement(By.CssSelector("textarea[data-testid='list-card-composer-textarea']")));
                cardTitle.SendKeys("Tarjeta Automatizada");
                TakeScreenshot("03_card_title_entered");

                var submitBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='list-card-composer-add-card-button']")));
                submitBtn.Click();
                TakeScreenshot("04_card_created");

                // LEER TARJETA
                var newCard = wait.Until(d =>
                    d.FindElement(By.XPath("//a[contains(@class,'list-card') and .//span[text()='Tarjeta Automatizada']]")));
                newCard.Click();
                TakeScreenshot("05_card_opened");

                // ACTUALIZAR TARJETA
                var descEditBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='card-back-description-edit-button']")));
                descEditBtn.Click();

                var descTextarea = wait.Until(d =>
                    d.FindElement(By.CssSelector("textarea[data-testid='card-back-description-textarea']")));
                descTextarea.Clear();
                descTextarea.SendKeys("Descripción editada por Selenium");
                TakeScreenshot("06_description_edited");

                var saveDescBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='card-back-description-save-button']")));
                saveDescBtn.Click();
                TakeScreenshot("07_description_saved");

                // ELIMINAR TARJETA
                var archiveBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='card-back-archive-button']")));
                archiveBtn.Click();
                TakeScreenshot("08_card_archived");

                var deleteBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='card-back-delete-card-button']")));
                deleteBtn.Click();

                var confirmDeleteBtn = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[data-testid='confirm-dialog-confirm-button']")));
                confirmDeleteBtn.Click();
                TakeScreenshot("09_card_deleted");

                Assert.Pass("CRUD completado exitosamente");
            }
            catch (Exception ex)
            {
                TakeScreenshot("99_crud_error");
                Assert.Fail($"Error en CRUD: {ex.Message}");
            }
        }

        [TearDown]
        public void Teardown()
        {
            try { driver.Quit(); } catch { }
        }

        private void TakeScreenshot(string name)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{DateTime.Now:yyyyMMdd_HHmmss}_{name}.png");
                screenshot.SaveAsFile(fileName);
                TestContext.AddTestAttachment(fileName);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error capturando pantalla: {ex.Message}");
            }
        }
    }
}
