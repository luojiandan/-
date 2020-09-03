using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;

namespace PBIT度量值分析
{
    public class clsReadData
    {
        //属性
        public string FileName { get; set; }  //文件路径

        /// <summary>
        /// 读取zip文件
        /// </summary>
        public List<MeasureEntity> ReadData()
        {
            ILog m_log = LogManager.GetLogger("log");
            m_log.Info("开始：读取模型内度量值");
            /* 模型内度量值 */
            ZipFile zip = ZipFile.Read(FileName);
            m_log.Info("执行：ZipFile zip = ZipFile.Read(FileName);");
            ZipEntry model = zip["DataModelSchema"];
            m_log.Info("执行：ZipEntry model = zip['DataModelSchema'];");
            if (model == null)
            {
                m_log.Info("执行：自动解压，未取得ZipEntry对象;");
                return null;
            }
            Stream stream = new MemoryStream();
            model.Extract(stream);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream, Encoding.Unicode);
            string json = sr.ReadToEnd();

            DataModelSchema dms = JsonConvert.DeserializeObject<DataModelSchema>(json);
            List<MeasureEntity> measures = dms.GetMeasures();
            dms.MeasureInMeasure(measures);
            m_log.Info("结束：读取模型内度量值，共取得"+measures.Count+"个度量值");

            /*
             * 视图内度量值
             */
            m_log.Info("开始：读取视图内度量值");
            ZipEntry layout = zip["Report\\Layout"];  //"DataModelSchema"  ;     
            Stream stream_layout = new MemoryStream();
            if (stream_layout == null) return null;
            layout.Extract(stream_layout);
            stream_layout.Position = 0;
            StreamReader sr_layout = new StreamReader(stream_layout, Encoding.Unicode);
            json = sr_layout.ReadToEnd();
            Layout pages = JsonConvert.DeserializeObject<Layout>(json);
            pages.MeasuresInModel = measures;
            pages.UpdateMeasureInLayout();
            m_log.Info("结束：读取视图内度量值，共更新" + measures.Count + "个度量值");
            return measures;
        }
    }
}
