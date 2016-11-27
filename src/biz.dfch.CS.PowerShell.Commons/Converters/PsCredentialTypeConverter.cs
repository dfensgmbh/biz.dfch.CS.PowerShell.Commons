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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Security;

namespace biz.dfch.CS.PowerShell.Commons.Converters
{
    /// <summary>
    /// Converts a comma separated string consisting of username and password to a PSCredential
    /// </summary>
    public class PsCredentialTypeConverter : TypeConverter
    {
        /// <summary>
        /// Delimiter for separating username and password
        /// </summary>
        public const char DELIMITER = '|';

        private const int ARRAY_COUNT_MIN = 2;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            var result = typeof(PSCredential) == sourceType || base.CanConvertFrom(context, sourceType);
            return result;
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
        {
            var valueAsString = value as string;
            var result = null != valueAsString ? ConvertToPsCredential(valueAsString) : base.ConvertFrom(context, culture, value);
            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var result = typeof(string) == destinationType ? ConvertToPsCredential((string) value) : base.ConvertTo(context, culture, value, destinationType);
            return result;
        }

        public static PSCredential ConvertToPsCredential(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(PSCredential);
            }

            var values = value.Split(DELIMITER);
            if (ARRAY_COUNT_MIN > values.Length)
            {
                return default(PSCredential);
            }

            var username = values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(username))
            {
                return default(PSCredential);
            }

            var password = value.Substring(username.Length + DELIMITER.ToString().Length);
            if (string.IsNullOrWhiteSpace(password))
            {
                return default(PSCredential);
            }

            var secureString = new SecureString();
            foreach (var singleCharacter in password)
            {
                secureString.AppendChar(singleCharacter);
            }

            var result = new PSCredential(username, secureString);
            return result;
        }
    }
}
