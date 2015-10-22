using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using System.ServiceModel.Web;
using System.IO.Compression;
using System.ServiceModel.Channels;

namespace WCFGenerico.Utils
{
    public class Util
    {
        public static string CompressString(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return sB.ToString();
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }


        public static string EncodeTo64(byte[] toEncode)
        {

            return System.Convert.ToBase64String(toEncode, 0, toEncode.Length);
        }

        public static byte[] DecodeFrom64(string toDecode)
        {
            return System.Convert.FromBase64String(toDecode);

        }


        public static void Log(string val)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";

            StreamWriter fileWriter = null;
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string file = dir + "Log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";

                if (File.Exists(file))
                    fileWriter = File.AppendText(file);
                else
                    fileWriter = File.CreateText(file);

                DateTime now = DateTime.Now;
                fileWriter.WriteLine(now.ToString() + "." + now.Millisecond.ToString("D3") + ": " + val);
            }
            finally
            {
                try
                {
                    if (fileWriter != null)
                    {
                        fileWriter.Close();
                        fileWriter.Dispose();
                    }
                }
                catch (Exception)
                {

                }

            }
        }



        public static string DataTableToJSON(DataTable dt)
        {

            string jsonString = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonString += "{";

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonString += "\"" + dt.Columns[j].ColumnName + "\": ";
                    if (dt.Columns[j].DataType == typeof(string) || dt.Columns[j].DataType == typeof(DateTime))
                    {
                        jsonString += "\"" + dt.Rows[i][j].ToString() + "\"";
                    }
                    else if (dt.Columns[j].DataType == typeof(byte[]))
                    {
                        jsonString += "\"" + Convert.ToBase64String((byte[])dt.Rows[i][j]) + "\"";
                    }
                    else
                        jsonString += dt.Rows[i][j];

                    //adiciona virgula para o proximo, caso não seja o último
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString += ",";
                    }
                }

                //adiciona virgula para o proximo
                if (i == dt.Rows.Count - 1)
                    jsonString += "}";
                else
                    jsonString += "},";

            }
            return "[" + jsonString + "]";
        }




        public static String DataTableToJSON2(DataTable dt)
        {
            String s = JsonConvert.SerializeObject(DatatableToDictionary(dt, "ID"), Newtonsoft.Json.Formatting.Indented);
            //s = s.Substring(1, s.Length - 2);
            return s.Trim();
        }



        private static Dictionary<string, Dictionary<string, object>> DatatableToDictionary(DataTable dt, string id)
        {
            var cols = dt.Columns.Cast<DataColumn>().Where(c => c.ColumnName != id);
            return dt.Rows.Cast<DataRow>()
                     .ToDictionary(r => r[id].ToString(),
                                   r => cols.ToDictionary(c => c.ColumnName, c => r[c.ColumnName]));
        }


        public static string DataTableColumnsInfoToJSON(DataTable dt)
        {

            string jsonString = string.Empty;

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                DataColumn column = dt.Columns[j];

                jsonString += "{";
                jsonString += "\"ColumnName\": \"" + column.ColumnName + "\",";
                jsonString += "\"DataType\": \"" + column.DataType.ToString() + "\"";
                //jsonString += "\"Length\": \"" + column.MaxLength + "\",";
                //jsonString += "\"Nullable\": \"" + column.AllowDBNull.ToString() + "\"";

                if (j == dt.Columns.Count - 1) jsonString += "}";
                else jsonString += "},";
            }

            return "[" + jsonString + "]";
        }




        public static string GetJSONObject(string key, object value)
        {
            StringBuilder JsonString = new StringBuilder();
            JsonString.Append("{");
            if (value.GetType() == typeof(string))
            {
                JsonString.Append(string.Format("\"{0}\":\"{1}\"", key, value));
            }
            else JsonString.Append(string.Format("\"{0}\":{1}", key, value));

            JsonString.Append("}");
            return JsonString.ToString();
        }

        public static string GetJSONArray(Dictionary<string, object> dictionary)
        {
            StringBuilder JsonString = new StringBuilder();

            JsonString.Append("[");
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                JsonString.Append("{");
                if (pair.Value == null)
                {
                    JsonString.Append(string.Format("\"{0}\":[]", pair.Key));
                }
                else if (pair.Key == "data" || pair.Key == "tables")
                {
                    JsonString.Append(string.Format("\"{0}\": {1}", pair.Key, pair.Value));
                }

                else if (pair.Value.GetType() == typeof(string))
                {
                    JsonString.Append(string.Format("\"{0}\":\"{1}\"", pair.Key, pair.Value));
                }
                else JsonString.Append(string.Format("\"{0}\":{1}", pair.Key, pair.Value));

                JsonString.Append("},");
            }


            //REMOVE A ULTIMA VIRGULA E ADICONA O ULTIMO COLCHETES DO ARRAY
            string str = JsonString.ToString();
            if (str.Length > 1)
            {
                str = str.Substring(0, str.LastIndexOf(',')) + "]";
            }

            return str.Replace("\"[{", "[{").Replace("}]\"", "}]");
        }



        public static MemoryStream GetJsonStream(String value)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json;charset=utf-8";
            //WebOperationContext.Current.OutgoingResponse.Headers["Accept"] = "application/json";
            WebOperationContext.Current.OutgoingResponse.Headers["Accept-Encoding"] = "gzip, deflate";
            WebOperationContext.Current.OutgoingResponse.Headers["Content-Encoding"] = "zlib, deflate, gzip";

            MemoryStream stream1 = new MemoryStream(Encoding.UTF8.GetBytes(value));

            return stream1;

        }

        static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzStream.Write(buffer.Array, buffer.Offset, buffer.Count);
            }

            byte[] compressedBytes = memoryStream.ToArray();
            int totalLength = messageOffset + compressedBytes.Length;
            byte[] bufferedBytes = bufferManager.TakeBuffer(totalLength);

            Array.Copy(compressedBytes, 0, bufferedBytes, messageOffset, compressedBytes.Length);

            bufferManager.ReturnBuffer(buffer.Array);
            ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferedBytes, messageOffset,
                bufferedBytes.Length - messageOffset);

            return byteArray;
        }


        internal static string Compress(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }
    }



}