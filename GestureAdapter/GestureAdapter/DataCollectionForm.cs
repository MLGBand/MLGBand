using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestureAdapter
{
    /// <summary>
    /// Provides prompts the user to perform a sequence of randomly-timed, random-duration gestures.
    /// </summary>
    partial class DataCollectionForm : Form
    {
        public DataCollectionForm()
        {
            InitializeComponent();
        }

        private Gesture currentGesture = Gesture.Relax;
        private int remainingSeconds = 1;
        private Dictionary<DateTime, Gesture> gestureHistory = new Dictionary<DateTime, Gesture>();
        private int isBad = 10;

        private Gesture FindGestureAt(DateTime time)
        {
            lock (gestureHistory)
            {
                return gestureHistory.LastOrDefault(x => x.Key < time).Value;
            }
        }

        public bool GetIsBad()
        {
            bool result = isBad > 0;
            if(result)
            {
                isBad--;
            }
            return result;
        }
        
        private bool IsRunning
        {
            get { return btnStartStop.Text == "Stop"; }
            set
            {
                if (value)
                {
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    btnStartStop.Text = "Start";
                }
            }

        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!IsRunning)
            {
                remainingSeconds = 10;
                IsRunning = true;
                timer.Start();
            }
            else
            {
                IsRunning = false;
                timer.Stop();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            if (remainingSeconds < 0)
            {
                var random = new Random();

                var lastGesture = (Gesture)currentGesture;

                if (lastGesture == Gesture.Relax)
                {
                    var delay = random.Next(2, 5);
                    currentGesture = (Gesture)random.Next(1, 9);
                    remainingSeconds = delay + 1;

                    lock (gestureHistory)
                    {
                        gestureHistory.Add(DateTime.Now, Gesture.Relax);
                        gestureHistory.Add(DateTime.Now.AddMilliseconds(remainingSeconds * timer.Interval), currentGesture);
                    }
                }
                else
                {
                    currentGesture = Gesture.Relax;
                    remainingSeconds = 0;
                    lock (gestureHistory)
                    {
                        gestureHistory.Add(DateTime.Now, Gesture.Relax);
                    }
                }

                //if (lastGesture != Gesture.Relax)
                //{
                //    currentGesture = 0;
                //    remainingSeconds = random.Next(0, 5);
                //    if (remainingSeconds > 3)
                //    {
                //        remainingSeconds += random.Next(1, 3); 
                //    }

                //    lock (gestureHistory)
                //    {
                //        gestureHistory.Add(DateTime.Now, currentGesture);
                //    }
                //}

                //else
                //{

                //}
            }

            //switch (currentGesture)
            //{
            //    case Gesture.Relax:
            //        currentGesture = Gesture.ClapIn2;
            //        remainingSeconds = 1;
            //        break;
            //    case Gesture.ClapIn2:
            //        currentGesture = Gesture.ClapIn1;
            //        remainingSeconds = 1;
            //        break;
            //    case Gesture.ClapIn1:
            //        currentGesture = Gesture.Clap;
            //        remainingSeconds = 1;
            //        break;
            //    case Gesture.Clap:
            //        remainingSeconds = random.Next(0, 5);
            //        if (remainingSeconds >= 3)
            //        {
            //            remainingSeconds += random.Next(0, 5);
            //        }
            //        currentGesture = Gesture.Relax;
            //        break;
            //}

            lblTime.Text = remainingSeconds.ToString();
            lblAction.Text = Enum.GetName(typeof(Gesture), currentGesture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isBad = 10;
        }

        static DataCollectionForm instance;
        
        public static Gesture GetGestureAt(DateTime time)
        {
            if (instance == null) return (Gesture)Gesture.Relax;
            return instance.FindGestureAt(time);
        }

        public static bool IsDataBad()
        {
            if (instance == null) return false;
            return instance.GetIsBad();

        }

        public static void ShowPrompts()
        {
            if (instance != null) return;
            instance = new DataCollectionForm();
            new Thread(() =>
            {
                instance.Show();
                Application.Run(instance);
            }).Start();
        }
    }

    enum Gesture
    {
        Relax,
        Push,
        Pull,
        ScrewIn,
        ScrewOut,
        Hit,
        Lift,
        DragLeft,
        DragRight
    }
}
