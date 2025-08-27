using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace inercya.EntityLite
{
	internal static class DefaultJsonSerializerOptions
	{
		public static readonly JsonSerializerOptions Instance = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
	}
}
