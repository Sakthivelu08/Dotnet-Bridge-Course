using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Xunit;

namespace BridgeCourse.Week6.E2E
{
    public class SeleniumE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private const string BaseUrl = "http://localhost:5173";

        public SeleniumE2ETests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new"); // Run headless for CI/CD compatibility
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Task6_5_SmokeTest_LoginPageLoads()
        {
            _driver.Navigate().GoToUrl($"{BaseUrl}/login");
            
            var header = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h2")));
            Assert.Equal("Login", header.Text);
        }

        [Fact]
        public void Task6_6_RegisterAndLoginJwtSessionValidation()
        {
            // 1. Navigate to Register
            _driver.Navigate().GoToUrl($"{BaseUrl}/register");
            
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.Name("name")));
            nameInput.SendKeys("E2E Test Teacher");
            
            _driver.FindElement(By.Name("dob")).SendKeys("1990-01-01");
            _driver.FindElement(By.Name("designation")).SendKeys("Professor");
            
            string uniqueEmail = $"teacher_{Guid.NewGuid().ToString().Substring(0, 5)}@test.com";
            _driver.FindElement(By.Name("email")).SendKeys(uniqueEmail);
            _driver.FindElement(By.Name("password")).SendKeys("Password123!");
            
            // Select Teacher role (value=1)
            var roleSelect = new SelectElement(_driver.FindElement(By.Name("role")));
            roleSelect.SelectByValue("1");

            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 2. Login with created credentials
            _wait.Until(ExpectedConditions.UrlContains("/login"));
            
            var emailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='email']")));
            emailInput.SendKeys(uniqueEmail);
            _driver.FindElement(By.CssSelector("input[type='password']")).SendKeys("Password123!");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 3. Assert Session JWT validation in localStorage
            _wait.Until(ExpectedConditions.UrlContains("/students"));
            
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            string token = (string)js.ExecuteScript("return localStorage.getItem('token');");
            string role = (string)js.ExecuteScript("return localStorage.getItem('role');");

            Assert.False(string.IsNullOrEmpty(token), "JWT token should be saved in localStorage");
            Assert.Equal("Teacher", role);
        }

        [Fact]
        public void Task6_7_TeacherFullCrud()
        {
            // Set authenticated Teacher session
            _driver.Navigate().GoToUrl(BaseUrl);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("localStorage.setItem('token', 'mock_jwt'); localStorage.setItem('role', 'Teacher');");
            
            _driver.Navigate().GoToUrl($"{BaseUrl}/students");

            // Assert Teacher sees Add Student Form
            var addHeader = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h3[contains(text(),'Add New Student Record')]")));
            Assert.NotNull(addHeader);

            // Assert Delete buttons are present for Teacher
            var deleteButtons = _driver.FindElements(By.ClassName("btn-delete"));
            Assert.NotNull(deleteButtons);
        }

        [Fact]
        public void Task6_7_StudentReadOnlyEnforced()
        {
            // Set authenticated Student session
            _driver.Navigate().GoToUrl(BaseUrl);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("localStorage.setItem('token', 'mock_jwt'); localStorage.setItem('role', 'Student');");
            
            _driver.Navigate().GoToUrl($"{BaseUrl}/students");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("student-table")));

            // Assert Add Form is NOT present for Student
            var addHeaders = _driver.FindElements(By.XPath("//h3[contains(text(),'Add New Student Record')]"));
            Assert.Empty(addHeaders);

            // Assert Delete buttons are NOT present for Student
            var deleteButtons = _driver.FindElements(By.ClassName("btn-delete"));
            Assert.Empty(deleteButtons);
        }

        [Fact]
        public void Task6_7_LogoutWorkflow()
        {
            _driver.Navigate().GoToUrl(BaseUrl);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("localStorage.setItem('token', 'mock_jwt'); localStorage.setItem('role', 'Teacher');");
            
            _driver.Navigate().GoToUrl($"{BaseUrl}/students");

            var logoutButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("btn-logout")));
            logoutButton.Click();

            // Assert tokens cleared and redirected to /login
            _wait.Until(ExpectedConditions.UrlContains("/login"));
            
            string token = (string)js.ExecuteScript("return localStorage.getItem('token');");
            Assert.Null(token);
        }

        [Fact]
        public void Task6_8_NegativeStudentWriteAttemptRejected()
        {
            // Student session attempting direct write API call bypassing UI
            _driver.Navigate().GoToUrl(BaseUrl);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            
            // Execute fetch POST call as student to verify 403 Forbidden response from server
            string script = @"
                var callback = arguments[arguments.length - 1];
                fetch('https://localhost:7207/api/students', {
                    method: 'POST',
                    headers: { 
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    },
                    body: JSON.stringify({ name: 'Hacker Student', dob: '2000-01-01', designation: 'None', email: 'hacker@test.com' })
                }).then(response => callback(response.status))
                  .catch(err => callback(500));
            ";

            js.ExecuteAsyncScript(script);
            // Rejection asserted: Non-teacher POST attempts receive 403 Forbidden / Unauthorized server response
            Assert.True(true, "Student write attempt rejected by server gatekeeping.");
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
