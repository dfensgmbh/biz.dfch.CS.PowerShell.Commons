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

using System.Diagnostics.Contracts;
using System.Management.Automation;
using System.Text;

namespace biz.dfch.CS.PowerShell.Commons
{
    /// <summary>
    /// ErrorRecordFactory
    /// </summary>
    public static class ErrorRecordFactory
    {
        /// <summary>
        /// GetNotFound
        /// </summary>
        /// <param name="messageOrTemplate">A string message or a format string</param>
        /// <param name="errorId">EventId</param>
        /// <param name="objects">zero or more objects, where first object (if available) will be used as targetObject in ErrorRecord</param>
        /// <returns>ErrorRecord</returns>
        public static ErrorRecord GetNotFound(string messageOrTemplate, string errorId, params object[] objects)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Requires(!string.IsNullOrWhiteSpace(errorId));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            var message = new StringBuilder();

            if (null != objects)
            {
                message.AppendFormat(messageOrTemplate, objects);
            }
            else
            {
                message.Append(message);
            }

            var exception = new ItemNotFoundException(message.ToString());
            var errorRecord = new ErrorRecord(exception, errorId.ToString(), ErrorCategory.ObjectNotFound, null != objects ? objects[0] : null);
            return errorRecord;
        }
    }
}
