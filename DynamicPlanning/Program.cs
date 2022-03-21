﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DynamicPlanning
{
    /*
     * 受到拍照范围和所要求覆盖矩形影响
     * 当要求覆盖矩形大小过大
     * 需要调整参数，使其动态规划的范围更大
     * 
     */
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            // [DllImport("kernel32.dll")]
            //static extern IntPtr GetConsoleWindow();

            //Graphics g = Graphics.FromHwnd(GetConsoleWindow());
            rect.ForEach(i => g.DrawRectangle(Pens.Red, i));
            /*
            ////测试
            //var InV = Classification(new Point(0, 0), rect);
            //InV.ForEach(i => g.DrawRectangle(Pens.Cornsilk, i));

            //var InB = ClassificationResult(new Point(0, 0), InV);
            //InB.ForEach(i => g.DrawRectangle(Pens.Cyan, i));
            */
            var InB = Total();
            //   InB.ForEach(i => g.DrawRectangle(Pens.Cyan, i));
            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
        #region 全局参数
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        private static Graphics g = Graphics.FromHwnd(GetConsoleWindow());
        private static Random rand = new();
        /// <summary>
        /// 选择框大小
        /// </summary>
        private static Point pt = new(40, 30);
        /// <summary>
        /// 输入矩形列表
        /// </summary>
        private static List<Rectangle> rect = new();
        /// <summary>
        /// 矩形列表拷贝，无须赋值
        /// </summary>
        private static List<Rectangle> rectClone = new();
        /// <summary>
        /// 最大范围
        /// </summary>
        private static Point maxRect = new(400, 400);
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="val">随机矩形个数</param>
        /// <param name="wide"></param>
        /// <param name="high"></param>
        private static void Init(int val = 200, int wide = 400, int high = 400)
        {
            for (int i = 0; i < val; i++)
            {
                Rectangle rectangle = new(rand.Next(0, wide - 10), rand.Next(0, high - 10), rand.Next(4, 4), rand.Next(4, 4));
                rect.Add(rectangle);
            }
        }
        /// <summary>
        /// 分类并排序
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private static List<Rectangle> Classification(Point start, List<Rectangle> rect)
        {
            /*
             * pt.Y*2是每次规划时的范围，适当减小，可以提高运行速度
             * 
             */
            Rectangle rectangle = new(start.X, start.Y, maxRect.X, pt.Y * 2);
            List<Rectangle> re = new();
            re.AddRange(rect.FindAll(o => rectangle.Contains(o)));
            re.Sort((a, b) => a.X.CompareTo(b.X));
            return re;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">起始点位置</param>
        /// <param name="rect">范围内兴趣涵盖矩形</param>
        /// <returns></returns>
        private static List<Rectangle> ClassificationResult(Point start, List<Rectangle> rect)
        {
            List<Rectangle> rectangles = new();
            List<Rectangle> result = new();
            List<Rectangle> rectClone = new();
            ///拷贝
            rect.ForEach(i => rectClone.Add(i));
            Rectangle rectangle = new();
            while (0 < rectClone.Count)
            {
                rectangles.Clear();
                rectangles.AddRange(rectClone.FindAll(o => (o.X >= rectClone[0].X) && ((o.X + o.Width) <= (rectClone[0].X + pt.X))));
                var minPtVal = rectangles.Min(o => o.Y);
                if ((rectClone[0].Bottom - minPtVal) <= pt.Y)
                {
                    if (minPtVal <= (start.Y + pt.Y))
                    {
                        rectangle = new Rectangle(rectClone[0].X, minPtVal, pt.X, pt.Y);
                        result.Add(rectangle);
                        rectClone.RemoveAll(o => rectangle.Contains(o));
                    }
                    else
                    {
                        rectClone.RemoveAt(0);
                    }
                }
                else
                {
                    rectClone.RemoveAt(0);
                }
            }
            Pen p = new Pen(Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));
            result.ForEach(i => g.DrawRectangle(p, i));
            return result;
        }
        private static List<Rectangle> Total()
        {
            ///拷贝
            rect.ForEach(i => rectClone.Add(i));
            List<Rectangle> re = new();
            ///兴趣部分 Interest
            while (rectClone.Count > 0)
            {
                int minVal = rectClone.Min(o => o.Y);
                var Interest = Classification(new Point(0, minVal), rectClone);
                var inb = ClassificationResult(new Point(0, minVal), Interest);
                re.AddRange(inb);
                rectClone.RemoveAll(o => Interest.Contains(o));
                for (int i = 0; i < inb.Count; i++)
                {
                    Interest.RemoveAll(o => inb[i].Contains(o));
                }
                rectClone.AddRange(Interest);
            }
            return re;
        }
    }
}
