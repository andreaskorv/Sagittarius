﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Sagittarius
{
    class Unit
    {
        public double x;
        public double y;
        public double first_r;
        public double second_r;
        public bool IsSet;
        public Ellipse m_El;
        public Line m_l1, m_l2;
        static double radius = 20;
        public Unit(double x, double y, Ellipse el)
        {
            this.x = x;
            this.y = y;
            m_El = el;
            first_r = 0;
            second_r = 0;
            IsSet = false;
        }

        public double GetR()
        {
            Random rnd = new Random();
            double for_return = first_r + (second_r - first_r) * rnd.NextDouble();
            while (for_return < first_r || for_return > second_r)
                for_return = (first_r + (second_r - first_r) * rnd.NextDouble());
            return for_return;
        }

        public bool IsCheck(double x, double y)
        {
            return Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)) > radius;
        }

        public bool IsCheck_(double x, double y)
        {
            return Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)) > radius * 2;
        }

        public bool IsHurt(double k, double b)
        {
            for (double x = this.x - 20; x <= this.x + 20; x += 0.1)
                if (!IsCheck(x, k * x + b))
                {
                    this.x = x;
                    y = k * x + b;
                    // чтобы прорисовать "линию выстрела"
                    return true;
                }
            return false;
        }
    }
}
