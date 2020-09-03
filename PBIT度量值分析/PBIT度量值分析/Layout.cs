using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PBIT度量值分析
{
    public class Layout
    {
        //页面
        public List<Page> Sections { get; set; }

        //模型中的度量值
        public List<MeasureEntity> MeasuresInModel { get; set; }

        public void UpdateMeasureInLayout()
        {
            //判断度量值是否在此模型中存在
            foreach (MeasureEntity m in MeasuresInModel)
            {
                //每页
                foreach (Page pg in Sections)  
                {
                    //每个视图对象
                    foreach (VisualContainer visual in pg.VisualContainers)
                    {
                        //筛选器内的度量值
                        string filter= visual.Filters;
                        if (filter.Contains(string.Format("\"Property\":\"{0}\"", m.Name)))
                        {
                            m.Used = true;
                            m.UsedInVisualCount++;  //次数加1
                        }
                        //视图对象内的度量值
                        QueryEntity q = visual.QueryMeasure;
                        if (q == null) continue;
                        if (q.SingleVisual == null) continue;
                        if (q.SingleVisual.PrototypeQuery == null) continue;
                        foreach (Measure1 n in q.SingleVisual.PrototypeQuery.Select)
                        {
                            if (n.Measure == null) continue;
                            if (n.Measure.Property.Equals(m.Name))
                            {
                                m.Used = true;
                                m.UsedInVisualCount++;  //次数加1
                            }
                        }
                    }
                }                
            }
        }
    }
    
    //页面
    public class Page
    {
        //页面内的各种视觉对象
        public List<VisualContainer> VisualContainers { get; set; }
    }
    //视觉对象
    public class VisualContainer
    {        
        public QueryEntity QueryMeasure { get; set; }
        string query;
        //查询
        public string Config
        {
            get { return query; }
            set
            {
                query = value;
                QueryMeasure = JsonConvert.DeserializeObject<QueryEntity>(value);
            }
        }

        //筛选器：filters
        public string Filters { get; set; }
    }

    public class QueryEntity
    {
        public string Name { get; set; }
        public QueryData SingleVisual { get; set; }
    }
    
    public class QueryData
    {
        public QueryInfo PrototypeQuery { get; set; }        
    }

    public class QueryInfo
    {
        public string Version { get; set; }
        public List<Measure1> Select { get; set; }
    }

    public class Measure1
    {
        public string Name { get; set; }
        public Measure2 Measure { get; set; }
    }

    public class Measure2
    {
        public string Property { get; set; }
    }
}
