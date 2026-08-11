using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BridgeCourse.Week3.Vault;
using BridgeCourse.Week3.Api.Controllers;
using BridgeCourse.Week3.Api.Services;
using Xunit;

namespace BridgeCourse.Week3.Tests
{
    public class VaultAndApiTests
    {
        [Fact]
        public void Pbkdf2_Derivation_IsDeterministic()
        {
            string password = "TestPassword123!";
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            byte[] key1 = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 32);
            byte[] key2 = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 32);

            Assert.Equal(key1, key2);
        }

        [Fact]
        public void AesGcm_RoundTrip_Succeeds()
        {
            byte[] key = new byte[32];
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(nonce);

            string plaintext = "Secret message payload.";
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];

            using (var aesGcm = new AesGcm(key, 16))
            {
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            byte[] decrypted = new byte[ciphertext.Length];
            using (var aesGcm = new AesGcm(key, 16))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, decrypted);
            }

            Assert.Equal(plaintext, Encoding.UTF8.GetString(decrypted));
        }

        [Fact]
        public void AesGcm_TamperedCiphertext_ThrowsCryptographicException()
        {
            byte[] key = new byte[32];
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(nonce);

            byte[] plaintext = Encoding.UTF8.GetBytes("Data");
            byte[] ciphertext = new byte[plaintext.Length];

            using (var aesGcm = new AesGcm(key, 16))
            {
                aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);
            }

            ciphertext[0] ^= 0x01; // Tamper

            byte[] decrypted = new byte[ciphertext.Length];
            Assert.ThrowsAny<CryptographicException>(() =>
            {
                using (var aesGcm = new AesGcm(key, 16))
                {
                    aesGcm.Decrypt(nonce, ciphertext, tag, decrypted);
                }
            });
        }

        [Fact]
        public void PasswordHasher_HashAndVerify_Succeeds()
        {
            string password = "MyUserPassword!";
            var (hash, salt) = PasswordHasher.HashPassword(password);

            bool verified = PasswordHasher.VerifyPassword(password, hash, salt);
            bool failed = PasswordHasher.VerifyPassword("WrongPassword", hash, salt);

            Assert.True(verified);
            Assert.False(failed);
        }

        [Fact]
        public void FixedTimeEquals_ReturnsExpectedResults()
        {
            byte[] a = Encoding.UTF8.GetBytes("HashSig123");
            byte[] b = Encoding.UTF8.GetBytes("HashSig123");
            byte[] c = Encoding.UTF8.GetBytes("Different");

            Assert.True(CryptographicOperations.FixedTimeEquals(a, b));
            Assert.False(CryptographicOperations.FixedTimeEquals(a, c));
        }

        [Fact]
        public void LargeFileStreaming_Gcm_RoundTrip_Succeeds()
        {
            string source = "test_src.dat";
            string encrypted = "test_enc.dat";
            string decrypted = "test_dec.dat";

            Day4Tasks.GenerateLargeFile(source, 2); // 2 MB test file

            byte[] key = new byte[32];
            RandomNumberGenerator.Fill(key);

            Day4Tasks.EncryptFileGcm(source, encrypted, key);
            Day4Tasks.DecryptFileGcm(encrypted, decrypted, key);

            bool match = Day4Tasks.CompareFiles(source, decrypted);
            Assert.True(match);

            Day4Tasks.CleanUpFiles(source, encrypted, decrypted);
        }

        [Fact]
        public void AuthController_ValidCredentials_ReturnsToken()
        {
            var controller = new AuthController();
            var request = new LoginRequest { Username = "teacher", Password = "teacher123" };

            var result = controller.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void AuthController_InvalidCredentials_ReturnsUnauthorized()
        {
            var controller = new AuthController();
            var request = new LoginRequest { Username = "teacher", Password = "WrongPassword" };

            var result = controller.Login(request);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void SecureController_Endpoints_HaveCorrectRoles()
        {
            var type = typeof(SecureController);

            var teacherMethod = type.GetMethod(nameof(SecureController.GetTeacherData));
            var teacherAuthAttr = teacherMethod?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(teacherAuthAttr);
            Assert.Equal("Teacher", teacherAuthAttr.Roles);

            var studentMethod = type.GetMethod(nameof(SecureController.GetStudentData));
            var studentAuthAttr = studentMethod?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(studentAuthAttr);
            Assert.Equal("Student", studentAuthAttr.Roles);
        }
    }
}
