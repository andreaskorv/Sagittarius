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
//        public bool? first_r_;
//        public bool? second_r_;
        public Ellipse m_El;
        static double radius = 20;
        public Unit(double x, double y, Ellipse el)
        {
            this.x = x;
            this.y = y;
            m_El = el;
            first_r = 0;
            second_r = 0;
            IsSet = false;
//            first_r_ = null;
//            second_r_ = null;
        }

        public double GetR()
        {
            Random rnd = new Random();
            return (first_r + (second_r - first_r) * rnd.NextDouble());
        }

        //public bool IsSet()
        //{
        //    return (first_r != 0 && second_r != 0);
        //}

        public double[] rad
        {
            set
            {
                first_r = value[0];
                second_r = value[1];
            }
            get
            {
                if (first_r == 0 && second_r == 0)
                    return null;
                else return new double[] { first_r, second_r };
            }
        }
        public bool IsCheck(double x, double y)
        {
            return Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)) > radius;
        }

        public bool IsCheck_(double x, double y)
        {
            return Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)) > radius*2;
        }

        public bool IsHurt(double k, double b)
        {
            for (double x = this.x - 20; x <= this.x + 20; x += 0.1)
                if (!IsCheck(x, k * x + b))
                    return true;
            return false;
        }
    }
}
