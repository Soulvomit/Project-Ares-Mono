using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using TileEngine.Sprite;

namespace TileEngine.LayerMap
{
    public class CollisionLayer
    {
        #region Fields
        private int[,] grid;
        #endregion

        #region Events
        public delegate void CollisionTwoHandler(object sender, CustomCollisionArgs cca);
        public event CollisionTwoHandler OnCollisionTwo;
        public delegate void CollisionThreeHandler(object sender, CustomCollisionArgs cca);
        public event CollisionThreeHandler OnCollisionThree;
        public delegate void CollisionFourHandler(object sender, CustomCollisionArgs cca);
        public event CollisionFourHandler OnCollisionFour;
        public delegate void CollisionFiveHandler(object sender, CustomCollisionArgs cca);
        public event CollisionFiveHandler OnCollisionFive;
        public delegate void CollisionSixHandler(object sender, CustomCollisionArgs cca);
        public event CollisionSixHandler OnCollisionSix;
        public delegate void CollisionSevenHandler(object sender, CustomCollisionArgs cca);
        public event CollisionSevenHandler OnCollisionSeven;
        public delegate void CollisionEightHandler(object sender, CustomCollisionArgs cca);
        public event CollisionEightHandler OnCollisionEight;
        public delegate void CollisionNineHandler(object sender, CustomCollisionArgs cca);
        public event CollisionNineHandler OnCollisionNine;
        #endregion

        #region Properties
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
        public CollisionLayer(int height, int width)
        {
            grid = new int[height, width];
        }
        #endregion

        #region FromFile
        public static CollisionLayer FromFile(string filename)
        {
            CollisionLayer layer;
            bool readingLayout = false;
            List<List<int>> tempLayout = new List<List<int>>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.Contains("[Layout]"))
                    {
                        readingLayout = true;
                    }
                    else if (readingLayout)
                    {
                        List<int> row = new List<int>();
                        string[] cells = line.Split(' ');
                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }
                        tempLayout.Add(row);
                    }
                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;
            layer = new CollisionLayer(height, width);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    layer.SetCellIndex(x, y, tempLayout[y][x]);

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
            //lock(grid)
            //{
                grid[point.Y, point.X] = cellIndex;
            //}
        }
        #endregion 

        #region HandleCustomEffects
        public void HandleCustomEffects(Vector2 spritePosition)
        {
            Point cell = Helper.VPointToCell(spritePosition);

            int colIndex = GetCellIndex(cell);

            if (colIndex != 0 || colIndex != 2)
            {
                if (colIndex == 2)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 2 @" + cell);
                    OnCollisionTwo(this, cca);
                }
                else if (colIndex == 3)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 3 @" + cell);
                    OnCollisionThree(this, cca);
                }
                else if (colIndex == 4)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 4 @" + cell);
                    OnCollisionFour(this, cca);
                }
                else if (colIndex == 5)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 5 @" + cell);
                    OnCollisionFive(this, cca);
                }
                else if (colIndex == 6)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 6 @" + cell);
                    OnCollisionSix(this, cca);
                }
                else if (colIndex == 7)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 7 @" + cell);
                    OnCollisionSeven(this, cca);
                }
                else if (colIndex == 8)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 8 @" + cell);
                    OnCollisionEight(this, cca);
                }
                else if (colIndex == 9)
                {
                    CustomCollisionArgs cca = new CustomCollisionArgs("Collision with cell index 9 @" + cell);
                    OnCollisionNine(this, cca);
                }
            }
        }

        public Vector2 HandleCustomEffects(Vector2 motionVector, ISprite sprite)
        {
            Point cell = Helper.VPointToCell(sprite.Center);

            int colIndex = GetCellIndex(cell);

            if (colIndex == 2)
            {
                if (sprite.GetType() == typeof(AnimatedSprite))
                    ((AnimatedSprite)sprite).CurrentAnimation.FramesPerSecond = 3;
                return motionVector * .5f;
            }
            if (colIndex == 3)
            {
                if (sprite.GetType() == typeof(AnimatedSprite))
                    ((AnimatedSprite)sprite).CurrentAnimation.FramesPerSecond = 10;
                return motionVector * 1.5f;
            }
            if (sprite.GetType() == typeof(AnimatedSprite))
                ((AnimatedSprite)sprite).CurrentAnimation.FramesPerSecond = 7;
            return motionVector;
        }
        #endregion

        #region HandleBlockedCells
        public void HandleBlockedCells(MovableObject mo)
        {
            byte intersectionDepth = 24;
            //get the 2d grid position the sprite center currently occupies
            Point spriteCell = Helper.VPointToCell(mo.Sprite.Center);
            //create nullable points for each possible neighboring location
            Point? upLeft = null, up = null, upRight = null,
                   left = null, right = null,
                   downLeft = null, down = null, downRight = null;

            //set neighboring points if they exist
            if (spriteCell.Y > 0)
                up = new Point(spriteCell.X, spriteCell.Y - 1);

            if (spriteCell.Y < Height - 1)
                down = new Point(spriteCell.X, spriteCell.Y + 1);

            if (spriteCell.X > 0)
                left = new Point(spriteCell.X - 1, spriteCell.Y);

            if (spriteCell.X < Width - 1)
                right = new Point(spriteCell.X + 1, spriteCell.Y);

            if (spriteCell.X > 0 && spriteCell.Y > 0)
                upLeft = new Point(spriteCell.X - 1, spriteCell.Y - 1);

            if (spriteCell.X < Width - 1 && spriteCell.Y > 0)
                upRight = new Point(spriteCell.X + 1, spriteCell.Y - 1);

            if (spriteCell.X > 0 && spriteCell.Y < Height - 1)
                downLeft = new Point(spriteCell.X - 1, spriteCell.Y + 1);

            if (spriteCell.X < Width - 1 && spriteCell.Y < Height - 1)
                downRight = new Point(spriteCell.X + 1, spriteCell.Y + 1);

            //if cell exists and collision is active on layer
            if (up != null && GetCellIndex(up.Value) == 0)
            {
                //create a rectangle around neighboring point
                Rectangle cellRect = Helper.RectangleForCell(up.Value);

                //if cell intersects with sprite bounds
                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if(temp.Y < -intersectionDepth)
                    {
                        //handle collision
                        mo.Sprite.PositionY = spriteCell.Y * Helper.TileHeight - intersectionDepth;
                        mo.CurrentSpeed = 0;
                        //mo.CurrentSpeed = mo.MinimalSpeed;
                        //mo.Bounce();
                    }
                }
            }
            //repeat for the 8 neighbours
            if (down != null && GetCellIndex(down.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(down.Value);

                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.Y > intersectionDepth)
                    {
                        mo.Sprite.PositionY = down.Value.Y * Helper.TileHeight - mo.Sprite.Bounds.Height + intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (left != null && GetCellIndex(left.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(left.Value);
                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X < -intersectionDepth)
                    {
                        mo.Sprite.PositionX = spriteCell.X * Helper.TileWidth - intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (right != null && GetCellIndex(right.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(right.Value);
                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X > intersectionDepth)
                    {
                        mo.Sprite.PositionX = right.Value.X * Helper.TileWidth - mo.Sprite.Bounds.Width + intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (upLeft != null && GetCellIndex(upLeft.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(upLeft.Value);
                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X < -intersectionDepth && temp.Y < -intersectionDepth)
                    {
                        mo.Sprite.PositionX = spriteCell.X * Helper.TileWidth - intersectionDepth;
                        mo.Sprite.PositionY = spriteCell.Y * Helper.TileHeight - intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (upRight != null && GetCellIndex(upRight.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(upRight.Value);

                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X > intersectionDepth && temp.Y < -intersectionDepth)
                    {
                        mo.Sprite.PositionX = right.Value.X * Helper.TileWidth - mo.Sprite.Bounds.Width + intersectionDepth;
                        mo.Sprite.PositionY = spriteCell.Y * Helper.TileHeight - intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (downLeft != null && GetCellIndex(downLeft.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(downLeft.Value);

                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X < -intersectionDepth && temp.Y > intersectionDepth)
                    {
                        mo.Sprite.PositionY = down.Value.Y * Helper.TileHeight - mo.Sprite.Bounds.Height + intersectionDepth;
                        mo.Sprite.PositionX = spriteCell.X * Helper.TileWidth - intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
            if (downRight != null && GetCellIndex(downRight.Value) == 0)
            {
                Rectangle cellRect = Helper.RectangleForCell(downRight.Value);

                if (cellRect.Intersects(mo.Sprite.Bounds))
                {
                    Vector2 temp = Helper.GetIntersectionDepth(cellRect, mo.Sprite.Bounds);
                    if (temp.X > intersectionDepth && temp.Y > intersectionDepth)
                    {
                        mo.Sprite.PositionX = right.Value.X * Helper.TileWidth - mo.Sprite.Bounds.Width + intersectionDepth;
                        mo.Sprite.PositionY = down.Value.Y * Helper.TileHeight - mo.Sprite.Bounds.Height + intersectionDepth;
                        mo.CurrentSpeed = 0;
                    }
                }
            }
        }
        #endregion
    }
}
