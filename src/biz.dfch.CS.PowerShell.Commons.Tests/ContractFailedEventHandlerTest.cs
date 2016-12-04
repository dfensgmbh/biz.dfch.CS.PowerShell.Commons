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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.PowerShell.Commons.Tests
{
    [TestClass]
    public class ContractFailedEventHandlerTest
    {

        public class ClassThatThrowsContractExceptions
        {
            public void InnerMostMethod(bool throwsIfFalse)
            {
                Contract.Assert(throwsIfFalse);
            }

            public void InbetweenMethodThatDoesRethrow(bool boolThatIsPassedThrough)
            {
                try
                {
                    InnerMostMethod(boolThatIsPassedThrough);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            public void InbetweenMethodThatDoesNotRethrow(bool boolThatIsPassedThrough)
            {
                try
                {
                    InnerMostMethod(boolThatIsPassedThrough);
                }
                catch (Exception ex)
                {
                    // ignore exception
                }
            }

            public void OuterMostMethodThatDoesRethrow(bool boolThatIsPassedThrough)
            {
                try
                {
                    InbetweenMethodThatDoesRethrow(boolThatIsPassedThrough);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            public void OuterMostMethodThatDoesNotRethrow(bool boolThatIsPassedThrough)
            {
                try
                {
                    InbetweenMethodThatDoesNotRethrow(boolThatIsPassedThrough);
                }
                catch (Exception ex)
                {
                    // ignore exception
                }
            }
        }

        [TestMethod]
        public void AssertThatMultipleCatchBlocksAndReThrowsOnlyGenerateASingleContractFailedEvent()
        {
            _queue = new ConcurrentQueue<string>();

            Contract.ContractFailed += EventHandler;

            try
            {
                var sut = new ClassThatThrowsContractExceptions();

                sut.OuterMostMethodThatDoesRethrow(false);
            }
            catch (Exception)
            {
                // ignore exception
            }
            finally
            {
                Contract.ContractFailed -= EventHandler;
            }

            // regardless of all catch blocks and rethrows we only get a single event handler
            Assert.AreEqual(1, _queue.Count);
        }

        private static ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        internal static void EventHandler(object sender, ContractFailedEventArgs args)
        {
            _queue.Enqueue(args.Message);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "false")]
        public void TestRegisterTraceSourceSucceeds()
        {
            var traceSource = Logger.Get("arbitrary-trace-source");
            ContractFailedEventHandler.RegisterTraceSource(traceSource);

            Contract.Assert(false);
        }
    }
}
