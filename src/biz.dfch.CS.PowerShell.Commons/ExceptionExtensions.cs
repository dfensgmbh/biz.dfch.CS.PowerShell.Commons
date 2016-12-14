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

namespace biz.dfch.CS.PowerShell.Commons
{
    public static class ExceptionExtensions
    {
        public static ErrorRecord GetGeneric(this Exception exception)
        {
            Contract.Requires(null != exception);
            Contract.Ensures(null != Contract.Result<ErrorRecord>());
            
            return ErrorRecordFactory.GetGeneric(exception, exception.Message, null);
        }

        public static ErrorRecord GetGeneric(this Exception exception, string messageOrTemplate)
        {
            Contract.Requires(null != exception);
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            return ErrorRecordFactory.GetGeneric(exception, messageOrTemplate: exception.Message, objects: null);
        }

        public static ErrorRecord GetGeneric(this Exception exception, string messageOrTemplate, params object[] objects)
        {
            Contract.Requires(null != exception);
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());
            
            return ErrorRecordFactory.GetGeneric(exception, messageOrTemplate: exception.Message, objects: objects);
        }
    }
}
