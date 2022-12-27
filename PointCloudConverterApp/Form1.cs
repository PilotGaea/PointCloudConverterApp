using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PilotGaea.Serialize;
using PilotGaea.TMPEngine;
using PilotGaea.Geometry;

namespace PointCloudConverterApp
{
    public partial class Form1 : Form
    {
        CPointCloudMaker m_Maker = null;
        Stopwatch m_Stopwatch = new Stopwatch();
        public Form1()
        {
            InitializeComponent();

            //加入功能列表
            List<string> featureNames = new List<string>();
            featureNames.Add("基本");
            comboBox_Features.Items.AddRange(featureNames.ToArray());
            comboBox_Features.SelectedIndex = 0;
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            EnableUI(false);
            //     生成一點雲圖層
            System.Environment.CurrentDirectory = @"C:\Program Files\PilotGaea\TileMap";//為了順利存取安裝目錄下的相關DLL
            m_Maker = new CPointCloudMaker();
            //設定必要參數
            //     圖層名稱
            string layerName = "test";
            //   DB路徑:
            string layerDBFile = string.Format(@"{0}\..\output\pointcloud_maker.DB", Application.StartupPath);
            //     點雲檔案路徑(.las)
            string[] pointCloudFileNames = new string[] { string.Format(@"{0}\..\data\pointcloud_maker\Test_point_cloud.las", Application.StartupPath) };
            //     參照的地形圖層名稱
            string terrainName = "terrain";
            //     地形圖層位於的資料庫路徑
            string terrainDBFile = string.Format(@"{0}\..\data\terrain_maker\terrain.DB", Application.StartupPath);
            //     來源EPSG
            int sourceEPSG = 3857;
            //     最大點數
            int maxPointCount = 20000;
            //     插入點位移
            GeoPoint offset = null;
            //     插入點Z軸旋轉值
            double rotateZ = 0;

            //監聽轉檔事件
            m_Maker.CreateLayerCompleted += M_Maker_CreateCompletedEvent;
            m_Maker.ProgressPercentChanged += M_Maker_ProgressChangedEvent;

            //設定進階參數
            switch (comboBox_Features.SelectedIndex)
            {
                case 0://"基本"
                    break;
            }


            m_Stopwatch.Restart();
            //開始非同步轉檔
            bool ret = m_Maker.Create(layerName, layerDBFile, pointCloudFileNames, sourceEPSG, EXPORT_TYPE.LET_DB, offset, rotateZ, terrainDBFile, terrainName, maxPointCount);

            string message = string.Format("參數檢查{0}", (ret ? "通過" : "失敗"));
            listBox_Main.Items.Add(message);
        }

        private void M_Maker_ProgressChangedEvent(double percent)
        {
            progressBar_Main.Value = Convert.ToInt32(percent);
        }


        private void M_Maker_CreateCompletedEvent(string layerName, bool isSuccess, string errorMessage)
        {
            m_Stopwatch.Stop();
            string message = string.Format("轉檔{0}", (isSuccess ? "成功" : "失敗"));
            listBox_Main.Items.Add(message);
            message = string.Format("耗時{0}分。", m_Stopwatch.Elapsed.TotalMinutes.ToString("0.00"));
            listBox_Main.Items.Add(message);
        }


        private void EnableUI(bool enable)
        {
            button_Start.Enabled = enable;
            comboBox_Features.Enabled = enable;
        }
    }
}