using System;
using System.Drawing;
using System.Windows.Forms;

namespace Custom_Title_Bar
{
    public partial class MainForm : Form
    {
        private PictureBox _title = new PictureBox(); // create a PictureBox
        private Label _minimise = new Label(); // this doesn't even have to be a label!
        private Label _maximise = new Label(); // this will simulate our this.maximise box
        private Label _close = new Label(); // simulates the this.close box

        private bool _drag = false; // determine if we should be moving the form
        private Point _startPoint = new Point(0, 0); // also for the moving

        public MainForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // get rid of the standard title bar

            _title.Location = this.Location; // assign the location to the form location
            _title.Width = this.Width; // make it the same width as the form
            _title.Height = 50; // give it a default height (you may want it taller/shorter)
            _title.BackColor = Color.Black; // give it a default colour (or load an image)
            this.Controls.Add(_title); // add it to the form's controls, so it gets displayed
            // if you have an image to display, you can load it, instead of assigning a bg colour
            // this.title.Image = new Bitmap(System.Environment.CurrentDirectory + "\\title.jpg");
            // if you displayed an image, alter the SizeMode to get it to display as you want it to
            // examples:
            // this.title.SizeMode = PictureBoxSizeMode.StretchImage;
            // this.title.SizeMode = PictureBoxSizeMode.CenterImage;
            // this.title.SizeMode = PictureBoxSizeMode.Zoom;
            // etc

            // you may want to use PictureBoxes and display images
            // or use buttons, there are many alternatives. This is a mere example.
            _minimise.Text = "Minimise"; // Doesn't have to be
            _minimise.Location = new Point(this.Location.X + 5, this.Location.Y + 5); // give it a default location
            _minimise.ForeColor = Color.Red; // Give it a colour that will make it stand out
            // this is why I didn't use an image, just to keep things simple:
            _minimise.BackColor = Color.Black; // make it the same as the PictureBox
            this.Controls.Add(_minimise); // add it to the form's controls
            _minimise.BringToFront(); // bring it to the front, to display it above the picture box

            _maximise.Text = "Maximise";
            // remember to make sure it's far enough away so as not to overlap our minimise option
            _maximise.Location = new Point(this.Location.X + 60, this.Location.Y + 5);
            _maximise.ForeColor = Color.Red;
            _maximise.BackColor = Color.Black; // remember, we want it to match the background
            _maximise.Width = 50;
            this.Controls.Add(_maximise); // add it to the form
            _maximise.BringToFront();

            _close.Text = "Close";
            _close.Location = new Point(this.Location.X + 120, this.Location.Y + 5);
            _close.ForeColor = Color.Red;
            _close.BackColor = Color.Black;
            _close.Width = 37; // this is just to make it fit nicely
            this.Controls.Add(_close);
            _close.BringToFront();

            // now we need to add some functionality. First off, let's give those labels
            // MouseHover and MouseLeave events, so they change colour
            // Since they're all going to change to the same colour, we can give them the same
            // event handler, which saves time of writing out all those extra functions
            _minimise.MouseEnter += new EventHandler(Control_MouseEnter);
            _maximise.MouseEnter += new EventHandler(Control_MouseEnter);
            _close.MouseEnter += new EventHandler(Control_MouseEnter);

            // and we need to do the same for MouseLeave events, to change it back
            _minimise.MouseLeave += new EventHandler(Control_MouseLeave);
            _maximise.MouseLeave += new EventHandler(Control_MouseLeave);
            _close.MouseLeave += new EventHandler(Control_MouseLeave);

            // and lastly, for these controls, we need to add some functionality
            _minimise.MouseClick += new MouseEventHandler(Control_MouseClick);
            _maximise.MouseClick += new MouseEventHandler(Control_MouseClick);
            _close.MouseClick += new MouseEventHandler(Control_MouseClick);

            // finally, wouldn't it be nice to get some moveability on this control?
            _title.MouseDown += new MouseEventHandler(Title_MouseDown);
            _title.MouseUp += new MouseEventHandler(Title_MouseUp);
            _title.MouseMove += new MouseEventHandler(Title_MouseMove);
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender.Equals(_close))
                _close.ForeColor = Color.White;
            else if (sender.Equals(_maximise))
                _maximise.ForeColor = Color.White;
            else // it's the minimise label
                _minimise.ForeColor = Color.White;
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        { // return them to their default colours
            if (sender.Equals(_close))
                _close.ForeColor = Color.Red;
            else if (sender.Equals(_maximise))
                _maximise.ForeColor = Color.Red;
            else // it's the minimise label
                _minimise.ForeColor = Color.Red;
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender.Equals(_close))
                this.Close(); // close the form
            else if (sender.Equals(_maximise))
            { // maximise is more interesting. We need to give it different functionality,
                // depending on the window state (Maximise/Restore)
                if (_maximise.Text == "Maximise")
                {
                    this.WindowState = FormWindowState.Maximized; // maximise the form
                    _maximise.Text = "Restore"; // change the text
                    _title.Width = this.Width; // stretch the title bar
                }
                else // we need to restore
                {
                    this.WindowState = FormWindowState.Normal;
                    _maximise.Text = "Maximise";
                }
            }
            else // it's the minimise label
                this.WindowState = FormWindowState.Minimized; // minimise the form
        }

        private void Title_MouseUp(object sender, MouseEventArgs e)
        {
            _drag = false;
        }

        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            _startPoint = e.Location;
            _drag = true;
        }

        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drag)
            { // if we should be dragging it, we need to figure out some movement
                Point p1 = new Point(e.X, e.Y);
                Point p2 = this.PointToScreen(p1);
                Point p3 = new Point(p2.X - _startPoint.X,
                                     p2.Y - _startPoint.Y);
                this.Location = p3;
            }
        }
    }
}