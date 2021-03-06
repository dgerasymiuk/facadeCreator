﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.Services
{
    [ComVisible(false)]
    public class DialogsService
    {
        private static RegeditService regService = new RegeditService();
        public static Bitmap getImageFromDisk() {
            Bitmap image = null;
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = regService.getOpenFilePath();
            openFileDialog1.Filter = "Jpeg files (*.jpg)|*.jpg|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image = ImageConversion.getImage(openFileDialog1.FileName);
                    regService.setOpenFilePath(openFileDialog1.FileName);
                    //if ((myStream = openFileDialog1.OpenFile()) != null)
                    //{
                    //    using (myStream)
                    //    {
                    //        image = new Bitmap(myStream);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            return image;
        }
        public static bool SaveFacadesToDisk(ICollection<Image> images)
        {
            return false;
        }
    }
}
