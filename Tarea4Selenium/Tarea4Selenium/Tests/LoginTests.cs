using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Threading;

namespace Tarea4Selenium.Tests
{
    [TestFixture]
    [Order(1)]
    public class LoginTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private const string UserEmail = "ryan142310@gmail.com";
        private const string UserPassword = "HolasoyRyan";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();

            // Configuración avanzada anti-detección
            options.AddArguments(
                "--disable-blink-features=AutomationControlled",
                "--disable-infobars",
                "--start-maximized",
                "--no-sandbox",
                "--disable-dev-shm-usage"
            );

            // Ocultar completamente la automatización
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void Login_Exitoso()
        {
            try
            {
                // 1. Navegar a login
                driver.Navigate().GoToUrl("https://trello.com/login");
                TakeScreenshot("01_login_page");

                // 2. Localizar e ingresar email (selector actualizado)
                var emailField = wait.Until(d =>
                    d.FindElement(By.CssSelector("input[id^='username-uid'][data-testid='username']")));
                emailField.Clear();
                emailField.SendKeys(UserEmail);
                TakeScreenshot("02_email_entered");

                // 3. Click en "Continuar" (selector robusto)
                var continueButton = wait.Until(d =>
                    d.FindElement(By.CssSelector("button[id='login-submit'][data-testid='login-submit-idf-testid']")));
                continueButton.Click();
                TakeScreenshot("03_continue_clicked");

                // 4. Esperar transición (máximo 15 segundos)
                wait.Until(d =>
                    d.FindElement(By.CssSelector("input#password")).Displayed);
                TakeScreenshot("04_password_page_loaded");

                // 5. Ingresar contraseña (selector exacto)
                var passwordField = wait.Until(d =>
                    d.FindElement(By.CssSelector("input#password[data-testid='password']")));
                passwordField.SendKeys(UserPassword);
                TakeScreenshot("05_password_entered");

                // 6. Click en "Iniciar sesión" (selector específico)
                var loginButton = wait.Until(d =>
                    d.FindElement(By.CssSelector("button#login-submit span.css-178ag6o")));
                loginButton.Click();
                TakeScreenshot("06_login_clicked");

                // 7. Verificación robusta
                var successCondition = wait.Until(d =>
                    d.Url.Contains("boards") ||
                    d.FindElements(By.CssSelector("div[data-testid='board-view']")).Count > 0);
                TakeScreenshot("07_login_success");

                Thread.Sleep(5000);

                Assert.That(successCondition, Is.True, "No se completó el login correctamente");
            }
            catch (Exception ex)
            {
                TakeScreenshot("99_error_final");
                Assert.Fail($"Error en login: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [TearDown]
        public void Teardown()
        {
            try 
            { 
                driver.Quit(); 
            } 
            catch { /* Ignorar errores al cerrar */ }
        }

        private void TakeScreenshot(string name)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var fileName = Path.Combine(
                    TestContext.CurrentContext.TestDirectory, 
                    $"{DateTime.Now:yyyyMMdd_HHmmss}_{name}.png");
                screenshot.SaveAsFile(fileName);
                TestContext.AddTestAttachment(fileName);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error al tomar captura: {ex.Message}");
            }
        }
    }
}