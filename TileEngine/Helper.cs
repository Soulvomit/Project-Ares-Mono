using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace TileEngine
{
    public static class Helper
    {
        #region Fields
        public static int TileWidth = 64;
        public static int TileHeight = 64;
        #endregion

        #region PointToCell
        /// <summary>
        /// Creates a 2D grid position from a pixel position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point PointToCell(Point position)
        {
            return new Point((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Vector2 PointToCell(Vector2 position)
        {
            return new Vector2((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Point VPointToCell(Vector2 position)
        {
            return new Point((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Vector2 VPointToCell(Point position)
        {
            return new Vector2((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        #endregion

        #region CellToPoint
        /// <summary>
        /// Converts a given cell to a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 CellToPoint(Point point)
        {
            Vector2 vector = Vector2.Zero;
            vector.X = point.X * TileWidth;
            vector.Y = point.Y * TileHeight;
            return vector;
        }
        #endregion

        #region CellToCellCenterPoint
        /// <summary>
        /// Offsets the cell position to a cell center point.
        /// </summary>
        /// <param name="x">Cell X position.</param>
        /// <param name="y">Cell Y position</param>
        /// <param name="spriteSize">Further offsets with a sprite size. Use to find the precise spot for placing a sprite in a cell. Set to Vector2.Zero if not needed</param>
        /// <returns></returns>
        public static Vector2 CellToCellCenterPoint(int x, int y, Vector2 spriteSize)
        {
            if (spriteSize != Vector2.Zero)
            {
                return CellToPoint(new Point(x, y)) + 
                                                    new Vector2
                                                    (
                                                        (Helper.TileWidth / 2) - (spriteSize.X / 2),
                                                        (Helper.TileHeight / 2) - (spriteSize.Y / 2)
                                                    );
            }
            else
            {
                return CellToPoint(new Point(x, y)) + 
                                                    new Vector2
                                                    (
                                                        (Helper.TileWidth / 2),
                                                        (Helper.TileHeight / 2)
                                                    );
            }
        }
        #endregion

        #region PointToCellCenterPoint
        /// <summary>
        /// Offsets the point position to a cell center point.
        /// </summary>
        /// <param name="x">Cell X position.</param>
        /// <param name="y">Cell Y position</param>
        /// <param name="spriteSize">Further offsets with a sprite size. Use to find the precise spot for placing a sprite in a cell. Set to Vector2.Zero if not needed</param>
        /// <returns></returns>
        public static Vector2 PointToCellCenterPoint(float x, float y, Vector2 spriteSize)
        {
            Point cell = VPointToCell(new Vector2(x, y));

            if (spriteSize != Vector2.Zero)
            {
                return CellToPoint(cell) +
                                        new Vector2
                                        (
                                            (Helper.TileWidth / 2) - (spriteSize.X / 2),
                                            (Helper.TileHeight / 2) - (spriteSize.Y / 2)
                                        );
            }
            else
            {
                return CellToPoint(cell) +
                                        new Vector2
                                        (
                                            (Helper.TileWidth / 2),
                                            (Helper.TileHeight / 2)
                                        );
            }
        }
        #endregion

        #region PointArrToCellCenter
        public static Vector2[] PointArrToCellCenter(Vector2[] arr, Vector2 size)
        {
            Vector2[] newArr = new Vector2[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = PointToCellCenterPoint(arr[i].X, arr[i].Y, size);
            }
            return newArr;
        }
        #endregion

        #region PointLinkedToCellCenter
        public static LinkedList<Vector2> PointLinkedToCellCenter(LinkedList<Vector2> list, Vector2 size)
        {
            LinkedList<Vector2> newList = new LinkedList<Vector2>();

            foreach (Vector2 vector in list)
            {
                newList.AddLast(PointToCellCenterPoint(vector.X, vector.Y, size));
            }

            return newList;
        }
        #endregion

        #region RectangleForCell
        public static Rectangle RectangleForCell(Point cell)
        {
            return new Rectangle(cell.X * TileWidth, cell.Y * TileHeight, TileWidth, TileHeight);
        }
        #endregion

        #region ToVector2
        public static Vector2 ToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        #endregion

        #region ToPoint
        public static Point ToPoint(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
        #endregion

        #region GetIntersectionDepth
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            //calculate half sizes
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            //calculate centers
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            //calculate current and minimum-non-intersecting distances between centers
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            //if we are not intersecting at all, return (0, 0)
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            //calculate and return intersection depths
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
        #endregion

        #region LinearInterpolations
        public static byte LinearInterpolate(byte a, byte b, double t)
        {
            return (byte)(a * (1 - t) + b * t);
        }
        public static float LinearInterpolate(float a, float b, double t)
        {
            return (float)(a * (1 - t) + b * t);
        }
        public static Vector2 LinearInterpolate(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t));
        }
        public static Vector4 LinearInterpolate(Vector4 a, Vector4 b, double t)
        {
            return new Vector4(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t), LinearInterpolate(a.Z, b.Z, t), LinearInterpolate(a.W, b.W, t));
        }
        public static Color LinearInterpolate(Color a, Color b, double t)
        {
            return new Color(LinearInterpolate(a.R, b.R, t), LinearInterpolate(a.G, b.G, t), LinearInterpolate(a.B, b.B, t), LinearInterpolate(a.A, b.A, t));
        }
        public static Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }
        #endregion

        #region CurveAngle
        public static float CurveAngle(float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = LinearInterpolate(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }
        #endregion
    }
}
