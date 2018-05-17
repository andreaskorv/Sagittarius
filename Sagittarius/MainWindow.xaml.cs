using System;
using System.Collections.Generic;
using System.IO;
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
using System.Net;
using System.Net.Sockets;
using System.Windows.Shapes;

namespace Sagittarius
{
    enum sost { Create, Set, Set1, Set2, Play, MoveUnit }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // параметры подключения к Интернету
        private Socket Client;
        private IPAddress ipAddress = null;
        private int iPort = 0;
        bool bIsOnline = false, bIsPC, bVisibility, m_bOurShoot, m_bClean;
        bool? bIsSetEnemy = false;
        int iCount, iCurrentElement, iOurArmor, iArmorOfEnemy;
        byte iLevel;
//        const double dMaxAbsR = 1.5200, iX = 30;
        sost m_Sost;
        ImageBrush m_brushForOurUnit, m_brushForEnemyUnit;
        Line l1, redline1, redline2;
        Random rnd = new Random();
        Canvas m_CanvasUnderWork;
        List<Unit> m_lMyunits = new List<Unit>(), m_lUnitsofEnemy = new List<Unit>(), m_lUnitsUnderWork;
        System.Windows.Forms.Timer m_timer;
        public MainWindow()
        {
            iLevel = 0;
            m_brushForOurUnit = new ImageBrush(new BitmapImage(new Uri("00.png", UriKind.Relative)));
            m_brushForEnemyUnit = new ImageBrush(new BitmapImage(new Uri("01.png", UriKind.Relative)));
            m_bClean = true;
            EnterCount ocr = new EnterCount();
            InitializeComponent();
            if (ocr.ShowDialog() == true)
            {
                bIsPC = (bool)ocr.checkBox1.IsChecked;
                if (bIsPC) button.Content = "Настроить вражеские юниты";
                iCount = ocr.iCount;
                iOurArmor = iCount * 15;
                iArmorOfEnemy = iOurArmor;
                m_Sost = sost.Create;
                bVisibility = ocr.bVisibility;
            }
            else
                Close();
            brush0.ImageSource = new BitmapImage(new Uri("pole1.jpg", UriKind.Relative));
            brush1.ImageSource = new BitmapImage(new Uri("pole2.jpg", UriKind.Relative));
            m_CanvasUnderWork = canvas;
            m_lUnitsUnderWork = m_lMyunits;

            // настройки подключения к Интернету

            if (bIsOnline)
            {
                var streamReader = new StreamReader("net_settings.dat");
                string[] buffer = streamReader.ReadToEnd().Split(':');
                streamReader.Close();
                ipAddress = IPAddress.Parse(buffer[0]);
                iPort = int.Parse(buffer[1]);
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.Connect(ipAddress, iPort);
            }
            l1 = new Line();
            l1.Tag = -1;
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 2;
            m_timer = new System.Windows.Forms.Timer();
            m_timer.Interval = 900;
            m_timer.Tick += M_timer_Tick;
        }

        private void vReload()
        {
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += canvas_MouseMove;
            canvasofenemies.MouseDown -= Canvas_MouseDown;
            canvasofenemies.MouseMove -= canvas_MouseMove;
            canvas.Children.Clear();
            canvasofenemies.Children.Clear();
            m_brushForOurUnit.ImageSource = new BitmapImage(new Uri(iLevel.ToString() + "0.png", UriKind.Relative));
            m_brushForEnemyUnit.ImageSource = new BitmapImage(new Uri(iLevel.ToString() + "1.png", UriKind.Relative));
            brush0.ImageSource = new BitmapImage(new Uri(iLevel.ToString() + "1.jpg", UriKind.Relative));
            brush1.ImageSource = new BitmapImage(new Uri(iLevel.ToString() + "2.jpg", UriKind.Relative));
            m_lMyunits = new List<Unit>();
            m_lUnitsofEnemy = new List<Unit>();
            m_Sost = sost.Create;
            iOurArmor = iCount * 15;
            iArmorOfEnemy = iCount * 15;
            label1.Content = "Количество оружия: " + iOurArmor;
            label2.Content = "Количество оружия: " + iArmorOfEnemy;
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            if (!m_bClean)
            {
                int iNumberOfShooter = rnd.Next(0, m_lUnitsofEnemy.Count);
                if (m_bOurShoot)
                    iNumberOfShooter = rnd.Next(0, m_lMyunits.Count);
                vShoot(iNumberOfShooter, m_bOurShoot);
                if (iOurArmor == 0 && iArmorOfEnemy == 0)
                    if (m_lMyunits.Count > m_lUnitsofEnemy.Count)
                    {
                        m_timer.Stop();
                        if (iLevel < 3)
                        {
                            MessageBox.Show("Вы лидируете по очкам и выходите на " + (iLevel + 1) + " уровень!");
                            iLevel++;
                            vReload();
                        }
                        else
                            MessageBox.Show("Вы лидируете по очкам и выигрываете!");
                    }
                    else
                    {
                            m_timer.Stop();
                            MessageBox.Show("Вы проиграли по очкам");
                    }
                if (m_bOurShoot)
                {                    
                    if (m_lUnitsofEnemy.Count == 0)
                    {
                        m_timer.Stop();
                        if (iLevel < 3)
                        {
                            MessageBox.Show("Войско противника уничтожено, вы выходите на " + (iLevel + 1) + " уровень!");
                            iLevel++;
                            vReload();
                        }
                        else
                            MessageBox.Show("Войско противника уничтожено, вы выиграли!");
                    }
                }
                else
                {
                    if (m_lMyunits.Count == 0)
                    {
                        m_timer.Stop();
                        MessageBox.Show("Ваше войско уничтожено, вы проиграли!");
                    }
                }
                vReset();
                m_bOurShoot = !m_bOurShoot;
            }
            else
            {
                canvas.Children.Remove(redline1);
                canvasofenemies.Children.Remove(redline2);
                canvas.Children.Remove(redline2);
                canvasofenemies.Children.Remove(redline1);
            }
            m_bClean = !m_bClean;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.Create && m_lUnitsUnderWork.Count < iCount)
            {
                button1.IsEnabled = true;
                Ellipse el = new Ellipse();
                m_lUnitsUnderWork.Add(new Unit(Mouse.GetPosition(m_CanvasUnderWork).X, Mouse.GetPosition(m_CanvasUnderWork).Y, el));
                m_CanvasUnderWork.Children.Add(el);
                if (m_CanvasUnderWork == canvas)
                    el.Fill = m_brushForOurUnit;
                else
                    el.Fill = m_brushForEnemyUnit;
                el.Stroke = Brushes.Green;
                el.MouseDown += El_MouseDown;
                el.Tag = m_lUnitsUnderWork.Count;
                Canvas.SetLeft(el, m_lUnitsUnderWork.Last().x - 20);
                Canvas.SetTop(el, m_lUnitsUnderWork.Last().y - 20);
            }
            else if (m_Sost==sost.Set1)
            {
                double x = Mouse.GetPosition(m_CanvasUnderWork).X;
                double y = Mouse.GetPosition(m_CanvasUnderWork).Y;
                if (m_lUnitsUnderWork[iCurrentElement].IsCheck(x, y))
                {
                    l1.Tag = iCurrentElement;
                    m_lUnitsUnderWork[iCurrentElement].m_l1 = l1;
                    m_lUnitsUnderWork[iCurrentElement].SetFirstR(x, y, bIsSetEnemy);
                    l1 = new Line();
                    l1.Tag = -1;
                    l1.Stroke = Brushes.Black;
                    l1.StrokeThickness = 2;
                    m_CanvasUnderWork.Children.Add(l1);
                    m_Sost = sost.Set2;
                }
            }
            else if (m_Sost == sost.Set2)
            {
                double x = Mouse.GetPosition(m_CanvasUnderWork).X;
                double y = Mouse.GetPosition(m_CanvasUnderWork).Y;
                m_Sost = sost.Create;
                l1.Tag = iCurrentElement;
                m_lUnitsUnderWork[iCurrentElement].m_l2 = l1;
                m_lUnitsUnderWork[iCurrentElement].SetSecondR(x, y, bIsSetEnemy);
                l1 = new Line();
                l1.Tag = -1;
                l1.Stroke = Brushes.Black;
                l1.StrokeThickness = 2;
                m_CanvasUnderWork.Children.Add(l1);
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
            if (bIsPC)
            {
                for (int i = 0; i < m_lMyunits.Count; i++)
                {
                    m_lMyunits[i].vCorrect(true);
                    m_lUnitsofEnemy[i].vCorrect(false);
                }
            }
            else {
                for (int i = 0; i < m_lMyunits.Count; i++)
                {
                    double x, y;
                    if (m_lMyunits[i].first_r > m_lMyunits[i].second_r)
                    {
                        double temp = m_lMyunits[i].first_r;
                        m_lMyunits[i].first_r = m_lMyunits[i].second_r;
                        m_lMyunits[i].second_r = temp;
                    }
                    m_lMyunits[i].vCorrect(true);
                    Random rnd = new Random();
                    do
                    {
                        x = rnd.NextDouble() * canvasofenemies.Width;
                        y = rnd.NextDouble() * canvasofenemies.Height;
                    }
                    while (!IsPlaceAvailable(x, y));
                    Ellipse el = new Ellipse();
                    canvasofenemies.Children.Add(el);
                    el.Height = 40;
                    if (bVisibility)
                        el.Visibility = Visibility.Visible;
                    else
                        el.Visibility = Visibility.Hidden;
                    el.Width = 40;
                    el.StrokeThickness = 1;
                    el.Fill = m_brushForEnemyUnit;
                    el.Stroke = Brushes.Green;
                    el.Tag = m_lUnitsofEnemy.Count;
                    el.MouseLeftButtonDown += El_MouseLeftButtonDown;
                    Canvas.SetLeft(el, x - 20);
                    Canvas.SetTop(el, y - 20);
                    m_lUnitsofEnemy.Add(new Unit(x, y, el));
                    m_lUnitsofEnemy.Last().first_r = rnd.NextDouble() * -Unit.dMaxAbsR;
                    m_lUnitsofEnemy.Last().second_r = rnd.NextDouble() * Unit.dMaxAbsR;
                }
            }
            m_Sost = sost.Play;
            MessageBox.Show("Параметры углов выстрела немного скорректированы, чтобы снизить количество выстрелов, не попадающих по врагу");
            m_timer.Start();
        }

        private void El_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.Play)
                vShoot((int)(sender as Ellipse).Tag, true);
        }

        void vReset()
        {
            List<List<Unit>> listoflists = new List<List<Unit>> { m_lMyunits, m_lUnitsofEnemy };
            for (int i = 0; i < listoflists.Count; i++)
                for (int j = 0; j < listoflists[i].Count; j++)
                {
                    listoflists[i][j].m_El.Tag = j;
                    if (i == 0)
                    {
                        listoflists[i][j].m_l1.Tag = j;
                        listoflists[i][j].m_l2.Tag = j;
                    }
                }
        }

        private void El_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (m_Sost == sost.Set)
            {
                Line l2 = new Line();
                if (!m_CanvasUnderWork.Children.Contains(l1)) m_CanvasUnderWork.Children.Add(l1);
                m_Sost = sost.Set1;
                iCurrentElement = (int)(sender as Ellipse).Tag;
                for (int i= m_CanvasUnderWork.Children.Count-1; i>=0; i--)
                    if (m_CanvasUnderWork.Children[i].GetType() == l2.GetType() && (int)((m_CanvasUnderWork.Children[i] as Line).Tag) == iCurrentElement)
                        m_CanvasUnderWork.Children.Remove(m_CanvasUnderWork.Children[i]);
            }
            else if (m_Sost==sost.Create)
            {
                m_Sost = sost.MoveUnit;
                iCurrentElement = (int)(sender as Ellipse).Tag;
            }
        }

        private bool IsUnitsSet()
        {
            if (m_lUnitsUnderWork.Count == iCount)
            {
                foreach (Unit u in m_lUnitsUnderWork)
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
                l1.X1 = m_lUnitsUnderWork[iCurrentElement].x;
                l1.Y1 = m_lUnitsUnderWork[iCurrentElement].y;
                l1.X2 = Mouse.GetPosition(m_CanvasUnderWork).X;
                l1.Y2 = Mouse.GetPosition(m_CanvasUnderWork).Y;
            }
            else if (m_Sost==sost.MoveUnit)
            {
                double x = Mouse.GetPosition(m_CanvasUnderWork).X;
                double y = Mouse.GetPosition(m_CanvasUnderWork).Y;
                Canvas.SetLeft(m_lUnitsUnderWork[iCurrentElement].m_El, Mouse.GetPosition(m_CanvasUnderWork).X - 20);
                Canvas.SetTop(m_lUnitsUnderWork[iCurrentElement].m_El, Mouse.GetPosition(m_CanvasUnderWork).Y - 20);
                foreach (UIElement uie in m_CanvasUnderWork.Children)
                    if (uie.GetType() == l1.GetType() && (int)((uie as Line).Tag) == iCurrentElement)
                    {
                        (uie as Line).X1 += (x - m_lUnitsUnderWork[iCurrentElement].x);
                        (uie as Line).X2 += (x - m_lUnitsUnderWork[iCurrentElement].x);
                        (uie as Line).Y1 += (y - m_lUnitsUnderWork[iCurrentElement].y);
                        (uie as Line).Y2 += (y - m_lUnitsUnderWork[iCurrentElement].y);
                    }
                m_lUnitsUnderWork[iCurrentElement].x = x;
                m_lUnitsUnderWork[iCurrentElement].y = y;
                // вписать сюда код перемещения юнита
            }
        }

        void vShoot(int iNumberOfShooter, bool bOurShoot)
        {
            if (bOurShoot)
            {
                if (iOurArmor == 0)
                    return;
                else
                {
                    iOurArmor--;
                    label1.Content = "Количество оружия: " + iOurArmor;
                }
            }
            else
            {
                if (iArmorOfEnemy == 0)
                    return;
                else
                {
                    iArmorOfEnemy--;
                    label2.Content = "Количество оружия: " + iArmorOfEnemy;
                }
            }
            double k, b;
            redline1 = new Line();
            redline2 = new Line();
            redline1.Stroke = Brushes.Black;
            redline1.StrokeThickness = 2;
            redline2.Stroke = Brushes.Black;
            redline2.StrokeThickness = 2;
            var listofhurtedunits = from u in m_lMyunits orderby u.x descending select u;
            if (bOurShoot)
            {
                k = m_lMyunits[iNumberOfShooter].GetR();
                b = (canvas.Width - m_lMyunits[iNumberOfShooter].x) * k + m_lMyunits[iNumberOfShooter].y;
                listofhurtedunits = from u in m_lUnitsofEnemy orderby u.x select u;
            }
            else
            {
                k = -m_lUnitsofEnemy[iNumberOfShooter].GetR();
                b = m_lUnitsofEnemy[iNumberOfShooter].x * k + m_lUnitsofEnemy[iNumberOfShooter].y;
            }
            double iB = b;
            double iK = k;
            if (!bOurShoot)
            {
                iB += k * canvas.Width;
                iK = -k;
            }
            redline1.Y2 = b;
            redline2.Y2 = b;
            if (b > 0)
                foreach (Unit u in listofhurtedunits)
                    if (u.IsHurt(iK, iB))
                    {
                        redline1.X2 = canvas.Width;
                        redline2.X2 = 0;
                        if (!bOurShoot)
                        {
                            redline1.X1 = u.x;
                            redline1.Y1 = u.y;
                            redline2.X1 = m_lUnitsofEnemy[iNumberOfShooter].x;
                            redline2.Y1 = m_lUnitsofEnemy[iNumberOfShooter].y;
                        }
                        else
                        {
                            redline1.X1 = m_lMyunits[iNumberOfShooter].x;
                            redline1.Y1 = m_lMyunits[iNumberOfShooter].y;
                            redline2.X1 = u.x;
                            redline2.Y1 = u.y;
                        }
                        canvas.Children.Add(redline1);
                        canvasofenemies.Children.Add(redline2);
                        //           System.Threading.Thread.Sleep(500);
                        // звуковой сигнал
                        /*                        canvas.Children.Remove(redline1);
                                                canvasofenemies.Children.Remove(redline2);*/
                        if (bOurShoot)
                        {
                            canvasofenemies.Children.Remove(u.m_El);
                            m_lUnitsofEnemy.Remove(u);                            
                        }
                        else
                        {
                            canvas.Children.Remove(u.m_El);
                            canvas.Children.Remove(u.m_l1);
                            canvas.Children.Remove(u.m_l2);
                            m_lMyunits.Remove(u);
                        }
                        return;
                    }
            redline1.X2 = canvas.Width;
            redline2.X2 = 0;
            if (!bOurShoot)
            {
                redline1.X1 = 0;
                redline1.Y1 = b + k * canvas.Width;
                redline2.X1 = m_lUnitsofEnemy[iNumberOfShooter].x;
                redline2.Y1 = m_lUnitsofEnemy[iNumberOfShooter].y;
            }
            else
            {
                redline1.X1 = m_lMyunits[iNumberOfShooter].x;
                redline1.Y1 = m_lMyunits[iNumberOfShooter].y;
                redline2.X1 = canvasofenemies.Width;
                redline2.Y1 = k * canvasofenemies.Width + b;
            }
            canvas.Children.Add(redline1);
            canvasofenemies.Children.Add(redline2);
            System.Threading.Thread.Sleep(500);
            // звуковой сигнал
         /*   canvas.Children.Remove(redline1);
            canvasofenemies.Children.Remove(redline2);*/
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            m_Sost = sost.Set;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (bIsPC && bIsSetEnemy==false)
            {
                button.IsEnabled = false;
                button1.IsEnabled = false;
                button.Content = "Играть";
                bIsSetEnemy = true;
                canvas.Children.Remove(l1);
                canvasofenemies.MouseDown += Canvas_MouseDown;
                canvasofenemies.MouseMove += canvas_MouseMove;
                canvas.MouseDown -= Canvas_MouseDown;
                canvas.MouseMove -= canvas_MouseMove;
                m_Sost = sost.Create;
                m_CanvasUnderWork = canvasofenemies;
                m_lUnitsUnderWork = m_lUnitsofEnemy;
            }
            else
                vPrepare();
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_Sost == sost.MoveUnit)
                m_Sost = sost.Create;
        }
    }
}