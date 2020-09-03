using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBIT度量值分析
{
    public class DataModelSchema
    {
        //名称
        public string Name { get; set; }
        //模型
        public Model Model { get; set; }

        /// <summary>
        /// 获取度量值
        /// </summary>
        /// <returns></returns>
        public List<MeasureEntity> GetMeasures()
        {
            List<MeasureEntity> lst_measures = new List<MeasureEntity>();
            foreach (Table tbl in Model.Tables)
            {
                if (tbl.Measures == null) continue;
                foreach (MeasureEntity m in tbl.Measures)
                {
                    lst_measures.Add(m);
                }
            }
            return lst_measures;
        }

        /// <summary>
        /// 度量值调用度量值
        /// </summary>
        /// <param name="measures"></param>
        public void MeasureInMeasure(List<MeasureEntity> measures)
        {
            foreach (MeasureEntity m in measures)
            {
                foreach (MeasureEntity n in measures)
                {
                    if (n.Expression.Contains(string.Format("[{0}]", m.Name)))
                    {
                        m.Used = true;
                        m.UsedInModelCount++;  //次数加1
                    }
                }
            }
        }
    }
    /// <summary>
    /// 模型
    /// </summary>
    public class Model
    {
        //表信息
        public List<Table> Tables { get; set; }
    }

    /// <summary>
    /// 表
    /// </summary>
    public class Table
    {
        //度量值
        public List<MeasureEntity> Measures { get; set; }
    }

    /// <summary>
    /// 度量值
    /// </summary>
    public class MeasureEntity
    {
        //名称
        public string Name { get; set; }
        //表达式
        public string Expression { get; set; }
        //是否使用
        public bool Used { get; set; }
        //模型中使用次数
        public int UsedInModelCount { get; set; }
        //视图中使用次数
        public int UsedInVisualCount { get; set; }
    }
}
