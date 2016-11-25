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
using System.Diagnostics.Contracts;
using System.Management.Automation;
using biz.dfch.CS.Testing.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace biz.dfch.CS.PowerShell.Commons.Tests
{
    [Cmdlet(VerbsDiagnostic.Test, "DefaultValues")]
    public class ArbitraryCmdlet : PsCmdletBase
    {
        public const int INT_PARAMETER1_DEFAULT_VALUE = 42;
        public const string STRING_PARAMETER1_DEFAULT_VALUE = "arbitrary-string-default-value1";
        public const string STRING_PARAMETER2_DEFAULT_VALUE = "arbitrary-string-default-value2";
        public const bool SWITCH_PARAMETER_DEFAULT_VALUE = true;

        [Parameter(Mandatory = true)]
        public int TestCase { get; set; }

        [Parameter(Mandatory = false)]
        [PSDefaultValue(Value = INT_PARAMETER1_DEFAULT_VALUE)]
        public int IntParameter1 { get; set; }

        [Parameter(Mandatory = false)]
        [PSDefaultValue(Value = STRING_PARAMETER1_DEFAULT_VALUE)]
        public string StringParameter1 { get; set; }

        [Parameter(Mandatory = true)]
        [PSDefaultValue(Value = STRING_PARAMETER2_DEFAULT_VALUE)]
        public string StringParameter2 { get; set; }

        [Parameter(Mandatory = false)]
        [PSDefaultValue(Value = true)]
        public SwitchParameter SwitchParameter { get; set; }

        protected override void ProcessRecord()
        {
            switch (TestCase)
            {
                case 1:
                    Contract.Assert(STRING_PARAMETER1_DEFAULT_VALUE == StringParameter1);
                    break;
                case 2:
                    Contract.Assert(STRING_PARAMETER1_DEFAULT_VALUE != StringParameter1);
                    break;
                case 3:
                    Contract.Assert(SwitchParameter == true);
                    break;
                case 4:
                    Contract.Assert(SwitchParameter == false);
                    break;
                default:
                    var invalidTestCase = false;
                    Contract.Assert(invalidTestCase);
                    break;
            }

            base.ProcessRecord();
        }
    }
        
    [TestClass]
    public class PsCmdletBaseTest
    {
        [TestMethod]
        public void TestCase1()
        {
            var sut = typeof(ArbitraryCmdlet);

            var parameters = "-TestCase 1 -StringParameter2 'tralala';";
            var result = PsCmdletAssert.Invoke(sut, parameters);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestCase2()
        {
            var sut = typeof(ArbitraryCmdlet);

            var parameters = "-TestCase 2 -StringParameter1 'some-string' -StringParameter2 'tralala';";
            var result = PsCmdletAssert.Invoke(sut, parameters);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestCase3()
        {
            var sut = typeof(ArbitraryCmdlet);

            var parameters = "-TestCase 3 -StringParameter2 'tralala';";
            var result = PsCmdletAssert.Invoke(sut, parameters);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            var sut = typeof(ArbitraryCmdlet);

            var parameters = "-TestCase 4 -SwitchParameter:$false -StringParameter2 'tralala';";
            var result = PsCmdletAssert.Invoke(sut, parameters);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
