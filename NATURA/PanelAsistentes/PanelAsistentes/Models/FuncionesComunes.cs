﻿using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Models;
using Syncfusion.Linq;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PanelAsistentes.Models
{
    public class FuncionesComunes
    {
        public string LlamadoApi(string data, string Baseurl)
        {
            var res = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Baseurl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                requestWriter.Write(data);
                requestWriter.Close();

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                res = responseReader.ReadToEnd();
                responseReader.Close();
                webStream.Close();
                webResponse.Close();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString();
            }

            return res;
        }

        public string CallService(string data, string Complementourl)
        {
            string respuesta = "";
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiDir"]);
                var responseTask = client.PostAsync(Complementourl, content);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    respuesta = readTask.Result;
                }
            }
            return respuesta;
        }

        public JsonResult DSGenerico(DataManager dm, IEnumerable Data)
        {
            int count = 0;
            DataResult result = new DataResult();
            List<string> str = new List<string>();
            DataOperations operation = new DataOperations();

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                Data = operation.PerformSorting(Data, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                Data = operation.PerformWhereFilter(Data, dm.Where, dm.Where[0].Condition);
            }
            if (dm.Aggregates != null)
            {
                for (var i = 0; i < dm.Aggregates.Count; i++)
                    str.Add(dm.Aggregates[i].Field);
                result.aggregate = operation.PerformSelect(Data, str);
            }
            try
            {
                count = Data.AsQueryable().Count();
            }
            catch (Exception ex)
            {
                count = 0;
            }
            if (dm.Skip != 0)
            {
                Data = operation.PerformSkip(Data, dm.Skip);
            }
            if (dm.Take == 0)
            {
                result.result = operation.PerformTake(Data, count);
            }
            else
            {
                result.result = operation.PerformTake(Data, dm.Take);
            }

            result.count = count;
            Data = operation.Execute(Data, dm);

            JsonResult jr = new JsonResult()
            {
                MaxJsonLength = Int32.MaxValue,
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return jr;
        }

        public class DataResult
        {
            public IEnumerable result { get; set; }
            public int count { get; set; }
            public IEnumerable aggregate { get; set; }
            public IEnumerable groupDs { get; set; }
        }

        public GridProperties ConvertGridObject(string gridProperty)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IEnumerable div = (IEnumerable)serializer.Deserialize(gridProperty, typeof(IEnumerable));
            GridProperties gridProp = new GridProperties();
            foreach (KeyValuePair<string, object> ds in div)
            {
                var property = gridProp.GetType().GetProperty(ds.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    Type type = property.PropertyType;
                    string serialize = serializer.Serialize(ds.Value);
                    object value = serializer.Deserialize(serialize, type);
                    property.SetValue(gridProp, value, null);
                }
            }

            AutoFormat auto = new AutoFormat()
            {
                HeaderBorderColor = Color.FromArgb(206, 206, 206),
                ContentBorderColor = Color.FromArgb(206, 206, 206),
                GHeaderBorderColor = Color.FromArgb(206, 206, 206),
                GHeaderBgColor = Color.LightGray,
                HeaderBgEndColor = Color.FromArgb(236, 236, 236),
                GContentFontColor = Color.FromArgb(51, 51, 51),
                AltRowBgColor = Color.FromArgb(255, 255, 255),
                ContentBgColor = Color.FromArgb(255, 255, 255)
            };
            gridProp.AutoFormat = auto;

            return gridProp;
        }

        public void QueryCellInfo(object currentCell)
        {
            IRange range = (IRange)currentCell;
            range.CellStyle.Borders.ColorRGB = Color.FromArgb(206, 206, 206);

            try
            {
                if (decimal.TryParse(range.Value, out decimal valor))
                {
                    if (decimal.Parse(range.Value) < 0)
                    {
                        range.CellStyle.Font.Color = ExcelKnownColors.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}