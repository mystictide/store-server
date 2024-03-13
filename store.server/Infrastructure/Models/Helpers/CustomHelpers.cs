using SkiaSharp;
using store.server.Infrastructure.Models.Product;
using store.server.Infrastructure.Data.Repo.Helpers;

namespace store.server.Helpers
{
    public class CustomHelpers
    {
        public static string SeralizeFilterCategories(List<string> Categories)
        {
            string categories = "";
            for (int i = 0; i < Categories.Count; i++)
            {
                categories += "'%" + Categories[i] + "%'";
                if ((i + 1) != Categories.Count)
                {
                    categories += ",";
                }
            }
            return categories;
        }

        public static string SeralizeFilterBrands(List<string> Brands)
        {
            string brands = "";
            for (int i = 0; i < Brands.Count; i++)
            {
                brands += "'%" + Brands[i] + "%'";
                if ((i + 1) != Brands.Count)
                {
                    brands += ",";
                }
            }
            return brands;
        }

        public static string SeralizeFilterMaterials(List<string> Materials)
        {
            string materials = "";
            for (int i = 0; i < Materials.Count; i++)
            {
                materials += "'%" + Materials[i] + "%'";
                if ((i + 1) != Materials.Count)
                {
                    materials += ",";
                }
            }
            return materials;
        }

        public static string SeralizeFilterColors(List<string> Colors)
        {
            string colors = "";
            for (int i = 0; i < Colors.Count; i++)
            {
                colors += "'%" + Colors[i] + "%'";
                if ((i + 1) != Colors.Count)
                {
                    colors += ",";
                }
            }
            return colors;
        }

        public static async Task<byte[]> ResizeImage(IFormFile file, int width, int height)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (SKMemoryStream sourceStream = new SKMemoryStream(stream.ToArray()))
                {
                    using (SKCodec codec = SKCodec.Create(sourceStream))
                    {
                        sourceStream.Seek(0);
                        using (SKImage image = SKImage.FromEncodedData(SKData.Create(sourceStream)))
                        {
                            int newHeight = image.Height;
                            int newWidth = image.Width;

                            if (height > 0 && newHeight > height)
                            {
                                double scale = (double)height / newHeight;
                                newHeight = height;
                                newWidth = (int)Math.Floor(newWidth * scale);
                            }
                            if (width > 0 && newWidth > width)
                            {
                                double scale = (double)width / newWidth;
                                newWidth = width;
                                newHeight = (int)Math.Floor(newHeight * scale);
                            }

                            var info = codec.Info.ColorSpace.IsSrgb ? new SKImageInfo(newWidth, newHeight) : new SKImageInfo();
                            using (SKSurface surface = SKSurface.Create(info))
                            {
                                using (SKPaint paint = new SKPaint())
                                {
                                    paint.IsAntialias = true;
                                    paint.FilterQuality = SKFilterQuality.High;

                                    surface.Canvas.Clear(SKColors.White);
                                    var rect = new SKRect(0, 0, newWidth, newHeight);
                                    surface.Canvas.DrawImage(image, rect, paint);
                                    surface.Canvas.Flush();

                                    using (SKImage newImage = surface.Snapshot())
                                    {
                                        using (SKData newImageData = newImage.Encode())
                                        {
                                            return newImageData.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<bool> WriteImage(byte[] file, string savePath, string fileName)
        {
            try
            {
                Directory.CreateDirectory(savePath);
                using (var fs = new FileStream(savePath + fileName, FileMode.Create, FileAccess.Write))
                {
                    await fs.WriteAsync(file);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }

        }

        public static async Task<string> SaveImage(int ProductID, string envPath, IFormFile file)
        {
            try
            {
                var savePath = envPath + "/media/products/" + ProductID + "/";
                var fileName = $"{Guid.NewGuid()}.jpg";

                var size = await ResizeImage(file, 600, 600);
                if (size != null)
                {
                    var write = await WriteImage(size, savePath, fileName);
                    if (write)
                    {
                        return fileName;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public static async Task<bool> DeleteImage(ProductImages image, string envPath)
        {
            try
            {
                var folderPath = envPath + "/media/products/" + image.ProductID + "/";
                var imagePath = folderPath + $"{image.Source}";

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    if (Directory.GetFiles(folderPath).Length == 0)
                    {
                        Directory.Delete(folderPath, false);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }
    }
}
