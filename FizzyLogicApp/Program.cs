using System;

namespace FizzyLogicApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Medical Risk Assessment System");
            
            //datas
            double bloodGlucose = 145.0; 
            double bmi = 29.5;      

            Console.WriteLine($"\nPatient Metrics:");
            Console.WriteLine($" - Blood Glucose: {bloodGlucose} mg/dL");
            Console.WriteLine($" - BMI:           {bmi} kg/m²");

            //fuzzi
            //glucose fuzzy sets: Normal, Prediabetic, Diabetic
            double gNormal = TrapezoidalMembership(bloodGlucose, 0, 0, 70, 100);
            double gPre = TriangularMembership(bloodGlucose, 90, 125, 160);
            double gDiab = TrapezoidalMembership(bloodGlucose, 140, 180, 300, 300);

            //bmi fuzzy sets: Normal, Overweight, Obese
            double bNormal = TrapezoidalMembership(bmi, 0, 0, 18.5, 24.9);
            double bOver = TriangularMembership(bmi, 23.0, 27.5, 32.0);
            double bObese = TrapezoidalMembership(bmi, 30.0, 35.0, 60.0, 60.0);

            Console.WriteLine("\n---Fuzzification Values---");
            Console.WriteLine($"Glucose - [Normal: {gNormal:F2}, Prediabetic: {gPre:F2}, Diabetic: {gDiab:F2}]");
            Console.WriteLine($"BMI     - [Normal: {bNormal:F2}, Overweight: {bOver:F2}, Obese: {bObese:F2}]");

            /*
             evaluation:
                - if glucose is normal, and bmi is normal, risk is low
                - if glucose is prediabetic or bmi is overweight, risk is moderate
                - if glucose is diabetic or glucose is prediabetic and bmi is obese, risk is high
             */

            double ruleLow = Math.Min(gNormal, bNormal);
            double ruleModerate = Math.Max(gPre, bOver);
            double ruleHigh = Math.Max(gDiab, Math.Min(gPre, bObese));

            Console.WriteLine("\n---Rule Strengths---");
            Console.WriteLine($"Rule 1 (Low Risk):      {ruleLow:F2}");
            Console.WriteLine($"Rule 2 (Moderate Risk): {ruleModerate:F2}");
            Console.WriteLine($"Rule 3 (High Risk):     {ruleHigh:F2}");

            //defuzz
            //output centers: Low Risk = 15%, Moderate Risk = 50%, High Risk = 85%
            double cLow = 15.0;
            double cMod = 50.0;
            double cHigh = 85.0;

            double numerator = (ruleLow * cLow) + (ruleModerate * cMod) + (ruleHigh * cHigh);
            double denominator = ruleLow + ruleModerate + ruleHigh;

            double riskScore = denominator > 0 ? (numerator / denominator) : 0.0;

            Console.WriteLine("\n---Diagnostic Summary---");
            Console.WriteLine($"Calculated Health Risk Index: {riskScore:F1}%");

            //triage classification
            if (riskScore < 30.0)
                Console.WriteLine("Action Plan: Routine annual checkup.");
            else if (riskScore < 65.0)
                Console.WriteLine("Action Plan: Lifestyle modification & follow-up in 3 months.");
            else
                Console.WriteLine("Action Plan: Immediate medical intervention required.");

            Console.ReadKey();
        }

        //triangular membership function
        static double TriangularMembership(double x, double a, double b, double c)
        {
            if (x <= a || x >= c) return 0.0;
            if (x == b) return 1.0;
            if (x > a && x < b) return (x - a) / (b - a);
            return (c - x) / (c - b);
        }

        //trapezoidal membership function
        static double TrapezoidalMembership(double x, double a, double b, double c, double d)
        {
            if (x <= a || x >= d) return 0.0;
            if (x >= b && x <= c) return 1.0;
            if (x > a && x < b) return (x - a) / (b - a);
            return (d - x) / (d - c);
        }
    }
}