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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.PowerShell.Commons.Converters;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.PowerShell.Commons.Tests.Converters
{
    [TestClass]
    public class PsCredentialTypeConverterTest
    {
        public static readonly string DELIM = PsCredentialTypeConverter.DELIMITER.ToString();

        [TestMethod]
        public void ConvertingNullStringReturnsNull()
        {
            var value = "    ";
            var result = PsCredentialTypeConverter.ConvertToPsCredential(value);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertingInvalidStringThrowsContractException()
        {
            var value = "username password with missing comma separator";
            PsCredentialTypeConverter.ConvertToPsCredential(value);
        }

        [TestMethod]
        //[ExpectContractFailure(MessagePattern = "Precondition.+value")]
        public void ConvertingInvalidStringWithEmptyUsernameThrowsContractException()
        {
            var value = DELIM + "password";
            PsCredentialTypeConverter.ConvertToPsCredential(value);
        }

        [TestMethod]
        //[ExpectContractFailure(MessagePattern = "Precondition.+value")]
        public void ConvertingInvalidStringWithEmptyPasswordThrowsContractException()
        {
            var value = "username" + DELIM;
            PsCredentialTypeConverter.ConvertToPsCredential(value);
        }

        [TestMethod]
        public void ConvertingStringSucceeds()
        {
            var username = "username";
            var password = "password";
            var value = username + DELIM + password;

            var result = PsCredentialTypeConverter.ConvertToPsCredential(value);

            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.UserName);
            Assert.AreEqual(password, result.GetNetworkCredential().Password);
        }

        [TestMethod]
        public void ConvertingStringWithMultipleSeparatorsInPasswordSucceeds()
        {
            var username = "username";
            var password = "password" + DELIM + "that" + DELIM + "consists" + DELIM + "of" + DELIM + "multiple" + DELIM + "characters";
            var value = username + DELIM + password;
            var result = PsCredentialTypeConverter.ConvertToPsCredential(value);

            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.UserName);
            Assert.AreEqual(password, result.GetNetworkCredential().Password);
        }
    }
}
