using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine.LayerMap
{
    public class TileLayer
    {
        #region Fields
        private List<Texture2D> textures = new List<Texture2D>();
        private int[,] grid;
        private float alpha = 1f;
        #endregion

        #region Properties
        public float Alpha 
        {
            get { return alpha; }
            set
            {
                alpha = MathHelper.Clamp(value, 0f, 1f);
            }
        }
        public int Width
        {
            get { return grid.GetLength(1); }
        }
        public int Height
        {
            get { return grid.GetLength(0); }
        }
        public int WidthInPixels
        {
            get { return grid.GetLength(1) * Helper.TileWidth; }
        }

        public int HeightInPixels
        {
            get { return grid.GetLength(0) * Helper.TileHeight; }
        }
        #endregion

        #region Constructors
        public TileLayer(int height, int width)
        {
            grid = new int[height, width];
        }
        public TileLayer(int[,] existingMap)
        {
            grid = (int[,])existingMap.Clone();
        }
        #endregion

        #region LoadTileTextures
        public void LoadTileTextures(ContentManager content, params string[] textureUrls) 
        {
            Texture2D texture;

            foreach (string url in textureUrls)
            {
                texture = content.Load<Texture2D>(url);
                textures.Add(texture);
            }
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int textureIndex = grid[y, x];
                    if (textureIndex == -1)
                    {
                        continue;
                    }
                    Texture2D texture = textures[textureIndex];

                    spriteBatch.Draw(
                        texture,
                        new Rectangle(x * Helper.TileWidth, y * Helper.TileHeight, Helper.TileWidth, Helper.TileHeight),
                        new Color(new Vector4(1f, 1f, 1f, this.Alpha)));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, Point min, Point max)
        {

            min.X = (int)Math.Max(min.X, 0);
            min.Y = (int)Math.Max(min.Y, 0);
            max.X = (int)Math.Min(max.X, Width);
            max.Y = (int)Math.Min(max.Y, Height);

            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    int textureIndex = grid[y, x];
                    if (textureIndex == 0)
                    {
                        continue;
                    }

                    Texture2D texture = textures[textureIndex - 1];

                    spriteBatch.Draw(texture, new Rectangle(x * Helper.TileWidth, y * Helper.TileHeight, Helper.TileWidth, Helper.TileHeight),
                                     new Color(new Vector4(1f, 1f, 1f, this.Alpha)));
                }
            }
        }
        #endregion

        #region FromFile
        public static TileLayer FromFile(ContentManager content, string filename)
        {
            TileLayer layer;
            bool readingTextures = false;
            bool readingLayout = false;
            List<string> textureNames = new List<string>();
            List<List<int>> tempLayout = new List<List<int>>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.Contains("[Textures]"))
                    {
                        readingTextures = true;
                        readingLayout = false;
                    }
                    else if (line.Contains("[Layout]"))
                    {
                        readingLayout = true;
                        readingTextures = false;
                    }
                    else if (readingTextures)
                    {
                        textureNames.Add(line);
                    }
                    else if (readingLayout)
                    {
                        List<int> row = new List<int>();
                        string[] cells = line.Split(' ');
                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                            {
                                row.Add(int.Parse(c));
                            }
                        }
                        tempLayout.Add(row);
                    }
                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;
            layer = new TileLayer(height, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    layer.SetCellIndex(x, y, tempLayout[y][x]);
                }
            }
            layer.LoadTileTextures(content, textureNames.ToArray());
            return layer;
        }
        #endregion

        #region GetCellIndex
        public int GetCellIndex(int x, int y)
        {
            x = (int)MathHelper.Clamp(x, 0, Width - 1);
            y = (int)MathHelper.Clamp(y, 0, Height - 1);

            return grid[y, x];
        }

        public int GetCellIndex(Point point)
        {
            point.X = (int)MathHelper.Clamp(point.X, 0, Width - 1);
            point.Y = (int)MathHelper.Clamp(point.Y, 0, Height - 1);

            return grid[point.Y, point.X];
        }
        #endregion

        #region SetCellIndex
        public void SetCellIndex(int x, int y, int cellIndex)
        {
            grid[y, x] = cellIndex;
        }

        public void SetCellIndex(Point point, int cellIndex)
        {
            grid[point.Y, point.X] = cellIndex;
        }
        #endregion 
    }
}
