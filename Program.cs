﻿using System;
using System.IO;
using System.Collections.Generic;

namespace CourseVMath
{
    public static class Program
    {
        const double EPS = 0.0001;
        
        private static double Shootingmethod(double[] y, double h)
        {
            double a = 0, b = 1;
            double[] ya = new double[2];
            double[] yb = new double[2];
            double[] yc = new double[2];
            ya = DoubleCounting(a, ya, h);

            bool f = ya[1] < y[1] ? true : false;
            while (f == (ya[1] < y[1]))
            {
                if (ya[1] < y[1])
                {
                    f = true; a = b;
                    b += h;
                }
                else
                {
                    f = false; b = a;
                    a -= h;
                }
                ya = DoubleCounting(a, ya, h);
            }

            b = a; a -= h;
            ya = DoubleCounting(a, ya, h);
            yb = DoubleCounting(b, yb, h);

            Console.WriteLine("Метод стрельб: ");

            Console.WriteLine($"y'(0) в интервале: [{Math.Round(a, 3)}, {Math.Round(b, 3)}]");

            Console.WriteLine($"Поиск по значениям y: [{ya[1]}, {yb[1]}]");

            double c = 0;
            while (Math.Abs(y[1] - yc[1]) > 0.125)
            {
                c = (a + b) / 2;
                yc = DoubleCounting(c, yc, h);
                if ((ya[1] - y[1]) * (yb[1] - y[1]) < 0)
                {
                    b = c; yb = yc;
                }
                else
                {
                    a = c; ya = yc;
                }
            }
            Console.WriteLine($"y'(0) = {c}\n");
            return c;
        }
        private static double[] DoubleCounting(double a, double[] y, double h)
        {
            double[] y1 = new double[2] { 1, a };
            double[] y2 = new double[2] { 1, a };
            double xi = 0;
            while (xi <= 1)
            {
                y1 = RungeKutt(xi, y1, h);
                y2 = RungeKutt(xi, y2, h / 2);
                y2 = RungeKutt(xi + h / 2, y2, h / 2);

                if (3 * EPS <= Math.Abs(y1[0] - y2[0]))
                {
                    xi = 0;
                    h /= 2;
                    y1 = new double[2] { 1, a };
                    y2 = new double[2] { 1, a };
                }
                else
                    xi += h;
            }

            y[0] = y2[0];
            y[1] = y2[1];
            return y;
        }
        private static double[] RungeKutt(double x, double[] y, double h)
        {
            double[] Ycash = new double[2];

            Ycash[0] = y[0];
            Ycash[1] = y[1];

            for (int i = 0; i < 2; i++)
            {
                Ycash[0] = Ycash[0] + (h / 2) * Ycash[1];
                Ycash[1] = Ycash[1] + (h / 2) * BisectionMethod(x + h, Ycash);
            }
            return Ycash;
        }
        private static double BisectionMethod(double x, double[] y)
        {
            double a = 0, b = 1, Fa = 1, Fb = 1, Fc, c;
            while (Fa * Fb > 0)
            {
                a--;
                b++;
                Fa = DifferentialEquation(x, y[0], y[1], a);
                Fb = DifferentialEquation(x, y[0], y[1], b);
            }

            while (true)
            {
                Fa = DifferentialEquation(x, y[0], y[1], a);
                c = (a + b) / 2;

                Fc = DifferentialEquation(x, y[0], y[1], c);
                if (Math.Abs(Fc) < EPS)
                    break;

                if (Fa * Fc < 0)
                    b = c;
                else
                    a = c;
            }
            return c;
        }
        private static double DifferentialEquation(double x, double y, double dy, double ddy)

        {
            return Math.Pow(ddy, 3) + 2 * Math.Sin(x) * ddy + 6 * Math.Cos(x) - 12 * Math.Exp(x) * dy - y / (Math.Pow((x + 1),2));
        }
        static void Main(string[] args)
        {
            double[,] result = new double[6, 3];
            double[] koshi = new double[2] { 1, 2.7183 }; //
            double h = 0.2;

            Console.WriteLine("Введите y(0) и y(1)");
            Console.WriteLine();
            Console.Write("Y(0)=");
            koshi[0] = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Y(1)=");
            koshi[1] = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine();

            double dy = Shootingmethod(koshi, h);
            double[] y = new double[2] { koshi[0], dy };
            double[] interpolation = new double[6];
            double[] xn = new double[6];
            int i;
            for (i = 0; i < 6; i++)
                xn[i] = 0.2 * i;
            double x = 0.0;
            i = 0;
            while (x <= 0.8)
            {
                result[i, 0] = x;
                result[i, 1] = y[0];
                result[i, 2] = y[1];
                interpolation[i] = y[0];
                i++;
                x += 0.2;
                if (x > 0.6 && x < 0.8)
                    x = 0.6;
                y = RungeKutt(x, y, h);
            }
            result[i, 0] = x;
            result[i, 1] = koshi[1];
            result[i, 2] = y[1];
            interpolation[i] = koshi[1];
            double[] interpolationResults = GetInterpolation(xn, interpolation);
            Console.WriteLine("Интеграл = " + Math.Pow(SimpsonMethod(0, 1, 1000, interpolationResults),2));
            Console.WriteLine();
            Console.Write("x");
            Console.Write($"{"y",20}");
            Console.WriteLine($"{"y'",21}");

            for (i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                    Console.Write($"{Math.Round(result[i, j], 5),-20}");
                Console.Write($"{Math.Round(result[i, 2], 5),-21}");
                Console.WriteLine();
            }
        }
        private static double F(double x, double[] y)
        {
            double z = 0; int i;
            for (i = 0; z < x; i++)
                z += 0.01;
            return y[i];

        }
        private static double SimpsonMethod(double a, double b, double n, double[] interpolation)
        {
            int k;
            double h = (b - a) / n;
            double answer = F(a, interpolation) + F(b, interpolation);
            for (int i = 1; i < n; i++)
            {
                if (i % 2 == 0)
                    k = 2;
                else
                    k = 4;
                answer += k * F(a + i * h, interpolation);
            }
            answer *= h / 3;

            return answer;
        }
        private static double AitkenInterpolation(double[] X, double[] Y, double x)
        {
            double[] P = new double[Y.Length];

            for (int i = 0; i < Y.Length; i++)
                P[i] = Y[i];
            for (int k = 1; k < Y.Length; k++)
                for (int i = 0; i < Y.Length - k; i++)
                    P[i] = (P[i] * (X[i + k] - x) - P[i + 1] * (X[i] - x)) / (X[i + k] - X[i]);

            return P[0];
        }
        private static double[] GetInterpolation(double[] x, double[] y)
        {
            Console.WriteLine("Интерполяция");
            List<double> result = new List<double>();

            for (double i = 0; i <= 1.01; i += 0.01)
            {
                var answer = AitkenInterpolation(x, y, i);
                Console.WriteLine($"x = {Math.Round(i, 2),-5}       y = {answer}");
                result.Add(answer);
            }
            using (StreamWriter sw = new StreamWriter("save.txt", false, System.Text.Encoding.Default))
            {
                double i = 0;
                foreach (var res in result)
                {
                    i += 0.01;
                    sw.Write("(" + i + ";" + res + ") ");
                }
            }
            Console.WriteLine();
            return result.ToArray();
        }
    }
}
