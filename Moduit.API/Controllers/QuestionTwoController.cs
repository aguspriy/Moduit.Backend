using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moduit.API.Controllers
{
    [Route("backend/question/two")]
    [ApiController]
    public class QuestionTwoController : ControllerBase
    {
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {            

            using (var httpClient = new HttpClient())
            {
                //var jsonObj;
                var jsonString = await httpClient.GetStringAsync("https://screening.moduit.id/backend/question/two");
                //dynamic json = JsonConvert.DeserializeObject(jsonString);
                DataTable dt = ConvertJsonToDatatable(jsonString);

                return new JsonResult(dt);
                //JArray array = JArray.Parse(jsonString);
                //foreach (JObject obj in array.Children<JObject>())
                //{
                //    foreach (JProperty singleProp in obj.Properties())
                //    {
                //        string name = singleProp.Name;
                //        string value = singleProp.Value.ToString().ToLower();
                //        ////Do something with name and value
                //        ////System.Windows.MessageBox.Show("name is "+name+" and value is "+value);
                //        if ((name == "description") && (value.Contains("and")))
                //        {
                //            obj.RemoveAll();                            
                //        }
                //        //continue;
                //        //return new JsonResult(obj);
                //    }
                //    continue;
                //}
                return new JsonResult(dt);
            }
        }

        protected DataTable ConvertJsonToDatatable(string jsonString)
        {
            DataTable dt = new DataTable();
            //strip out bad characters
            string[] jsonParts = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            //hold column names
            List<string> dtColumns = new List<string>();
            //get columns
            foreach (string jp in jsonParts)
            {
                //only loop thru once to get column names
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                foreach (string rowData in propData)
                {


                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1);
                        string v = rowData.Substring(idx + 1);
                        if (!dtColumns.Contains(n))
                        {
                            dtColumns.Add(n.Replace("\"", ""));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", rowData));
                    }
                }
                break; // TODO: might not be correct. Was : Exit For
            }
            //build dt
            foreach (string c in dtColumns)
            {
                dt.Columns.Add(c);
            }
            //get table data
            foreach (string jp in jsonParts)
            {
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string v = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[n] = v;
                        if ((n == "description") && v.Contains("Ergonomi") || (n == "tags") && v.Contains("Sports") )
                        {
                            nr[n] = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                if ((nr["description"].ToString() != "") || (nr["description"].ToString() != ""))
                {
                    dt.Rows.Add(nr);
                    dt = dt.AsEnumerable().Reverse().Take(3).CopyToDataTable();
                    DataView dv = new DataView(dt);
                    dv.Sort = "createdAt DESC";
                    dt = dv.ToTable();
                } 
            }
            return dt;
        }

    }
}
