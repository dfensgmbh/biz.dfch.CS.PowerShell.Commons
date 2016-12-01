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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using biz.dfch.CS.Commons.Diagnostics;
using TraceSource = biz.dfch.CS.Commons.Diagnostics.TraceSource;

namespace biz.dfch.CS.PowerShell.Commons
{
    /// <summary>
    /// Base class all dfch PSCmdlets must derive from
    /// This class should be moved to biz.dfch.CS.PowerShell.Commons
    /// </summary>
    public class PsCmdletBase : PSCmdlet
    {
        protected TraceSource TraceSource;

        public const int EVENT_ID_START = ushort.MaxValue - 1;
        public const int EVENT_ID_STOP  = ushort.MaxValue - 2;

        /// <summary>
        /// Preamble of Cmdlet used for initialising parameters to its default values
        /// </summary>
        protected override void BeginProcessing()
        {
            var moduleName = this.MyInvocation.MyCommand.ModuleName;
            if(string.IsNullOrWhiteSpace(moduleName)) { moduleName = this.GetType().Namespace; }

            TraceSource = Logger.Get(moduleName);

            TraceSource.TraceEvent(TraceEventType.Start, EVENT_ID_START, MyInvocation.InvocationName);

            SetDefaultValues();

            base.BeginProcessing();
        }

        /// <summary>
        /// Sets all bound parameters of the current Cmdlet to its default values (if defined)
        /// </summary>
        protected virtual void SetDefaultValues()
        {
            var propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
                .Where
                (
                    propertyInfo => 
                    !MyInvocation.BoundParameters.ContainsKey(propertyInfo.Name) 
                    &&
                    Attribute.GetCustomAttributes(propertyInfo, typeof(ParameterAttribute)).Any()
                );

            foreach (var propertyInfo in propertyInfos)
            {
                // only process Parameter with a PSDefaultValue
                var psDefaultValueAttribute = (PSDefaultValueAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(PSDefaultValueAttribute));
                if (null == psDefaultValueAttribute)
                {
                    continue;
                }

                var value = propertyInfo.PropertyType == typeof(SwitchParameter) ? 
                    new SwitchParameter(isPresent: (bool) psDefaultValueAttribute.Value) : psDefaultValueAttribute.Value;
                propertyInfo.SetValue(this, value, null);
            }
        }

        protected override void EndProcessing()
        {
            TraceSource.TraceEvent(TraceEventType.Stop, EVENT_ID_STOP, MyInvocation.InvocationName);

            base.EndProcessing();
        }

        public void WriteError(ErrorRecord errorRecord, bool writeToTraceSource)
        {
            Contract.Requires(null != errorRecord);
            Contract.Requires(null != errorRecord.Exception);

            if (writeToTraceSource && null != TraceSource)
            {
                TraceSource.TraceException(errorRecord.Exception);
            }

            base.WriteError(errorRecord);
        }
    }
}
