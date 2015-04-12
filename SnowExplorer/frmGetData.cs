﻿using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowExplorer
{
    public partial class frmGetData : Form
    {
        public frmGetData()
        {
            InitializeComponent();
        }

        //this is for accessing the main map from frmMain
        public DotSpatial.Controls.Map MainMap;

        private RasterSymbolizer MakeSnowSymbolizer()
        {
            RasterSymbolizer sym = new RasterSymbolizer();
            ColorScheme scheme = new ColorScheme();

            ColorCategory c1 = new ColorCategory(1, 100, Color.LightBlue, Color.LightBlue);
            c1.LegendText = "0 - 100";
            scheme.AddCategory(c1);

            ColorCategory c2 = new ColorCategory(100, 10000, Color.DarkBlue, Color.DarkBlue);
            c2.LegendText = "> 100";
            scheme.AddCategory(c2);

            sym.Scheme = scheme;

            return sym;
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            MapRasterLayer snowLayer = (MapRasterLayer)MainMap.AddRasterLayer();
            
            //we add new categories to the snow layer

            try
            {
                snowLayer.Symbolizer = MakeSnowSymbolizer();
                snowLayer.WriteBitmap();
            }
            catch { }

            this.Close();
        }

        private void btnInternet_Click(object sender, EventArgs e)
        {
            bool masked = true;

            lblProgress.Text = "Downloading snow data...";
            DataFetcher fetcher = new DataFetcher();
            string snowDataFile = fetcher.FetchSnow(dtpTime.Value, masked);
            lblProgress.Text = "Snow data downloaded: " + snowDataFile;
            lblProgress.Text = "Reading binary grid..";
            string snowBgdFile = snowDataFile.Replace(".dat", ".bgd");
            IRaster snowRaster = fetcher.MakeSnowRaster(snowDataFile, snowBgdFile);
            lblProgress.Text = "Created snow raster: " + snowBgdFile;
            //finally, we add the snow raster to our map with proper color scheme
            RasterSymbolizer sym = MakeSnowSymbolizer();
            MapRasterLayer snowLayer = new MapRasterLayer(snowRaster);
            snowLayer.Symbolizer = sym;
            MainMap.Layers.Add(snowLayer);
            snowLayer.WriteBitmap();

        }
    }
}
