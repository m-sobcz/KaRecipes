using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.OPC
{
    public static class OpcDataConvert
    {
        public static object DataValueToNetType(DataValue input)
        {
            if (input?.WrappedValue.TypeInfo.ValueRank == -1)
            {
                return GetSingleObjectFromDataValue(input.Value, input.WrappedValue.TypeInfo.BuiltInType);
            }
            else
            {
                List<object> converedList = new();
                foreach (var item in input.Value as IEnumerable)
                {
                    var converted = GetSingleObjectFromDataValue(item, input.WrappedValue.TypeInfo.BuiltInType);
                    converedList.Add(converted);
                }
                return converedList;
            }
        }
       public static object GetSingleObjectFromDataValue(object input, BuiltInType opcType)
        {
            object converted;
            switch (opcType)
            {
                case BuiltInType.Boolean:
                    {
                        converted = Convert.ToBoolean(input);
                        break;
                    }

                case BuiltInType.SByte:
                    {
                        converted = Convert.ToSByte(input);
                        break;
                    }

                case BuiltInType.Byte:
                    {
                        converted = Convert.ToByte(input);
                        break;
                    }

                case BuiltInType.Int16:
                    {
                        converted = Convert.ToInt16(input);
                        break;
                    }

                case BuiltInType.UInt16:
                    {
                        converted = Convert.ToUInt16(input);
                        break;
                    }

                case BuiltInType.Int32:
                    {
                        converted = Convert.ToInt32(input);
                        break;
                    }

                case BuiltInType.UInt32:
                    {
                        converted = Convert.ToUInt32(input);
                        break;
                    }

                case BuiltInType.Int64:
                    {
                        converted = Convert.ToInt64(input);
                        break;
                    }

                case BuiltInType.UInt64:
                    {
                        converted = Convert.ToUInt64(input);
                        break;
                    }

                case BuiltInType.Float:
                    {
                        converted = Convert.ToSingle(input);
                        break;
                    }

                case BuiltInType.Double:
                    {
                        converted = Convert.ToDouble(input);
                        break;
                    }

                default:
                    {
                        converted = input;
                        break;
                    }
            }
            return converted;
        }
    }
}
