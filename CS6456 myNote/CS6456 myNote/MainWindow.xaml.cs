using System;
using System.Collections;
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

namespace CS6456_myNote
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();            

            my_UIInitial();            
        }
        
        bool isDragDropInEffect = false;
        int isStickReady = 0;       
        System.Windows.Point pos = new System.Windows.Point();
        bool isAllowedDrag = true;

        int index_stick_current = -1, index_stick_target = -1;
        ArrayList myUIList = new ArrayList();
        
        // my UI components and functions
        class myUI
        {
            public myUI(double i_0, double i_1, double i_2, double i_3)
            {
                my_L = i_0;
                my_T = i_1;
                my_Width = i_2;
                my_Height = i_3;
            }
            public void setLocation(double i_0, double i_1){
                my_L = i_0;
                my_T = i_1;
            }
            public double getLocation_L()
            {
                return my_L;
            }
            public double getLocation_T()
            {
                return my_T;
            }
            public double getWidth()
            {
                return my_Width;
            }
            public double getHeight()
            {
                return my_Height;
            }
            public int isClosed_Stich(int i)
            {
                return 6;
            }

            public Boolean my_isSticked = false;
            public int stick_target = -1;
            public ArrayList stick_children = new ArrayList();
            public ArrayList stick_children_direction = new ArrayList();
            double my_L, my_T;
            double my_Width, my_Height;

            public String myName;

            UIElement my_UIele;

        }

        void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragDropInEffect)
            {
                if (!isAllowedDrag) return;

                FrameworkElement currEle = sender as FrameworkElement;
                int i_1 = my_NoteArea.Children.IndexOf(currEle);

                int i_3 = my_FindStickRoot(i_1);

                double xPos = e.GetPosition(null).X - pos.X;
                double yPos = e.GetPosition(null).Y - pos.Y;


                my_MoveAllStickUICom(i_3, xPos, yPos);

                pos = e.GetPosition(null);

                //When two UI components get close, show indicator and do some operations

                //((myUI)myUIList[i_1]).setLocation((double)currEle.GetValue(Canvas.LeftProperty), (double)currEle.GetValue(Canvas.TopProperty));

                foreach (UIElement uiEle in my_NoteArea.Children)
                {
                    int i_2 = my_NoteArea.Children.IndexOf(uiEle);
                    if (uiEle.Equals(com_Indicator)) continue;
                    if (uiEle.Equals(currEle)) continue;

                    if (my_isCloseStick(i_1, i_2) == 1)
                    {
                        isStickReady = 1;
                        index_stick_target = i_2;
                        index_stick_current = i_1;

                        return;
                    }
                    if (my_isCloseStick(i_1, i_2) == 2)
                    {
                        isStickReady = 2;
                        index_stick_target = i_2;
                        index_stick_current = i_1;
                        return;
                    }
                }

                isStickReady = 0;
                index_stick_target = -1;
                index_stick_current = -1;
                com_Indicator.SetValue(Canvas.LeftProperty, 575d);
            }

        }

        void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            FrameworkElement fEle = sender as FrameworkElement;
            isDragDropInEffect = true;
            pos = e.GetPosition(null);
            fEle.CaptureMouse();
            fEle.Cursor = Cursors.Hand;
        }

        void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragDropInEffect)
            {
                FrameworkElement ele = sender as FrameworkElement;
                isDragDropInEffect = false;
                ele.ReleaseMouseCapture();
            }

            if (isStickReady != 0 && isAllowedDrag)
            {
                my_StickOperation(index_stick_current, index_stick_target, isStickReady);
            }

            com_Indicator.SetValue(Canvas.LeftProperty, 575d);
        }

        void Element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("11111");
            // in drag status do merge or in non-drag status seperate 
            if (isAllowedDrag)
            {

            }
            else
            {
                FrameworkElement CurUIEle = sender as FrameworkElement;
                int i_0 = my_NoteArea.Children.IndexOf(CurUIEle);
                int i_1 = ((myUI)myUIList[i_0]).stick_target;

                if (i_1 == -1) return;

                int i_2 = ((myUI)myUIList[i_1]).stick_children.IndexOf(i_0);
                Console.WriteLine("  " + i_0 + "  " +  i_1 + "  " + i_2);

                ((myUI)myUIList[i_0]).stick_target = -1;
                if (((myUI)myUIList[i_0]).stick_children.Count == 0) ((myUI)myUIList[i_0]).my_isSticked = false;

                my_MoveAllStickUICom(i_0, 20d, 10d);


                ((myUI)myUIList[i_1]).stick_children.RemoveAt(i_2);
                ((myUI)myUIList[i_1]).stick_children_direction.RemoveAt(i_2);
                if (((myUI)myUIList[i_1]).stick_children.Count == 0 && ((myUI)myUIList[i_1]).stick_target == -1)
                {
                    ((myUI)myUIList[i_1]).my_isSticked = false;
                }
            }
        } 


        void my_UIInitial(){

            foreach (UIElement uiEle in my_NoteArea.Children)
            {
                //WPF's problem: Button.Clicked event can Supress!!!!    Mouse.MouseLeftButtonDown..... events.
                //without this, Button、TextBox can't be dragged
                if (uiEle.Equals(com_Indicator)) continue;

                if (uiEle is Button || uiEle is TextBox)
                {
                    uiEle.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Element_MouseLeftButtonDown), true);
                    uiEle.AddHandler(Button.MouseMoveEvent, new MouseEventHandler(Element_MouseMove), true);
                    uiEle.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Element_MouseLeftButtonUp), true);

                    uiEle.AddHandler(Button.MouseDoubleClickEvent, new MouseButtonEventHandler(Element_MouseDoubleClick), true);
                    continue;
                }
                //
                uiEle.MouseMove += new MouseEventHandler(Element_MouseMove);
                uiEle.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseLeftButtonDown);
                uiEle.MouseLeftButtonUp += new MouseButtonEventHandler(Element_MouseLeftButtonUp);

                // it doesn't work for image
                //uiEle.AddHandler(Button.MouseDoubleClickEvent, new MouseButtonEventHandler(Element_MouseDoubleClick), true);
            }

            myUIList.Add(new myUI(-1, -1, -1, -1));
            ((myUI)myUIList[0]).myName = "com_Indicator";
            myUIList.Add(new myUI(154, 82, 96, 56));
            myUIList.Add(new myUI(93, 181, 106, 96));
            myUIList.Add(new myUI(297, 317, 82, 100));
            myUIList.Add(new myUI(0, 0, 0, 0));

            //myDocViewer.Document = ConvertWordToXPS("my_Resources/15-Speech.pdf").GetFixedDocumentSequence();
        }

        int my_isCloseStick(int i_1, int i_2)
        {
            //i_2 target   i_1 current UI ele
            double d_2L = (double)my_NoteArea.Children[i_2].GetValue(Canvas.LeftProperty);// ((myUI)myUIList[i_2]).getLocation_L();
            double d_2T = (double)my_NoteArea.Children[i_2].GetValue(Canvas.TopProperty);
            double d_2W = (double)my_NoteArea.Children[i_2].GetValue(Canvas.WidthProperty);
            double d_2H = (double)my_NoteArea.Children[i_2].GetValue(Canvas.HeightProperty);

            double d_1L = (double)my_NoteArea.Children[i_1].GetValue(Canvas.LeftProperty);
            double d_1T = (double)my_NoteArea.Children[i_1].GetValue(Canvas.TopProperty);

            if (((myUI)myUIList[i_1]).stick_target != -1) return 6;    

            //Find out which part of notes are get close enough to combine
            //come from right, show dotted line
            if (Math.Abs(d_2L + d_2W - d_1L) <= 5.0  &&  d_1T - d_2T > -5.0 && d_1T - (d_2T + d_2H / 1.5) < 5.0)
            {
                com_Indicator.SetValue(Canvas.LeftProperty, d_2L + d_2W - 5.0);
                com_Indicator.SetValue(Canvas.TopProperty, d_2T - 5.0);
                com_Indicator.SetValue(Canvas.WidthProperty, 10d);//.Width = 10;
                com_Indicator.Height = d_2H + 10;
                return 1;
            }
            //come from bottom, show dotted line
            if (Math.Abs(d_2T + d_2H - d_1T) <= 5.0 && d_1L - d_2L > -5.0 && d_1L - (d_2L + d_2W / 1.5) < 5.0)
            {
                com_Indicator.SetValue(Canvas.LeftProperty, d_2L  - 5.0);
                com_Indicator.SetValue(Canvas.TopProperty, d_2T + d_2H - 5.0);
                com_Indicator.Width = d_2W + 10;
                com_Indicator.Height = 10;
                return 2;
            }

            return 6;
        }

        

        void my_StickOperation(int i_1, int i_2, int i_3)
        {
            //i_2 target   i_1 current UI ele
            double d_2L = (double)my_NoteArea.Children[i_2].GetValue(Canvas.LeftProperty);// ((myUI)myUIList[i_2]).getLocation_L();
            double d_2T = (double)my_NoteArea.Children[i_2].GetValue(Canvas.TopProperty);
            double d_2W = (double)my_NoteArea.Children[i_2].GetValue(Canvas.WidthProperty);
            double d_2H = (double)my_NoteArea.Children[i_2].GetValue(Canvas.HeightProperty);
            ((myUI)myUIList[i_2]).my_isSticked = true;
            ((myUI)myUIList[i_2]).stick_children.Add(i_1);


            double d_1L = (double)my_NoteArea.Children[i_1].GetValue(Canvas.LeftProperty);
            double d_1T = (double)my_NoteArea.Children[i_1].GetValue(Canvas.TopProperty);
            ((myUI)myUIList[i_1]).my_isSticked = true;
            ((myUI)myUIList[i_1]).stick_target = i_2;





            //Depends on different kind of combination, do sth
            if (i_3 == 1)
            {
                // use L,Toffset to make all UI (which is sticked together) to move to a new positon 
                double myLOffset = d_2L + d_2W - d_1L;
                double myTOffset = d_2T - d_1T;
                my_MoveAllStickUICom(i_1, myLOffset, myTOffset);
                //my_NoteArea.Children[i_1].SetValue(Canvas.LeftProperty, d_2L + d_2W);
                //my_NoteArea.Children[i_1].SetValue(Canvas.TopProperty, d_2T);
                //((myUI)myUIList[i_1]).setLocation(d_2L + d_2W, d_2T);
                ((myUI)myUIList[i_2]).stick_children_direction.Add(1);
                return;  
            }
            if (i_3 == 2)
            {
                double myLOffset = d_2L - d_1L;
                double myTOffset = d_2T + d_2H - d_1T;
                my_MoveAllStickUICom(i_1, myLOffset, myTOffset);
                //my_NoteArea.Children[i_1].SetValue(Canvas.LeftProperty, d_2L);
                //my_NoteArea.Children[i_1].SetValue(Canvas.TopProperty, d_2T + d_2H);
                //((myUI)myUIList[i_1]).setLocation(d_2L, d_2T + d_2H);
                ((myUI)myUIList[i_2]).stick_children_direction.Add(2);
                return;
            }
        }

        int my_FindStickRoot(int i_1)
        {
            if (!((myUI)myUIList[i_1]).my_isSticked)
            {
                return i_1;
            }
            else if (((myUI)myUIList[i_1]).stick_target != -1)
            {
                return my_FindStickRoot(((myUI)myUIList[i_1]).stick_target);
            }
            return i_1;
        }

        void my_MoveAllStickUICom(int i_1, double myLeft, double myTop)
        {
            UIElement curEle = my_NoteArea.Children[i_1];
            curEle.SetValue(Canvas.LeftProperty, myLeft + (double)curEle.GetValue(Canvas.LeftProperty));
            curEle.SetValue(Canvas.TopProperty, myTop + (double)curEle.GetValue(Canvas.TopProperty));

            foreach (int i_2 in ((myUI)myUIList[i_1]).stick_children)
            {
                // need to get rid of children when it's not sticked any more, OR it will move twice
                my_MoveAllStickUICom(i_2, myLeft, myTop);
            }
        }

        // use rightControl to decide to edit or drag( will cause non-editable)
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.RightCtrl))
            {
                isAllowedDrag = !isAllowedDrag;
                if (isAllowedDrag)
                {
                    Ctrl_Sig.Background = Brushes.Green;
                }
                else
                {
                    Ctrl_Sig.Background = Brushes.Red;

                }
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            my_NoteArea.Children.Add(new TextBox());

            UIElement newUIE = my_NoteArea.Children[my_NoteArea.Children.Count - 1];

            newUIE.SetValue(Canvas.LeftProperty, 40d);
            newUIE.SetValue(Canvas.TopProperty, 40d);
            newUIE.SetValue(Canvas.WidthProperty, 60d);
            newUIE.SetValue(Canvas.HeightProperty, 60d);

            newUIE.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Element_MouseLeftButtonDown), true);
            newUIE.AddHandler(Button.MouseMoveEvent, new MouseEventHandler(Element_MouseMove), true);
            newUIE.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Element_MouseLeftButtonUp), true);

            newUIE.AddHandler(Button.MouseDoubleClickEvent, new MouseButtonEventHandler(Element_MouseDoubleClick), true);

            myUIList.Add(new myUI(0, 0, 0, 0));
        }



    ////////////////////////////////////////////  UI Operations   Chutian Wang 
    
    }
}
