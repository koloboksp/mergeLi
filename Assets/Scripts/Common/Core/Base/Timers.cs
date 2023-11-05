using System.Diagnostics;

namespace Atom
{
    namespace Timers
    {
        public class StepTimer
        {
            public delegate void DelegateOnStep();

            private event DelegateOnStep OnStep;

            public StepTimer(double step, DelegateOnStep onStep)
            {
                OnStep = onStep;
                Step = step;
                mTime = 0.0;
            }

            public void Clear(double timeFull)
            {
                mTime = timeFull;
            }

            public void Update(double timeFull)
            {
                var ret = Conversion.ToUInt64(mTime / Step) != Conversion.ToUInt64(timeFull / Step);

                mTime = timeFull;

                if (ret)
                {
                    OnStep?.Invoke();
                }
            }

            private double mTime;
            public double Step { get; }
        }

        public class SmallTimer
        {
            private double mOldTime;
            private double mStart;
            private readonly Stopwatch mWatch = new Stopwatch();

            public SmallTimer()
            {
                Start();
            }

            /// <summary>
            /// получить полное время 
            /// </summary>
            public double FullTime { get; internal set; }

            /// <summary>
            /// получить разницу времени между апдейтами (c)
            /// </summary>
            public double DeltaTime { get; internal set; }

            /// <summary>
            /// обновление  таймера
            /// </summary>
            /// <returns>DeltaTime</returns>
            public double Update()
            {
                FullTime = Conversion.ToDouble(mWatch.ElapsedTicks) / Conversion.ToDouble(Stopwatch.Frequency) - mStart;
                DeltaTime = FullTime - mOldTime;
                mOldTime = FullTime;

                return DeltaTime;
            }

            /// <summary>
            /// очистка всех значений
            /// </summary>
            public void Start()
            {
                mWatch.Reset();
                mWatch.Start();

                mStart = Conversion.ToDouble(mWatch.ElapsedTicks) / Conversion.ToDouble(Stopwatch.Frequency);
                FullTime = mOldTime = DeltaTime = 0.0f;
            }
        }

        public class Timer : SmallTimer
        {
            public delegate void DelegateOnTick();

            private event DelegateOnTick OnTick;

            //фпс
            private int mCounterFPS;

            public Timer(DelegateOnTick onTick)
            {
                Start();
                OnTick = onTick;
            }

            public Timer()
            {
                Start();
            }

            //фпс
            public int FPS { get; internal set; }

            /// <summary>
            /// обновление  таймера
            /// </summary>
            public new void Update()
            {
                base.Update();

                mCounterFPS++;
                
                if (Conversion.ToUInt64(FullTime - DeltaTime) == Conversion.ToUInt64(FullTime))
                    return;

                FPS = mCounterFPS;
                mCounterFPS = 0;

                OnTick?.Invoke();
            }

            /// <summary>
            /// очистка всех значений
            /// </summary>
            public new void Start()
            {
                base.Start();
                mCounterFPS = FPS = 0;
            }
        }
    }
}
