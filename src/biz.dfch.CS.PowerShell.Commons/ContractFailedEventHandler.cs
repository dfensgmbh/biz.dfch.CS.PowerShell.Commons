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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;
using TraceSource = biz.dfch.CS.Commons.Diagnostics.TraceSource;

namespace biz.dfch.CS.PowerShell.Commons
{
    /// <summary>
    /// ContractEventHandler
    /// </summary>
    public class ContractFailedEventHandler
    {
        private const int CALLING_FRAME = 1;
        
        /// <summary>
        /// ContractFailed.EventHandler EventId
        /// </summary>
        public const int EVENT_ID = ushort.MaxValue;

        private const string NAMESPACE_CLASS_METHOD_FORMAT = "{0}.{1}";
        private const string METHOD_MESSAGE_FORMAT = "{0}: {1}";
        private const string STACK_FRAME_AT_FORMAT = "at {0}.{1}";
        private const string METHOD_MESSAGE_AND_STACKTRACE_FORMAT = "{0}: {1}{2}";
        private const string CALLINGFRAME_GETMETHOD_NAME_TRACESOURCE = "[CallingFrame].GetMethod().Name '{0}'. TraceSource '{1}'";

        private const string SYSTEM_NAMESPACE = "System.";
        private const string DO_EXECUTE = "DoExecute";
        private const string SYSTEM_MANAGEMENT_AUTOMATION_COMMANDPROCESSORBASE = "System.Management.Automation.CommandProcessorBase";

        private static readonly ConcurrentDictionary<string, TraceSource> _traceSources = 
            new ConcurrentDictionary<string, TraceSource>();

        private static readonly Lazy<bool> _isEventHandlerRegistered = new Lazy<bool>(() =>
        {
            Contract.ContractFailed += EventHandler;

            return true;
        });

        /// <summary>
        /// Sets a ContractFailed event handler for the calling assembly and trace source
        /// Calling this method multiple times for the same assembly has no effect
        /// </summary>
        /// <param name="traceSource">Sepcifies a TraceSource to send message to</param>
        public static void RegisterTraceSource(TraceSource traceSource)
        {
            Contract.Requires(null != traceSource);

            var stackFrame = new StackFrame(CALLING_FRAME);

            var declaringType = stackFrame.GetMethod().DeclaringType;
            var message = string.Format(CALLINGFRAME_GETMETHOD_NAME_TRACESOURCE, stackFrame.GetMethod().Name, traceSource.Name);
            Contract.Assert(null != declaringType, message);

            RegisterTraceSource(declaringType.Assembly, traceSource);
        }

        /// <summary>
        /// Sets a ContractFailed event handler for the specified assembly and trace source
        /// Calling this method multiple times for the same assembly has no effect
        /// </summary>
        /// <param name="assembly">Assembly for which the specified TraceSource wll be registered</param>
        /// <param name="traceSource">Sepcifies a TraceSource to send message to</param>
        public static void RegisterTraceSource(Assembly assembly, TraceSource traceSource)
        {
            Contract.Requires(null != assembly);
            Contract.Requires(null != traceSource);

            if (_traceSources.ContainsKey(assembly.FullName))
            {
                return;
            }
                
            _traceSources.AddOrUpdate(assembly.FullName, traceSource, (key, originalTraceSource) => originalTraceSource);

            // register event handler only once
            Contract.Assert(_isEventHandlerRegistered.Value);
        }

        /// <summary>
        /// EventHandler
        /// </summary>
        /// <param name="sender">always null</param>
        /// <param name="args">holds the assertion being triggered</param>
        internal static void EventHandler(object sender, ContractFailedEventArgs args)
        {
            var index = CALLING_FRAME;
            var stackTrace = new StackTrace(index);
            
            var frames = stackTrace.GetFrames();
            Contract.Assume(null != frames);

            // wait for first frame that is not a System / native frame
            var frame = default(StackFrame);
            var declaringType = default(Type);
            for (index = CALLING_FRAME; index < frames.Length; index++)
            {
                var currentFrame = frames[index];
                declaringType = currentFrame.GetMethod().DeclaringType;
                if (null == declaringType)
                {
                    continue;
                }

                if (declaringType.FullName.StartsWith(SYSTEM_NAMESPACE))
                {
                    continue;
                }

                frame = currentFrame;
                break;
            }

            // very unlikely - but maybe this was a .NET all internal contract exception
            if (null == frame)
            {
                return;
            }

            //// extract method name that caused the assertion and the traceSource
            //var traceSource = _traceSources.GetOrAdd(declaringType.Assembly.FullName, assembly => default(TraceSource));
            //if (null == traceSource)
            //{
            //    return;
            //}
            var traceSource = default(TraceSource);
            var methodName = string.Format(NAMESPACE_CLASS_METHOD_FORMAT, declaringType.FullName, frame.GetMethod().Name);

            var message = new StringBuilder();
            for(var c = index; c < frames.Length; c++)
            {
                frame = frames[c];
                var method = frame.GetMethod();

                declaringType = method.DeclaringType;
                if (null != declaringType)
                {
                    if (null == traceSource)
                    {
                        traceSource = _traceSources.GetOrAdd(declaringType.Assembly.FullName, assembly => default(TraceSource));
                    }

                    if (null != traceSource)
                    {
                        if(!traceSource.Switch.ShouldTrace(TraceEventType.Error))
                        {
                            return;
                        }

                        // for Pre and Post conditions we just log the message
                        if (ContractFailureKind.Precondition == args.FailureKind ||
                            ContractFailureKind.Postcondition == args.FailureKind)
                        {
                            traceSource.TraceEvent(TraceEventType.Error, EVENT_ID, METHOD_MESSAGE_FORMAT, methodName, args.Message);

                            return;
                        }
                    }
            
                    if (DO_EXECUTE.Equals(method.Name) && 
                        SYSTEM_MANAGEMENT_AUTOMATION_COMMANDPROCESSORBASE.Equals(declaringType.ToString()))
                    {
                        break;
                    }
                }

                message.AppendLine();
                message.AppendFormat(STACK_FRAME_AT_FORMAT, declaringType, method.Name);
            }

            if (null == traceSource)
            {
                return;
            }
            
            traceSource.TraceEvent(TraceEventType.Error, EVENT_ID, METHOD_MESSAGE_AND_STACKTRACE_FORMAT, methodName, args.Message, message);
        }
    }
}
