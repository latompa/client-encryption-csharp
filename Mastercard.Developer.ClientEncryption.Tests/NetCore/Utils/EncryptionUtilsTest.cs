﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Mastercard.Developer.ClientEncryption.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.ClientEncryption.Tests.NetCore.Utils
{
    [TestClass]
    public class EncryptionUtilsTest
    {
        [TestMethod]
        public void TestLoadEncryptionCertificate_ShouldSupportDer()
        {
            // GIVEN
            const string certificatePath = "./_Resources/Certificates/test_certificate-2048.der";

            // WHEN
            var certificate = EncryptionUtils.LoadEncryptionCertificate(certificatePath);

            // THEN
            Assert.IsNotNull(certificate);
            Assert.IsNotNull(certificate.GetPublicKey());
            Assert.AreEqual("X509", certificate.GetFormat());
        }

        [TestMethod]
        public void TestLoadEncryptionCertificate_ShouldSupportPem()
        {
            // GIVEN
            const string certificatePath = "./_Resources/Certificates/test_certificate-2048.pem";

            // WHEN
            var certificate = EncryptionUtils.LoadEncryptionCertificate(certificatePath);

            // THEN
            Assert.IsNotNull(certificate);
            Assert.IsNotNull(certificate.GetPublicKey());
            Assert.AreEqual("X509", certificate.GetFormat());
        }

        [TestMethod]
        public void TestLoadDecryptionKey_ShouldSupportUnencryptedKeyFile()
        {
            // GIVEN
            const string keyPath = "./_Resources/Keys/Pkcs8/test_key_pkcs8-2048.der";

            // WHEN
            var privateKey = EncryptionUtils.LoadDecryptionKey(keyPath);

            // THEN
            Assert.IsNotNull(privateKey);
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.IsTrue(privateKey.KeyExchangeAlgorithm != null && privateKey.KeyExchangeAlgorithm.StartsWith("RSA"));
        }

        [TestMethod]
        public void TestLoadDecryptionKey_ShouldSupportPkcs12()
        {
            // GIVEN
            const string keyContainerPath = "./_Resources/Keys/Pkcs12/test_key.p12";
            const string keyAlias = "mykeyalias";
            const string keyPassword = "Password1";

            // WHEN
            const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable; // https://github.com/dotnet/corefx/issues/14745
            var privateKey = EncryptionUtils.LoadDecryptionKey(keyContainerPath, keyAlias, keyPassword, flags);

            // THEN
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.IsTrue(privateKey.KeyExchangeAlgorithm != null && privateKey.KeyExchangeAlgorithm.StartsWith("RSA"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))] // THEN
        public void TestLoadDecryptionKey_ShouldThrowArgumentException_WhenInvalidKey()
        {
            // GIVEN
            const string keyPath = "./_Resources/Keys/Pkcs8/test_invalid_key.der";

            // WHEN
            EncryptionUtils.LoadDecryptionKey(keyPath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))] // THEN
        public void TestLoadDecryptionKey_ShouldThrowFileNotFoundException_WhenKeyFileDoesNotExist()
        {
            // GIVEN
            const string keyPath = "./_Resources/Keys/some_file";

            // WHEN
            EncryptionUtils.LoadDecryptionKey(keyPath);
        }
    }
}
