﻿using OxyPlot.Axes;
using System;
using System.Diagnostics;

namespace Diplomna.Helpers
{
    [DebuggerDisplay("X: {X} => {FinalX} | Y: {Y} => {FinalY}")]
    public class DateValue
    {
        public double FinalX { get; set; }
        public double FinalY { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public DateTime Time
        {
            get { return DateTimeAxis.ToDateTime(this.X); }
            set
            {
                var finalX = DateTimeAxis.ToDouble(value);

                this.FinalX = finalX;
                this.X = finalX;
            }
        }

        public double Value
        {
            get { return this.Y; }
            set
            {
                this.FinalY = value;
                this.Y = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:HH:mm} {1:0.0}", this.Time, this.Value);
        }
    }
}
