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
using System.Text;

namespace biz.dfch.CS.PowerShell.Commons
{
    /// <summary>
    /// ErrorRecordFactory
    /// </summary>
    public static class ErrorRecordFactory
    {
        public static ErrorRecord GetGeneric(Exception exception)
        {
            Contract.Requires(null != exception);
            Contract.Ensures(null != Contract.Result<ErrorRecord>());
            
            return GetGeneric(exception, exception.Message, null);
        }

        public static ErrorRecord GetGeneric(Exception exception, string messageOrTemplate)
        {
            Contract.Requires(null != exception);
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            return GetGeneric(exception, messageOrTemplate: exception.Message, objects: null);
        }

        public static ErrorRecord GetGeneric(Exception exception, string messageOrTemplate, params object[] objects)
        {
            Contract.Requires(null != exception);
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
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

            var errorRecord = new ErrorRecord(exception, ErrorCategory.NotSpecified.ToString(), ErrorCategory.NotSpecified, null != objects ? objects[0] : null);
            return errorRecord;
        }

        public static ErrorRecord GetGeneric(string messageOrTemplate)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            return GetGeneric(messageOrTemplate, errorId: ErrorCategory.NotSpecified.ToString(), errorCategory: ErrorCategory.NotSpecified, objects: null);
        }

        public static ErrorRecord GetGeneric(string messageOrTemplate, params object[] objects)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            return GetGeneric(messageOrTemplate, errorId: ErrorCategory.NotSpecified.ToString(), errorCategory: ErrorCategory.NotSpecified, objects: objects);
        }

        public static ErrorRecord GetGeneric(string messageOrTemplate, string errorId, params object[] objects)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Requires(!string.IsNullOrWhiteSpace(errorId));
            Contract.Ensures(null != Contract.Result<ErrorRecord>());

            return GetGeneric(messageOrTemplate, errorId: errorId, errorCategory: ErrorCategory.NotSpecified, objects: objects);
        }

        public static ErrorRecord GetGeneric(string messageOrTemplate, string errorId, ErrorCategory errorCategory, params object[] objects)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageOrTemplate));
            Contract.Requires(!string.IsNullOrWhiteSpace(errorId));
            Contract.Requires(null != errorCategory);
            Contract.Ensures(null != Contract.Result<ErrorRecord>());
            // must be Assert as we do not have constant values
            Contract.Assert(Enum.IsDefined(typeof(ErrorCategory), errorCategory), errorCategory.ToString());

            var message = new StringBuilder();

            if (null != objects)
            {
                message.AppendFormat(messageOrTemplate, objects);
            }
            else
            {
                message.Append(message);
            }

            var exception = new Exception(message.ToString());
            var errorRecord = new ErrorRecord(exception, errorId, errorCategory, null != objects ? objects[0] : null);
            return errorRecord;
        }

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
            var errorRecord = new ErrorRecord(exception, errorId, ErrorCategory.ObjectNotFound, null != objects ? objects[0] : null);
            return errorRecord;
        }
    }
}
