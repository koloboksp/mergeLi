using System.Diagnostics;

namespace Core
{
    public class SmallTimer
    {
        private double _oldTime;
        private double _start;
        private readonly Stopwatch _watch = new Stopwatch();

        public SmallTimer()
        {
            Start();
        }

       
        public double FullTime { get; internal set; }
        public double DeltaTime { get; internal set; }
        
        public double Update()
        {
            FullTime = (double)_watch.ElapsedTicks / Stopwatch.Frequency - _start;
            DeltaTime = FullTime - _oldTime;
            _oldTime = FullTime;

            return DeltaTime;
        }

        public void Start()
        {
            _watch.Reset();
            _watch.Start();

            _start = (double)_watch.ElapsedTicks / Stopwatch.Frequency;
            FullTime = _oldTime = DeltaTime = 0.0f;
        }
    }
}