using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace UHSampleGame
{
    public static class EnumHelper
    {
        public static Enum[] EnumToArray(Enum enumeration)
        {
            //get the enumeration type
            Type et = enumeration.GetType();

            //get the public static fields (members of the enum)
            System.Reflection.FieldInfo[] fi = et.GetFields(BindingFlags.Static | BindingFlags.Public);

            //create a new enum array
            Enum[] values = new Enum[fi.Length];

            //populate with the values
            for (int iEnum = 0; iEnum < fi.Length; iEnum++)
            {
                values[iEnum] = (Enum)fi[iEnum].GetValue(enumeration);
            }

            //return the array
            return values;
        }
    }
}
