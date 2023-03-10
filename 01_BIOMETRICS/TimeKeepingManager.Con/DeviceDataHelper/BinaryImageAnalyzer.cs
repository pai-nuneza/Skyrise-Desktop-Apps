using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

namespace TimeKeepingManager.Con
{
    public class BinaryImageAnalyzer
    {
        private string ImageDirectory = Application.StartupPath + @"\EmployeeImage";

        public BinaryImageAnalyzer(String DeviceName)
        {
            ImageDirectory = ImageDirectory + (DeviceName.StartsWith(@"\") ? DeviceName : (@"\" + DeviceName));
        }

        public String BinaryToImageFile(DataRow drEmpImage)
        {
            return BinaryToImageFile(drEmpImage, false);
        }

        public String BinaryToImageFile(DataRow drEmpImage, bool CreateNew)
        {
            String ImageFile = "";
            if (drEmpImage != null)
            {
                try
                {
                    string ExpectedFile = ImageDirectory + @"\" + drEmpImage["EmployeeID"].ToString().Trim() + ".jpg";

                    if (!Directory.Exists(ImageDirectory))
                        Directory.CreateDirectory(ImageDirectory);
                    if (CreateNew)
                    {
                        if (File.Exists(ExpectedFile))
                        {
                            File.Delete(ExpectedFile);
                        }
                    }

                    try
                    {
                        if (!CreateNew && File.Exists(ExpectedFile))
                        {
                            //File already exist
                            ImageFile = ExpectedFile;
                        }
                        else
                        {


                            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                                NewMemoryStream = new MemoryStream())
                            {
                                byte[] PassedImage = ((byte[])(drEmpImage["Picture"]));
                                int LargestSide = 250;

                                // write the string to the stream  
                                StartMemoryStream.Write(PassedImage, 0, PassedImage.Length);

                                // create the start Bitmap from the MemoryStream that contains the image  
                                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                                // set thumbnail height and width proportional to the original image.  
                                int newHeight;
                                int newWidth;
                                double HW_ratio;
                                if (startBitmap.Height > startBitmap.Width)
                                {
                                    newHeight = LargestSide;
                                    HW_ratio = (double)((double)LargestSide / (double)startBitmap.Height);
                                    newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                                }
                                else
                                {
                                    newWidth = LargestSide;
                                    HW_ratio = (double)((double)LargestSide / (double)startBitmap.Width);
                                    newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                                }

                                // create a new Bitmap with dimensions for the thumbnail.  
                                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                                // Copy the image from the START Bitmap into the NEW Bitmap.  
                                // This will create a thumnail size of the same image.  
                                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

                                // Save this image to the specified stream in the specified format.  
                                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                                Image empImage = Image.FromStream(NewMemoryStream);
                                empImage.Save(ImageDirectory + @"\" + drEmpImage["EmployeeID"].ToString().Trim() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                ImageFile = ImageDirectory + @"\" + drEmpImage["EmployeeID"].ToString().Trim() + ".jpg";
                            }
                        }
                    }
                    catch
                    { }
                }
                catch
                { }
            }

            return ImageFile;
        }

        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        } 

        public byte[] ImageFileToBinary(String ImageFile)
        {
            string FullPath = ImageDirectory + (ImageFile.StartsWith(@"\") ? ImageFile : @"\" + ImageFile);
            //FullPath = @"E:\IMAGES\NPAX\Candy Land Xmas\SAM_1136.JPG";
            FileStream fS = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fS.Length];
            fS.Read(b, 0, (int)fS.Length);
            fS.Close();
            return b;
        }
    }
}
