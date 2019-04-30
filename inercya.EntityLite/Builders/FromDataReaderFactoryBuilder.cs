/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
//using Microsoft.SqlServer.Types;
using inercya.EntityLite.Extensions;
//using Newtonsoft.Json.Linq;

namespace inercya.EntityLite.Builders
{

    public sealed class FromDataReaderFactoryBuilder
    {
        private IDataReader reader;
        private ILGenerator cil;
        private Type entityType;

        private static readonly Dictionary<Type, MethodInfo> dataReaderGetMethods = new Dictionary<Type, MethodInfo>()
        {
            { typeof(bool), typeof(IDataRecord).GetMethod("GetBoolean") },
            { typeof(byte), typeof(IDataRecord).GetMethod("GetByte") },
            { typeof(char), typeof(IDataRecord).GetMethod("GetChar") },
            { typeof(DateTime), typeof(IDataRecord).GetMethod("GetDateTime") },
            { typeof(decimal), typeof(IDataRecord).GetMethod("GetDecimal") },
            { typeof(double), typeof(IDataRecord).GetMethod("GetDouble") },
            { typeof(float), typeof(IDataRecord).GetMethod("GetFloat") },
            { typeof(Guid), typeof(IDataRecord).GetMethod("GetGuid") },
            { typeof(short), typeof(IDataRecord).GetMethod("GetInt16") },
            { typeof(int), typeof(IDataRecord).GetMethod("GetInt32") },
            { typeof(long), typeof(IDataRecord).GetMethod("GetInt64") },
            { typeof(string), typeof(IDataRecord).GetMethod("GetString") },
            { typeof(object), typeof(IDataRecord).GetMethod("GetValue") },
            { typeof(byte[]), typeof(DataReaderExtensions).GetMethod("GetByteArray") },
            { typeof(UInt16), typeof(DataReaderExtensions).GetMethod("GetUInt16")},
            { typeof(UInt32), typeof(DataReaderExtensions).GetMethod("GetUInt32")},
            { typeof(UInt64), typeof(DataReaderExtensions).GetMethod("GetUInt64")},
            { typeof(TimeSpan), typeof(DataReaderExtensions).GetMethod("GetTimeSpan") },
            { typeof(DateTimeOffset), typeof(DataReaderExtensions).GetMethod("GetDateTimeOffset") },
			//{ typeof(SqlHierarchyId), typeof(DataReaderExtensions).GetMethod("GetSqlHierarchyId") },
			//{ typeof(SqlGeometry), typeof(DataReaderExtensions).GetMethod("GetSqlGeometry") },
			//{ typeof(SqlGeography), typeof(DataReaderExtensions).GetMethod("GetSqlGeography") },
            { typeof(sbyte), typeof(DataReaderExtensions).GetMethod("GetSByte") }
        };


        private static readonly MethodInfo IsDbNullMethodInfo = typeof(IDataRecord).GetMethod("IsDBNull");

        private static MethodInfo GetGetDataReaderMethodForFieldType(Type fieldType)
        {
            MethodInfo mi;

            if (dataReaderGetMethods.TryGetValue(fieldType, out mi))
            {
                return mi;
            }
            else
            {
                throw new NotSupportedException(string.Format("Fields of type {0} are not supported", fieldType.FullName));
            }
        }


        public FromDataReaderFactoryBuilder(Type entityType, IDataReader reader)
        {
            this.reader = reader;
            this.entityType = entityType;
        }

        private void EmitCilForOneField(int fieldOrdinal, PropertyInfo pi)
        {
            Label endField = cil.DefineLabel();
            Label setPropertyFromField = cil.DefineLabel();

            //var IsDbNullMethodInfo = typeof(IDataRecord).GetMethod("IsDBNull");

            /* 
                if (!reader.IsDBNull( fieldOrdinal ) goto setPropertyFromField 
            */
            // cargar el datareader en la pila (argumento 0 del método)
            cil.Emit(OpCodes.Ldarg_0);
            // cargar fieldOrdinal en la pila
            cil.Emit(OpCodes.Ldc_I4, fieldOrdinal);
            // llamar al método IsDBNull del datareader
            var IsDbNullMethodInfo = FromDataReaderFactoryBuilder.IsDbNullMethodInfo;
            cil.Emit(OpCodes.Callvirt, IsDbNullMethodInfo);
            // si no es nulo ir a establecer la propiedad = valor del campo
            cil.Emit(OpCodes.Brfalse_S, setPropertyFromField);

			// entity.Property = null
			EmitSetPropertyToNull(pi);

            // goto endField:
            cil.Emit(OpCodes.Br_S, endField);

			// setPropertyFromField:
            cil.MarkLabel(setPropertyFromField);

			// entity.Property = (PropertyType) reader.GetXX(fieldOrdinal)
			EmitSetPropertyValueFromField(fieldOrdinal, pi);

			// endField:
            cil.MarkLabel(endField);
        }

		private void EmitSetPropertyValueFromField(int fieldOrdinal, PropertyInfo pi)
		{
			/*
			 * entity.Property = (ProperyType) reader.GetXX(fieldOrdinal)
			 */

			// cargar la entidad en la pila (variable local 0)
			cil.Emit(OpCodes.Ldloc_0);
			// cargar el datareader en la pila (argumento 1 del método)
			cil.Emit(OpCodes.Ldarg_0);
			// cargar fieldOrdinal en la pila
			cil.Emit(OpCodes.Ldc_I4, fieldOrdinal);
			// llamar al método GetXX
            MethodInfo getDataReaderMethod = null;
            if (pi.PropertyType == typeof(object))
            {
                getDataReaderMethod = GetGetDataReaderMethodForFieldType(typeof(object));
            }
            else
            {
                getDataReaderMethod = GetGetDataReaderMethodForFieldType(reader.GetFieldType(fieldOrdinal));
            }
			if (getDataReaderMethod.IsStatic)
			{
				cil.Emit(OpCodes.Call, getDataReaderMethod);
			}
			else
			{
				cil.Emit(OpCodes.Callvirt, getDataReaderMethod);
			}

			// averiguamos si el tipo de la propiedad es valor nullable y su tipo subyacente
			bool isNullableValueType = pi.PropertyType.IsNullableValueType();
			Type underlyingPropertyType = pi.PropertyType;
			if (isNullableValueType)
			{
				underlyingPropertyType = underlyingPropertyType.GetGenericArguments()[0];
			}

			// emitir código de conversión de tipos
            if (underlyingPropertyType != typeof(object))
            {
                EmitConversionCode(reader.GetFieldType(fieldOrdinal), underlyingPropertyType);
            }

			// si el tipo es Nullable<> hay que llamar al constructor
			if (isNullableValueType)
			{
				ConstructorInfo constructor = pi.PropertyType.GetConstructor(new Type[] { underlyingPropertyType });
				cil.Emit(OpCodes.Newobj, constructor);
			}
			// establecer el valor de la propiedad
			cil.Emit(OpCodes.Call, pi.GetSetMethod());
		}

		private void EmitSetPropertyToNull(PropertyInfo pi)
		{
			if (!pi.PropertyType.IsValueType)
			{
				/* 
					* entity.Property = null (para propiedad de tipo referencia)
					*/
				// cargar la entidad en la pila (variable local 0)
				cil.Emit(OpCodes.Ldloc_0);
				// cargar null en la pila;
				cil.Emit(OpCodes.Ldnull);
				// llamar al métdodo set de la propiedad
				cil.Emit(OpCodes.Call, pi.GetSetMethod());

			}
			else if (pi.PropertyType.IsNullableValueType())
			{
				/* 
					* entity.Property = null; (para propiedad de tipo Nullable<T>)
					*/
				// declarar una variable local
				LocalBuilder nullableNull = cil.DeclareLocal(pi.PropertyType);
				// cargar la entidad en la pila (variable local 0)
				cil.Emit(OpCodes.Ldloc_0);
				// cargar la dirección de la variable local nullableNull en la pila
				cil.Emit(OpCodes.Ldloca, nullableNull);
				// inicializar la variable nullbleNull (cuya dirección está en la pila)
				cil.Emit(OpCodes.Initobj, pi.PropertyType);
				// cargar la variable nullableNull en la pila
				cil.Emit(OpCodes.Ldloc, nullableNull);
				// llamar al método set de la propiedad
				cil.Emit(OpCodes.Call, pi.GetSetMethod());
			}
			else
			{
				/* 
					* La propiedad es de tipo valor no nulable, y el valor del campo es DbNull
					* Tenemos dos opciones:
					* 1.- lanzar una excepción ya que no podemos asignar el valor nulo
					* 2.- No hacer nada dejando la propiedad en su valor predeterminado 
					*/

				// dejamos la propiedad en su valor predeterminado.
				// Debug.WriteLine(string.Format("la propiedad {0} no es nulable, pero el campo sí, entidad {1}", pi.Name, pi.DeclaringType.Name));
			}
		}


        private static Dictionary<Type, OpCode> nativeConversionOpcodes = new Dictionary<Type, OpCode>()
        {
            { typeof(byte), OpCodes.Conv_U1 },
            { typeof(sbyte), OpCodes.Conv_I1 },
            { typeof(short), OpCodes.Conv_I2 },
            { typeof(ushort), OpCodes.Conv_U2 },
            { typeof(int), OpCodes.Conv_I4 },
            { typeof(uint), OpCodes.Conv_U4 },
            { typeof(long), OpCodes.Conv_I8 },
            { typeof(ulong), OpCodes.Conv_U8 },
            { typeof(float), OpCodes.Conv_R4 },
            { typeof(double), OpCodes.Conv_R8 }
        };


        private void EmitConversionCode(Type fromType, Type toType)
        {
            MethodInfo parseMethod = null;
            MethodInfo conversionMethod = null;
            // si los tipos son iguales no hay que emitir ningún código
            if (fromType == toType) return;
            if (fromType.IsEnum) fromType = fromType.GetField("value__").FieldType;
            if (toType.IsEnum) toType = toType.GetField("value__").FieldType;
            if (fromType.IsPrimitive && toType.IsPrimitive)
            {
                OpCode opCode;
                if (!nativeConversionOpcodes.TryGetValue(toType, out opCode))
                {
                    throw new InvalidCastException(string.Format("Cannot convert from {0} to {1}", fromType.FullName, toType.FullName));
                }
                cil.Emit(opCode);
            }
            else if (fromType == typeof(string) && (parseMethod = toType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null)) != null )
            {
                cil.Emit(OpCodes.Call, parseMethod);
            }
            else if (toType == typeof(string))
            {
                MethodInfo toStringMethod = fromType.GetMethod("ToString", Type.EmptyTypes);
                if (toStringMethod == null)
                {
                    throw new InvalidCastException(string.Format("Cannot convert from {0} to {1}", fromType.FullName, toType.FullName));
                }
                if (fromType.IsValueType)
                {
                    LocalBuilder localVar = cil.DeclareLocal(fromType);
                    cil.Emit(OpCodes.Stloc, localVar);
                    cil.Emit(OpCodes.Ldloca, localVar.LocalIndex);
                }
                cil.Emit(OpCodes.Call, toStringMethod);
            }
            else if ((conversionMethod = ConversionMethods.GetConversionMethod(fromType, toType)) != null)
            {
                cil.Emit(OpCodes.Call, conversionMethod);
            }
            else
            {
                throw new InvalidCastException(string.Format("Cannot convert from {0} to {1}", fromType.FullName, toType.FullName));
            }
        }

        private bool TryFindMappingProperty(Dictionary<string, PropertyMetadata> columnToPropertyMap, Dictionary<string, PropertyInfo> properties, string fieldName, out PropertyInfo pi )
        {
            if (columnToPropertyMap != null && columnToPropertyMap.TryGetValue(fieldName, out var propertyMetadata) && propertyMetadata.PropertyInfo != null)
            {
                pi = propertyMetadata.PropertyInfo;
                return true;
            }
            string candidatePropertyName = fieldName;
            if (properties.TryGetValue(candidatePropertyName, out pi)) return true;
            candidatePropertyName = fieldName.ToPascalNamingConvention();
            if (properties.TryGetValue(candidatePropertyName, out pi)) return true;
            if (fieldName.EndsWith("json", StringComparison.InvariantCultureIgnoreCase))
            {
                fieldName = fieldName.Substring(0, fieldName.Length - 4).TrimEnd('_');
                candidatePropertyName = fieldName;
                if (properties.TryGetValue(candidatePropertyName, out pi)) return true;
                candidatePropertyName = fieldName.ToPascalNamingConvention();
                return properties.TryGetValue(candidatePropertyName, out pi);
            }
            return false;
        }

        private DynamicMethod CreateDynamicMethod()
        {
            DynamicMethod dm = new DynamicMethod("ObjectFromDataReader<" + this.entityType.FullName + ">", typeof(object), new Type[] { typeof(IDataReader) });
            cil = dm.GetILGenerator();

			// T entity = new T();
			LocalBuilder entityVar = cil.DeclareLocal(this.entityType);
            cil.Emit(OpCodes.Newobj, this.entityType.GetConstructor(new Type[] { }));
            cil.Emit(OpCodes.Stloc_0);

            var properties = entityType.GetProperties().ToDictionary(x => x.Name);
            var columnToPropertyMap = EntityMetadata.GetEntityMetadata(entityType)?.ColumnToPropertyMap;
            int fieldCount = reader.FieldCount;
            for (int fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
            {
                string fieldName = reader.GetName(fieldOrdinal);
                if (TryFindMappingProperty(columnToPropertyMap, properties, fieldName, out PropertyInfo pi))
                {
                    EmitCilForOneField(fieldOrdinal, pi);
                }
            }

			// return entity;
            cil.Emit(OpCodes.Ldloc_0);
            cil.Emit(OpCodes.Ret);
            return dm;
        }

        public Func<IDataReader, object> CreateFactory()
        {
            return (Func<IDataReader, object>)CreateDynamicMethod().CreateDelegate(typeof(Func<IDataReader, object>));
        }
    }
}
