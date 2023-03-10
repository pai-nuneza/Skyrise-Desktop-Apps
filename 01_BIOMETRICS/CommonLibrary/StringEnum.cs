using System;
using System.Collections;
using System.Reflection;

namespace CommonLibrary
{

	public class StringEnum
	{

		public static string GetStringValue(Enum value)
		{
			string output = null;
			Type type = value.GetType();

			FieldInfo fi = type.GetField(value.ToString());
			StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
			if (attrs.Length > 0)
			{
				output = attrs[0].Value;
			}
					
			return output;

		}

        public static string GetEnumDisplay(Enum value)
        {

            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Display;
            }

            return output == null || output.Equals(string.Empty) ? value.ToString() : output;
        }
	}
}