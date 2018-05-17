using System;
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
        public static double dMaxAbsR = 1.5200, iX = 30, radius = 20;
        public Unit(double x, double y, Ellipse el)
        {
            this.x = x;
            this.y = y;
            el.Height = 40;
            el.Width = 40;
            el.StrokeThickness = 1;

            m_El = el;
            first_r = 0;
            second_r = 0;
            IsSet = false;
        }

        public void SetFirstR(double x, double y, bool? IsEnemy)
        {
            if (IsEnemy == true)
            {
                if (x >= this.x)
                    if (y > this.y)
                        first_r = dMaxAbsR;
                    else first_r = -dMaxAbsR;
                else
                    first_r = (y - this.y) / (x - this.x);
            }
            else
            {
                if (x <= this.x)
                    if (y > this.y)
                        first_r = dMaxAbsR;
                    else first_r = -dMaxAbsR;
                else
                    first_r = (y - this.y) / (x - this.x);
            }
        }

        public void SetSecondR(double x, double y, bool? bIsSetEnemy)
        {
            if (bIsSetEnemy == true)
            {
                if (x >= this.x)
                    if (y > this.y)
                        second_r = dMaxAbsR;
                    else second_r = -dMaxAbsR;
                else
                    second_r = (y - this.y) / (x - this.x);
            }
            else
            {
                if (x <= this.x)
                    if (y > this.y)
                        second_r = dMaxAbsR;
                    else second_r = -dMaxAbsR;
                else
                    second_r = (y - this.y) / (x - this.x);
            }
            IsSet = true;
        }

        public void vCorrect(bool IsUnitOur)
        {
            if (IsUnitOur)
            {
                if (first_r <= -dMaxAbsR)
                    first_r = -dMaxAbsR;
                if (second_r <= -dMaxAbsR)
                    second_r = -dMaxAbsR;
                if (first_r >= dMaxAbsR)
                    first_r = dMaxAbsR;
                if (second_r >= dMaxAbsR)
                    second_r = dMaxAbsR;
                m_l1.X2 = m_l1.X1 + iX;
                m_l1.Y2 = m_l1.Y1 + iX * first_r;
                m_l2.X2 = m_l2.X1 + iX;
                m_l2.Y2 = m_l2.Y1 + iX * second_r;
            }
            else
            {
                if (first_r <= -dMaxAbsR)
                    first_r = -dMaxAbsR;
                if (second_r <= -dMaxAbsR)
                    second_r = -dMaxAbsR;
                if (first_r >= dMaxAbsR)
                    first_r = dMaxAbsR;
                if (second_r >= dMaxAbsR)
                    second_r = dMaxAbsR;
                m_l1.X2 = m_l1.X1 - iX;
                m_l1.Y2 = m_l1.Y1 - iX * first_r;
                m_l2.X2 = m_l2.X1 - iX;
                m_l2.Y2 = m_l2.Y1 - iX * second_r;
            }
        }

        public double GetR()
        {
            if (first_r > second_r)
            {
                double x = first_r;
                first_r = second_r;
                second_r = x;
            }
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
