﻿using Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class Files
    {
        /// <summary>
        /// 没有文件, 使用 0 是因为方便存数据库
        /// </summary>
        public const int NOT_FILES = 0;
        /// <summary>
        /// 缩略图最宽
        /// </summary>
        public const double THUMBNAIL_WIDTH = 200;
        /// <summary>
        /// 缩略图最高
        /// </summary>
        public const double THUMBNAIL_HEIGHT = 200;
        /// <summary>
        /// GIF
        /// </summary>
        public const string GIF = ".GIF";
        /// <summary>
        /// 默认缩略图扩展名
        /// </summary>
        public const string DEFAULT_THUMBNAIL_EXTENSIONS = ".jpeg";

        /// <summary>
        /// 保存 WEB 文件夹路径
        /// </summary>
        public static string WebSaveDirectory => Config.GetValue("Files:UploadFilesDir");
        /// <summary>
        /// 保存 WEB 缩略图文件夹路径
        /// </summary>
        public static string WebSaveThumbnailDirectory => Config.GetValue("Files:ThumbnailsDir");
        /// <summary>
        /// 保存 WEB 临时文件夹路径
        /// </summary>
        public static string WebSaveTempDirectory => Config.GetValue("Files:TempDir");
        /// <summary>
        /// 保存缩略图文件夹路径
        /// </summary>
        public static string SaveThumbnailPath
        {
            get
            {
                string path = Directory.GetCurrentDirectory();
                path += WebSaveThumbnailDirectory;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        /// <summary>
        /// 保存文件夹路径
        /// </summary>
        public static string SavePath
        {
            get
            {
                string path = Directory.GetCurrentDirectory();
                path += WebSaveDirectory;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        /// <summary>
        /// 临时文件夹路径
        /// </summary>
        public static string SaveTempPath
        {
            get
            {
                string path = Directory.GetCurrentDirectory();
                path += WebSaveTempDirectory;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public async Task<List<DB.Tables.File>> SaveImagesAsync(List<IFormFile> files)
        {
            List<DB.Tables.File> list = new List<DB.Tables.File>(files.Count());

            foreach (IFormFile file in files)
            {
                var f = await SaveImageAsync(file);
                if (f is null)
                    continue;
                list.Add(f);
            }
            return list;
        }

        /// <summary>
        /// 保存图片, 返回图片信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<DB.Tables.File> SaveImageAsync(IFormFile file)
        {
            /*
             * 保存图片到数据库, 同时保存图片的缩略图
             */

            if (file is null)
                return null;

            //  源文件名
            string fileName = file.FileName;
            //  扩展名
            string ExtensionName = Path.GetExtension(fileName);
            //  保存名
            string saveName = string.Concat(Guid.NewGuid().ToString(), ExtensionName);
            //  源文件大小
            long size = file.Length;
            //  缩略图名字
            //  原图保存名 + '-min' + 缩略图扩展名
            string thumbnailName = saveName.Replace(ExtensionName, $"-min{DEFAULT_THUMBNAIL_EXTENSIONS}");
            //  缩略图保存路径
            string thumbnailPath = Path.Combine(SaveThumbnailPath, thumbnailName);
            //  原图的保存路径
            string sourceSavePath = Path.Combine(SavePath, saveName);
            //  保存原图
            using (Stream stream = System.IO.File.Create(sourceSavePath))
            {
                await file.CopyToAsync(stream);
            }

            string coverTempName = saveName.Replace(ExtensionName, $"-cover{DEFAULT_THUMBNAIL_EXTENSIONS}");
            string coverTempPath = null;
            //  制作缩略图的源文件
            //  静态图是源文件
            //  GIF 图是截取的封面
            string thumbnailSourcePath;

            //  是否 GIF
            //  是的话, 保存 GIF 的封面的缩略图
            if (ExtensionName.ToUpper() == GIF)
            {
                /*
                 *  先截取封面所临时缩略图
                 *  再使用里临时缩略图制作缩略图
                 */
                coverTempPath = Path.Combine(SaveTempPath, coverTempName);
                GetGIFCover(sourceSavePath, coverTempPath);
                thumbnailSourcePath = coverTempPath;
            }
            else
                thumbnailSourcePath = sourceSavePath;

            //  保存缩略图
            Task thumbnailT = MakeThumbnail(thumbnailSourcePath, thumbnailPath);

            DB.Tables.File fileModel = new DB.Tables.File
            {
                Name = fileName,
                ExtensionName = ExtensionName,
                Size = size,
                Path = Path.Combine(WebSaveDirectory, saveName),
                Thumbnail = Path.Combine(WebSaveThumbnailDirectory, thumbnailName)
            };
            using DB.DarkContext db = new DB.DarkContext();
            db.Files.Add(fileModel);
            int suc = await db.SaveChangesAsync();
            //  等待缩略图保存完成
            await thumbnailT;

            //  删除临时文件
            if (!string.IsNullOrWhiteSpace(coverTempPath))
                System.IO.File.Delete(coverTempPath);
            if (suc == 1)
                return fileModel;
            return null;
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="sourcePath">源文件</param>
        /// <param name="fileSavePath">缩略图保存路径</param>
        /// <param name="thumbnailWidth">缩略图宽</param>
        /// <param name="thumbnailHeight">缩略图高</param>
        public Task MakeThumbnail(string sourcePath, string fileSavePath, double thumbnailWidth = THUMBNAIL_WIDTH, double thumbnailHeight = THUMBNAIL_HEIGHT)
        {
            return Task.Run(() =>
            {
                //从文件取得图片对象，并使用流中嵌入的颜色管理信息
                using Image myImage = Image.FromFile(sourcePath, true);
                //缩略图宽、高
                double newWidth = myImage.Width, newHeight = myImage.Height;
                //宽大于模版的横图
                if (myImage.Width > myImage.Height || myImage.Width == myImage.Height)
                {
                    if (myImage.Width > thumbnailWidth)
                    {
                        //宽按模版，高按比例缩放
                        newWidth = thumbnailWidth;
                        newHeight = myImage.Height * (newWidth / myImage.Width);
                    }
                }
                //高大于模版的竖图
                else
                {
                    if (myImage.Height > thumbnailHeight)
                    {
                        //高按模版，宽按比例缩放
                        newHeight = thumbnailHeight;
                        newWidth = myImage.Width * (newHeight / myImage.Height);
                    }
                }
                //取得图片大小
                Size mySize = new Size((int)newWidth, (int)newHeight);
                //新建一个bmp图片
                using Image bitmap = new Bitmap(mySize.Width, mySize.Height);
                //新建一个画板
                using Graphics g = Graphics.FromImage(bitmap);
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空一下画布
                g.Clear(Color.White);
                //在指定位置画图
                g.DrawImage(myImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle(0, 0, myImage.Width, myImage.Height),
                GraphicsUnit.Pixel);

                //保存缩略图
                bitmap.Save(fileSavePath, ImageFormat.Jpeg);
            });
        }

        /// <summary>
        /// 获取 GIF 图的封面
        /// </summary>
        /// <param name="sourceFileFullPath"></param>
        /// <param name="saveFullPath"></param>
        public void GetGIFCover(string sourceFile, string saveFullPath)
        {
            using Image gif = Image.FromFile(sourceFile, true);
            FrameDimension ImgFrmDim = new FrameDimension(gif.FrameDimensionsList[0]);
            gif.SelectActiveFrame(ImgFrmDim, 0);
            gif.Save(saveFullPath, ImageFormat.Png);
        }
    }
}