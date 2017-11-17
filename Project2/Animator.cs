using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace Project2
{
    public class Animator
    {
        private const int RandomPolygonCountMax = 100;
        private const int RandomPolygonCountMin = 0;
        private readonly Canvas _canvas;
        private readonly Random _rng = new Random();
        private readonly Scene _scene;
        private readonly object _syncRoot = new object();

        private int _fps;
        private DateTime _lastTick = DateTime.Now;
        private DateTime _now = DateTime.Now;

        private int _randomPolygonCount = 1;
        public List<MyPolygon> RandomPolygons = new List<MyPolygon>();
        public bool Run = true;

        public Animator(Canvas canvas, Scene scene)
        {
            _canvas = canvas;
            _scene = scene;
        }

        public string RandomPolygonsCount
        {
            get => $"{_randomPolygonCount}";
            set
            {
                if (int.TryParse(value, out var randomPolygonCount) && RandomPolygonCountMin <= randomPolygonCount &&
                    randomPolygonCount <= RandomPolygonCountMax)
                    Interlocked.Exchange(ref _randomPolygonCount, randomPolygonCount);
            }
        }

        public string Fps
        {
            get => $"{_fps}";
            set
            {
                if (int.TryParse(value, out var v))
                    _fps = v;
            }
        }


        public void DoTick()
        {
            _now = DateTime.Now;
            var dt = _now.Subtract(_lastTick).TotalMilliseconds;
            _scene.fpsCounter.Dispatcher.Invoke(() => { _scene.fpsCounter.Content = $"FPS: {(int) (1000 / dt)}"; });
            foreach (var polygon in RandomPolygons)
                polygon.Transform();
            RandomPolygons.RemoveAll(polygon => polygon.MaxX <= 0);
            for (var i = RandomPolygons.Count; i < _randomPolygonCount; i++)
            {
                var poly = MyPolygon.GenerateRandomPolygon(1920, 1080, _rng, _canvas, _scene,
                    _scene.velocityMin, _scene.velocityMax);
                RandomPolygons.Add(poly);
            }
            _lastTick = DateTime.Now;
        }
    }
}