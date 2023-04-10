using System;
using System.Timers;
using System.Drawing;
using System.Drawing.Drawing2D;
using Client.ClientBase.Utils;

namespace Client.ClientBase.UI.Notification
{
    public class Notification
    {
        private readonly string _text;
        public Rectangle _boxRect;
        private readonly Font _titleFont;
        private readonly Font _textFont;
        private readonly Color _boxColor;
        private readonly Color _lineColor;

        public bool isShowing;
        public event EventHandler Expired;
        public Form form;

        private readonly System.Timers.Timer _timer;
        private readonly int _timerInterval = 5000; // 5 seconds in milliseconds
        private DateTime _startTime;
        private readonly int _animationTime = 500; // animation time in milliseconds
        private bool _enterAnimationCompleted = false;
        private bool _exitAnimationCompleted = false;

        private int _slideDistance;
        private int _slideStartPos;

        private readonly Action<Notification> _onTimerElapsedCallback;

        public Notification(string text, Rectangle boxRect, Font titleFont, Font textFont, Color boxColor, Color lineColor, Form form, Action<Notification> onTimerElapsedCallback = null)
        {
            _text = text;
            _boxRect = boxRect;
            _titleFont = titleFont;
            _textFont = textFont;
            _boxColor = boxColor;
            _lineColor = lineColor;
            this.form = form;

            _onTimerElapsedCallback = onTimerElapsedCallback;

            _timer = new System.Timers.Timer();
            _timer.Interval = _timerInterval;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = false;
        }

        public void Show()
        {
            _slideStartPos = _boxRect.X - _boxRect.Width;
            _slideDistance = _boxRect.Width;
            isShowing = true;
            _startTime = DateTime.Now;
            _enterAnimationCompleted = false;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            isShowing = false;
            Expired?.Invoke(this, EventArgs.Empty);
            _onTimerElapsedCallback?.Invoke(this);
        }

        public void OnDraw(Graphics g)
        {
            if (isShowing)
            {
                if (!_enterAnimationCompleted)
                {
                    TimeSpan timeElapsed = DateTime.Now - _startTime;
                    float progress = (float)Math.Min(1, timeElapsed.TotalMilliseconds / _animationTime);

                    _boxRect.X = (int)(_slideStartPos + _slideDistance * progress);
                    if (progress >= 1)
                    {
                        _enterAnimationCompleted = true;
                    }
                }
                else if (_enterAnimationCompleted && (DateTime.Now - _startTime).TotalMilliseconds >= (_timerInterval - _animationTime) && !_exitAnimationCompleted)
                {
                    TimeSpan timeElapsed = DateTime.Now - _startTime - TimeSpan.FromMilliseconds(_timerInterval - _animationTime);
                    float progress = (float)Math.Min(1, timeElapsed.TotalMilliseconds / _animationTime);
                    _boxRect.X = (int)(_slideStartPos + _slideDistance * (1 - progress));
                    if (progress >= .90)
                    {
                        _exitAnimationCompleted = true;
                    }
                }

                using (SolidBrush boxBrush = new SolidBrush(_boxColor))
                using (SolidBrush lineBrush = new SolidBrush(Color.FromArgb(210, _lineColor)))
                using (StringFormat format = new StringFormat())
                {
                    using (GraphicsPath path = RenderUtil.CustomRoundedRectangle(_boxRect, 0, 8, 8, 0))
                    {
                        g.FillPath(boxBrush, path);
                    }

                    TimeSpan timeElapsed = DateTime.Now - _startTime;
                    int borderLength = _boxRect.Width - (int)(timeElapsed.TotalMilliseconds / _timerInterval * _boxRect.Width);
                    g.FillRectangle(lineBrush, _boxRect.X, _boxRect.Y, borderLength, 3);

                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    Rectangle titleRect = new Rectangle(_boxRect.X + 3, _boxRect.Y + 1, _boxRect.Width - 20, _boxRect.Height - 20);
                    g.DrawString("Notification", _titleFont, Brushes.White, titleRect, format);
                    Rectangle textRect = new Rectangle(_boxRect.X + 3, _boxRect.Y + 25, _boxRect.Width - 20, _boxRect.Height - 20);
                    g.DrawString(_text, _textFont, Brushes.White, textRect, format);
                }
            }
        }
    }
}