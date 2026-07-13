using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZYUnityKit
{
    [Serializable]
    public class ZYCurve
    {

        public CurveType curveType;
        public CurveDir curveDir;

        public static ZYCurve LinearIn { get { return new ZYCurve(CurveType.Linear, CurveDir.In); } }
        public static ZYCurve LinearOut { get { return new ZYCurve(CurveType.Linear, CurveDir.Out); } }
        public static ZYCurve QuadraticIn { get { return new ZYCurve(CurveType.Quadratic, CurveDir.In); } }
        public static ZYCurve QuadraticOut { get { return new ZYCurve(CurveType.Quadratic, CurveDir.Out); } }
        public static ZYCurve CubicIn { get { return new ZYCurve(CurveType.Cubic, CurveDir.In); } }
        public static ZYCurve CubicOut { get { return new ZYCurve(CurveType.Cubic, CurveDir.Out); } }
        public static ZYCurve QuarticIn { get { return new ZYCurve(CurveType.Quartic, CurveDir.In); } }
        public static ZYCurve QuarticOut { get { return new ZYCurve(CurveType.Quartic, CurveDir.Out); } }
        public static ZYCurve QuinticIn { get { return new ZYCurve(CurveType.Quintic, CurveDir.In); } }
        public static ZYCurve QuinticOut { get { return new ZYCurve(CurveType.Quintic, CurveDir.Out); } }
        public static ZYCurve QuadraticDoubleIn { get { return new ZYCurve(CurveType.QuadraticDouble, CurveDir.In); } }
        public static ZYCurve QuadraticDoubleOut { get { return new ZYCurve(CurveType.QuadraticDouble, CurveDir.Out); } }
        public static ZYCurve CubicDoubleIn { get { return new ZYCurve(CurveType.CubicDouble, CurveDir.In); } }
        public static ZYCurve CubicDoubleOut { get { return new ZYCurve(CurveType.CubicDouble, CurveDir.Out); } }
        public static ZYCurve QuarticDoubleIn { get { return new ZYCurve(CurveType.QuarticDouble, CurveDir.In); } }
        public static ZYCurve QuarticDoubleOut { get { return new ZYCurve(CurveType.QuarticDouble, CurveDir.Out); } }
        public static ZYCurve QuinticDoubleIn { get { return new ZYCurve(CurveType.QuinticDouble, CurveDir.In); } }
        public static ZYCurve QuinticDoubleOut { get { return new ZYCurve(CurveType.QuinticDouble, CurveDir.Out); } }
        public static ZYCurve SineIn { get { return new ZYCurve(CurveType.Sine, CurveDir.In); } }
        public static ZYCurve SineOut { get { return new ZYCurve(CurveType.Sine, CurveDir.Out); } }
        public static ZYCurve SineDoubleIn { get { return new ZYCurve(CurveType.SineDouble, CurveDir.In); } }
        public static ZYCurve SineDoubleOut { get { return new ZYCurve(CurveType.SineDouble, CurveDir.Out); } }
        public static ZYCurve ExpoIn { get { return new ZYCurve(CurveType.Expo, CurveDir.In); } }
        public static ZYCurve ExpoOut { get { return new ZYCurve(CurveType.Expo, CurveDir.Out); } }
        public static ZYCurve ExpoDoubleIn { get { return new ZYCurve(CurveType.ExpoDouble, CurveDir.In); } }
        public static ZYCurve ExpoDoubleOut { get { return new ZYCurve(CurveType.ExpoDouble, CurveDir.Out); } }
        public static ZYCurve ElasticIn { get { return new ZYCurve(CurveType.Elastic, CurveDir.In); } }
        public static ZYCurve ElasticOut { get { return new ZYCurve(CurveType.Elastic, CurveDir.Out); } }
        public static ZYCurve ElasticDoubleIn { get { return new ZYCurve(CurveType.ElasticDouble, CurveDir.In); } }
        public static ZYCurve ElasticDoubleOut { get { return new ZYCurve(CurveType.ElasticDouble, CurveDir.Out); } }
        public static ZYCurve CircIn { get { return new ZYCurve(CurveType.Circ, CurveDir.In); } }
        public static ZYCurve CircOut { get { return new ZYCurve(CurveType.Circ, CurveDir.Out); } }
        public static ZYCurve CircDoubleIn { get { return new ZYCurve(CurveType.CircDouble, CurveDir.In); } }
        public static ZYCurve CircDoubleOut { get { return new ZYCurve(CurveType.CircDouble, CurveDir.Out); } }
        public static ZYCurve BackIn { get { return new ZYCurve(CurveType.Back, CurveDir.In); } }
        public static ZYCurve BackOut { get { return new ZYCurve(CurveType.Back, CurveDir.Out); } }
        public static ZYCurve BackDoubleIn { get { return new ZYCurve(CurveType.BackDouble, CurveDir.In); } }
        public static ZYCurve BackDoubleOut { get { return new ZYCurve(CurveType.BackDouble, CurveDir.Out); } }
        public static ZYCurve BounceIn { get { return new ZYCurve(CurveType.Bounce, CurveDir.In); } }
        public static ZYCurve BounceOut { get { return new ZYCurve(CurveType.Bounce, CurveDir.Out); } }
        public static ZYCurve BounceDoubleIn { get { return new ZYCurve(CurveType.BounceDouble, CurveDir.In); } }
        public static ZYCurve BounceDoubleOut { get { return new ZYCurve(CurveType.BounceDouble, CurveDir.Out); } }
        public Func<float, float> func { get; private set; }
        public ZYCurve(CurveType curveType, CurveDir curveDir)
        {
            this.curveType = curveType;
            this.curveDir = curveDir;
        }
        public ZYCurve(Func<float, float> func)
        {
            this.func = func;
        }

        public ZYCurve Reverse()
        {
            return new ZYCurve(curveType, curveDir == CurveDir.In ? CurveDir.Out : CurveDir.In);
        }
    }
}
