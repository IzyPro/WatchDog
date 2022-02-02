using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WatchDog.src.Helpers
{
    public static class JsonDBHelper
	{
		public static string jsonFile;
		public static string jsonLocation;
		private static JObject _dbObject;


		public static JObject Load(string filePathLoaded)
		{
			try
			{
				if (!File.Exists(filePathLoaded))
					File.Create(filePathLoaded);
				jsonFile = File.ReadAllText(filePathLoaded);
				jsonLocation = filePathLoaded;
				return _dbObject = JObject.Parse(jsonFile);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		public static void Add(this object data, string table, object newData)
		{
			if (data != null)
			{
				try
				{
					var parsedData = JObject.Parse(data.ToString());
					var result = string.IsNullOrEmpty(table) ? (object)parsedData : (object)parsedData[table];
					var dataArray = result.ToDataList();
					dataArray.Add(JObject.Parse(JsonConvert.SerializeObject(newData, Formatting.Indented)));
					parsedData[table] = dataArray;
					string newJsonResult = parsedData.ToString();
					File.WriteAllText(jsonLocation, newJsonResult);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message.ToString());
				}
			}
			else
			{
				throw new Exception("The preloaded data is null");
			}
		}

		public static List<dynamic> Where(this object data, string key = "", dynamic value = null)
		{
			if (data != null)
			{
				var dataArray = data.ToDataList();
				var newDataArray = new List<dynamic>();
				foreach (var singleData in dataArray.Where(x => x[key] as dynamic == value))
				{
					newDataArray.Add(singleData);
				}
				return newDataArray;
			}
			else
			{
				throw new Exception("The preloaded data is null");
			}

		}

		public static string ToJsonString(this object data)
		{
			if (data != null)
			{
				return JsonConvert.SerializeObject(data);
			}
			else
			{
				throw new Exception("The preloaded data is null");
			}
		}

		public static JArray ToDataList(this object data)
		{
			if (data != null)
			{
				try
				{
					var token = JToken.Parse(data.ToString());
					if (token is JArray)
					{

						JArray dataArray = (JArray)data;
						return dataArray;
					}
					else
					{
						throw new Exception("The preloaded data is not a Json Array");
					}
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message.ToString());
				}
			}
			else
			{
				throw new Exception("The preloaded data is null");
			}
		}
	}
}
