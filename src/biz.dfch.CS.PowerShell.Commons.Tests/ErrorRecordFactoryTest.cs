/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.PowerShell.Commons.Tests
{
    [TestClass]
    public class ErrorRecordFactoryTest
    {
        [TestMethod]
        [ExpectContractFailure(MessagePattern = "messageOrTemplate")]
        public void GetNotFoundWithEmptyMessageThrowsContractException()
        {
            var messageOrTemplate = string.Empty;
            var errorId = string.Empty;
            var objects = new object[] { };

            ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
        }

        [TestMethod]
        [ExpectException(ExceptionType = typeof(FormatException), MessagePattern = "Index")]
        public void GetNotFoundWithInvalidFormtStringThrowsContractException()
        {
            var messageOrTemplate = "arbitrary-message-'{0}'";
            var errorId = "arbitrary-errorid";
            var objects = new object[] { };

            ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "errorId")]
        public void GetNotFoundWithEmptyErrorIdThrowsContractException()
        {
            var messageOrTemplate = "arbitrary-message";
            var errorId = string.Empty;
            var objects = new object[] { };

            ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
        }

        [TestMethod]
        public void GetNotFound0Succeeds()
        {
            var messageOrTemplate = "arbitrary-message";
            var errorId = "arbitrary-errorid";
            var objects = new object[] { "tralala" };

            var result = ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FullyQualifiedErrorId.StartsWith(errorId));
            Assert.AreEqual(messageOrTemplate, result.Exception.Message);
            Assert.IsFalse(result.Exception.Message.Contains(objects[0].ToString()));
            Assert.AreEqual(messageOrTemplate, result.Exception.Message);
            Assert.AreEqual(objects[0].ToString(), result.TargetObject.ToString());
        }

        [TestMethod]
        public void GetNotFound1Succeeds()
        {
            var messageOrTemplateStart = "arbitrary-message-";
            var messageOrTemplate = messageOrTemplateStart + "'{0}'";
            var errorId = "arbitrary-errorid";
            var objects = new object[] { "tralala" };

            var result = ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FullyQualifiedErrorId.StartsWith(errorId));
            Assert.AreNotEqual(messageOrTemplate, result.Exception.Message);
            Assert.IsTrue(result.Exception.Message.Contains(objects[0].ToString()));
            Assert.IsTrue(result.Exception.Message.StartsWith(messageOrTemplateStart));
        }

        [TestMethod]
        public void GetNotFound2Succeeds()
        {
            var messageOrTemplateStart = "arbitrary-message-";
            var messageOrTemplate = messageOrTemplateStart + "'{0}'";
            var errorId = "arbitrary-errorid";
            var objects = new object[] { "tralala" , "rosebud" };

            var result = ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FullyQualifiedErrorId.StartsWith(errorId));
            Assert.AreNotEqual(messageOrTemplate, result.Exception.Message);
            Assert.IsTrue(result.Exception.Message.Contains(objects[0].ToString()));
            Assert.IsFalse(result.Exception.Message.Contains(objects[1].ToString()));
            Assert.IsTrue(result.Exception.Message.StartsWith(messageOrTemplateStart));
        }

        [TestMethod]
        public void GetNotFound3Succeeds()
        {
            var messageOrTemplateStart = "arbitrary-message-";
            var messageOrTemplate = messageOrTemplateStart + "'{0}' '{1}'";
            var errorId = "arbitrary-errorid";
            var objects = new object[] { "tralala" , "rosebud" };

            var result = ErrorRecordFactory.GetNotFound(messageOrTemplate, errorId, objects);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FullyQualifiedErrorId.StartsWith(errorId));
            Assert.AreNotEqual(messageOrTemplate, result.Exception.Message);
            Assert.IsTrue(result.Exception.Message.Contains(objects[0].ToString()));
            Assert.IsTrue(result.Exception.Message.Contains(objects[1].ToString()));
            Assert.IsTrue(result.Exception.Message.StartsWith(messageOrTemplateStart));
        }

    }
}
