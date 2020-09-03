using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PBIT度量值分析
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<MeasureEntity> measures = new List<MeasureEntity>();
        private void button1_Click(object sender, EventArgs e)
        {
            ILog m_log = LogManager.GetLogger("log");
            try
            {
                string filePath = txtPath.Text.Trim();
                m_log.Info("开始：判断文件是否存在？");
                if (!File.Exists(filePath))
                {
                    txtPath.Text = "文件不存在，请重新输入！";
                    return;  //不存在，退出
                }
                m_log.Info("结束：文件存在！");
                clsReadData cd = new clsReadData();
                cd.FileName = txtPath.Text.Trim(); //@"D:\000\PBI\Data.pbit";
                measures = cd.ReadData();
                if (measures == null)
                {
                    m_log.Info("信息：未取得“度量值”信息，程序退出！");
                    return;
                }
                dgvData.DataSource = measures;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private int _sorterOrder;   //1表示升序，0表示降序
        private int _previousIndex = -1;    //记录前一次点击的列索引
        /// <summary>
        /// List集合比较器
        /// </summary>
        /// <param name="propName">属性名</param>
        private void ListComparison(string propName)
        {
            Type type = typeof(MeasureEntity);
            PropertyInfo property = type.GetProperty(propName);
            List<MeasureEntity> modelList = dgvData.DataSource as List<MeasureEntity>;
            modelList.Sort((x, y) =>
            {
                string value1 = property.GetValue(x, null).ToString();
                string value2 = property.GetValue(y, null).ToString();
                double number1, number2;
                //如果属性值为数字则转换为浮点数进行比较，否则按照字符串比较
                if (double.TryParse(value1, out number1) && double.TryParse(value2, out number2))
                {
                    return this._sorterOrder == 1 ? number1.CompareTo(number2) : number2.CompareTo(number1);
                }

                return this._sorterOrder == 1 ? value1.CompareTo(value2) : value2.CompareTo(value1);
            });
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text.Trim());
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel2.Text.Trim());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtPath.AllowDrop = true;
            txtPath.DragEnter += TxtPath_DragEnter;
            txtPath.DragDrop += TxtPath_DragDrop;
        }

        private void TxtPath_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            txtPath.Text = path;
        }

        private void TxtPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //是否有数据
            if (dgvData.RowCount == 0) return;
            DialogResult dr = saveFileDialog.ShowDialog();
            saveFileDialog.Filter = "Excel文件|*.xls";
            saveFileDialog.FilterIndex = 1;
            string filePath = "";
            if (dr==DialogResult.OK)
            {
                filePath = saveFileDialog.FileName;
                if (filePath.Trim() == "") return;                
            }
            else
            {
                return;
            }
            

            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet0");//创建一个名称为Sheet0的表  
            int rowCount = dgvData.RowCount;//行数  
            int columnCount =dgvData.ColumnCount;//列数  

            //设置列头  
            IRow row = sheet.CreateRow(0);//excel第一行设为列头  
            ICell cell;
            for (int c = 0; c < columnCount; c++)
            {
                cell = row.CreateCell(c);
                cell.SetCellValue(dgvData.Columns[c].HeaderText);
            }

            //设置每行每列的单元格,  
            for (int i = 0; i < rowCount; i++)
            {
                row = sheet.CreateRow(i + 1);
                for (int j = 0; j < columnCount; j++)
                {
                    cell = row.CreateCell(j);//excel第二行开始写入数据  
                    cell.SetCellValue(dgvData.Rows[i].Cells[j].Value.ToString());
                }
            }
            using (FileStream  fs = File.OpenWrite(filePath))
            {
                workbook.Write(fs);//向打开的这个xls文件中写入数据  

                MessageBox.Show("数据导出完毕！", "度量值分析",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        
        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //判断列标头是否被连续点击，是则改变上次排序规则，否则按升序排序
            if (this._previousIndex == e.ColumnIndex)
            {
                this._sorterOrder = -this._sorterOrder;
            }
            else
            {
                this._sorterOrder = 1;
            }
            this._previousIndex = e.ColumnIndex;
            ListComparison(dgvData.Columns[e.ColumnIndex].DataPropertyName);

            dgvData.Refresh();
        }
    }
}
