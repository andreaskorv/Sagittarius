using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sagittarius
{
    enum sost { Create, Set, Set1, Set2, Play, MoveUnit }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int iCount;
        int iCurrentElement;
        double dR;
        bool bVisibility;
        sost m_Sost;
        Line l1;
        List<Unit> m_lMyunits = new List<Unit>();
        List<Unit> m_lUnitsofEnemy = new List<Unit>();
        public MainWindow()
        {
            InitializeComponent();
            EnterCount ocr = new EnterCount();
            if (ocr.ShowDialog() == true)
            {
                iCount = ocr.iCount;
                m_Sost = sost.Create;
                bVisibility = ocr.bVisibility;
            }
            l1 = new Line();
            l1.Tag = -1;
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 2;
        }

        


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.Create && m_lMyunits.Count < iCount)
            {
                Ellipse el = new Ellipse();
                canvas.Children.Add(el);
                el.Height = 40;
                el.Width = 40;
                el.StrokeThickness = 1;
                el.Fill = Brushes.Red;
                el.Stroke = Brushes.Green;
                el.MouseDown += El_MouseDown;
                el.Tag = m_lMyunits.Count;
                Canvas.SetLeft(el, Mouse.GetPosition(canvas).X - 20);
                Canvas.SetTop(el, Mouse.GetPosition(canvas).Y - 20);
                m_lMyunits.Add(new Unit(Mouse.GetPosition(canvas).X, Mouse.GetPosition(canvas).Y, el));
            }
            else if (m_Sost==sost.Set1)
            {
                double x = Mouse.GetPosition(canvas).X;
                double y = Mouse.GetPosition(canvas).Y;
                if (m_lMyunits[iCurrentElement].IsCheck(x, y))
                {
                    if (x <= m_lMyunits[iCurrentElement].x)
                        if (y > m_lMyunits[iCurrentElement].y)
                            m_lMyunits[iCurrentElement].first_r = Double.MaxValue;
                        else m_lMyunits[iCurrentElement].first_r = Double.MinValue;
                    else
                        m_lMyunits[iCurrentElement].first_r = (m_lMyunits[iCurrentElement].x - x) / (m_lMyunits[iCurrentElement].y - y);
                    l1.Tag = iCurrentElement;
                    l1 = new Line();
                    l1.Tag = -1;
                    l1.Stroke = Brushes.Black;
                    l1.StrokeThickness = 2;
                    canvas.Children.Add(l1);
                    m_Sost = sost.Set2;
                }
            }
            else if (m_Sost == sost.Set2)
            {
                double x = Mouse.GetPosition(canvas).X;
                double y = Mouse.GetPosition(canvas).Y;
                if (x <= m_lMyunits[iCurrentElement].x)
                    if (y > m_lMyunits[iCurrentElement].y)
                        m_lMyunits[iCurrentElement].second_r = Double.MaxValue;
                    else m_lMyunits[iCurrentElement].second_r = Double.MinValue;
                else m_lMyunits[iCurrentElement].second_r = (m_lMyunits[iCurrentElement].x - x) / (m_lMyunits[iCurrentElement].y - y);
                m_Sost = sost.Create;
                l1.Tag = iCurrentElement;
                l1 = new Line();
                l1.Tag = -1;
                l1.Stroke = Brushes.Black;
                l1.StrokeThickness = 2;
                canvas.Children.Add(l1);
                button1.IsEnabled = true;
                if (IsUnitsSet())
                    button.IsEnabled = true;
            }
        }

        private bool IsPlaceAvailable(double x, double y)
        {
            foreach (Unit u in m_lUnitsofEnemy)
                if (!u.IsCheck_(x, y))
                    return false;
            return true;
        }

        private void vPrepare()
        {
            button1.IsEnabled = false;
            button.IsEnabled = false;
            for (int i = 0; i < m_lMyunits.Count; i++)
            {
                double x, y;
                Random rnd = new Random();
                do
                {
                    x = rnd.Next((int)canvasofenemies.Width);
                    y = rnd.Next((int)canvasofenemies.Height);
                }
                while (!IsPlaceAvailable(x, y));
                Ellipse el = new Ellipse();
                canvasofenemies.Children.Add(el);
                el.Height = 40;
               // el. = bVisibility;
                el.Width = 40;
                el.StrokeThickness = 1;
                el.Fill = Brushes.Red;
                el.Stroke = Brushes.Green;
                el.Tag = m_lUnitsofEnemy.Count; ;
                el.MouseLeftButtonDown += El_MouseLeftButtonDown;
                Canvas.SetLeft(el, x - 20);
                Canvas.SetTop(el, y - 20);
                m_lUnitsofEnemy.Add(new Unit(x, y, el));
                m_lUnitsofEnemy.Last().first_r = rnd.NextDouble() * Double.MaxValue;
                m_lUnitsofEnemy.Last().second_r = rnd.NextDouble() * Double.MinValue;
            }
            m_Sost = sost.Play;
            vSimulator();
        }

        private void El_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.Play)
                vShoot((int)(sender as Ellipse).Tag, false);
        }

        void vReset()
        {
            for (int i = 0; i < m_lMyunits.Count; i++)
                m_lMyunits[i].m_El.Tag = i;
            for (int i = 0; i < m_lUnitsofEnemy.Count; i++)
                m_lUnitsofEnemy[i].m_El.Tag = i;
        }

        private void El_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (m_Sost == sost.Set)
            {
                Line l2 = new Line();
                if (!canvas.Children.Contains(l1)) canvas.Children.Add(l1);
                m_Sost = sost.Set1;
                iCurrentElement = (int)(sender as Ellipse).Tag;
                for (int i=canvas.Children.Count-1; i>=0; i--)
                    if (canvas.Children[i].GetType() == l2.GetType() && (int)((canvas.Children[i] as Line).Tag) == iCurrentElement)
                        canvas.Children.Remove(canvas.Children[i]);
            }
            else if (m_Sost==sost.Create)
            {
                m_Sost = sost.MoveUnit;
                iCurrentElement = (int)(sender as Ellipse).Tag;
            }
        }

        private bool IsUnitsSet()
        {
            if (m_lMyunits.Count == iCount)
            {
                foreach (Unit u in m_lMyunits)
                    if (!u.IsSet)
                        return false;
                return true;
            }
            else return false;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_Sost == sost.Set1 || m_Sost == sost.Set2)
            {
                l1.X1 = m_lMyunits[iCurrentElement].x;
                l1.Y1 = m_lMyunits[iCurrentElement].y;
                l1.X2 = Mouse.GetPosition(canvas).X;
                l1.Y2 = Mouse.GetPosition(canvas).Y;
            }
            else if (m_Sost==sost.MoveUnit)
            {
                double x = Mouse.GetPosition(canvas).X;
                double y = Mouse.GetPosition(canvas).Y;
                Canvas.SetLeft(m_lMyunits[iCurrentElement].m_El, Mouse.GetPosition(canvas).X - 20);
                Canvas.SetTop(m_lMyunits[iCurrentElement].m_El, Mouse.GetPosition(canvas).Y - 20);
                foreach (UIElement uie in canvas.Children)
                    if (uie.GetType() == l1.GetType() && (int)((uie as Line).Tag) == iCurrentElement)
                    {
                        (uie as Line).X1 += (x - m_lMyunits[iCurrentElement].x);
                        (uie as Line).X2 += (x - m_lMyunits[iCurrentElement].x);
                        (uie as Line).Y1 += (y - m_lMyunits[iCurrentElement].y);
                        (uie as Line).Y2 += (y - m_lMyunits[iCurrentElement].y);
                    }
                m_lMyunits[iCurrentElement].x = x;
                m_lMyunits[iCurrentElement].y = y;
                // вписать сюда код перемещения юнита
            }
        }

        void vShoot(int iNumberOfShooter, bool fl)
        {
            double k = 0;
            double b = 0;
            // придумать, как мы будем заполнять эти элементы! Это угол наклона траектории полёта снаряда и координата начала

            var listofhurtedunits = from u in m_lUnitsofEnemy orderby u.x select u;
            if (fl)
                listofhurtedunits = from u in m_lMyunits orderby u.x descending select u;
            foreach (Unit u in listofhurtedunits)
                if (u.IsHurt(k, b))
                {
                    // вставим сюда красную линию, показывающую смерть!
                    m_lMyunits.Remove(u);
                    m_lUnitsofEnemy.Remove(u);
                    break;
                }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            m_Sost = sost.Set;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            vPrepare();
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.MoveUnit)
                m_Sost = sost.Create;
        }

        private void vSimulator()
        {
            Random rnd = new Random();
            do
            {
                vShoot(rnd.Next(m_lMyunits.Count), false);
                vShoot(rnd.Next(m_lUnitsofEnemy.Count), true);
                vReset();
                System.Threading.Thread.Sleep(1000);
            }
            while (true);
        }
    }
}