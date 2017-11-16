using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project2
{
    public class Animator
    {
        private readonly object SyncRoot = new object();
        private readonly Random rng = new Random();
        public List<MyPolygon> randomPolygons = new List<MyPolygon>();
        private readonly Canvas Canvas;
        private readonly Scene Scene;
        public bool Run = true;

        public string RandomPolygonsCount
        {
            get => $"{_randomPolygonCount}";
            set
            {
                if (int.TryParse(value, out var randomPolygonCount) &&
                    (_randomPolygonCountMin <= randomPolygonCount && randomPolygonCount <= _randomPolygonCountMax))
                {
                    Interlocked.Exchange(ref _randomPolygonCount, randomPolygonCount);
                }
            }
        }

        private int _randomPolygonCount = 5;
        private int _randomPolygonCountMin = 0;
        private int _randomPolygonCountMax = 100;

        public Animator(Canvas canvas, Scene scene)
        {
            Canvas = canvas;
            Scene = scene;
            thread = new Thread(Animate);
            thread.Start();
        }

        public void EndAnimation()
        {
            Run = false;
            thread.Abort();
            thread.Join();
        }
        private readonly Thread thread;
        public void Animate()
        {
            try
            {
                while (Run)
                {
                    lock (SyncRoot)
                    {
                        foreach (var polygon in randomPolygons)
                        {
                            polygon.Transform();
                        }
                        randomPolygons.RemoveAll(polygon => polygon.MaxX <= 0);
                        for (var i = randomPolygons.Count; i < _randomPolygonCount; i++)
                            randomPolygons.Add(MyPolygon.GenerateRandomPolygon(1920, 1080, rng, Canvas, Scene,
                                Scene.velocityMin, Scene.velocityMax));
                        Thread.Sleep(1000/60);
                    }
                }
            }
            catch { }
        }
    }
}
