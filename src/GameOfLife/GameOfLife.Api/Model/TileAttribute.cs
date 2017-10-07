﻿namespace GameOfLife.Api.Model
{
    public abstract class TileAttribute
    {
        public double Value { get; set; }
        public double Step { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public double MedianValue { get; }

        protected TileAttribute(double step, double minimum, double maximum) : this(step, minimum, maximum, (maximum + minimum) / 2)
        {
        }

        protected TileAttribute(double step, double minimum, double maximum, double medianValue)
        {
            Step = step;
            Minimum = minimum;
            Maximum = maximum;
            MedianValue = medianValue;
            Value = medianValue;
        }
    }
}