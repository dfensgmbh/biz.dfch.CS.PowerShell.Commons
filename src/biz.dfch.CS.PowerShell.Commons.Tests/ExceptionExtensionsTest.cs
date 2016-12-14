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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.PowerShell.Commons;

namespace biz.dfch.CS.PowerShell.Commons.Tests
{
    [TestClass]
    public class ExceptionExtensionsTest
    {
        [TestMethod]
        public void GetErrorRecordFromExceptionSucceeds()
        {
            var message = "arbitraryMessage";
            var sut = new Exception(message);

            var result = sut.GetGeneric();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(Exception));
            Assert.AreEqual(message, result.Exception.Message);
        }

        [TestMethod]
        public void GetErrorRecordFromExceptionWithParamsSucceeds()
        {
            var param1 = "param1";
            var param2 = 42L;
            var messageTemplate = "arbitraryMessage param1 '{0}' param2 '{1}'";

            var message = "arbitraryMessage";
            var sut = new Exception(message);

            var result = sut.GetGeneric(messageTemplate, param1, param2);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(Exception));
            Assert.AreEqual(message, result.Exception.Message);
        }

        [TestMethod]
        public void GetErrorRecordFromArgumentExceptionSucceeds()
        {
            var message = "arbitraryMessage";
            var sut = new ArgumentException(message);

            var result = sut.GetGeneric();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);
            Assert.IsInstanceOfType(result.Exception, typeof(Exception));
            Assert.AreEqual(message, result.Exception.Message);
        }

    }
}
